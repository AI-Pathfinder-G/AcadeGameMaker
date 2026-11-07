using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AcadeGameMaker.Profile;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.Profile
{
    // REQ-M5D3-001..006; AC-M5D3-001..006. Engine-free immutable progression projection matrix.
    public sealed class ProfileProgressionSnapshotTests
    {
        [TestCase(-1)]
        [TestCase(101)]
        [TestCase(202)]
        [TestCase(303)]
        [TestCase(404)]
        public void AC001_RevisionAndApprovedSeedValuesAreExactAtConstructionAndRevalidation(int seedValue)
        {
            int? seed = seedValue < 0 ? (int?)null : seedValue;
            AssertSnapshot(new ProfileProgressionSnapshot(0, seed, null, ProfileConsentState.RefusesOwnershipTransfer, null, EmptyBranches()), 0, seed);
            AssertSnapshot(new ProfileProgressionSnapshot(long.MaxValue, seed, null, ProfileConsentState.ImprintSevered, null, EmptyBranches()), long.MaxValue, seed);

            Assert.Throws<ArgumentOutOfRangeException>(() => new ProfileProgressionSnapshot(-1, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, EmptyBranches()));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ProfileProgressionSnapshot(1, 100, null, ProfileConsentState.RefusesOwnershipTransfer, null, EmptyBranches()));

            var invalidRevision = Corrupt(ValidNoChoice(), "_profileRevision", -1L);
            var invalidSeed = Corrupt(ValidNoChoice(), "_lastOfferedSeed", (int?)999);
            Assert.Throws<InvalidOperationException>(() => invalidRevision.Validate());
            Assert.Throws<InvalidOperationException>(() => invalidSeed.Validate());
        }

        [Test]
        public void AC002_NoChoiceAndBothExactPairsProjectWithoutPartialOrInvalidFallback()
        {
            var noChoice = ValidNoChoice();
            Assert.That(noChoice.HasCommittedPair, Is.False);
            Assert.That(noChoice.TryGetCommittedPair(out var ignored), Is.False);

            AssertExactPair(new ProfileProgressionSnapshot(4, 101, ProfileChoice.Extraction,
                ProfileConsentState.RefusesOwnershipTransfer, ProfileSkill.CompressionVerdict, EmptyBranches()),
                ProfileChoice.Extraction, ProfileSkill.CompressionVerdict);
            AssertExactPair(new ProfileProgressionSnapshot(5, 202, ProfileChoice.Solidarity,
                ProfileConsentState.OffersScopedResonance, ProfileSkill.CommonReferencePlane, EmptyBranches()),
                ProfileChoice.Solidarity, ProfileSkill.CommonReferencePlane);

            Assert.Throws<ArgumentException>(() => new ProfileProgressionSnapshot(1, null, ProfileChoice.Extraction, ProfileConsentState.RefusesOwnershipTransfer, null, EmptyBranches()));
            Assert.Throws<ArgumentException>(() => new ProfileProgressionSnapshot(1, null, null, ProfileConsentState.RefusesOwnershipTransfer, ProfileSkill.CompressionVerdict, EmptyBranches()));
            Assert.Throws<ArgumentException>(() => new ProfileProgressionSnapshot(1, null, ProfileChoice.Extraction, ProfileConsentState.RefusesOwnershipTransfer, ProfileSkill.CommonReferencePlane, EmptyBranches()));
            Assert.Throws<ArgumentException>(() => new ProfileProgressionSnapshot(1, null, (ProfileChoice)0, ProfileConsentState.RefusesOwnershipTransfer, null, EmptyBranches()));
            Assert.Throws<ArgumentException>(() => new ProfileProgressionSnapshot(1, null, null, ProfileConsentState.RefusesOwnershipTransfer, (ProfileSkill)99, EmptyBranches()));

            var bypassed = Corrupt(ValidExtraction(), "_committedChoice", (ProfileChoice?)0);
            Assert.Throws<InvalidOperationException>(() => bypassed.Validate());
            Assert.Throws<InvalidOperationException>(() => { var unused = bypassed.HasCommittedPair; });
            Assert.Throws<InvalidOperationException>(() => bypassed.TryGetCommittedPair(out var unused));
        }

        [Test]
        public void AC003_AllConsentValuesArePreservedWithoutPairConsentInferenceOrTransitions()
        {
            foreach (ProfileConsentState consent in Enum.GetValues(typeof(ProfileConsentState)))
            {
                var extraction = new ProfileProgressionSnapshot(7, null, ProfileChoice.Extraction, consent,
                    ProfileSkill.CompressionVerdict, EmptyBranches());
                var solidarity = new ProfileProgressionSnapshot(8, null, ProfileChoice.Solidarity, consent,
                    ProfileSkill.CommonReferencePlane, EmptyBranches());
                Assert.That(extraction.ConsentState, Is.EqualTo(consent));
                Assert.That(solidarity.ConsentState, Is.EqualTo(consent));
            }

            Assert.Throws<ArgumentOutOfRangeException>(() => new ProfileProgressionSnapshot(1, null, null, (ProfileConsentState)0, null, EmptyBranches()));
            Assert.Throws<ArgumentOutOfRangeException>(() => new ProfileProgressionSnapshot(1, null, null, (ProfileConsentState)99, null, EmptyBranches()));
            Assert.Throws<InvalidOperationException>(() => Corrupt(ValidNoChoice(), "_consentState", (ProfileConsentState)99).Validate());
        }

        [Test]
        public void AC004_CompletedBranchesAreCanonicalUniqueAndDefensivelyCopied()
        {
            AssertBranches(EmptyBranches());
            AssertBranches(new[] { ProfileBranch.Extraction }, ProfileBranch.Extraction);
            AssertBranches(new[] { ProfileBranch.Solidarity }, ProfileBranch.Solidarity);
            AssertBranches(new[] { ProfileBranch.Extraction, ProfileBranch.Solidarity }, ProfileBranch.Extraction, ProfileBranch.Solidarity);

            Assert.Throws<ArgumentNullException>(() => new ProfileProgressionSnapshot(1, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, null));
            Assert.Throws<ArgumentException>(() => new ProfileProgressionSnapshot(1, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, new[] { ProfileBranch.Solidarity, ProfileBranch.Extraction }));
            Assert.Throws<ArgumentException>(() => new ProfileProgressionSnapshot(1, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, new[] { ProfileBranch.Extraction, ProfileBranch.Extraction }));
            Assert.Throws<ArgumentException>(() => new ProfileProgressionSnapshot(1, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, new[] { (ProfileBranch)99 }));

            var source = new[] { ProfileBranch.Extraction };
            var snapshot = new ProfileProgressionSnapshot(1, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, source);
            source[0] = ProfileBranch.Solidarity;
            Assert.That(snapshot.CompletedBranches[0], Is.EqualTo(ProfileBranch.Extraction));
            var returned = snapshot.CompletedBranches;
            Assert.Throws<NotSupportedException>(() => ((IList)returned)[0] = ProfileBranch.Solidarity);
            Assert.That(snapshot.CompletedBranches[0], Is.EqualTo(ProfileBranch.Extraction));

            Assert.Throws<InvalidOperationException>(() => Corrupt(snapshot, "_completedBranches", new[] { ProfileBranch.Solidarity, ProfileBranch.Extraction }).Validate());
        }

        [Test]
        public void AC005_DefaultAndBypassedValuesThrowRatherThanMasqueradingAsNoChoice()
        {
            var defaultSnapshot = default(ProfileProgressionSnapshot);
            Assert.Throws<InvalidOperationException>(() => defaultSnapshot.Validate());
            Assert.Throws<InvalidOperationException>(() => { var unused = defaultSnapshot.HasCommittedPair; });
            Assert.Throws<InvalidOperationException>(() => defaultSnapshot.TryGetCommittedPair(out var unused));

            var defaultPair = default(ProfileChoiceSkillPair);
            Assert.Throws<InvalidOperationException>(() => defaultPair.Validate());
            Assert.Throws<InvalidOperationException>(() => { var unused = defaultPair.Choice; });
            Assert.Throws<InvalidOperationException>(() => { var unused = defaultPair.Skill; });

            var validAfterFailure = ValidNoChoice();
            validAfterFailure.Validate();
            Assert.That(validAfterFailure.CompletedBranches.Count, Is.Zero);
        }

        [Test]
        public void AC006_ProfileRuntimeStaysEngineFreeAndOmitsPersistenceAuthority()
        {
            var root = FindProjectRoot();
            var asmdef = File.ReadAllText(Path.Combine(root, "Assets", "AcadeGameMaker", "Runtime", "Profile", "AcadeGameMaker.Profile.asmdef"));
            var source = File.ReadAllText(Path.Combine(root, "Assets", "AcadeGameMaker", "Runtime", "Profile", "ProfileProgressionSnapshot.cs"));
            StringAssert.Contains("\"noEngineReferences\": true", asmdef);
            StringAssert.Contains("REQ-M5D3-001..006", source);
            foreach (var forbidden in new[] { "UnityEngine", "System.IO", "Cryptography", "Json", "DateTime", "Random", "System.Net", "callback" })
                StringAssert.DoesNotContain(forbidden, source);
        }

        private static ProfileProgressionSnapshot ValidNoChoice() => new ProfileProgressionSnapshot(3, null, null,
            ProfileConsentState.RefusesOwnershipTransfer, null, EmptyBranches());

        private static ProfileProgressionSnapshot ValidExtraction() => new ProfileProgressionSnapshot(3, 101,
            ProfileChoice.Extraction, ProfileConsentState.RefusesOwnershipTransfer, ProfileSkill.CompressionVerdict, EmptyBranches());

        private static ProfileBranch[] EmptyBranches() => Array.Empty<ProfileBranch>();

        private static void AssertSnapshot(ProfileProgressionSnapshot snapshot, long expectedRevision, int? expectedSeed)
        {
            snapshot.Validate();
            Assert.That(snapshot.ProfileRevision, Is.EqualTo(expectedRevision));
            Assert.That(snapshot.LastOfferedSeed, Is.EqualTo(expectedSeed));
        }

        private static void AssertExactPair(ProfileProgressionSnapshot snapshot, ProfileChoice choice, ProfileSkill skill)
        {
            Assert.That(snapshot.HasCommittedPair, Is.True);
            Assert.That(snapshot.TryGetCommittedPair(out var pair), Is.True);
            Assert.That(pair.Choice, Is.EqualTo(choice));
            Assert.That(pair.Skill, Is.EqualTo(skill));
        }

        private static void AssertBranches(IReadOnlyList<ProfileBranch> source, params ProfileBranch[] expected)
        {
            var snapshot = new ProfileProgressionSnapshot(1, null, null, ProfileConsentState.RefusesOwnershipTransfer, null, source);
            Assert.That(snapshot.CompletedBranches, Is.EqualTo(expected));
        }

        private static ProfileProgressionSnapshot Corrupt(ProfileProgressionSnapshot source, string fieldName, object value)
        {
            object boxed = source;
            typeof(ProfileProgressionSnapshot).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(boxed, value);
            return (ProfileProgressionSnapshot)boxed;
        }

        private static string FindProjectRoot()
        {
            var current = new DirectoryInfo(TestContext.CurrentContext.TestDirectory);
            while (current != null)
            {
                if (File.Exists(Path.Combine(current.FullName, "Packages", "manifest.json"))) return current.FullName;
                current = current.Parent;
            }
            throw new DirectoryNotFoundException("Project root was not found.");
        }
    }
}
