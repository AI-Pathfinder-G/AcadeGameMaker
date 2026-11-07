using System;
using System.IO;
using System.Reflection;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Profile;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.EditMode.InputUnity
{
    // REQ-M5D7QC3-002/004; AC-M5D7QC3-002/004.  Normal rows obtain both
    // observations through a real launched adapter/router cohort. Reflection
    // is used only to prove that a corrupted issued row fails closed.
    internal sealed class ProfileNewGameConfirmationV1Tests
    {
        [Test]
        public void AC004_ActualSameLeaseEvidenceRecapturesAsTheSameIdentity()
        {
            using (var fixture = Fixture.Create())
            {
                File.WriteAllBytes(Path.Combine(fixture.Root, "profile.json"), ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes);
                var owner = new object();
                var epoch = new object();
                var displayed = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);
                var recaptured = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);

                Assert.That(displayed.Outcome, Is.EqualTo(ProfileNewGameCaptureOutcomeV1.Captured));
                Assert.That(recaptured.Outcome, Is.EqualTo(ProfileNewGameCaptureOutcomeV1.Captured));
                Assert.That(ProfileNewGameConfirmationV1.MatchesRecapturedIdentity(fixture.Adapter, fixture.Router, owner, epoch, displayed, recaptured), Is.True);
            }
        }

        [Test]
        public void AC001_NoConfirmationCaptureMintsOneOpaqueRequestAndOneReciprocalCommit()
        {
            using (var fixture = Fixture.Create())
            {
                File.WriteAllBytes(Path.Combine(fixture.Root, "profile.json"), ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes);
                var owner = new object();
                var epoch = new object();
                var capture = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);
                var resolution = ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture, fixture.Adapter, fixture.Router, owner, epoch, 1, 1);

                Assert.That(resolution.Disposition, Is.EqualTo(ProfileNewGameConfirmedDispositionV1.Confirmed));
                Assert.That(resolution.FreshClassification, Is.EqualTo(NewGameObservationClassificationV1.NoConfirmationRequired));
                var request = resolution.ConfirmedRequest;
                var forgedRequest = new ConfirmedProfileResetRequestV1();
                Assert.That(() => ProfileNewGameConfirmationV1.ReserveExecutionCommit(forgedRequest, fixture.Adapter, fixture.Router, owner, epoch, 1, 1), Throws.InvalidOperationException);
                Assert.That(() => ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture, fixture.Adapter, fixture.Router, owner, epoch, 1, 1), Throws.InvalidOperationException);
                var reservation = ProfileNewGameConfirmationV1.ReserveExecutionCommit(request, fixture.Adapter, fixture.Router, owner, epoch, 1, 1);
                Assert.That(() => ProfileNewGameConfirmationV1.CompleteExecutionCommit(request, reservation, fixture.Adapter, fixture.Router, new object(), epoch, 1, 1), Throws.InvalidOperationException);
                Assert.That(ProfileNewGameConfirmationV1.CompleteExecutionCommit(request, reservation, fixture.Adapter, fixture.Router, owner, epoch, 1, 1), Is.SameAs(request));
                Assert.That(() => ProfileNewGameConfirmationV1.ReserveExecutionCommit(request, fixture.Adapter, fixture.Router, owner, epoch, 1, 1), Throws.InvalidOperationException);
            }
        }

        [Test]
        public void AC008_ClosedReservationCannotRollbackOrComplete()
        {
            using (var fixture = Fixture.Create())
            {
                File.WriteAllBytes(Path.Combine(fixture.Root, "profile.json"), ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes);
                var owner = new object();
                var epoch = new object();
                var capture = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);
                var request = ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture, fixture.Adapter, fixture.Router, owner, epoch, 2, 3).ConfirmedRequest;
                var reservation = ProfileNewGameConfirmationV1.ReserveExecutionCommit(request, fixture.Adapter, fixture.Router, owner, epoch, 2, 3);
                var forgedReservation = new ProfileNewGameExecutionCommitReservationV1();
                Assert.That(() => ProfileNewGameConfirmationV1.CompleteExecutionCommit(request, forgedReservation, fixture.Adapter, fixture.Router, owner, epoch, 2, 3), Throws.InvalidOperationException);
                Assert.That(() => ProfileNewGameConfirmationV1.CompleteExecutionCommit(request, reservation, fixture.Adapter, fixture.Router, owner, epoch, 2, 3), Throws.InvalidOperationException);
                Assert.That(() => ProfileNewGameConfirmationV1.ReserveExecutionCommit(request, fixture.Adapter, fixture.Router, owner, epoch, 2, 3), Throws.InvalidOperationException);
                Assert.That(() => ProfileNewGameConfirmationV1.CloseExecutionCommit(request, fixture.Adapter, fixture.Router, owner, epoch, 2, 3), Throws.InvalidOperationException);
            }
        }

        [Test]
        public void AC008_ReservedProjectionOrRootCorruptionClosesTheOriginalRequest()
        {
            using (var fixture = Fixture.Create())
            {
                File.WriteAllBytes(Path.Combine(fixture.Root, "profile.json"), ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes);
                var owner = new object();
                var epoch = new object();
                var capture = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);
                var request = ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture, fixture.Adapter, fixture.Router, owner, epoch, 6, 1).ConfirmedRequest;
                var reservation = ProfileNewGameConfirmationV1.ReserveExecutionCommit(request, fixture.Adapter, fixture.Router, owner, epoch, 6, 1);
                var observed = Field<object>(capture, "_capture");
                var primary = Field<object>(observed, "Primary");
                try
                {
                    SetField(observed, "Primary", null);
                    Assert.That(() => ProfileNewGameConfirmationV1.CompleteExecutionCommit(request, reservation, fixture.Adapter, fixture.Router, owner, epoch, 6, 1), Throws.InvalidOperationException);
                }
                finally { SetField(observed, "Primary", primary); }
                Assert.That(() => ProfileNewGameConfirmationV1.CompleteExecutionCommit(request, reservation, fixture.Adapter, fixture.Router, owner, epoch, 6, 1), Throws.InvalidOperationException);
            }

            using (var fixture = Fixture.Create())
            {
                File.WriteAllBytes(Path.Combine(fixture.Root, "profile.json"), ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes);
                var owner = new object();
                var epoch = new object();
                var capture = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);
                var request = ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture, fixture.Adapter, fixture.Router, owner, epoch, 6, 2).ConfirmedRequest;
                var reservation = ProfileNewGameConfirmationV1.ReserveExecutionCommit(request, fixture.Adapter, fixture.Router, owner, epoch, 6, 2);
                var root = Field<string>(fixture.Adapter, "_launchRootWitnessA");
                try
                {
                    SetField(fixture.Adapter, "_launchRootWitnessA", Path.Combine(fixture.Root, "foreign-root"));
                    Assert.That(() => ProfileNewGameConfirmationV1.CompleteExecutionCommit(request, reservation, fixture.Adapter, fixture.Router, owner, epoch, 6, 2), Throws.InvalidOperationException);
                }
                finally { SetField(fixture.Adapter, "_launchRootWitnessA", root); }
                Assert.That(() => ProfileNewGameConfirmationV1.CompleteExecutionCommit(request, reservation, fixture.Adapter, fixture.Router, owner, epoch, 6, 2), Throws.InvalidOperationException);
            }
        }

        [Test]
        public void AC004_ConfirmedRecaptureMintsOnlyTheFreshActualCapture()
        {
            using (var fixture = Fixture.Create())
            {
                File.WriteAllBytes(Path.Combine(fixture.Root, "profile.json"), ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes);
                var owner = new object();
                var epoch = new object();
                var displayed = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);
                var fresh = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);
                var resolution = ProfileNewGameConfirmationV1.ResolveConfirmedRecapture(displayed, fresh, fixture.Adapter, fixture.Router, owner, epoch, 3, 7);

                Assert.That(resolution.Disposition, Is.EqualTo(ProfileNewGameConfirmedDispositionV1.Confirmed));
                Assert.That(resolution.ConfirmedRequest, Is.Not.Null);
                Assert.That(() => ProfileNewGameConfirmationV1.ResolveConfirmedRecapture(displayed, fresh, fixture.Adapter, fixture.Router, owner, epoch, 3, 7), Throws.InvalidOperationException);
                Assert.That(() => ProfileNewGameConfirmationV1.ResolveConfirmedRecapture(displayed, displayed, fixture.Adapter, fixture.Router, owner, epoch, 3, 8), Throws.InvalidOperationException);
            }
        }

        [Test]
        public void AC004_ChangedAmbiguousActualRecaptureReturnsNoRequest()
        {
            using (var fixture = Fixture.Create())
            {
                var primary = Path.Combine(fixture.Root, "profile.json");
                File.WriteAllBytes(primary, ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes);
                var owner = new object();
                var epoch = new object();
                var displayed = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);
                File.WriteAllBytes(primary, new byte[] { 0x00, 0xFF, 0x11 });
                var fresh = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);

                var resolution = ProfileNewGameConfirmationV1.ResolveConfirmedRecapture(displayed, fresh, fixture.Adapter, fixture.Router, owner, epoch, 4, 9);
                Assert.That(resolution.Disposition, Is.EqualTo(ProfileNewGameConfirmedDispositionV1.FreshDecisionRequired));
                Assert.That(resolution.FreshClassification, Is.EqualTo(NewGameObservationClassificationV1.ConfirmationRequiredAmbiguous));
                Assert.That(() => { var ignored = resolution.ConfirmedRequest; }, Throws.InvalidOperationException);
            }
        }

        [Test]
        public void AC004_ChangedActualLeafRequiresFreshDecisionAndForeignEvidenceFailsClosed()
        {
            using (var fixture = Fixture.Create())
            {
                var primary = Path.Combine(fixture.Root, "profile.json");
                File.WriteAllBytes(primary, ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes);
                var owner = new object();
                var epoch = new object();
                var displayed = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);
                File.Delete(primary);
                var changed = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);

                Assert.That(displayed.Outcome, Is.EqualTo(ProfileNewGameCaptureOutcomeV1.Captured));
                Assert.That(changed.Outcome, Is.EqualTo(ProfileNewGameCaptureOutcomeV1.Captured));
                Assert.That(ProfileNewGameConfirmationV1.MatchesRecapturedIdentity(fixture.Adapter, fixture.Router, owner, epoch, displayed, changed), Is.False);
                Assert.That(() => ProfileNewGameConfirmationV1.MatchesRecapturedIdentity(fixture.Adapter, fixture.Router, new object(), epoch, displayed, changed), Throws.InvalidOperationException);
                Assert.That(() => ProfileNewGameConfirmationV1.MatchesRecapturedIdentity(fixture.Adapter, fixture.Router, owner, new object(), displayed, changed), Throws.InvalidOperationException);
            }
        }

        [Test]
        public void AC001_ReflectedCaptureResultFieldsOrCloneCannotAuthenticateRecapture()
        {
            using (var fixture = Fixture.Create())
            {
                var owner = new object();
                var epoch = new object();
                var displayed = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);
                var recaptured = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);
                var field = typeof(ProfileNewGameCaptureResultV1).GetField("_owner", BindingFlags.Instance | BindingFlags.NonPublic);
                var original = field.GetValue(recaptured);

                try
                {
                    field.SetValue(recaptured, new object());
                    Assert.That(() => ProfileNewGameConfirmationV1.MatchesRecapturedIdentity(fixture.Adapter, fixture.Router, owner, epoch, displayed, recaptured), Throws.InvalidOperationException);
                }
                finally { field.SetValue(recaptured, original); }

                var rootField = typeof(ProfileNewGameCaptureResultV1).GetField("_root", BindingFlags.Instance | BindingFlags.NonPublic);
                var root = rootField.GetValue(recaptured);
                try
                {
                    rootField.SetValue(recaptured, Path.Combine(fixture.Root, "foreign-root"));
                    Assert.That(() => ProfileNewGameConfirmationV1.MatchesRecapturedIdentity(fixture.Adapter, fixture.Router, owner, epoch, displayed, recaptured), Throws.InvalidOperationException);
                }
                finally { rootField.SetValue(recaptured, root); }

                var outcomeField = typeof(ProfileNewGameCaptureResultV1).GetField("_outcome", BindingFlags.Instance | BindingFlags.NonPublic);
                var outcome = outcomeField.GetValue(recaptured);
                try
                {
                    outcomeField.SetValue(recaptured, ProfileNewGameCaptureOutcomeV1.Unreadable);
                    Assert.That(() => ProfileNewGameConfirmationV1.MatchesRecapturedIdentity(fixture.Adapter, fixture.Router, owner, epoch, displayed, recaptured), Throws.InvalidOperationException);
                }
                finally { outcomeField.SetValue(recaptured, outcome); }

                Assert.That(ProfileNewGameConfirmationV1.MatchesRecapturedIdentity(fixture.Adapter, fixture.Router, owner, epoch, displayed, recaptured), Is.True);
            }
        }

        [Test]
        public void AC001_ProjectionCorruptionAndImpossibleResolutionFailClosed()
        {
            using (var fixture = Fixture.Create())
            {
                File.WriteAllBytes(Path.Combine(fixture.Root, "profile.json"), ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes);
                var owner = new object();
                var epoch = new object();
                var capture = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);
                var observed = Field<object>(capture, "_capture");
                var projection = Field<object>(observed, "Primary");
                try
                {
                    SetField(observed, "Primary", null);
                    Assert.That(() => ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture, fixture.Adapter, fixture.Router, owner, epoch, 5, 1), Throws.InvalidOperationException);
                }
                finally { SetField(observed, "Primary", projection); }

                var impossible = new ProfileNewGameConfirmedResolutionV1(ProfileNewGameConfirmedDispositionV1.Confirmed, NewGameObservationClassificationV1.NoConfirmationRequired, null);
                Assert.That(() => { var ignored = impossible.Disposition; }, Throws.InvalidOperationException);
                var unregistered = new ProfileNewGameCaptureResultV1(owner, epoch, null, null, ProfileNewGameCaptureOutcomeV1.Captured, NewGameObservationClassificationV1.NoConfirmationRequired);
                Assert.That(() => ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(unregistered, fixture.Adapter, fixture.Router, owner, epoch, 5, 2), Throws.InvalidOperationException);
            }
        }

        [Test]
        public void AC004_AllThreeActualLeafProjectionsAreRequiredForFreshComparison()
        {
            using (var fixture = Fixture.Create())
            {
                var bytes = ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes;
                File.WriteAllBytes(Path.Combine(fixture.Root, "profile.json"), bytes);
                File.WriteAllBytes(Path.Combine(fixture.Root, "profile.prev.json"), bytes);
                File.WriteAllBytes(Path.Combine(fixture.Root, "profile.tmp.json"), bytes);
                var owner = new object();
                var epoch = new object();
                var displayed = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);
                var fresh = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);
                var capture = Field<object>(fresh, "_capture");
                var previous = Field<object>(capture, "Previous");
                try
                {
                    SetField(capture, "Previous", null);
                    Assert.That(() => ProfileNewGameConfirmationV1.MatchesRecapturedIdentity(fixture.Adapter, fixture.Router, owner, epoch, displayed, fresh), Throws.InvalidOperationException);
                }
                finally { SetField(capture, "Previous", previous); }
                Assert.That(ProfileNewGameConfirmationV1.MatchesRecapturedIdentity(fixture.Adapter, fixture.Router, owner, epoch, displayed, fresh), Is.True);
            }
        }

        [Test]
        public void AC001_ActualMemberwiseClonesCannotBorrowIssuerRegistration()
        {
            using (var fixture = Fixture.Create())
            {
                var owner = new object(); var epoch = new object();
                var capture = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);
                var clone = Clone(capture);
                Assert.That(() => ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(clone, fixture.Adapter, fixture.Router, owner, epoch, 1, 1), Throws.InvalidOperationException);
                var observationClone = Clone(Field<object>(capture, "_capture"));
                Assert.That(() => Invoke(observationClone, "Validate"), Throws.TypeOf<TargetInvocationException>().With.InnerException.TypeOf<InvalidOperationException>());
                var resolution = ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture, fixture.Adapter, fixture.Router, owner, epoch, 1, 1);
                var resolutionClone = Clone(resolution);
                Assert.That(() => { var ignored = resolutionClone.ConfirmedRequest; }, Throws.InvalidOperationException);
                var request = resolution.ConfirmedRequest;
                Assert.That(() => ProfileNewGameConfirmationV1.ReserveExecutionCommit(Clone(request), fixture.Adapter, fixture.Router, owner, epoch, 1, 1), Throws.InvalidOperationException);
                var reservation = ProfileNewGameConfirmationV1.ReserveExecutionCommit(request, fixture.Adapter, fixture.Router, owner, epoch, 1, 1);
                Assert.That(() => ProfileNewGameConfirmationV1.CompleteExecutionCommit(request, Clone(reservation), fixture.Adapter, fixture.Router, owner, epoch, 1, 1), Throws.InvalidOperationException);
                Assert.That(() => ProfileNewGameConfirmationV1.CompleteExecutionCommit(request, reservation, fixture.Adapter, fixture.Router, owner, epoch, 1, 1), Throws.InvalidOperationException);
            }
        }

        [TestCase("result", "_capture", false)] [TestCase("result", "_capture", true)]
        [TestCase("result", "_classification", false)] [TestCase("result", "_classification", true)]
        [TestCase("issuer", "Root", false)] [TestCase("issuer", "Root", true)]
        [TestCase("issuer", "Capture", false)] [TestCase("issuer", "Capture", true)]
        [TestCase("issuer", "Outcome", false)] [TestCase("issuer", "Outcome", true)]
        [TestCase("issuer", "Classification", false)] [TestCase("issuer", "Classification", true)]
        [TestCase("request", "Capture", false)] [TestCase("request", "Capture", true)]
        [TestCase("request", "Root", false)] [TestCase("request", "Root", true)]
        [TestCase("request", "SourceResult", false)] [TestCase("request", "SourceResult", true)]
        [TestCase("request", "SourceWitness", false)] [TestCase("request", "SourceWitness", true)]
        public void AC008_OriginalIssuanceSingleFieldReplacementClosesRequest(string targetKind, string fieldName, bool afterReserve)
        {
            using (var fixture = Fixture.Create())
            {
                var owner = new object(); var epoch = new object();
                var original = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);
                var request = ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(original, fixture.Adapter, fixture.Router, owner, epoch, 1, 1).ConfirmedRequest;
                var reservation = afterReserve ? ProfileNewGameConfirmationV1.ReserveExecutionCommit(request, fixture.Adapter, fixture.Router, owner, epoch, 1, 1) : null;
                // Another genuine capture from the SAME cohort/root is not the mint source.
                File.WriteAllBytes(Path.Combine(fixture.Root, "profile.tmp.json"), new byte[] { 0xFF });
                var other = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);
                var target = targetKind == "result" ? (object)original : Witness(targetKind == "issuer" ? "IssuedResults" : "IssuedConfirmedRequests", targetKind == "issuer" ? (object)original : request);
                var saved = Field<object>(target, fieldName);
                object changed;
                if (fieldName == "Root") changed = Path.Combine(fixture.Root, "foreign");
                else if (fieldName == "Outcome") changed = ProfileNewGameCaptureOutcomeV1.Unreadable;
                else if (fieldName == "Classification" || fieldName == "_classification") changed = NewGameObservationClassificationV1.ConfirmationRequiredAmbiguous;
                else if (fieldName == "SourceResult") changed = other;
                else if (fieldName == "SourceWitness") changed = Witness("IssuedResults", other);
                else changed = Field<object>(other, "_capture");
                try
                {
                    SetField(target, fieldName, changed);
                    if (afterReserve) Assert.That(() => ProfileNewGameConfirmationV1.CompleteExecutionCommit(request, reservation, fixture.Adapter, fixture.Router, owner, epoch, 1, 1), Throws.InvalidOperationException);
                    else Assert.That(() => ProfileNewGameConfirmationV1.ReserveExecutionCommit(request, fixture.Adapter, fixture.Router, owner, epoch, 1, 1), Throws.InvalidOperationException);
                }
                finally { SetField(target, fieldName, saved); }
                Assert.That(() => ProfileNewGameConfirmationV1.ReserveExecutionCommit(request, fixture.Adapter, fixture.Router, owner, epoch, 1, 1), Throws.InvalidOperationException, "repair cannot resurrect the request");
                if (afterReserve) Assert.That(() => ProfileNewGameConfirmationV1.CompleteExecutionCommit(request, reservation, fixture.Adapter, fixture.Router, owner, epoch, 1, 1), Throws.InvalidOperationException);
            }
        }

        [TestCase("Primary", "issue", false)] [TestCase("Previous", "issue", false)] [TestCase("Temp", "issue", false)]
        [TestCase("Primary", "reserve", false)] [TestCase("Previous", "reserve", false)] [TestCase("Temp", "reserve", false)]
        [TestCase("Primary", "complete", false)] [TestCase("Previous", "complete", false)] [TestCase("Temp", "complete", false)]
        [TestCase("Primary", "issue", true)] [TestCase("Previous", "issue", true)] [TestCase("Temp", "issue", true)]
        [TestCase("Primary", "reserve", true)] [TestCase("Previous", "reserve", true)] [TestCase("Temp", "reserve", true)]
        [TestCase("Primary", "complete", true)] [TestCase("Previous", "complete", true)] [TestCase("Temp", "complete", true)]
        public void AC004_EachLeafProjectionAndFingerprintRejectsAtEveryLowerBoundary(string role, string boundary, bool leafFingerprint)
        {
            using (var fixture = Fixture.Create())
            {
                var bytes = ProfileRecoveryPlannerV1.PlanDefaultBootstrap().ResultDocument.CanonicalFileBytes;
                foreach (var name in new[] { "profile.json", "profile.prev.json", "profile.tmp.json" }) File.WriteAllBytes(Path.Combine(fixture.Root, name), bytes);
                var owner = new object(); var epoch = new object();
                var displayed = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);
                var fresh = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);
                ConfirmedProfileResetRequestV1 request = null;
                ProfileNewGameExecutionCommitReservationV1 reservation = null;
                if (boundary != "issue") request = ProfileNewGameConfirmationV1.ResolveConfirmedRecapture(displayed, fresh, fixture.Adapter, fixture.Router, owner, epoch, 1, 1).ConfirmedRequest;
                if (boundary == "complete") reservation = ProfileNewGameConfirmationV1.ReserveExecutionCommit(request, fixture.Adapter, fixture.Router, owner, epoch, 1, 1);
                var observed = Field<object>(fresh, "_capture");
                var target = observed; var fieldName = role;
                if (leafFingerprint)
                {
                    var identity = Field<object>(observed, "Identity");
                    var leaves = Field<Array>(identity, "Leaves");
                    target = leaves.GetValue(role == "Primary" ? 0 : role == "Previous" ? 1 : 2); fieldName = "Hash";
                }
                var saved = Field<object>(target, fieldName);
                try
                {
                    SetField(target, fieldName, leafFingerprint ? new string('0', 64) : null);
                    if (boundary == "issue") Assert.That(() => ProfileNewGameConfirmationV1.ResolveConfirmedRecapture(displayed, fresh, fixture.Adapter, fixture.Router, owner, epoch, 1, 1), Throws.InvalidOperationException);
                    else if (boundary == "reserve") Assert.That(() => ProfileNewGameConfirmationV1.ReserveExecutionCommit(request, fixture.Adapter, fixture.Router, owner, epoch, 1, 1), Throws.InvalidOperationException);
                    else Assert.That(() => ProfileNewGameConfirmationV1.CompleteExecutionCommit(request, reservation, fixture.Adapter, fixture.Router, owner, epoch, 1, 1), Throws.InvalidOperationException);
                }
                finally { SetField(target, fieldName, saved); }
                if (boundary == "issue") Assert.That(ProfileNewGameConfirmationV1.ResolveConfirmedRecapture(displayed, fresh, fixture.Adapter, fixture.Router, owner, epoch, 1, 1).ConfirmedRequest, Is.Not.Null);
                else
                {
                    Assert.That(() => ProfileNewGameConfirmationV1.ReserveExecutionCommit(request, fixture.Adapter, fixture.Router, owner, epoch, 1, 1), Throws.InvalidOperationException);
                    if (reservation != null) Assert.That(() => ProfileNewGameConfirmationV1.CompleteExecutionCommit(request, reservation, fixture.Adapter, fixture.Router, owner, epoch, 1, 1), Throws.InvalidOperationException);
                }
            }
        }

        [Test]
        public void AC008_AuthenticatedRootDamageBeforeReserveIsTerminalAfterRepair()
        {
            using (var fixture = Fixture.Create())
            {
                var owner = new object(); var epoch = new object();
                var capture = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);
                var request = ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture, fixture.Adapter, fixture.Router, owner, epoch, 1, 1).ConfirmedRequest;
                var originalRoot = Field<object>(fixture.Adapter, "_launchRootWitnessA");
                try
                {
                    SetField(fixture.Adapter, "_launchRootWitnessA", Path.Combine(fixture.Root, "foreign"));
                    Assert.That(() => ProfileNewGameConfirmationV1.ReserveExecutionCommit(request, fixture.Adapter, fixture.Router, owner, epoch, 1, 1), Throws.InvalidOperationException);
                }
                finally { SetField(fixture.Adapter, "_launchRootWitnessA", originalRoot); }
                Assert.That(() => ProfileNewGameConfirmationV1.ReserveExecutionCommit(request, fixture.Adapter, fixture.Router, owner, epoch, 1, 1), Throws.InvalidOperationException);
            }
        }

        [TestCase("adapter")] [TestCase("router")] [TestCase("owner")] [TestCase("epoch")] [TestCase("interaction")] [TestCase("decision")]
        public void AC001_ForeignContextCannotConsumeOriginalReserveOrCompletion(string foreignKind)
        {
            using (var fixture = Fixture.Create())
            using (var foreign = Fixture.Create())
            {
                var owner = new object(); var epoch = new object();
                var capture = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);
                var request = ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture, fixture.Adapter, fixture.Router, owner, epoch, 1, 1).ConfirmedRequest;
                var adapter = foreignKind == "adapter" ? foreign.Adapter : fixture.Adapter;
                var router = foreignKind == "router" ? foreign.Router : fixture.Router;
                var suppliedOwner = foreignKind == "owner" ? new object() : owner;
                var suppliedEpoch = foreignKind == "epoch" ? new object() : epoch;
                var interaction = foreignKind == "interaction" ? 2 : 1;
                var decision = foreignKind == "decision" ? 2 : 1;
                Assert.That(() => ProfileNewGameConfirmationV1.ReserveExecutionCommit(request, adapter, router, suppliedOwner, suppliedEpoch, interaction, decision), Throws.InvalidOperationException);
                var reservation = ProfileNewGameConfirmationV1.ReserveExecutionCommit(request, fixture.Adapter, fixture.Router, owner, epoch, 1, 1);
                Assert.That(() => ProfileNewGameConfirmationV1.CompleteExecutionCommit(request, reservation, adapter, router, suppliedOwner, suppliedEpoch, interaction, decision), Throws.InvalidOperationException);
                Assert.That(ProfileNewGameConfirmationV1.CompleteExecutionCommit(request, reservation, fixture.Adapter, fixture.Router, owner, epoch, 1, 1), Is.SameAs(request));
                Assert.That(() => ProfileNewGameConfirmationV1.CloseExecutionCommit(request, fixture.Adapter, fixture.Router, owner, epoch, 1, 1), Throws.InvalidOperationException);
            }
        }

        [TestCase("_disposition")] [TestCase("_classification")] [TestCase("_request")]
        public void AC001_IssuedResolutionSingleFieldTamperCannotPublishRequest(string fieldName)
        {
            using (var fixture = Fixture.Create())
            {
                var owner = new object(); var epoch = new object();
                var capture = ProfileNewGameConfirmationV1.Capture(fixture.Adapter, fixture.Router, owner, epoch);
                var resolution = ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture, fixture.Adapter, fixture.Router, owner, epoch, 1, 1);
                var originalRequest = resolution.ConfirmedRequest;
                var saved = Field<object>(resolution, fieldName);
                object changed = fieldName == "_disposition" ? (object)ProfileNewGameConfirmedDispositionV1.FreshDecisionRequired : fieldName == "_classification" ? (object)NewGameObservationClassificationV1.ConfirmationRequiredAmbiguous : new ConfirmedProfileResetRequestV1();
                try
                {
                    SetField(resolution, fieldName, changed);
                    Assert.That(() => { var ignored = resolution.ConfirmedRequest; }, Throws.InvalidOperationException);
                }
                finally { SetField(resolution, fieldName, saved); }
                Assert.That(resolution.ConfirmedRequest, Is.SameAs(originalRequest));
            }
        }

        private static T Clone<T>(T value) where T : class
        { return (T)typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(value, null); }

        private static object Witness(string tableName, object key)
        {
            var table = typeof(ProfileNewGameConfirmationV1).GetField(tableName, BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            var arguments = new object[] { key, null };
            Assert.That(table.GetType().GetMethod("TryGetValue").Invoke(table, arguments), Is.EqualTo(true));
            return arguments[1];
        }

        [Test]
        public void AC001_MintedCaptureStateRollbackCannotIssueSecondRequest()
        {
            using (var fixture=Fixture.Create())
            {
                var owner=new object(); var epoch=new object();
                var capture=ProfileNewGameConfirmationV1.Capture(fixture.Adapter,fixture.Router,owner,epoch);
                var request=ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture,fixture.Adapter,fixture.Router,owner,epoch,1,1).ConfirmedRequest;
                var witness=Witness("IssuedResults",capture);
                SetField(witness,"RequestMintState",0);
                Assert.That(() => ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture,fixture.Adapter,fixture.Router,owner,epoch,1,1),Throws.InvalidOperationException);
                SetField(witness,"RequestMintState",1);
                Assert.That(() => ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture,fixture.Adapter,fixture.Router,owner,epoch,1,1),Throws.InvalidOperationException);
                var reservation=ProfileNewGameConfirmationV1.ReserveExecutionCommit(request,fixture.Adapter,fixture.Router,owner,epoch,1,1);
                Assert.That(ProfileNewGameConfirmationV1.CompleteExecutionCommit(request,reservation,fixture.Adapter,fixture.Router,owner,epoch,1,1),Is.SameAs(request));
            }
        }

        [Test]
        public void AC001_FailedMintStateRollbackCannotRetrySameCapture()
        {
            using (var fixture=Fixture.Create())
            {
                var owner=new object(); var epoch=new object();
                var capture=ProfileNewGameConfirmationV1.Capture(fixture.Adapter,fixture.Router,owner,epoch);
                var table=typeof(ProfileNewGameConfirmationV1).GetField("IssuedResolutions",BindingFlags.Static|BindingFlags.NonPublic);
                var saved=table.GetValue(null);
                try
                {
                    // Actual resolution registration fails after the request was registered.
                    table.SetValue(null,null);
                    Assert.That(() => ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture,fixture.Adapter,fixture.Router,owner,epoch,1,1),Throws.TypeOf<NullReferenceException>());
                }
                finally { table.SetValue(null,saved); }
                var witness=Witness("IssuedResults",capture);
                var lifecycle=Witness("MintLifecycles",witness);
                var history=Field<object>(lifecycle,"ClosedHistory");
                var partialRequest=Field<ConfirmedProfileResetRequestV1>(history,"Request");
                var partialWitness=Field<object>(history,"RequestWitness");
                Assert.That(Witness("IssuedConfirmedRequests",partialRequest),Is.SameAs(partialWitness));
                Assert.That(Field<object>(Field<object>(lifecycle,"Head"),"Reservation"),Is.Null,"prebuilt close manufactures no future pair");
                var requestLifecycle=Witness("RequestLifecycles",partialWitness);
                Assert.That(Field<object>(requestLifecycle,"Head"),Is.SameAs(Field<object>(requestLifecycle,"FailureClosed")));
                SetField(witness,"RequestMintState",0);
                Assert.That(() => ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture,fixture.Adapter,fixture.Router,owner,epoch,1,1),Throws.InvalidOperationException);
                Assert.That(() => ProfileNewGameConfirmationV1.ReserveExecutionCommit(partialRequest,fixture.Adapter,fixture.Router,owner,epoch,1,1),Throws.InvalidOperationException);
            }
        }

        [Test]
        public void AC008_ReservedStateRollbackCannotReserveAgain()
        {
            using (var fixture=Fixture.Create())
            {
                var owner=new object(); var epoch=new object();
                var capture=ProfileNewGameConfirmationV1.Capture(fixture.Adapter,fixture.Router,owner,epoch);
                var request=ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture,fixture.Adapter,fixture.Router,owner,epoch,1,1).ConfirmedRequest;
                var reservation=ProfileNewGameConfirmationV1.ReserveExecutionCommit(request,fixture.Adapter,fixture.Router,owner,epoch,1,1);
                var witness=Witness("IssuedConfirmedRequests",request);
                SetField(witness,"State",0);
                Assert.That(() => ProfileNewGameConfirmationV1.ReserveExecutionCommit(request,fixture.Adapter,fixture.Router,owner,epoch,1,1),Throws.InvalidOperationException);
                SetField(witness,"State",1);
                Assert.That(() => ProfileNewGameConfirmationV1.CompleteExecutionCommit(request,reservation,fixture.Adapter,fixture.Router,owner,epoch,1,1),Throws.InvalidOperationException);
            }
        }

        [TestCase(0)] [TestCase(1)]
        public void AC008_CompletedStateRollbackCannotReserveOrCompleteAgain(int rollback)
        {
            using (var fixture=Fixture.Create())
            {
                var owner=new object(); var epoch=new object();
                var capture=ProfileNewGameConfirmationV1.Capture(fixture.Adapter,fixture.Router,owner,epoch);
                var request=ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture,fixture.Adapter,fixture.Router,owner,epoch,1,1).ConfirmedRequest;
                var reservation=ProfileNewGameConfirmationV1.ReserveExecutionCommit(request,fixture.Adapter,fixture.Router,owner,epoch,1,1);
                ProfileNewGameConfirmationV1.CompleteExecutionCommit(request,reservation,fixture.Adapter,fixture.Router,owner,epoch,1,1);
                var witness=Witness("IssuedConfirmedRequests",request); var lifecycle=Witness("RequestLifecycles",witness);
                var completed=Field<object>(lifecycle,"Head");
                SetField(witness,"State",rollback);
                Assert.That(() => ProfileNewGameConfirmationV1.ReserveExecutionCommit(request,fixture.Adapter,fixture.Router,owner,epoch,1,1),Throws.InvalidOperationException);
                Assert.That(() => ProfileNewGameConfirmationV1.CompleteExecutionCommit(request,reservation,fixture.Adapter,fixture.Router,owner,epoch,1,1),Throws.InvalidOperationException);
                SetField(witness,"State",2);
                Assert.That(() => ProfileNewGameConfirmationV1.CompleteExecutionCommit(request,reservation,fixture.Adapter,fixture.Router,owner,epoch,1,1),Throws.InvalidOperationException);
                Assert.That(Field<object>(lifecycle,"Head"),Is.SameAs(completed));
            }
        }

        [TestCase(false,0)] [TestCase(false,1)] [TestCase(true,0)] [TestCase(true,1)]
        public void AC008_ClosedStateRollbackCannotReserveOrComplete(bool reserved, int rollback)
        {
            using (var fixture=Fixture.Create())
            {
                var owner=new object(); var epoch=new object();
                var capture=ProfileNewGameConfirmationV1.Capture(fixture.Adapter,fixture.Router,owner,epoch);
                var request=ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture,fixture.Adapter,fixture.Router,owner,epoch,1,1).ConfirmedRequest;
                var reservation=reserved ? ProfileNewGameConfirmationV1.ReserveExecutionCommit(request,fixture.Adapter,fixture.Router,owner,epoch,1,1) : null;
                ProfileNewGameConfirmationV1.CloseExecutionCommit(request,fixture.Adapter,fixture.Router,owner,epoch,1,1);
                var witness=Witness("IssuedConfirmedRequests",request); var lifecycle=Witness("RequestLifecycles",witness);
                var closed=Field<object>(lifecycle,"Head");
                SetField(witness,"State",rollback);
                Assert.That(() => ProfileNewGameConfirmationV1.ReserveExecutionCommit(request,fixture.Adapter,fixture.Router,owner,epoch,1,1),Throws.InvalidOperationException);
                Assert.That(() => ProfileNewGameConfirmationV1.CompleteExecutionCommit(request,reservation,fixture.Adapter,fixture.Router,owner,epoch,1,1),Throws.InvalidOperationException);
                SetField(witness,"State",3);
                Assert.That(() => ProfileNewGameConfirmationV1.ReserveExecutionCommit(request,fixture.Adapter,fixture.Router,owner,epoch,1,1),Throws.InvalidOperationException);
                Assert.That(Field<object>(lifecycle,"Head"),Is.SameAs(closed));
            }
        }

        [TestCase(false)] [TestCase(true)]
        public void AC008_SingleStateInvalidValueClosesWithoutRepairResurrection(bool reserved)
        {
            using (var fixture=Fixture.Create())
            {
                var owner=new object(); var epoch=new object();
                var capture=ProfileNewGameConfirmationV1.Capture(fixture.Adapter,fixture.Router,owner,epoch);
                var request=ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture,fixture.Adapter,fixture.Router,owner,epoch,1,1).ConfirmedRequest;
                var reservation=reserved ? ProfileNewGameConfirmationV1.ReserveExecutionCommit(request,fixture.Adapter,fixture.Router,owner,epoch,1,1) : null;
                var witness=Witness("IssuedConfirmedRequests",request);
                SetField(witness,"State",99);
                Assert.That(() => ProfileNewGameConfirmationV1.ReserveExecutionCommit(request,fixture.Adapter,fixture.Router,owner,epoch,1,1),Throws.InvalidOperationException);
                SetField(witness,"State",reserved ? 1 : 0);
                Assert.That(() => ProfileNewGameConfirmationV1.ReserveExecutionCommit(request,fixture.Adapter,fixture.Router,owner,epoch,1,1),Throws.InvalidOperationException);
                if (reserved) Assert.That(() => ProfileNewGameConfirmationV1.CompleteExecutionCommit(request,reservation,fixture.Adapter,fixture.Router,owner,epoch,1,1),Throws.InvalidOperationException);
            }
        }

        [Test]
        public void AC008_IntermediateRegistrationFailureLeavesTerminalActualPair()
        {
            using (var fixture=Fixture.Create())
            {
                var owner=new object(); var epoch=new object();
                var capture=ProfileNewGameConfirmationV1.Capture(fixture.Adapter,fixture.Router,owner,epoch);
                var request=ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture,fixture.Adapter,fixture.Router,owner,epoch,1,1).ConfirmedRequest;
                var table=typeof(ProfileNewGameConfirmationV1).GetField("ReservationLifecycles",BindingFlags.Static|BindingFlags.NonPublic);
                var saved=table.GetValue(null);
                try
                {
                    table.SetValue(null,null);
                    Assert.That(() => ProfileNewGameConfirmationV1.ReserveExecutionCommit(request,fixture.Adapter,fixture.Router,owner,epoch,1,1),Throws.TypeOf<NullReferenceException>());
                }
                finally { table.SetValue(null,saved); }
                var witness=Witness("IssuedConfirmedRequests",request); var lifecycle=Witness("RequestLifecycles",witness);
                var history=Field<object>(lifecycle,"ClosedHistory");
                var reservation=Field<ProfileNewGameExecutionCommitReservationV1>(history,"Reservation");
                Assert.That(Witness("IssuedReservations",reservation),Is.SameAs(Field<object>(history,"ReservationWitness")),"actual first registration survives second registration failure");
                Assert.That(Field<object>(Field<object>(lifecycle,"Head"),"Reservation"),Is.Null);
                SetField(witness,"State",1);
                Assert.That(() => ProfileNewGameConfirmationV1.CompleteExecutionCommit(request,reservation,fixture.Adapter,fixture.Router,owner,epoch,1,1),Throws.InvalidOperationException);
                SetField(witness,"State",0);
                Assert.That(() => ProfileNewGameConfirmationV1.ReserveExecutionCommit(request,fixture.Adapter,fixture.Router,owner,epoch,1,1),Throws.InvalidOperationException);
            }
        }

        [Test]
        public void AC001_ConcurrentMintHasOneWinnerAndLoserDoesNotCloseWinner()
        {
            using (var fixture=Fixture.Create())
            {
                var owner=new object(); var epoch=new object();
                var capture=ProfileNewGameConfirmationV1.Capture(fixture.Adapter,fixture.Router,owner,epoch);
                var lifecycle=Witness("MintLifecycles",Witness("IssuedResults",capture));
                ConfirmedProfileResetRequestV1 request=null;
                ConcurrentLoser(Field<object>(lifecycle,"Gate"),
                    () => ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture,fixture.Adapter,fixture.Router,owner,epoch,1,1),
                    () => { request=ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture,fixture.Adapter,fixture.Router,owner,epoch,1,1).ConfirmedRequest; });
                var reservation=ProfileNewGameConfirmationV1.ReserveExecutionCommit(request,fixture.Adapter,fixture.Router,owner,epoch,1,1);
                Assert.That(ProfileNewGameConfirmationV1.CompleteExecutionCommit(request,reservation,fixture.Adapter,fixture.Router,owner,epoch,1,1),Is.SameAs(request));
            }
        }

        [Test]
        public void AC008_ConcurrentReserveAndCompleteHaveOneWinnerAndPreserveHistory()
        {
            using (var fixture=Fixture.Create())
            {
                var owner=new object(); var epoch=new object();
                var capture=ProfileNewGameConfirmationV1.Capture(fixture.Adapter,fixture.Router,owner,epoch);
                var request=ProfileNewGameConfirmationV1.IssueNoConfirmationRequired(capture,fixture.Adapter,fixture.Router,owner,epoch,1,1).ConfirmedRequest;
                var lifecycle=Witness("RequestLifecycles",Witness("IssuedConfirmedRequests",request));
                var gate=Field<object>(lifecycle,"Gate");
                ProfileNewGameExecutionCommitReservationV1 reservation=null;
                ConcurrentLoser(gate,
                    () => ProfileNewGameConfirmationV1.ReserveExecutionCommit(request,fixture.Adapter,fixture.Router,owner,epoch,1,1),
                    () => { reservation=ProfileNewGameConfirmationV1.ReserveExecutionCommit(request,fixture.Adapter,fixture.Router,owner,epoch,1,1); });
                ConcurrentLoser(gate,
                    () => ProfileNewGameConfirmationV1.CompleteExecutionCommit(request,reservation,fixture.Adapter,fixture.Router,owner,epoch,1,1),
                    () => { Assert.That(ProfileNewGameConfirmationV1.CompleteExecutionCommit(request,reservation,fixture.Adapter,fixture.Router,owner,epoch,1,1),Is.SameAs(request)); });
                var completed=Field<object>(lifecycle,"Head");
                Assert.That(() => ProfileNewGameConfirmationV1.CloseExecutionCommit(request,fixture.Adapter,fixture.Router,owner,epoch,1,1),Throws.InvalidOperationException);
                Assert.That(Field<object>(lifecycle,"Head"),Is.SameAs(completed));
            }
        }

        private static void ConcurrentLoser(object gate, Action loserCall, Action mainThreadWinner)
        {
            // Unity's active-object inspection remains on the main thread. The
            // genuine competing API invocation sees a completed lock transition.
            Exception loserError=null;
            using (var started=new System.Threading.ManualResetEventSlim(false))
            {
                var worker=new System.Threading.Thread(() => { started.Set(); try { loserCall(); } catch (Exception error) { loserError=error; } });
                worker.IsBackground=true;
                try
                {
                    lock (gate)
                    {
                        worker.Start();
                        Assert.That(started.Wait(TimeSpan.FromSeconds(10)),Is.True,"competing worker started");
                        Assert.That(System.Threading.SpinWait.SpinUntil(() => (worker.ThreadState & System.Threading.ThreadState.WaitSleepJoin) != 0,TimeSpan.FromSeconds(10)),Is.True,"competing API reached the held lifecycle gate");
                        mainThreadWinner();
                    }
                }
                finally
                {
                    // Gate is released even if the winner assertion fails.
                    Assert.That(worker.Join(TimeSpan.FromSeconds(10)),Is.True,"competing worker collected");
                }
                Assert.That(loserError,Is.TypeOf<InvalidOperationException>(),"the loser rejects rather than closing or publishing authority");
            }
        }

        private static T Field<T>(object target, string name)
        {
            return (T)target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(target);
        }

        private static void SetField(object target, string name, object value)
        {
            target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(target, value);
        }

        private static void Invoke(object target, string name)
        {
            target.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(target, null);
        }

        private sealed class Fixture : IDisposable
        {
            internal readonly string Root;
            internal readonly GameObject Host;
            internal readonly InputRouter Router;
            internal readonly DesktopProfileLaunchAdapterV1 Adapter;

            private Fixture(string root, GameObject host, InputRouter router, DesktopProfileLaunchAdapterV1 adapter)
            {
                Root = root; Host = host; Router = router; Adapter = adapter;
            }

            internal static Fixture Create()
            {
                var root = Path.Combine(Path.GetTempPath(), "AcadeGameMaker-C3-confirmation-" + Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(root);
                var host = new GameObject("c3-confirmation");
                host.SetActive(false);
                var routerObject = new GameObject("router");
                routerObject.transform.SetParent(host.transform);
                var adapterObject = new GameObject("adapter");
                adapterObject.transform.SetParent(host.transform);
                var router = routerObject.AddComponent<InputRouter>();
                router.ConfigureHubUiOnlyForAuthoring();
                var adapter = adapterObject.AddComponent<DesktopProfileLaunchAdapterV1>();
                adapter.ConfigureForTests(router, new EnvironmentPort(root), new DesktopProfileLaunchPreparationPortV1());
                host.SetActive(true);
                if (!router.PreparedLaunchAwakeEntered) Invoke(router, "Awake");
                if (!(bool)typeof(DesktopProfileLaunchAdapterV1).GetField("_awakeAttempted", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(adapter)) Invoke(adapter, "Awake");
                Invoke(adapter, "Start");
                Assert.That(adapter.CurrentReceipt.HasValue, Is.True, "actual launch receipt");
                return new Fixture(root, host, router, adapter);
            }

            public void Dispose()
            {
                UnityEngine.Object.DestroyImmediate(Host);
                var root = Path.GetFullPath(Root);
                var temp = Path.GetFullPath(Path.GetTempPath());
                Assert.That(root.StartsWith(temp, StringComparison.OrdinalIgnoreCase) && Path.GetFileName(root).StartsWith("AcadeGameMaker-C3-confirmation-", StringComparison.Ordinal) && !File.GetAttributes(root).HasFlag(FileAttributes.ReparsePoint), Is.True, "owned non-reparse temporary root");
                if (Directory.Exists(root)) Directory.Delete(root, true);
            }
        }

        private sealed class EnvironmentPort : IDesktopProfileLaunchEnvironmentV1
        {
            private readonly string _root;
            internal EnvironmentPort(string root) { _root = root; }
            public string GetPersistentDataPath() { return _root; }
            public DateTimeOffset GetUtcNow() { return DateTimeOffset.UtcNow; }
        }
    }
}
