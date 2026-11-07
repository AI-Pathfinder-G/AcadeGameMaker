using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using AcadeGameMaker.Costumes;
using AcadeGameMaker.Costumes.IO;
using AcadeGameMaker.Presentation;
using AcadeGameMaker.Presentation.Unity;
using NUnit.Framework;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.PresentationUnity
{
    // AC-CUA-001/007/008: isolated fixture; no scene, global lookup, real media, or live renderer.
    public sealed class CostumeUnityPresentationAdapterV1Tests
    {
        private static readonly string[] CurrentTupleCorruptions = { "bindingActor", "mediaActor", "snapshotActor", "stateAbsent", "stateDifferent", "bindingCostume", "portraitSet", "atlasSet", "catalogRevision", "presentationRevision", "portraitHash", "atlasHash", "clipMapHash", "missingAction", "extraAction", "frameX", "frameY", "frameWidth", "frameHeight", "framePivotX", "framePivotY", "frameBaseline", "frameDuration" };
        private static readonly string[] NonJsonValidationMutations = { "definitionActor", "definitionCostume", "definitionPortraitSet", "definitionGameplaySet", "definitionPending", "definitionRejected", "syntheticFalse", "packageCatalogRevision", "packagePresentationRevision", "bindingActor", "bindingCostume", "bindingPortraitSet", "bindingAtlasSet", "bindingCatalogRevision", "bindingPresentationRevision", "bindingPortraitHash", "bindingAtlasHash", "bindingClipHash", "bindingMissingAction", "bindingExtraAction", "bindingEmpty" };
        private static readonly string[] R4B1b1Mutations = { "portraitBytes", "atlasBytes", "clipMapBytes", "nullPortrait", "nullAtlas", "destroyedPortraitBeforeValidate", "destroyedAtlasBeforeValidate", "destroyedPortraitBeforeTrySelect", "destroyedAtlasBeforeTrySelect", "portraitFilterDrift", "atlasFilterDrift", "ppuMismatch" };
        private static readonly string[] R4B1b2ConstructorMutations = { "negativeX", "negativeY", "widthZero", "widthNegative", "heightZero", "heightNegative", "durationZero", "durationNegative" };
        private static readonly string[] R4B1b2RejectedMutations = { "ordinaryXOverrun", "ordinaryYOverrun", "rightOverrun", "topOverrun", "xInt32MaxValueWidth1", "xInt32MaxMinusOneWidthInt32MaxValue", "yInt32MaxValueHeight1", "yInt32MaxMinusOneHeightInt32MaxValue", "pivotXMismatch", "pivotYMismatch", "baselineMismatch", "withinClipAdjacentDuplicate", "withinClipNonAdjacentDuplicate" };
        private static readonly string[] R4B1b2AcceptedMutations = { "exactRightTopEdge", "crossClipRectangleReuse" };
        private static readonly string[] R4B2ClipMapByteMutations = { "utf8Bom", "leadingWhitespace", "trailingWhitespace", "interiorWhitespace", "reorderSchemaActor", "reorderActorCostume", "reorderPpuCell", "unknownProperty", "duplicateProperty", "schemaVersion", "actorId", "costumeId", "gameplaySetId", "ppu", "cellWidth", "cellHeight", "pivotX", "pivotY", "baseline", "clipsUnsorted", "duplicateActionFacing", "missingAction", "missingLeft", "missingRight", "unknownFacing", "frameOrderSwap", "frameX", "frameY", "frameWidth", "frameHeight", "framePivotX", "framePivotY", "frameBaseline", "frameDuration" };
        private static readonly string[] R4B3SourceManifestByteMutations = { "utf8Bom", "leadingWhitespace", "trailingWhitespace", "interiorWhitespace", "reorderActorCostume", "reorderPortraitGameplaySet", "reorderCatalogPresentationRevision", "unknownProperty", "duplicateProperty", "actorId", "costumeId", "portraitSetId", "gameplaySetId", "catalogRevision", "presentationRevision", "portraitHash", "atlasHash", "clipMapHash", "requiredActionIdsMissing", "requiredActionIdsExtra", "requiredActionIdsUnsorted", "requiredActionIdsDuplicate", "requiredFacingIdsMissingLeft", "requiredFacingIdsMissingRight", "requiredFacingIdsUnsorted", "requiredFacingIdsExtra", "requiredFacingIdsDuplicate", "clipMapSchemaVersion" };
        private static readonly string[] R4B4Cases = { "clipPropertyOrder", "framePropertyOrder", "foreignPackage", "stalePackage", "previouslyRejectedCandidate", "missingSnapshotAction" };
        public static IEnumerable<TestCaseData> NonJsonValidationCases() { foreach (var value in NonJsonValidationMutations) yield return new TestCaseData(value).SetName("AC_CUA_002_NonJson_" + value); }
        public static IEnumerable<TestCaseData> R4B1b1Cases() { foreach (var value in R4B1b1Mutations) yield return new TestCaseData(value).SetName("AC_CUA_002_R4_B1b1_" + value); }
        public static IEnumerable<TestCaseData> R4B1b2ConstructorCases() { foreach (var value in R4B1b2ConstructorMutations) yield return new TestCaseData(value).SetName("AC_CUA_002_R4_B1b2_" + value); }
        public static IEnumerable<TestCaseData> R4B1b2RejectedCases() { foreach (var value in R4B1b2RejectedMutations) yield return new TestCaseData(value).SetName("AC_CUA_002_R4_B1b2_" + value); }
        public static IEnumerable<TestCaseData> R4B1b2AcceptedCases() { foreach (var value in R4B1b2AcceptedMutations) yield return new TestCaseData(value).SetName("AC_CUA_002_R4_B1b2_" + value); }
        public static IEnumerable<TestCaseData> R4B2ClipMapByteCases() { foreach (var value in R4B2ClipMapByteMutations) yield return new TestCaseData(value).SetName("AC_CUA_002_R4_B2_" + value); }
        public static IEnumerable<TestCaseData> R4B3SourceManifestByteCases() { foreach (var value in R4B3SourceManifestByteMutations) yield return new TestCaseData(value).SetName("AC_CUA_002_R4_B3_" + value); }
        public static IEnumerable<TestCaseData> R4B4CasesSource() { foreach (var value in R4B4Cases) yield return new TestCaseData(value).SetName("AC_CUA_002_R4_B4_" + value); }
        [TestCaseSource(nameof(NonJsonValidationCases))]
        public void AC_CUA_002_ConstructibleNonJsonMutationRejectsWithoutTouchingPublishedPair(string mutation)
        {
            using (var fixture = AcceptedFixture.Create())
            {
                var port = new MemoryPort(); var projection = new Projection(); var adapter = PublishedAdapter(fixture, port, projection); var oldPublished = adapter.Published; var oldMedia = oldPublished.Media; var oldBinding = oldPublished.Binding;
                Assert.That(adapter.Highlight(fixture.PackageTwo.CostumeId), Is.EqualTo(CostumeUnityAdapterRejectionV1.HighlightOnly));
                var beforeView = DeepViewSnapshot.Capture(adapter.SnapshotView()); var replaceCalls = port.ReplaceCalls;
                var definition = fixture.Catalog.Find(fixture.PackageTwo.CostumeId); var binding = new CostumePresentationBindingV1("fixture", fixture.PackageTwo.CostumeId, "fixture.portrait.v1", definition.PortraitHash, "fixture.gameplay.v1", definition.AtlasHash, definition.ClipMapHash, 5, 9, ActionIds()); var media = fixture.PackageTwo;
                if (mutation == "definitionPending" || mutation == "definitionRejected") definition = new CostumeDefinitionV1(definition.ActorId, definition.CostumeId, "synthetic", new string('a',64), "synthetic", mutation == "definitionPending" ? CostumeDisposition.ProductionPending : CostumeDisposition.Rejected, false, null, null, null, null, null, definition.PresentationRevision);
                else if (mutation == "definitionActor") definition = new CostumeDefinitionV1("other", "other.costume.synthetic.v1", "synthetic", new string('a',64), "synthetic", CostumeDisposition.Accepted, true, definition.PortraitSetId, definition.GameplaySetId, definition.PortraitHash, definition.AtlasHash, definition.ClipMapHash, definition.PresentationRevision);
                else if (mutation == "definitionCostume") definition = new CostumeDefinitionV1("fixture", "fixture.costume.other.v1", "synthetic", new string('a',64), "synthetic", CostumeDisposition.Accepted, true, definition.PortraitSetId, definition.GameplaySetId, definition.PortraitHash, definition.AtlasHash, definition.ClipMapHash, definition.PresentationRevision);
                else if (mutation == "definitionPortraitSet" || mutation == "definitionGameplaySet") definition = new CostumeDefinitionV1(definition.ActorId, definition.CostumeId, "synthetic", new string('a',64), "synthetic", CostumeDisposition.Accepted, true, mutation == "definitionPortraitSet" ? "other.portrait.v1" : definition.PortraitSetId, mutation == "definitionGameplaySet" ? "other.gameplay.v1" : definition.GameplaySetId, definition.PortraitHash, definition.AtlasHash, definition.ClipMapHash, definition.PresentationRevision);
                else if (mutation == "syntheticFalse") typeof(CostumeUnityMediaPackageV1).GetField("<Synthetic>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(media, false);
                else if (mutation == "packageCatalogRevision") typeof(CostumeUnityMediaPackageV1).GetField("<CatalogRevision>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(media, media.CatalogRevision + 1);
                else if (mutation == "packagePresentationRevision") typeof(CostumeUnityMediaPackageV1).GetField("<PresentationRevision>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(media, media.PresentationRevision + 1);
                else { binding = MutateBinding(binding, mutation); if (!IsBindingOnlyNonJsonMutation(mutation)) definition = DefinitionFromBinding(binding, definition.Disposition); }
                var ExpectedValidationRejection = ExpectedValidation(mutation);
                Assert.That(media.Validate(definition, binding), Is.EqualTo(IsCatalogIdentityNonJsonMutation(mutation) ? CostumeUnityPackageRejectionV1.Identity : ExpectedValidationRejection), mutation);

                // Binding revision/clip variants are validator-only: no binding object may enter the adapter surface.
                if (IsBindingOnlyNonJsonMutation(mutation))
                {
                    AssertNoPublicBindingIngress();
                    AssertNonJsonPreserved(adapter, port, projection, oldPublished, oldMedia, oldBinding, beforeView, replaceCalls);
                    return;
                }

                // Actor/costume binding rows prove direct validation first, then project the resulting definition into
                // the catalog. The adapter still receives media only, so Selection is the catalog-identity boundary.
                ReplaceDefinition(fixture.Catalog, fixture.PackageTwo.CostumeId, definition);
                var postInjectionView = DeepViewSnapshot.Capture(adapter.SnapshotView());
                Assert.That(adapter.TrySelect(media), Is.EqualTo(IsCatalogIdentityNonJsonMutation(mutation) ? CostumeUnityAdapterRejectionV1.Selection : ExpectedAdapter(mutation)), mutation);
                AssertNonJsonPreserved(adapter, port, projection, oldPublished, oldMedia, oldBinding, postInjectionView, replaceCalls);
            }
        }

        // AC-CUA-002/003: R4-B1b1 independently exercises payload, reference, import, and PPU boundaries.
        // Constructor-only admission is not treated as validation evidence; each row reaches Validate and, where
        // selection is admissible, the concrete adapter boundary with the B1a old-pair preservation oracle.
        [TestCaseSource(nameof(R4B1b1Cases))]
        public void AC_CUA_002_R4_B1b1_MediaMutationRejectsAtItsNamedBoundary(string mutation)
        {
            using (var fixture = AcceptedFixture.Create())
            {
                var portrait = new Texture2D(2, 2, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
                var atlas = new Texture2D(4, 2, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
                try
                {
                    var definition = fixture.Catalog.Find(fixture.PackageTwo.CostumeId);
                    var ppu = CostumeUnityMediaPackageV1.SyntheticApprovedPpu;
                    if (mutation == "ppuMismatch")
                    {
                        ppu++;
                        definition = CreateB1b1Definition(fixture.PackageTwo.CostumeId, portrait, atlas, ppu);
                        ReplaceDefinition(fixture.Catalog, fixture.PackageTwo.CostumeId, definition);
                    }

                    var portraitBytes = mutation == "portraitBytes" ? MutateBytes(new byte[] { 1, 2 }) : null;
                    var atlasBytes = mutation == "atlasBytes" ? MutateBytes(new byte[] { 3, 4 }) : null;
                    byte[] clipMapBytes = null;
                    var candidatePortrait = mutation == "nullPortrait" ? null : portrait;
                    var candidateAtlas = mutation == "nullAtlas" ? null : atlas;
                    var candidate = CreateB1b1Package(definition, candidatePortrait, candidateAtlas, atlas, ppu, portraitBytes, atlasBytes, clipMapBytes);
                    if (mutation == "clipMapBytes")
                    {
                        clipMapBytes = MutateBytes(candidate.ClipMapBytes);
                        candidate = CreateB1b1Package(definition, candidatePortrait, candidateAtlas, atlas, ppu, null, null, clipMapBytes);
                    }
                    if (mutation == "portraitFilterDrift") portrait.filterMode = FilterMode.Bilinear;
                    if (mutation == "atlasFilterDrift") atlas.filterMode = FilterMode.Bilinear;
                    if (mutation == "destroyedPortraitBeforeValidate" || mutation == "destroyedPortraitBeforeTrySelect") UnityEngine.Object.DestroyImmediate(portrait);
                    if (mutation == "destroyedAtlasBeforeValidate" || mutation == "destroyedAtlasBeforeTrySelect") UnityEngine.Object.DestroyImmediate(atlas);

                    var binding = BindingFor(definition);
                    Assert.That(candidate.Validate(definition, binding), Is.EqualTo(ExpectedB1b1Validation(mutation)), mutation);
                    if (mutation == "destroyedPortraitBeforeValidate" || mutation == "destroyedAtlasBeforeValidate") return;

                    var port = new MemoryPort();
                    var projection = new Projection();
                    var adapter = PublishedAdapter(fixture, port, projection);
                    Assert.That(adapter.Highlight(fixture.PackageTwo.CostumeId), Is.EqualTo(CostumeUnityAdapterRejectionV1.HighlightOnly));
                    var oldPublished = adapter.Published;
                    var oldMedia = oldPublished.Media;
                    var oldBinding = oldPublished.Binding;
                    var beforeView = DeepViewSnapshot.Capture(adapter.SnapshotView());
                    var moves = port.MoveCalls;
                    var replaces = port.ReplaceCalls;
                    var projections = projection.Count;
                    var status = adapter.PersistenceStatus;

                    Assert.That(adapter.TrySelect(candidate), Is.EqualTo(CostumeUnityAdapterRejectionV1.Package), mutation);
                    AssertB1aPublishedPairPreserved(adapter, port, projection, oldPublished, oldMedia, oldBinding, beforeView, moves, replaces, projections, status);
                }
                finally
                {
                    if (portrait != null) UnityEngine.Object.DestroyImmediate(portrait);
                    if (atlas != null) UnityEngine.Object.DestroyImmediate(atlas);
                }
            }
        }

        // AC-CUA-002/003: R4-B1b2 distinguishes frame-constructor bounds from package geometry validation.
        [TestCaseSource(nameof(R4B1b2ConstructorCases))]
        public void AC_CUA_002_R4_B1b2_FrameConstructorRejectsInvalidScalar(string mutation)
        {
            var x = 0; var y = 0; var width = 1; var height = 1; var duration = 1;
            if (mutation == "negativeX") x = -1;
            else if (mutation == "negativeY") y = -1;
            else if (mutation == "widthZero") width = 0;
            else if (mutation == "widthNegative") width = -1;
            else if (mutation == "heightZero") height = 0;
            else if (mutation == "heightNegative") height = -1;
            else if (mutation == "durationZero") duration = 0;
            else if (mutation == "durationNegative") duration = -1;
            Assert.Throws<ArgumentOutOfRangeException>(() => new CostumeUnityFrameV1(x, y, width, height, 500, 0, 0, duration), mutation);
        }

        [TestCaseSource(nameof(R4B1b2RejectedCases))]
        public void AC_CUA_002_R4_B1b2_ConstructibleGeometryOrDuplicateRejectsWithoutTouchingPublishedPair(string mutation)
        {
            using (var fixture = AcceptedFixture.Create())
            {
                var portrait = new Texture2D(2, 2, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
                var atlas = new Texture2D(4, 2, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
                try
                {
                    CostumeDefinitionV1 definition;
                    var candidate = CreateB1b2Package(fixture.PackageTwo.CostumeId, portrait, atlas, B1b2Clips(mutation), out definition);
                    var binding = BindingFor(definition);
                    Assert.That(candidate.Validate(definition, binding), Is.EqualTo(ExpectedB1b2Validation(mutation)), mutation);
                    ReplaceDefinition(fixture.Catalog, fixture.PackageTwo.CostumeId, definition);

                    var port = new MemoryPort(); var projection = new Projection(); var adapter = PublishedAdapter(fixture, port, projection);
                    Assert.That(adapter.Highlight(fixture.PackageTwo.CostumeId), Is.EqualTo(CostumeUnityAdapterRejectionV1.HighlightOnly));
                    var oldPublished = adapter.Published; var oldMedia = oldPublished.Media; var oldBinding = oldPublished.Binding;
                    var beforeView = DeepViewSnapshot.Capture(adapter.SnapshotView());
                    var moves = port.MoveCalls; var replaces = port.ReplaceCalls; var projections = projection.Count; var status = adapter.PersistenceStatus;
                    Assert.That(adapter.TrySelect(candidate), Is.EqualTo(CostumeUnityAdapterRejectionV1.Package), mutation);
                    AssertB1aPublishedPairPreserved(adapter, port, projection, oldPublished, oldMedia, oldBinding, beforeView, moves, replaces, projections, status);
                }
                finally { UnityEngine.Object.DestroyImmediate(portrait); UnityEngine.Object.DestroyImmediate(atlas); }
            }
        }

        [TestCaseSource(nameof(R4B1b2AcceptedCases))]
        public void AC_CUA_002_R4_B1b2_ValidEdgeOrCrossClipReusePublishesActualReplacement(string mutation)
        {
            using (var fixture = AcceptedFixture.Create())
            {
                var portrait = new Texture2D(2, 2, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
                var atlas = new Texture2D(4, 2, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
                try
                {
                    CostumeDefinitionV1 definition;
                    var candidate = CreateB1b2Package(fixture.PackageTwo.CostumeId, portrait, atlas, B1b2Clips(mutation), out definition);
                    Assert.That(candidate.Validate(definition, BindingFor(definition)), Is.EqualTo(CostumeUnityPackageRejectionV1.None), mutation);
                    ReplaceDefinition(fixture.Catalog, fixture.PackageTwo.CostumeId, definition);
                    var port = new MemoryPort(); var projection = new Projection(); var adapter = PublishedAdapter(fixture, port, projection);
                    var oldPublished = adapter.Published;
                    Assert.That(adapter.TrySelect(candidate), Is.EqualTo(CostumeUnityAdapterRejectionV1.None), mutation);
                    Assert.That(adapter.Published, Is.Not.SameAs(oldPublished));
                    Assert.That(adapter.Published.Media, Is.SameAs(candidate));
                    Assert.That(adapter.Published.State.Current("fixture"), Is.EqualTo(fixture.PackageTwo.CostumeId));
                    Assert.That(port.MoveCalls, Is.EqualTo(2)); Assert.That(port.ReplaceCalls, Is.EqualTo(1)); Assert.That(projection.Count, Is.EqualTo(2));
                    Assert.That(adapter.PersistenceStatus, Is.EqualTo(CostumeUnityPersistenceStatusV1.Ready));
                }
                finally { UnityEngine.Object.DestroyImmediate(portrait); UnityEngine.Object.DestroyImmediate(atlas); }
            }
        }

        // AC-CUA-002/003: R4-B2 mutates only canonical clip-map bytes.  The candidate clips stay canonical;
        // definition and binding receive the mutated payload hash so validation must reach Manifest rather than Hash.
        [TestCaseSource(nameof(R4B2ClipMapByteCases))]
        public void AC_CUA_002_R4_B2_ClipMapByteMutationReachesManifestAndPreservesPublishedPair(string mutation)
        {
            using (var fixture = AcceptedFixture.Create())
            {
                CostumeDefinitionV1 definition;
                var candidate = CreateB2Package(fixture, MutateClipMapBytes(fixture.PackageTwo.ClipMapBytes, mutation), out definition);
                var binding = BindingFor(definition);
                Assert.That(CostumeUnityMediaPackageV1.Hash(candidate.ClipMapBytes), Is.Not.EqualTo(CostumeUnityMediaPackageV1.Hash(fixture.PackageTwo.ClipMapBytes)), mutation);
                Assert.That(CostumeUnityMediaPackageV1.Hash(candidate.ClipMapBytes), Is.EqualTo(definition.ClipMapHash), mutation);
                Assert.That(binding.ClipMapHash, Is.EqualTo(definition.ClipMapHash), mutation);
                Assert.That(candidate.Validate(definition, binding), Is.EqualTo(CostumeUnityPackageRejectionV1.Manifest), mutation);
                ReplaceDefinition(fixture.Catalog, fixture.PackageTwo.CostumeId, definition);
                var port = new MemoryPort(); var projection = new Projection(); var adapter = PublishedAdapter(fixture, port, projection);
                Assert.That(adapter.Highlight(fixture.PackageTwo.CostumeId), Is.EqualTo(CostumeUnityAdapterRejectionV1.HighlightOnly));
                var oldPublished = adapter.Published; var oldMedia = oldPublished.Media; var oldBinding = oldPublished.Binding;
                var beforeView = DeepViewSnapshot.Capture(adapter.SnapshotView());
                var moves = port.MoveCalls; var replaces = port.ReplaceCalls; var projections = projection.Count; var status = adapter.PersistenceStatus;
                Assert.That(adapter.TrySelect(candidate), Is.EqualTo(CostumeUnityAdapterRejectionV1.Package), mutation);
                AssertB1aPublishedPairPreserved(adapter, port, projection, oldPublished, oldMedia, oldBinding, beforeView, moves, replaces, projections, status);
            }
        }

        [Test]
        public void AC_CUA_002_R4_B2_CanonicalClipMapControlValidatesAndPublishesReplacement()
        {
            using (var fixture = AcceptedFixture.Create())
            {
                CostumeDefinitionV1 definition;
                var candidate = CreateB2Package(fixture, fixture.PackageTwo.ClipMapBytes, out definition);
                Assert.That(candidate.Validate(definition, BindingFor(definition)), Is.EqualTo(CostumeUnityPackageRejectionV1.None));
                ReplaceDefinition(fixture.Catalog, fixture.PackageTwo.CostumeId, definition);
                var port = new MemoryPort(); var projection = new Projection(); var adapter = PublishedAdapter(fixture, port, projection);
                var oldPublished = adapter.Published;
                Assert.That(adapter.TrySelect(candidate), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(adapter.Published, Is.Not.SameAs(oldPublished));
                Assert.That(adapter.Published.Media, Is.SameAs(candidate));
                Assert.That(adapter.Published.State.Current("fixture"), Is.EqualTo(fixture.PackageTwo.CostumeId));
                Assert.That(port.MoveCalls, Is.EqualTo(2)); Assert.That(port.ReplaceCalls, Is.EqualTo(1)); Assert.That(projection.Count, Is.EqualTo(2));
            }
        }

        // AC-CUA-002/003: R4-B3 mutates only source-manifest evidence bytes.  Catalog hashes remain canonical.
        [TestCaseSource(nameof(R4B3SourceManifestByteCases))]
        public void AC_CUA_002_R4_B3_SourceManifestByteMutationReachesManifestAndPreservesPublishedPair(string mutation)
        {
            using (var fixture = AcceptedFixture.Create())
            {
                var definition = fixture.Catalog.Find(fixture.PackageTwo.CostumeId);
                var canonicalManifest = fixture.PackageTwo.CanonicalSourceManifestBytes(definition);
                var candidate = CreateB3Package(fixture, MutateSourceManifestBytes(canonicalManifest, mutation));
                var binding = BindingFor(definition);
                Assert.That(CostumeUnityMediaPackageV1.Hash(candidate.SourceManifestBytes), Is.Not.EqualTo(CostumeUnityMediaPackageV1.Hash(canonicalManifest)), mutation);
                Assert.That(CostumeUnityMediaPackageV1.Hash(candidate.PortraitBytes), Is.EqualTo(definition.PortraitHash), mutation);
                Assert.That(CostumeUnityMediaPackageV1.Hash(candidate.AtlasBytes), Is.EqualTo(definition.AtlasHash), mutation);
                Assert.That(CostumeUnityMediaPackageV1.Hash(candidate.ClipMapBytes), Is.EqualTo(definition.ClipMapHash), mutation);
                Assert.That(binding.PortraitHash, Is.EqualTo(definition.PortraitHash), mutation);
                Assert.That(binding.GameplayAtlasHash, Is.EqualTo(definition.AtlasHash), mutation);
                Assert.That(binding.ClipMapHash, Is.EqualTo(definition.ClipMapHash), mutation);
                Assert.That(candidate.Validate(definition, binding), Is.EqualTo(CostumeUnityPackageRejectionV1.Manifest), mutation);
                var port = new MemoryPort(); var projection = new Projection(); var adapter = PublishedAdapter(fixture, port, projection);
                Assert.That(adapter.Highlight(fixture.PackageTwo.CostumeId), Is.EqualTo(CostumeUnityAdapterRejectionV1.HighlightOnly));
                var oldPublished = adapter.Published; var oldMedia = oldPublished.Media; var oldBinding = oldPublished.Binding;
                var beforeView = DeepViewSnapshot.Capture(adapter.SnapshotView());
                var moves = port.MoveCalls; var replaces = port.ReplaceCalls; var projections = projection.Count; var status = adapter.PersistenceStatus;
                Assert.That(adapter.TrySelect(candidate), Is.EqualTo(CostumeUnityAdapterRejectionV1.Package), mutation);
                AssertB1aPublishedPairPreserved(adapter, port, projection, oldPublished, oldMedia, oldBinding, beforeView, moves, replaces, projections, status);
            }
        }

        [Test]
        public void AC_CUA_002_R4_B3_CanonicalSourceManifestControlValidatesAndPublishesReplacement()
        {
            using (var fixture = AcceptedFixture.Create())
            {
                var definition = fixture.Catalog.Find(fixture.PackageTwo.CostumeId);
                var candidate = CreateB3Package(fixture, fixture.PackageTwo.CanonicalSourceManifestBytes(definition));
                Assert.That(candidate.Validate(definition, BindingFor(definition)), Is.EqualTo(CostumeUnityPackageRejectionV1.None));
                var port = new MemoryPort(); var projection = new Projection(); var adapter = PublishedAdapter(fixture, port, projection);
                var oldPublished = adapter.Published;
                Assert.That(adapter.TrySelect(candidate), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(adapter.Published, Is.Not.SameAs(oldPublished));
                Assert.That(adapter.Published.Media, Is.SameAs(candidate));
                Assert.That(adapter.Published.State.Current("fixture"), Is.EqualTo(fixture.PackageTwo.CostumeId));
                Assert.That(port.MoveCalls, Is.EqualTo(2)); Assert.That(port.ReplaceCalls, Is.EqualTo(1)); Assert.That(projection.Count, Is.EqualTo(2));
            }
        }

        // AC-CUA-002/003: R4-B4 closes the remaining canonical-order and candidate-lifetime boundaries.
        [TestCaseSource(nameof(R4B4CasesSource))]
        public void AC_CUA_002_R4_B4_ExactCandidateBoundaryPreservesPublishedPair(string mutation)
        {
            using (var fixture = AcceptedFixture.Create())
            {
                if (mutation == "clipPropertyOrder" || mutation == "framePropertyOrder")
                {
                    CostumeDefinitionV1 definition;
                    var candidate = CreateB2Package(fixture, MutateB4ClipMapBytes(fixture.PackageTwo.ClipMapBytes, mutation), out definition);
                    Assert.That(CostumeUnityMediaPackageV1.Hash(candidate.ClipMapBytes), Is.EqualTo(definition.ClipMapHash), mutation);
                    Assert.That(candidate.Validate(definition, BindingFor(definition)), Is.EqualTo(CostumeUnityPackageRejectionV1.Manifest), mutation);
                    ReplaceDefinition(fixture.Catalog, fixture.PackageTwo.CostumeId, definition);
                    AssertB1aCandidateRejection(fixture, candidate, CostumeUnityAdapterRejectionV1.Package, mutation);
                    return;
                }

                if (mutation == "foreignPackage")
                {
                    CostumeDefinitionV1 foreignDefinition;
                    var foreign = CreateForeignPackage(fixture, out foreignDefinition);
                    Assert.That(foreign.Validate(foreignDefinition, BindingForMedia(foreignDefinition, foreign)), Is.EqualTo(CostumeUnityPackageRejectionV1.None));
                    AssertB1aCandidateRejection(fixture, foreign, CostumeUnityAdapterRejectionV1.Selection, mutation);
                    return;
                }

                if (mutation == "stalePackage")
                {
                    var definition = fixture.Catalog.Find(fixture.PackageTwo.CostumeId);
                    var stale = CreateStalePackage(fixture, definition);
                    Assert.That(stale.Validate(definition, BindingForMedia(definition, stale)), Is.EqualTo(CostumeUnityPackageRejectionV1.None));
                    AssertB1aCandidateRejection(fixture, stale, CostumeUnityAdapterRejectionV1.Package, mutation);
                    return;
                }

                if (mutation == "previouslyRejectedCandidate")
                {
                    CostumeDefinitionV1 definition;
                    var rejected = CreateB2Package(fixture, MutateB4ClipMapBytes(fixture.PackageTwo.ClipMapBytes, "clipPropertyOrder"), out definition);
                    Assert.That(rejected.Validate(definition, BindingFor(definition)), Is.EqualTo(CostumeUnityPackageRejectionV1.Manifest));
                    ReplaceDefinition(fixture.Catalog, fixture.PackageTwo.CostumeId, definition);
                    var port = new MemoryPort(); var projection = new Projection(); var adapter = PublishedAdapter(fixture, port, projection);
                    Assert.That(adapter.Highlight(fixture.PackageTwo.CostumeId), Is.EqualTo(CostumeUnityAdapterRejectionV1.HighlightOnly));
                    var oldPublished = adapter.Published; var oldMedia = oldPublished.Media; var oldBinding = oldPublished.Binding;
                    var beforeView = DeepViewSnapshot.Capture(adapter.SnapshotView());
                    var moves = port.MoveCalls; var replaces = port.ReplaceCalls; var projections = projection.Count; var status = adapter.PersistenceStatus;
                    Assert.That(adapter.TrySelect(rejected), Is.EqualTo(CostumeUnityAdapterRejectionV1.Package));
                    AssertB1aPublishedPairPreserved(adapter, port, projection, oldPublished, oldMedia, oldBinding, beforeView, moves, replaces, projections, status);
                    var sameRejected = rejected;
                    Assert.That(sameRejected, Is.SameAs(rejected));
                    Assert.That(adapter.TrySelect(sameRejected), Is.EqualTo(CostumeUnityAdapterRejectionV1.Package));
                    AssertB1aPublishedPairPreserved(adapter, port, projection, oldPublished, oldMedia, oldBinding, beforeView, moves, replaces, projections, status);
                    return;
                }

                if (mutation == "missingSnapshotAction")
                {
                    CostumeDefinitionV1 definition;
                    var candidate = CreateB1b2Package(fixture.PackageTwo.CostumeId, fixture.PackageTwo.Portrait, fixture.PackageTwo.Atlas, ClipsWithoutAction("bowdraw"), out definition);
                    Assert.That(candidate.Validate(definition, BindingForMedia(definition, candidate)), Is.EqualTo(CostumeUnityPackageRejectionV1.None));
                    ReplaceDefinition(fixture.Catalog, fixture.PackageTwo.CostumeId, definition);
                    var port = new MemoryPort(); var projection = new Projection(); var adapter = PublishedAdapter(fixture, port, projection);
                    Assert.That(adapter.CompletedSnapshots.Observe(new CompletedActorSnapshotV1(11, "fixture", "bowdraw", 500, "left", 3, 4)), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                    Assert.That(adapter.Highlight(fixture.PackageTwo.CostumeId), Is.EqualTo(CostumeUnityAdapterRejectionV1.HighlightOnly));
                    var oldPublished = adapter.Published; var oldMedia = oldPublished.Media; var oldBinding = oldPublished.Binding;
                    var beforeView = DeepViewSnapshot.Capture(adapter.SnapshotView());
                    var moves = port.MoveCalls; var replaces = port.ReplaceCalls; var projections = projection.Count; var status = adapter.PersistenceStatus;
                    Assert.That(adapter.TrySelect(candidate), Is.EqualTo(CostumeUnityAdapterRejectionV1.Snapshot));
                    AssertB1aPublishedPairPreserved(adapter, port, projection, oldPublished, oldMedia, oldBinding, beforeView, moves, replaces, projections, status);
                    return;
                }

                throw new ArgumentOutOfRangeException(nameof(mutation));
            }
        }

        // AC-CUA-003/006: R5 observes the real concrete CIO boundary without introducing a detached receipt seam.
        [Test]
        public void AC_CUA_003_R5_FirstSaveMoveSeesNoPublicationThenProjectionSeesCompleteDurableTuple()
        {
            using (var fixture = AcceptedFixture.Create())
            {
                var port = new MemoryPort(); var projection = new Projection(); CostumeUnityPresentationAdapterV1 adapter = null;
                var moveObserved = false; var projectionObserved = false;
                adapter = fixture.NewAdapter(port, projection);
                port.BeforeMove = () => { moveObserved = true; Assert.That(adapter.Published, Is.Null); };
                var snapshot = new CompletedActorSnapshotV1(31, "fixture", "idle", 1000, "right", 17, -4);
                projection.OnProject = published => { projectionObserved = true; Assert.That(adapter.Published, Is.SameAs(published)); AssertCompleteTuple(published, fixture.Package, snapshot); };
                var staged = StageSelection(fixture.Catalog, fixture.State, fixture.Package.CostumeId);
                var expectedBytes = CostumeCanonicalCodecV1.Encode(staged);
                Assert.That(adapter.CompletedSnapshots.Observe(snapshot), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(adapter.TrySelect(fixture.Package), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(moveObserved, Is.True); Assert.That(projectionObserved, Is.True);
                Assert.That(adapter.PersistenceStatus, Is.EqualTo(CostumeUnityPersistenceStatusV1.Ready));
                Assert.That(port.MoveCalls, Is.EqualTo(1)); Assert.That(port.ReplaceCalls, Is.EqualTo(0)); CollectionAssert.AreEqual(new[] { "Move" }, port.CommitTrace);
                AssertDurableState(port, CostumeFileAdapterV1.PrimaryFileName, adapter.Published.State.Revision, fixture.Package.CostumeId);
                AssertByteEqual(port.BytesForFileName(CostumeFileAdapterV1.PrimaryFileName), expectedBytes);
                AssertTempMissing(port);
                AssertLastSaveResult(adapter, CostumeFileSaveOutcomeV1.CommittedFirst, CostumeFileSaveStageV1.PrimaryDecode, staged.Revision);
            }
        }

        [Test]
        public void AC_CUA_003_R5_ReplacementReplaceBoundaryRetainsOldTupleThenProjectsOneCompleteNewTuple()
        {
            using (var fixture = AcceptedFixture.Create())
            {
                var port = new MemoryPort(); var projection = new Projection(); var adapter = fixture.NewAdapter(port, projection);
                var first = new CompletedActorSnapshotV1(40, "fixture", "idle", 500, "left", 7, -3);
                Assert.That(adapter.CompletedSnapshots.Observe(first), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(adapter.TrySelect(fixture.Package), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                var old = adapter.Published; var oldState = old.State; var oldBinding = old.Binding; var oldMedia = old.Media; var oldSnapshot = old.Snapshot; var oldFrame = old.Frame;
                var replaceObserved = false; var projectionObserved = false;
                port.BeforeReplace = () =>
                {
                    replaceObserved = true; Assert.That(adapter.Published, Is.SameAs(old)); Assert.That(adapter.Published.State, Is.SameAs(oldState)); Assert.That(adapter.Published.Media, Is.SameAs(oldMedia)); Assert.That(adapter.Published.Binding, Is.EqualTo(oldBinding)); Assert.That(SnapshotEqual(adapter.Published.Snapshot, oldSnapshot), Is.True); Assert.That(FrameEqual(adapter.Published.Frame, oldFrame), Is.True); Assert.That(adapter.SnapshotView().CurrentId, Is.EqualTo(fixture.Package.CostumeId));
                };
                projection.OnProject = published => { projectionObserved = true; Assert.That(adapter.Published, Is.SameAs(published)); AssertCompleteTuple(published, fixture.PackageTwo, first); };
                var staged = StageSelection(fixture.Catalog, oldState, fixture.PackageTwo.CostumeId);
                var expectedBytes = CostumeCanonicalCodecV1.Encode(staged);
                var preReplacementPrimary = port.BytesForFileName(CostumeFileAdapterV1.PrimaryFileName);
                Assert.That(adapter.TrySelect(fixture.PackageTwo), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(replaceObserved, Is.True); Assert.That(projectionObserved, Is.True);
                Assert.That(adapter.Published, Is.Not.SameAs(old)); Assert.That(adapter.Published.State.Revision, Is.EqualTo(oldState.Revision + 1));
                Assert.That(port.MoveCalls, Is.EqualTo(2)); Assert.That(port.ReplaceCalls, Is.EqualTo(1)); CollectionAssert.AreEqual(new[] { "Move", "Replace" }, port.CommitTrace);
                AssertDurableState(port, CostumeFileAdapterV1.PrimaryFileName, oldState.Revision + 1, fixture.PackageTwo.CostumeId);
                AssertDurableState(port, CostumeFileAdapterV1.PreviousFileName, oldState.Revision, fixture.Package.CostumeId);
                AssertByteEqual(port.BytesForFileName(CostumeFileAdapterV1.PrimaryFileName), expectedBytes);
                AssertByteEqual(port.BytesForFileName(CostumeFileAdapterV1.PreviousFileName), preReplacementPrimary);
                AssertTempMissing(port);
                AssertLastSaveResult(adapter, CostumeFileSaveOutcomeV1.CommittedReplacement, CostumeFileSaveStageV1.PrimaryDecode, staged.Revision);
            }
        }

        [Test]
        public void AC_CUA_003_R5_ReplacementProjectionFaultRetainsDurableNewTupleAndClosesMutation()
        {
            using (var fixture = AcceptedFixture.Create())
            {
                var port = new MemoryPort(); var projection = new Projection(); var adapter = fixture.NewAdapter(port, projection);
                var snapshot = new CompletedActorSnapshotV1(50, "fixture", "idle", 500, "right", 8, 6);
                Assert.That(adapter.CompletedSnapshots.Observe(snapshot), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(adapter.TrySelect(fixture.Package), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                var oldDrawn = projection.LastSuccessful; var oldState = adapter.Published.State;
                var staged = StageSelection(fixture.Catalog, oldState, fixture.PackageTwo.CostumeId);
                var expectedBytes = CostumeCanonicalCodecV1.Encode(staged);
                var preReplacementPrimary = port.BytesForFileName(CostumeFileAdapterV1.PrimaryFileName);
                projection.Throw = true;
                Assert.That(adapter.TrySelect(fixture.PackageTwo), Is.EqualTo(CostumeUnityAdapterRejectionV1.ReloadRequired));
                var durable = adapter.Published;
                Assert.That(durable, Is.Not.SameAs(oldDrawn)); AssertCompleteTuple(durable, fixture.PackageTwo, snapshot);
                Assert.That(adapter.PersistenceStatus, Is.EqualTo(CostumeUnityPersistenceStatusV1.ReloadRequired)); Assert.That(projection.LastSuccessful, Is.SameAs(oldDrawn));
                AssertDurableState(port, CostumeFileAdapterV1.PrimaryFileName, oldState.Revision + 1, fixture.PackageTwo.CostumeId);
                AssertDurableState(port, CostumeFileAdapterV1.PreviousFileName, oldState.Revision, fixture.Package.CostumeId);
                AssertByteEqual(port.BytesForFileName(CostumeFileAdapterV1.PrimaryFileName), expectedBytes);
                AssertByteEqual(port.BytesForFileName(CostumeFileAdapterV1.PreviousFileName), preReplacementPrimary);
                AssertTempMissing(port);
                AssertLastSaveResult(adapter, CostumeFileSaveOutcomeV1.CommittedReplacement, CostumeFileSaveStageV1.PrimaryDecode, staged.Revision);
                var primary = port.BytesForFileName(CostumeFileAdapterV1.PrimaryFileName); var previous = port.BytesForFileName(CostumeFileAdapterV1.PreviousFileName); var moves = port.MoveCalls; var replaces = port.ReplaceCalls; var projections = projection.Count;
                Assert.That(adapter.TrySelect(fixture.Package), Is.EqualTo(CostumeUnityAdapterRejectionV1.ReloadRequired));
                Assert.That(adapter.CompletedSnapshots.Observe(new CompletedActorSnapshotV1(51, "fixture", "idle", 0, "right", 8, 6)), Is.EqualTo(CostumeUnityAdapterRejectionV1.ReloadRequired));
                Assert.That(adapter.Highlight(fixture.Package.CostumeId), Is.EqualTo(CostumeUnityAdapterRejectionV1.ReloadRequired));
                Assert.That(adapter.Published, Is.SameAs(durable)); Assert.That(port.MoveCalls, Is.EqualTo(moves)); Assert.That(port.ReplaceCalls, Is.EqualTo(replaces)); Assert.That(projection.Count, Is.EqualTo(projections)); AssertByteEqual(port.BytesForFileName(CostumeFileAdapterV1.PrimaryFileName), primary); AssertByteEqual(port.BytesForFileName(CostumeFileAdapterV1.PreviousFileName), previous);
            }
        }

        [Test]
        public void AC_CUA_006_R5_ReplacementSameRevisionDifferentPrimaryBytesClosesWithoutPublicationOrRetry()
        {
            using (var fixture = AcceptedFixture.Create())
            {
                var port = new MemoryPort(); var projection = new Projection(); var adapter = fixture.NewAdapter(port, projection);
                var snapshot = new CompletedActorSnapshotV1(60, "fixture", "idle", 0, "left", 3, 4);
                Assert.That(adapter.CompletedSnapshots.Observe(snapshot), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(adapter.TrySelect(fixture.Package), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                var old = adapter.Published; var oldState = old.State; var projections = projection.Count;
                var staged = StageSelection(fixture.Catalog, oldState, fixture.PackageTwo.CostumeId);
                var expectedBytes = CostumeCanonicalCodecV1.Encode(staged);
                port.CorruptPrimaryWithSameRevisionDifferentBytes = true;
                Assert.That(adapter.TrySelect(fixture.PackageTwo), Is.EqualTo(CostumeUnityAdapterRejectionV1.ReloadRequired));
                Assert.That(adapter.PersistenceStatus, Is.EqualTo(CostumeUnityPersistenceStatusV1.CommitOutcomeUncertain)); Assert.That(adapter.Published, Is.SameAs(old)); Assert.That(projection.Count, Is.EqualTo(projections));
                CollectionAssert.AreEqual(new[] { "Move", "Replace" }, port.CommitTrace); Assert.That(port.MoveCalls, Is.EqualTo(2)); Assert.That(port.ReplaceCalls, Is.EqualTo(1));
                AssertDurableState(port, CostumeFileAdapterV1.PrimaryFileName, oldState.Revision + 1, fixture.Package.CostumeId);
                AssertDurableState(port, CostumeFileAdapterV1.PreviousFileName, oldState.Revision, fixture.Package.CostumeId);
                Assert.That(ByteArraysEqual(port.BytesForFileName(CostumeFileAdapterV1.PrimaryFileName), expectedBytes), Is.False);
                AssertTempMissing(port);
                AssertLastSaveResult(adapter, CostumeFileSaveOutcomeV1.CommitOutcomeUncertain, CostumeFileSaveStageV1.PrimaryEquality, staged.Revision);
                var primary = port.BytesForFileName(CostumeFileAdapterV1.PrimaryFileName); var previous = port.BytesForFileName(CostumeFileAdapterV1.PreviousFileName); var moves = port.MoveCalls; var replaces = port.ReplaceCalls;
                Assert.That(adapter.TrySelect(fixture.PackageTwo), Is.EqualTo(CostumeUnityAdapterRejectionV1.ReloadRequired)); Assert.That(adapter.CompletedSnapshots.Observe(new CompletedActorSnapshotV1(61, "fixture", "idle", 0, "left", 3, 4)), Is.EqualTo(CostumeUnityAdapterRejectionV1.ReloadRequired)); Assert.That(adapter.Highlight(fixture.PackageTwo.CostumeId), Is.EqualTo(CostumeUnityAdapterRejectionV1.ReloadRequired));
                Assert.That(port.MoveCalls, Is.EqualTo(moves)); Assert.That(port.ReplaceCalls, Is.EqualTo(replaces)); AssertByteEqual(port.BytesForFileName(CostumeFileAdapterV1.PrimaryFileName), primary); AssertByteEqual(port.BytesForFileName(CostumeFileAdapterV1.PreviousFileName), previous);
            }
        }

        [Test]
        public void AC_CUA_006_R5_AdapterPublicSurfaceHasNoReceiptResultDelegateOrAlternateSaveSeam()
        {
            var type = typeof(CostumeUnityPresentationAdapterV1);
            var hasLastSaveResult = type.GetField("_hasLastSaveResult", BindingFlags.Instance | BindingFlags.NonPublic);
            var lastSaveResult = type.GetField("_lastSaveResult", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(hasLastSaveResult, Is.Not.Null); Assert.That(hasLastSaveResult.IsPrivate, Is.True);
            Assert.That(lastSaveResult, Is.Not.Null); Assert.That(lastSaveResult.IsPrivate, Is.True); Assert.That(lastSaveResult.FieldType, Is.EqualTo(typeof(CostumeFileSaveResultV1)));
            Assert.That(type.GetProperty("LastSaveResult", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic), Is.Null);
            Assert.That(type.GetMethod("GetLastSaveResult", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic), Is.Null);
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Public)) Assert.Fail("Public mutable seam: " + field.Name);
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)) { Assert.That(property.CanWrite, Is.False, property.Name); Assert.That(property.Name.IndexOf("receipt", StringComparison.OrdinalIgnoreCase), Is.EqualTo(-1)); Assert.That(property.Name.IndexOf("result", StringComparison.OrdinalIgnoreCase), Is.EqualTo(-1)); }
            foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                if (method.IsSpecialName) continue;
                Assert.That(method.Name, Is.EqualTo("TrySelect").Or.EqualTo("Highlight").Or.EqualTo("SnapshotView"));
                Assert.That(method.Name.IndexOf("save", StringComparison.OrdinalIgnoreCase), Is.EqualTo(-1));
                foreach (var parameter in method.GetParameters()) { Assert.That(typeof(Delegate).IsAssignableFrom(parameter.ParameterType), Is.False, method.Name); Assert.That(parameter.ParameterType, Is.Not.EqualTo(typeof(CostumeFileSaveResultV1))); }
            }
            var selection = type.GetMethod("TrySelect", BindingFlags.Instance | BindingFlags.Public);
            Assert.That(selection, Is.Not.Null); Assert.That(selection.GetParameters().Length, Is.EqualTo(1)); Assert.That(selection.GetParameters()[0].ParameterType, Is.EqualTo(typeof(CostumeUnityMediaPackageV1)));
        }

        public static IEnumerable<TestCaseData> CurrentTupleCorruptionCases() { foreach (var value in CurrentTupleCorruptions) yield return new TestCaseData(value).SetName("AC_CUA_003_CurrentTuple_" + value + "_TrySelectClosed"); }
        public static IEnumerable<TestCaseData> CurrentTupleObservationCorruptionCases() { foreach (var value in CurrentTupleCorruptions) yield return new TestCaseData(value).SetName("AC_CUA_005_CurrentTuple_" + value + "_ObserveClosed"); }

        [TestCaseSource(nameof(CurrentTupleCorruptionCases))]
        public void AC_CUA_003_EachMalformedCurrentTupleClosesTrySelect(string mutation)
        {
            using (var fixture = AcceptedFixture.Create())
            {
                var port = new MemoryPort(); var projection = new Projection(); var adapter = PublishedAdapter(fixture, port, projection);
                AssertTerminalAfterCorruption(adapter, fixture, port, projection, mutation, false);
            }
        }

        [TestCaseSource(nameof(CurrentTupleObservationCorruptionCases))]
        public void AC_CUA_005_EachMalformedCurrentTupleClosesHigherObservation(string mutation)
        {
            using (var fixture = AcceptedFixture.Create())
            {
                var port = new MemoryPort(); var projection = new Projection(); var adapter = PublishedAdapter(fixture, port, projection);
                AssertTerminalAfterCorruption(adapter, fixture, port, projection, mutation, true);
            }
        }
        [Test]
        public void AC_CUA_001_And_008_RealSeryeongRowsRemainPendingAndHighlightIsTransient()
        {
            var catalog = CostumeCatalogV1.SeryeongInventoryPending();
            var state = new CostumeStateV1(7, new string[0], new KeyValuePair<string, string>[0]);
            var adapter = new CostumeUnityPresentationAdapterV1(catalog, state, "seryeong", new CostumeFileAdapterV1(Path.Combine(Path.GetTempPath(), "cua-isolated-test")), new Projection());
            var before = adapter.SnapshotView();
            Assert.That(before.Rows.Count, Is.EqualTo(36));
            Assert.That(before.Availability, Is.EqualTo(CostumeAvailability.NoAcceptedDefault));
            foreach (var row in before.Rows) { Assert.That(row.Disposition, Is.EqualTo(CostumeDisposition.ProductionPending)); Assert.That(row.CanPrepareSelection, Is.False); }
            Assert.That(adapter.Highlight(before.Rows[0].CostumeId), Is.EqualTo(CostumeUnityAdapterRejectionV1.HighlightOnly));
            var after = adapter.SnapshotView();
            Assert.That(after.StateRevision, Is.EqualTo(before.StateRevision));
            Assert.That(after.CurrentId, Is.EqualTo(before.CurrentId));
            Assert.That(after.TransientPreviewId, Is.EqualTo(before.Rows[0].CostumeId));
            Assert.That(adapter.Published, Is.Null);
        }

        [Test]
        public void AC_CUA_002_InvalidSyntheticClipGeometryCannotBecomeAValidFrame()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new CostumeUnityFrameV1(0, 0, 0, 1, 500, 0, 0, 1));
            Assert.Throws<ArgumentException>(() => new CostumeUnityClipV1(null, "left", true, new[] { new CostumeUnityFrameV1(0, 0, 1, 1, 500, 0, 0, 1) }));
            Assert.Throws<ArgumentException>(() => new CostumeUnityClipV1(String.Empty, "left", true, new[] { new CostumeUnityFrameV1(0, 0, 1, 1, 500, 0, 0, 1) }));
            Assert.Throws<ArgumentException>(() => new CostumeUnityClipV1("idle", "up", true, new[] { new CostumeUnityFrameV1(0, 0, 1, 1, 500, 0, 0, 1) }));
        }

        [Test]
        public void AC_CUA_002_SyntheticPackageUsesExactCanonicalHashesAndBothFacings()
        {
            var portrait = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            var atlas = new Texture2D(4, 2, TextureFormat.RGBA32, false);
            portrait.filterMode = FilterMode.Point;
            atlas.filterMode = FilterMode.Point;
            try
            {
                var clips = Clips();
                var seed = new CostumeUnityMediaPackageV1("fixture", "fixture.costume.synthetic.v1", "fixture.portrait.v1", "fixture.gameplay.v1", 9, 5, true, portrait, atlas, new byte[] { 1, 2 }, new byte[] { 3, 4 }, new byte[0], new byte[0], CostumeUnityMediaPackageV1.SyntheticApprovedPpu, clips);
                var clipMap = seed.CanonicalClipMapBytes();
                var definition = new CostumeDefinitionV1("fixture", "fixture.costume.synthetic.v1", "synthetic", new string('a', 64), "synthetic-hair", CostumeDisposition.Accepted, true, "fixture.portrait.v1", "fixture.gameplay.v1", CostumeUnityMediaPackageV1.Hash(seed.PortraitBytes), CostumeUnityMediaPackageV1.Hash(seed.AtlasBytes), CostumeUnityMediaPackageV1.Hash(clipMap), 5);
                var binding = new CostumePresentationBindingV1("fixture", "fixture.costume.synthetic.v1", "fixture.portrait.v1", definition.PortraitHash, "fixture.gameplay.v1", definition.AtlasHash, definition.ClipMapHash, 5, 9, ActionIds());
                var package = new CostumeUnityMediaPackageV1("fixture", "fixture.costume.synthetic.v1", "fixture.portrait.v1", "fixture.gameplay.v1", 9, 5, true, portrait, atlas, seed.PortraitBytes, seed.AtlasBytes, clipMap, seed.CanonicalSourceManifestBytes(definition), CostumeUnityMediaPackageV1.SyntheticApprovedPpu, clips);
                Assert.That(package.Validate(definition, binding), Is.EqualTo(CostumeUnityPackageRejectionV1.None));
                portrait.filterMode = FilterMode.Bilinear;
                Assert.That(package.Validate(definition, binding), Is.EqualTo(CostumeUnityPackageRejectionV1.Filter));
            }
            finally { UnityEngine.Object.DestroyImmediate(portrait); UnityEngine.Object.DestroyImmediate(atlas); }
        }

        [Test]
        public void AC_CUA_004_ExactLoopAndNonLoopPhaseEndpointsDoNotRestartNonLoop()
        {
            var a = new CostumeUnityFrameV1(0, 0, 1, 1, 500, 0, 0, 1);
            var b = new CostumeUnityFrameV1(1, 0, 1, 1, 500, 0, 0, 1);
            var nonLoop = new CostumeUnityClipV1("idle", "left", false, new[] { a, b });
            var loop = new CostumeUnityClipV1("run", "left", true, new[] { a, b });
            Assert.That(nonLoop.FrameFor(1000).X, Is.EqualTo(1));
            Assert.That(loop.FrameFor(999).X, Is.EqualTo(1));
            Assert.That(loop.FrameFor(1000).X, Is.EqualTo(0));
        }

        [Test]
        public void AC_CUA_003_And_006_ConcreteSynchronousCioPublishesOnlyAfterDurableSuccess()
        {
            using (var fixture = AcceptedFixture.Create())
            {
                var port = new MemoryPort();
                var projection = new Projection();
                var adapter = fixture.NewAdapter(port, projection);
                var snapshot = new CompletedActorSnapshotV1(11, "fixture", "idle", 1000, "right", 17, -4);
                Assert.That(adapter.CompletedSnapshots.Observe(snapshot), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(adapter.TrySelect(fixture.Package), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(port.MoveCalls, Is.EqualTo(1));
                Assert.That(projection.Count, Is.EqualTo(1));
                Assert.That(adapter.Published.Snapshot.Tick, Is.EqualTo(11));
                Assert.That(adapter.Published.Snapshot.AnchorX, Is.EqualTo(17));
                Assert.That(adapter.Published.Snapshot.AnchorY, Is.EqualTo(-4));
                Assert.That(adapter.Published.Frame.X, Is.EqualTo(1)); // non-loop 1000 maps to final frame
            }
        }

        [Test]
        public void AC_CUA_004_PortOnlySnapshotIsRequiredAndIsMonotonic()
        {
            using (var fixture = AcceptedFixture.Create())
            {
                var adapter = fixture.NewAdapter(new MemoryPort(), new Projection());
                Assert.That(adapter.TrySelect(fixture.Package), Is.EqualTo(CostumeUnityAdapterRejectionV1.Snapshot));
                Assert.That(adapter.CompletedSnapshots.Observe(new CompletedActorSnapshotV1(2, "other", "idle", 0, "left", 0, 0)), Is.EqualTo(CostumeUnityAdapterRejectionV1.Snapshot));
                Assert.That(adapter.CompletedSnapshots.Observe(new CompletedActorSnapshotV1(5, "fixture", "idle", 250, "left", 4, 9)), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(adapter.CompletedSnapshots.Observe(new CompletedActorSnapshotV1(5, "fixture", "idle", 250, "left", 4, 9)), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(adapter.CompletedSnapshots.Observe(new CompletedActorSnapshotV1(5, "fixture", "idle", 251, "left", 4, 9)), Is.EqualTo(CostumeUnityAdapterRejectionV1.Snapshot));
                Assert.That(adapter.CompletedSnapshots.Observe(new CompletedActorSnapshotV1(4, "fixture", "idle", 250, "left", 4, 9)), Is.EqualTo(CostumeUnityAdapterRejectionV1.Snapshot));
            }
        }

        [Test]
        public void AC_CUA_003_ReplacementUsesSecondAcceptedPackageAndPreservesObservedSnapshot()
        {
            using (var fixture = AcceptedFixture.Create())
            {
                var port = new MemoryPort(); var projection = new Projection(); var adapter = fixture.NewAdapter(port, projection);
                var first = new CompletedActorSnapshotV1(10, "fixture", "idle", 500, "left", 7, -3);
                Assert.That(adapter.CompletedSnapshots.Observe(first), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(adapter.TrySelect(fixture.Package), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                var old = adapter.Published;
                var replacement = new CompletedActorSnapshotV1(11, "fixture", "idle", 500, "left", 7, -3);
                Assert.That(adapter.CompletedSnapshots.Observe(replacement), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(adapter.TrySelect(fixture.PackageTwo), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(port.MoveCalls, Is.EqualTo(2));
                Assert.That(port.ReplaceCalls, Is.EqualTo(1));
                Assert.That(adapter.Published, Is.Not.SameAs(old));
                Assert.That(adapter.Published.State.Current("fixture"), Is.EqualTo(fixture.PackageTwo.CostumeId));
                Assert.That(adapter.Published.Snapshot.Tick, Is.EqualTo(11));
                Assert.That(adapter.Published.Snapshot.AnchorX, Is.EqualTo(7));
                Assert.That(adapter.Published.Snapshot.AnchorY, Is.EqualTo(-3));
            }
        }

        [Test]
        public void AC_CUA_002_OverflowBoundsAndDuplicateClipRectangleAreRejected()
        {
            var portrait = new Texture2D(2, 2, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
            var atlas = new Texture2D(4, 2, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
            try
            {
                var repeated = new CostumeUnityFrameV1(0, 0, 1, 1, 500, 0, 0, 1);
                var duplicateClips = new[] { new CostumeUnityClipV1("idle", "left", false, new[] { repeated, repeated }), new CostumeUnityClipV1("idle", "right", false, new[] { repeated }) };
                var seed = new CostumeUnityMediaPackageV1("fixture", "fixture.costume.synthetic.v1", "fixture.portrait.v1", "fixture.gameplay.v1", 9, 5, true, portrait, atlas, new byte[] { 1 }, new byte[] { 2 }, new byte[0], new byte[0], 1, duplicateClips);
                var map = seed.CanonicalClipMapBytes();
                var def = new CostumeDefinitionV1("fixture", "fixture.costume.synthetic.v1", "synthetic", new string('a', 64), "synthetic", CostumeDisposition.Accepted, true, "fixture.portrait.v1", "fixture.gameplay.v1", CostumeUnityMediaPackageV1.Hash(seed.PortraitBytes), CostumeUnityMediaPackageV1.Hash(seed.AtlasBytes), CostumeUnityMediaPackageV1.Hash(map), 5);
                var package = new CostumeUnityMediaPackageV1("fixture", "fixture.costume.synthetic.v1", "fixture.portrait.v1", "fixture.gameplay.v1", 9, 5, true, portrait, atlas, seed.PortraitBytes, seed.AtlasBytes, map, seed.CanonicalSourceManifestBytes(def), 1, duplicateClips);
                var binding = new CostumePresentationBindingV1("fixture", "fixture.costume.synthetic.v1", "fixture.portrait.v1", def.PortraitHash, "fixture.gameplay.v1", def.AtlasHash, def.ClipMapHash, 5, 9, new[] { "idle" });
                Assert.That(package.Validate(def, binding), Is.EqualTo(CostumeUnityPackageRejectionV1.Clips));
                var huge = new CostumeUnityFrameV1(Int32.MaxValue, 0, 1, 1, 500, 0, 0, 1);
                Assert.That(huge.X, Is.EqualTo(Int32.MaxValue)); // package validator exercises widened bounds with this constructible input.
            }
            finally { UnityEngine.Object.DestroyImmediate(portrait); UnityEngine.Object.DestroyImmediate(atlas); }
        }

        [Test]
        public void AC_CUA_003_And_006_PreCommitAndUncertainCioFailuresNeverPublish()
        {
            using (var fixture = AcceptedFixture.Create())
            {
                var preCommit = new MemoryPort { FailOpen = true };
                var failed = fixture.NewAdapter(preCommit, new Projection());
                var snapshot = new CompletedActorSnapshotV1(1, "fixture", "idle", 0, "left", 0, 0);
                Assert.That(failed.CompletedSnapshots.Observe(snapshot), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(failed.TrySelect(fixture.Package), Is.EqualTo(CostumeUnityAdapterRejectionV1.Save));
                Assert.That(failed.PersistenceStatus, Is.EqualTo(CostumeUnityPersistenceStatusV1.FailedBeforeCommit));
                Assert.That(failed.Published, Is.Null);

                var uncertain = new MemoryPort { FailMove = true };
                var closed = fixture.NewAdapter(uncertain, new Projection());
                Assert.That(closed.CompletedSnapshots.Observe(snapshot), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(closed.TrySelect(fixture.Package), Is.EqualTo(CostumeUnityAdapterRejectionV1.ReloadRequired));
                Assert.That(closed.PersistenceStatus, Is.EqualTo(CostumeUnityPersistenceStatusV1.CommitOutcomeUncertain));
                Assert.That(closed.Published, Is.Null);
            }
        }

        [Test]
        public void AC_CUA_006_WrongPrimaryAndProjectionFaultCloseWithoutRollback()
        {
            using (var fixture = AcceptedFixture.Create())
            {
                var wrongPrimary = fixture.NewAdapter(new MemoryPort { CorruptPrimary = true }, new Projection());
                Assert.That(wrongPrimary.CompletedSnapshots.Observe(new CompletedActorSnapshotV1(1, "fixture", "idle", 0, "left", 0, 0)), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(wrongPrimary.TrySelect(fixture.Package), Is.EqualTo(CostumeUnityAdapterRejectionV1.ReloadRequired));
                Assert.That(wrongPrimary.Published, Is.Null);
                Assert.That(wrongPrimary.PersistenceStatus, Is.EqualTo(CostumeUnityPersistenceStatusV1.CommitOutcomeUncertain));

                var projection = new Projection { Throw = true };
                var adapter = fixture.NewAdapter(new MemoryPort(), projection);
                var observed = new CompletedActorSnapshotV1(2, "fixture", "idle", 500, "right", 8, 6);
                Assert.That(adapter.CompletedSnapshots.Observe(observed), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(adapter.TrySelect(fixture.Package), Is.EqualTo(CostumeUnityAdapterRejectionV1.ReloadRequired));
                var durable = adapter.Published;
                Assert.That(durable, Is.Not.Null);
                Assert.That(durable.Snapshot.Tick, Is.EqualTo(2));
                Assert.That(adapter.PersistenceStatus, Is.EqualTo(CostumeUnityPersistenceStatusV1.ReloadRequired));
                Assert.That(adapter.Highlight(fixture.PackageTwo.CostumeId), Is.EqualTo(CostumeUnityAdapterRejectionV1.ReloadRequired));
                Assert.That(adapter.CompletedSnapshots.Observe(new CompletedActorSnapshotV1(3, "fixture", "idle", 0, "right", 8, 6)), Is.EqualTo(CostumeUnityAdapterRejectionV1.ReloadRequired));
                Assert.That(adapter.TrySelect(fixture.PackageTwo), Is.EqualTo(CostumeUnityAdapterRejectionV1.ReloadRequired));
                Assert.That(adapter.Published, Is.SameAs(durable));
            }
        }

        [Test]
        public void AC_CUA_004_AllActionsBothFacingsAndEndpointAgesPreserveCompletedSnapshot()
        {
            using (var fixture = AcceptedFixture.Create())
            {
                var adapter = fixture.NewAdapter(new MemoryPort(), new Projection());
                long tick = 1;
                Assert.That(adapter.CompletedSnapshots.Observe(new CompletedActorSnapshotV1(tick, "fixture", "idle", 0, "left", 1, 2)), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(adapter.TrySelect(fixture.Package), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                foreach (var action in ActionIds()) foreach (var facing in new[] { "left", "right" }) foreach (var age in new long[] { 0, 500, 999, 1000 })
                {
                    tick++;
                    var next = new CompletedActorSnapshotV1(tick, "fixture", action, age, facing, 101, -99);
                    Assert.That(adapter.CompletedSnapshots.Observe(next), Is.EqualTo(CostumeUnityAdapterRejectionV1.None), action + "/" + facing + "/" + age);
                    Assert.That(adapter.Published.Snapshot.ActionId, Is.EqualTo(action));
                    Assert.That(adapter.Published.Snapshot.Facing, Is.EqualTo(facing));
                    Assert.That(adapter.Published.Snapshot.ActionAgeQ1000, Is.EqualTo(age));
                    Assert.That(adapter.Published.Snapshot.AnchorX, Is.EqualTo(101));
                    Assert.That(adapter.Published.Snapshot.AnchorY, Is.EqualTo(-99));
                    var clip = fixture.Package.FindClip(action, facing); var index = clip.Loop && age == 1000 ? 0 : (int)(age * clip.Frames.Count / 1000); if (!clip.Loop && index >= clip.Frames.Count) index = clip.Frames.Count - 1;
                    var expected = clip.Frames[index]; var actual = adapter.Published.Frame;
                    Assert.That(actual.X, Is.EqualTo(expected.X)); Assert.That(actual.Y, Is.EqualTo(expected.Y)); Assert.That(actual.Width, Is.EqualTo(expected.Width)); Assert.That(actual.Height, Is.EqualTo(expected.Height)); Assert.That(actual.PivotXQ1000, Is.EqualTo(expected.PivotXQ1000)); Assert.That(actual.PivotYQ1000, Is.EqualTo(expected.PivotYQ1000)); Assert.That(actual.BaselineY, Is.EqualTo(expected.BaselineY)); Assert.That(actual.DurationTicks, Is.EqualTo(expected.DurationTicks));
                }
            }
        }

        [Test]
        public void AC_CUA_005_IsolatedMechanicsAndPhysicsProbeRemainByteForByteUntouched()
        {
            var owner = new GameObject("cua-owned-mechanics-probe");
            var body = owner.AddComponent<Rigidbody2D>(); var collider = owner.AddComponent<BoxCollider2D>();
            owner.transform.position = new Vector3(31, -8, 0); owner.transform.localScale = new Vector3(2, 3, 1); body.gravityScale = 2.5f; body.freezeRotation = true; collider.size = new Vector2(7, 9); collider.enabled = true;
            var beforePosition = owner.transform.position; var beforeScale = owner.transform.localScale; var beforeGravity = body.gravityScale; var beforeFreeze = body.freezeRotation; var beforeSize = collider.size; var beforeEnabled = collider.enabled; var actionToken = "idle"; var targetToken = "target"; var phaseToken = "phase"; var aiToken = "ai"; var calls = 17;
            try
            {
                using (var fixture = AcceptedFixture.Create())
                {
                    var adapter = fixture.NewAdapter(new MemoryPort(), new Projection());
                    Assert.That(adapter.CompletedSnapshots.Observe(new CompletedActorSnapshotV1(1, "fixture", "idle", 0, "left", 5, 6)), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                    Assert.That(adapter.TrySelect(fixture.Package), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                    Assert.That(adapter.CompletedSnapshots.Observe(new CompletedActorSnapshotV1(2, "fixture", "locomotion", 500, "right", 5, 6)), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                }
                Assert.That(owner.transform.position, Is.EqualTo(beforePosition)); Assert.That(owner.transform.localScale, Is.EqualTo(beforeScale)); Assert.That(body.gravityScale, Is.EqualTo(beforeGravity)); Assert.That(body.freezeRotation, Is.EqualTo(beforeFreeze)); Assert.That(collider.size, Is.EqualTo(beforeSize)); Assert.That(collider.enabled, Is.EqualTo(beforeEnabled));
                Assert.That(actionToken, Is.EqualTo("idle")); Assert.That(targetToken, Is.EqualTo("target")); Assert.That(phaseToken, Is.EqualTo("phase")); Assert.That(aiToken, Is.EqualTo("ai")); Assert.That(calls, Is.EqualTo(17));
            }
            finally { UnityEngine.Object.DestroyImmediate(owner); }
        }

        // REQUIREMENT R3-P1-004; AC-CUA-004: the presentation adapter is not an authority over an independent mechanics rig.
        [Test]
        public void AC_CUA_004_R6_FirstSelectionSuccessLeavesIndependentMechanicsProbeExact()
        {
            using (var probe = MechanicsProbe.Create())
            using (var fixture = AcceptedFixture.Create())
            {
                var adapter = fixture.NewAdapter(new MemoryPort(), new Projection());
                var before = MechanicsProbeSnapshot.Capture(probe);
                Assert.That(adapter.CompletedSnapshots.Observe(new CompletedActorSnapshotV1(101, "fixture", "idle", 0, "left", 5, 6)), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                AssertMechanicsProbeExact(before, probe);
                Assert.That(adapter.TrySelect(fixture.Package), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                AssertMechanicsProbeExact(before, probe);
            }
        }

        // REQUIREMENT R3-P1-004; AC-CUA-004: replacement cannot alter mechanics state outside the injected presentation ports.
        [Test]
        public void AC_CUA_004_R6_ReplacementSuccessLeavesIndependentMechanicsProbeExact()
        {
            using (var probe = MechanicsProbe.Create())
            using (var fixture = AcceptedFixture.Create())
            {
                var port = new MemoryPort(); var adapter = fixture.NewAdapter(port, new Projection());
                var before = MechanicsProbeSnapshot.Capture(probe);
                Assert.That(adapter.CompletedSnapshots.Observe(new CompletedActorSnapshotV1(201, "fixture", "idle", 0, "left", 5, 6)), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(adapter.TrySelect(fixture.Package), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                AssertMechanicsProbeExact(before, probe);
                Assert.That(adapter.CompletedSnapshots.Observe(new CompletedActorSnapshotV1(202, "fixture", "locomotion", 500, "right", 5, 6)), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                AssertMechanicsProbeExact(before, probe);
                Assert.That(adapter.TrySelect(fixture.PackageTwo), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(port.ReplaceCalls, Is.EqualTo(1));
                AssertMechanicsProbeExact(before, probe);
            }
        }

        // REQUIREMENT R3-P1-004; AC-CUA-004: a valid durable snapshot observation changes only the published presentation tuple.
        [Test]
        public void AC_CUA_004_R6_AcceptedDurableObservationLeavesIndependentMechanicsProbeExact()
        {
            using (var probe = MechanicsProbe.Create())
            using (var fixture = AcceptedFixture.Create())
            {
                var projection = new Projection(); var adapter = fixture.NewAdapter(new MemoryPort(), projection);
                Assert.That(adapter.CompletedSnapshots.Observe(new CompletedActorSnapshotV1(301, "fixture", "idle", 0, "left", 5, 6)), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(adapter.TrySelect(fixture.Package), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                var before = MechanicsProbeSnapshot.Capture(probe);
                var next = new CompletedActorSnapshotV1(302, "fixture", "bowdraw", 500, "right", 8, -3);
                Assert.That(adapter.CompletedSnapshots.Observe(next), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(adapter.Published.Snapshot.Tick, Is.EqualTo(next.Tick)); Assert.That(projection.Count, Is.EqualTo(2));
                AssertMechanicsProbeExact(before, probe);
            }
        }

        // REQUIREMENT R3-P1-004; AC-CUA-004: rejected observations are exact no-ops for the independent mechanics rig.
        [Test]
        public void AC_CUA_004_R6_RejectedObservationLeavesIndependentMechanicsProbeExact()
        {
            using (var probe = MechanicsProbe.Create())
            using (var fixture = AcceptedFixture.Create())
            {
                var projection = new Projection(); var adapter = fixture.NewAdapter(new MemoryPort(), projection);
                Assert.That(adapter.CompletedSnapshots.Observe(new CompletedActorSnapshotV1(401, "fixture", "idle", 0, "left", 5, 6)), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                Assert.That(adapter.TrySelect(fixture.Package), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                var before = MechanicsProbeSnapshot.Capture(probe); var projectionCount = projection.Count;
                Assert.That(adapter.CompletedSnapshots.Observe(new CompletedActorSnapshotV1(402, "fixture", "idle", 0, "up", 5, 6)), Is.EqualTo(CostumeUnityAdapterRejectionV1.Snapshot));
                Assert.That(projection.Count, Is.EqualTo(projectionCount));
                AssertMechanicsProbeExact(before, probe);
            }
        }

        // REQUIREMENT R3-P1-004; AC-CUA-004: post-durable projection failure cannot reach an independent mechanics rig.
        [Test]
        public void AC_CUA_004_R6_ProjectionFaultLeavesIndependentMechanicsProbeExact()
        {
            using (var probe = MechanicsProbe.Create())
            using (var fixture = AcceptedFixture.Create())
            {
                var projection = new Projection { Throw = true }; var adapter = fixture.NewAdapter(new MemoryPort(), projection);
                var before = MechanicsProbeSnapshot.Capture(probe);
                Assert.That(adapter.CompletedSnapshots.Observe(new CompletedActorSnapshotV1(501, "fixture", "idle", 0, "right", 5, 6)), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
                AssertMechanicsProbeExact(before, probe);
                Assert.That(adapter.TrySelect(fixture.Package), Is.EqualTo(CostumeUnityAdapterRejectionV1.ReloadRequired));
                Assert.That(adapter.Published, Is.Not.Null); Assert.That(adapter.PersistenceStatus, Is.EqualTo(CostumeUnityPersistenceStatusV1.ReloadRequired));
                AssertMechanicsProbeExact(before, probe);
            }
        }

        private static CostumeUnityPackageRejectionV1 ExpectedB1b1Validation(string mutation)
        {
            if (mutation == "portraitBytes" || mutation == "atlasBytes" || mutation == "clipMapBytes") return CostumeUnityPackageRejectionV1.Hash;
            if (mutation == "portraitFilterDrift" || mutation == "atlasFilterDrift") return CostumeUnityPackageRejectionV1.Filter;
            if (mutation == "ppuMismatch") return CostumeUnityPackageRejectionV1.Ppu;
            return CostumeUnityPackageRejectionV1.Reference;
        }

        private static byte[] MutateBytes(byte[] bytes)
        {
            var result = (byte[])bytes.Clone();
            result[0] ^= 0x7f;
            return result;
        }

        private static CostumePresentationBindingV1 BindingFor(CostumeDefinitionV1 definition)
        {
            return new CostumePresentationBindingV1("fixture", definition.CostumeId, definition.PortraitSetId, definition.PortraitHash, definition.GameplaySetId, definition.AtlasHash, definition.ClipMapHash, definition.PresentationRevision, 9, ActionIds());
        }

        private static CostumeDefinitionV1 CreateB1b1Definition(string costumeId, Texture2D portrait, Texture2D atlas, int ppu)
        {
            var seed = new CostumeUnityMediaPackageV1("fixture", costumeId, "fixture.portrait.v1", "fixture.gameplay.v1", 9, 5, true, portrait, atlas, new byte[] { 1, 2 }, new byte[] { 3, 4 }, new byte[0], new byte[0], ppu, Clips());
            return new CostumeDefinitionV1("fixture", costumeId, "synthetic", new string('b', 64), "synthetic-hair", CostumeDisposition.Accepted, false, "fixture.portrait.v1", "fixture.gameplay.v1", CostumeUnityMediaPackageV1.Hash(seed.PortraitBytes), CostumeUnityMediaPackageV1.Hash(seed.AtlasBytes), CostumeUnityMediaPackageV1.Hash(seed.CanonicalClipMapBytes()), 5);
        }

        private static CostumeUnityMediaPackageV1 CreateB1b1Package(CostumeDefinitionV1 definition, Texture2D portrait, Texture2D atlas, Texture2D canonicalAtlas, int ppu, byte[] portraitBytes, byte[] atlasBytes, byte[] clipMapBytes)
        {
            var sourcePortraitBytes = new byte[] { 1, 2 };
            var sourceAtlasBytes = new byte[] { 3, 4 };
            // canonicalAtlas exists solely to construct canonical bytes when the candidate deliberately has a null atlas.
            var seed = new CostumeUnityMediaPackageV1("fixture", definition.CostumeId, definition.PortraitSetId, definition.GameplaySetId, 9, definition.PresentationRevision, true, portrait, canonicalAtlas, sourcePortraitBytes, sourceAtlasBytes, new byte[0], new byte[0], ppu, Clips());
            var canonicalClipMap = seed.CanonicalClipMapBytes();
            return new CostumeUnityMediaPackageV1("fixture", definition.CostumeId, definition.PortraitSetId, definition.GameplaySetId, 9, definition.PresentationRevision, true, portrait, atlas, portraitBytes ?? sourcePortraitBytes, atlasBytes ?? sourceAtlasBytes, clipMapBytes ?? canonicalClipMap, seed.CanonicalSourceManifestBytes(definition), ppu, Clips());
        }

        private static CostumeUnityPackageRejectionV1 ExpectedB1b2Validation(string mutation)
        {
            if (mutation == "pivotXMismatch" || mutation == "pivotYMismatch") return CostumeUnityPackageRejectionV1.Pivot;
            if (mutation == "baselineMismatch") return CostumeUnityPackageRejectionV1.Baseline;
            if (mutation == "withinClipAdjacentDuplicate" || mutation == "withinClipNonAdjacentDuplicate") return CostumeUnityPackageRejectionV1.Clips;
            return CostumeUnityPackageRejectionV1.Geometry;
        }

        private static CostumeUnityMediaPackageV1 CreateB1b2Package(string costumeId, Texture2D portrait, Texture2D atlas, CostumeUnityClipV1[] clips, out CostumeDefinitionV1 definition)
        {
            var seed = new CostumeUnityMediaPackageV1("fixture", costumeId, "fixture.portrait.v1", "fixture.gameplay.v1", 9, 5, true, portrait, atlas, new byte[] { 1, 2 }, new byte[] { 3, 4 }, new byte[0], new byte[0], CostumeUnityMediaPackageV1.SyntheticApprovedPpu, clips);
            var clipMap = seed.CanonicalClipMapBytes();
            definition = new CostumeDefinitionV1("fixture", costumeId, "synthetic", new string('b', 64), "synthetic-hair", CostumeDisposition.Accepted, false, "fixture.portrait.v1", "fixture.gameplay.v1", CostumeUnityMediaPackageV1.Hash(seed.PortraitBytes), CostumeUnityMediaPackageV1.Hash(seed.AtlasBytes), CostumeUnityMediaPackageV1.Hash(clipMap), 5);
            return new CostumeUnityMediaPackageV1("fixture", costumeId, "fixture.portrait.v1", "fixture.gameplay.v1", 9, 5, true, portrait, atlas, seed.PortraitBytes, seed.AtlasBytes, clipMap, seed.CanonicalSourceManifestBytes(definition), CostumeUnityMediaPackageV1.SyntheticApprovedPpu, clips);
        }

        private static CostumeUnityClipV1[] B1b2Clips(string mutation)
        {
            var clips = Clips();
            if (mutation == "ordinaryXOverrun") return ReplaceClip(clips, "idle", "left", new[] { new CostumeUnityFrameV1(4, 0, 1, 1, 500, 0, 0, 1) });
            if (mutation == "ordinaryYOverrun") return ReplaceClip(clips, "idle", "left", new[] { new CostumeUnityFrameV1(0, 2, 1, 1, 500, 0, 0, 1) });
            if (mutation == "rightOverrun") return ReplaceClip(clips, "idle", "left", new[] { new CostumeUnityFrameV1(3, 0, 2, 1, 500, 0, 0, 1) });
            if (mutation == "topOverrun") return ReplaceClip(clips, "idle", "left", new[] { new CostumeUnityFrameV1(0, 1, 1, 2, 500, 0, 0, 1) });
            if (mutation == "xInt32MaxValueWidth1") return ReplaceClip(clips, "idle", "left", new[] { new CostumeUnityFrameV1(Int32.MaxValue, 0, 1, 1, 500, 0, 0, 1) });
            if (mutation == "xInt32MaxMinusOneWidthInt32MaxValue") return ReplaceClip(clips, "idle", "left", new[] { new CostumeUnityFrameV1(Int32.MaxValue - 1, 0, Int32.MaxValue, 1, 500, 0, 0, 1) });
            if (mutation == "yInt32MaxValueHeight1") return ReplaceClip(clips, "idle", "left", new[] { new CostumeUnityFrameV1(0, Int32.MaxValue, 1, 1, 500, 0, 0, 1) });
            if (mutation == "yInt32MaxMinusOneHeightInt32MaxValue") return ReplaceClip(clips, "idle", "left", new[] { new CostumeUnityFrameV1(0, Int32.MaxValue - 1, 1, Int32.MaxValue, 500, 0, 0, 1) });
            if (mutation == "pivotXMismatch") return ReplaceClip(clips, "idle", "left", new[] { new CostumeUnityFrameV1(0, 0, 1, 1, 499, 0, 0, 1) });
            if (mutation == "pivotYMismatch") return ReplaceClip(clips, "idle", "left", new[] { new CostumeUnityFrameV1(0, 0, 1, 1, 500, 1, 0, 1) });
            if (mutation == "baselineMismatch") return ReplaceClip(clips, "idle", "left", new[] { new CostumeUnityFrameV1(0, 0, 1, 1, 500, 0, 1, 1) });
            if (mutation == "withinClipAdjacentDuplicate") { var repeated = new CostumeUnityFrameV1(0, 0, 1, 1, 500, 0, 0, 1); return ReplaceClip(clips, "idle", "left", new[] { repeated, repeated }); }
            if (mutation == "withinClipNonAdjacentDuplicate") { var repeated = new CostumeUnityFrameV1(0, 0, 1, 1, 500, 0, 0, 1); return ReplaceClip(clips, "idle", "left", new[] { repeated, new CostumeUnityFrameV1(1, 0, 1, 1, 500, 0, 0, 1), repeated }); }
            if (mutation == "exactRightTopEdge") return ReplaceClip(clips, "idle", "left", new[] { new CostumeUnityFrameV1(3, 0, 1, 1, 500, 0, 0, 1), new CostumeUnityFrameV1(2, 1, 1, 1, 500, 0, 0, 1) });
            if (mutation == "crossClipRectangleReuse")
            {
                var shared = new CostumeUnityFrameV1(3, 1, 1, 1, 500, 0, 0, 1);
                clips = ReplaceClip(clips, "idle", "left", new[] { shared, new CostumeUnityFrameV1(2, 1, 1, 1, 500, 0, 0, 1) });
                return ReplaceClip(clips, "locomotion", "left", new[] { shared, new CostumeUnityFrameV1(1, 1, 1, 1, 500, 0, 0, 1) });
            }
            throw new ArgumentOutOfRangeException(nameof(mutation));
        }

        private static CostumeUnityClipV1[] ReplaceClip(CostumeUnityClipV1[] clips, string actionId, string facing, CostumeUnityFrameV1[] frames)
        {
            var result = (CostumeUnityClipV1[])clips.Clone();
            for (var i = 0; i < result.Length; i++)
            {
                if (result[i].ActionId == actionId && result[i].Facing == facing)
                {
                    result[i] = new CostumeUnityClipV1(actionId, facing, result[i].Loop, frames);
                    return result;
                }
            }
            throw new ArgumentException("Missing fixture clip.");
        }

        private static CostumeUnityMediaPackageV1 CreateB2Package(AcceptedFixture fixture, byte[] clipMapBytes, out CostumeDefinitionV1 definition)
        {
            var source = fixture.PackageTwo;
            definition = new CostumeDefinitionV1(source.ActorId, source.CostumeId, "synthetic", new string('c', 64), "synthetic-hair", CostumeDisposition.Accepted, false, source.PortraitSetId, source.GameplaySetId, CostumeUnityMediaPackageV1.Hash(source.PortraitBytes), CostumeUnityMediaPackageV1.Hash(source.AtlasBytes), CostumeUnityMediaPackageV1.Hash(clipMapBytes), source.PresentationRevision);
            return new CostumeUnityMediaPackageV1(source.ActorId, source.CostumeId, source.PortraitSetId, source.GameplaySetId, source.CatalogRevision, source.PresentationRevision, true, source.Portrait, source.Atlas, source.PortraitBytes, source.AtlasBytes, clipMapBytes, source.CanonicalSourceManifestBytes(definition), source.Ppu, source.Clips);
        }

        private static CostumeUnityMediaPackageV1 CreateB3Package(AcceptedFixture fixture, byte[] sourceManifestBytes)
        {
            var source = fixture.PackageTwo;
            return new CostumeUnityMediaPackageV1(source.ActorId, source.CostumeId, source.PortraitSetId, source.GameplaySetId, source.CatalogRevision, source.PresentationRevision, true, source.Portrait, source.Atlas, source.PortraitBytes, source.AtlasBytes, source.ClipMapBytes, sourceManifestBytes, source.Ppu, source.Clips);
        }

        private static CostumeUnityMediaPackageV1 CreateForeignPackage(AcceptedFixture fixture, out CostumeDefinitionV1 definition)
        {
            var source = fixture.PackageTwo;
            var seed = new CostumeUnityMediaPackageV1("foreign", "foreign.costume.v1", "foreign.portrait.v1", "foreign.gameplay.v1", source.CatalogRevision, source.PresentationRevision, true, source.Portrait, source.Atlas, source.PortraitBytes, source.AtlasBytes, new byte[0], new byte[0], source.Ppu, Clips());
            var clipMap = seed.CanonicalClipMapBytes();
            definition = new CostumeDefinitionV1("foreign", "foreign.costume.v1", "synthetic", new string('f', 64), "synthetic-hair", CostumeDisposition.Accepted, false, "foreign.portrait.v1", "foreign.gameplay.v1", CostumeUnityMediaPackageV1.Hash(seed.PortraitBytes), CostumeUnityMediaPackageV1.Hash(seed.AtlasBytes), CostumeUnityMediaPackageV1.Hash(clipMap), seed.PresentationRevision);
            return new CostumeUnityMediaPackageV1(seed.ActorId, seed.CostumeId, seed.PortraitSetId, seed.GameplaySetId, seed.CatalogRevision, seed.PresentationRevision, true, seed.Portrait, seed.Atlas, seed.PortraitBytes, seed.AtlasBytes, clipMap, seed.CanonicalSourceManifestBytes(definition), seed.Ppu, seed.Clips);
        }

        private static CostumeUnityMediaPackageV1 CreateStalePackage(AcceptedFixture fixture, CostumeDefinitionV1 definition)
        {
            var source = fixture.PackageTwo;
            var staleRevision = source.CatalogRevision - 1;
            var seed = new CostumeUnityMediaPackageV1(source.ActorId, source.CostumeId, source.PortraitSetId, source.GameplaySetId, staleRevision, source.PresentationRevision, true, source.Portrait, source.Atlas, source.PortraitBytes, source.AtlasBytes, source.ClipMapBytes, new byte[0], source.Ppu, source.Clips);
            return new CostumeUnityMediaPackageV1(seed.ActorId, seed.CostumeId, seed.PortraitSetId, seed.GameplaySetId, seed.CatalogRevision, seed.PresentationRevision, true, seed.Portrait, seed.Atlas, seed.PortraitBytes, seed.AtlasBytes, seed.ClipMapBytes, seed.CanonicalSourceManifestBytes(definition), seed.Ppu, seed.Clips);
        }

        private static CostumePresentationBindingV1 BindingForMedia(CostumeDefinitionV1 definition, CostumeUnityMediaPackageV1 media)
        {
            return new CostumePresentationBindingV1(definition.ActorId, definition.CostumeId, definition.PortraitSetId, definition.PortraitHash, definition.GameplaySetId, definition.AtlasHash, definition.ClipMapHash, definition.PresentationRevision, media.CatalogRevision, ActionIds(media));
        }

        private static CostumeUnityClipV1[] ClipsWithoutAction(string actionId)
        {
            var clips = new List<CostumeUnityClipV1>();
            foreach (var clip in Clips()) if (clip.ActionId != actionId) clips.Add(clip);
            return clips.ToArray();
        }

        private static byte[] MutateB4ClipMapBytes(byte[] canonicalBytes, string mutation)
        {
            var text = new UTF8Encoding(false, true).GetString(canonicalBytes);
            if (mutation == "clipPropertyOrder") return Utf8(ReplaceExactOnce(text, "{\"actionId\":\"airborne\",\"facing\":\"left\",\"loop\":false,\"frames\":[", "{\"facing\":\"left\",\"actionId\":\"airborne\",\"loop\":false,\"frames\":["));
            if (mutation == "framePropertyOrder") return Utf8(ReplaceExactOnce(text, "{\"x\":0,\"y\":0,\"width\":1,\"height\":1", "{\"y\":0,\"x\":0,\"width\":1,\"height\":1"));
            throw new ArgumentOutOfRangeException(nameof(mutation));
        }

        private static void AssertB1aCandidateRejection(AcceptedFixture fixture, CostumeUnityMediaPackageV1 candidate, CostumeUnityAdapterRejectionV1 expected, string mutation)
        {
            var port = new MemoryPort(); var projection = new Projection(); var adapter = PublishedAdapter(fixture, port, projection);
            Assert.That(adapter.Highlight(fixture.PackageTwo.CostumeId), Is.EqualTo(CostumeUnityAdapterRejectionV1.HighlightOnly));
            var oldPublished = adapter.Published; var oldMedia = oldPublished.Media; var oldBinding = oldPublished.Binding;
            var beforeView = DeepViewSnapshot.Capture(adapter.SnapshotView());
            var moves = port.MoveCalls; var replaces = port.ReplaceCalls; var projections = projection.Count; var status = adapter.PersistenceStatus;
            Assert.That(adapter.TrySelect(candidate), Is.EqualTo(expected), mutation);
            AssertB1aPublishedPairPreserved(adapter, port, projection, oldPublished, oldMedia, oldBinding, beforeView, moves, replaces, projections, status);
        }

        private static void AssertCompleteTuple(PublishedCostumePresentationV1 published, CostumeUnityMediaPackageV1 media, CompletedActorSnapshotV1 snapshot)
        {
            Assert.That(published, Is.Not.Null); Assert.That(published.Media, Is.SameAs(media)); Assert.That(published.State.Current("fixture"), Is.EqualTo(media.CostumeId)); Assert.That(published.Binding.ActorId, Is.EqualTo("fixture")); Assert.That(published.Binding.CostumeId, Is.EqualTo(media.CostumeId)); Assert.That(SnapshotEqual(published.Snapshot, snapshot), Is.True);
            var expectedFrame = media.FindClip(snapshot.ActionId, snapshot.Facing).FrameFor(snapshot.ActionAgeQ1000);
            Assert.That(FrameEqual(published.Frame, expectedFrame), Is.True);
        }

        private static bool SnapshotEqual(CompletedActorSnapshotV1 a, CompletedActorSnapshotV1 b)
        {
            return a.Tick == b.Tick && a.ActorId == b.ActorId && a.ActionId == b.ActionId && a.ActionAgeQ1000 == b.ActionAgeQ1000 && a.Facing == b.Facing && a.AnchorX == b.AnchorX && a.AnchorY == b.AnchorY;
        }

        private static bool FrameEqual(CostumeUnityFrameV1 a, CostumeUnityFrameV1 b)
        {
            return a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height && a.PivotXQ1000 == b.PivotXQ1000 && a.PivotYQ1000 == b.PivotYQ1000 && a.BaselineY == b.BaselineY && a.DurationTicks == b.DurationTicks;
        }

        private static void AssertDurableState(MemoryPort port, string fileName, long expectedRevision, string expectedCurrentId)
        {
            CostumeStateV1 state; Assert.That(CostumeCanonicalCodecV1.TryDecode(port.BytesForFileName(fileName), out state), Is.True, fileName); Assert.That(state.Revision, Is.EqualTo(expectedRevision), fileName); Assert.That(state.Current("fixture"), Is.EqualTo(expectedCurrentId), fileName);
        }

        private static CostumeStateV1 StageSelection(CostumeCatalogV1 catalog, CostumeStateV1 current, string costumeId)
        {
            var preparation = CostumeSelectionServiceV1.PrepareSelection(catalog, current, "fixture", costumeId, current.Revision, catalog.Revision);
            Assert.That(preparation.IsPrepared, Is.True);
            return CostumeSelectionServiceV1.Commit(preparation, catalog, current);
        }

        private static void AssertTempMissing(MemoryPort port)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), "cua-isolated-memory-port", CostumeFileAdapterV1.TempFileName);
            Assert.That(port.Inspect(tempPath).Kind, Is.EqualTo(CostumeFilePathKindV1.Missing));
            Assert.That(port.BytesForFileName(CostumeFileAdapterV1.TempFileName), Is.Null);
        }

        private static void AssertLastSaveResult(CostumeUnityPresentationAdapterV1 adapter, CostumeFileSaveOutcomeV1 outcome, CostumeFileSaveStageV1 stage, long expectedRevision)
        {
            var type = typeof(CostumeUnityPresentationAdapterV1);
            var hasLastSaveResult = type.GetField("_hasLastSaveResult", BindingFlags.Instance | BindingFlags.NonPublic);
            var lastSaveResult = type.GetField("_lastSaveResult", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.That(hasLastSaveResult, Is.Not.Null); Assert.That(lastSaveResult, Is.Not.Null); Assert.That((bool)hasLastSaveResult.GetValue(adapter), Is.True);
            var result = (CostumeFileSaveResultV1)lastSaveResult.GetValue(adapter);
            Assert.That(result.Outcome, Is.EqualTo(outcome)); Assert.That(result.Stage, Is.EqualTo(stage)); Assert.That(result.ExpectedRevision, Is.EqualTo(expectedRevision));
        }

        private static void AssertByteEqual(byte[] actual, byte[] expected)
        {
            Assert.That(actual, Is.Not.Null); Assert.That(expected, Is.Not.Null); Assert.That(actual.Length, Is.EqualTo(expected.Length)); for (var i = 0; i < actual.Length; i++) Assert.That(actual[i], Is.EqualTo(expected[i]), i.ToString());
        }

        private static bool ByteArraysEqual(byte[] left, byte[] right)
        {
            if (left == null || right == null || left.Length != right.Length) return false;
            for (var i = 0; i < left.Length; i++) if (left[i] != right[i]) return false;
            return true;
        }

        private static void AssertMechanicsProbeExact(MechanicsProbeSnapshot expected, MechanicsProbe probe)
        {
            var actual = MechanicsProbeSnapshot.Capture(probe);
            Assert.That(actual.OwnerReference, Is.SameAs(expected.OwnerReference), "GameObject identity"); Assert.That(actual.TransformReference, Is.SameAs(expected.TransformReference), "Transform identity"); Assert.That(actual.BodyReference, Is.SameAs(expected.BodyReference), "Rigidbody2D identity"); Assert.That(actual.ColliderReference, Is.SameAs(expected.ColliderReference), "BoxCollider2D identity");
            Assert.That(actual.OwnerReference.transform, Is.SameAs(actual.TransformReference), "owner/transform relation"); Assert.That(actual.BodyReference.gameObject, Is.SameAs(actual.OwnerReference), "body/owner relation"); Assert.That(actual.BodyReference.transform, Is.SameAs(actual.TransformReference), "body/transform relation"); Assert.That(actual.ColliderReference.gameObject, Is.SameAs(actual.OwnerReference), "collider/owner relation"); Assert.That(actual.ColliderReference.transform, Is.SameAs(actual.TransformReference), "collider/transform relation"); Assert.That(actual.ColliderReference.attachedRigidbody, Is.SameAs(actual.BodyReference), "collider/body relation");
            Assert.That(actual.Position, Is.EqualTo(expected.Position), "transform position"); Assert.That(actual.Rotation, Is.EqualTo(expected.Rotation), "transform rotation"); Assert.That(actual.LocalScale, Is.EqualTo(expected.LocalScale), "transform localScale");
            Assert.That(actual.BodyType, Is.EqualTo(expected.BodyType), "bodyType"); Assert.That(actual.Simulated, Is.EqualTo(expected.Simulated), "simulated"); Assert.That(actual.UseAutoMass, Is.EqualTo(expected.UseAutoMass), "useAutoMass"); Assert.That(actual.UseFullKinematicContacts, Is.EqualTo(expected.UseFullKinematicContacts), "useFullKinematicContacts"); Assert.That(actual.GravityScale, Is.EqualTo(expected.GravityScale), "gravityScale"); Assert.That(actual.Mass, Is.EqualTo(expected.Mass), "mass"); Assert.That(actual.LinearDamping, Is.EqualTo(expected.LinearDamping), "linearDamping"); Assert.That(actual.AngularDamping, Is.EqualTo(expected.AngularDamping), "angularDamping"); Assert.That(actual.CenterOfMass, Is.EqualTo(expected.CenterOfMass), "centerOfMass"); Assert.That(actual.WorldCenterOfMass, Is.EqualTo(expected.WorldCenterOfMass), "worldCenterOfMass"); Assert.That(actual.Inertia, Is.EqualTo(expected.Inertia), "inertia"); Assert.That(actual.BodyPosition, Is.EqualTo(expected.BodyPosition), "Rigidbody2D position"); Assert.That(actual.BodyRotation, Is.EqualTo(expected.BodyRotation), "Rigidbody2D rotation"); Assert.That(actual.LinearVelocity, Is.EqualTo(expected.LinearVelocity), "linearVelocity"); Assert.That(actual.AngularVelocity, Is.EqualTo(expected.AngularVelocity), "angularVelocity"); Assert.That(actual.Constraints, Is.EqualTo(expected.Constraints), "constraints"); Assert.That(actual.FreezeRotation, Is.EqualTo(expected.FreezeRotation), "freezeRotation"); Assert.That(actual.FreezeRotationConstraint, Is.EqualTo(expected.FreezeRotationConstraint), "freezeRotation constraints oracle"); Assert.That(actual.FreezeRotation, Is.EqualTo(actual.FreezeRotationConstraint), "freezeRotation must agree with constraints"); Assert.That(actual.CollisionDetectionMode, Is.EqualTo(expected.CollisionDetectionMode), "collisionDetectionMode"); Assert.That(actual.Interpolation, Is.EqualTo(expected.Interpolation), "interpolation"); Assert.That(actual.SleepMode, Is.EqualTo(expected.SleepMode), "sleepMode"); Assert.That(actual.IncludeLayers, Is.EqualTo(expected.IncludeLayers), "includeLayers"); Assert.That(actual.ExcludeLayers, Is.EqualTo(expected.ExcludeLayers), "excludeLayers"); Assert.That(actual.BodyMaterial, Is.SameAs(expected.BodyMaterial), "Rigidbody2D sharedMaterial identity");
            Assert.That(actual.ColliderEnabled, Is.EqualTo(expected.ColliderEnabled), "collider enabled"); Assert.That(actual.ColliderTrigger, Is.EqualTo(expected.ColliderTrigger), "collider isTrigger"); Assert.That(actual.ColliderOffset, Is.EqualTo(expected.ColliderOffset), "collider offset"); Assert.That(actual.ColliderSize, Is.EqualTo(expected.ColliderSize), "collider size"); Assert.That(actual.ColliderMaterial, Is.SameAs(expected.ColliderMaterial), "collider sharedMaterial identity"); Assert.That(actual.AttachedColliderCount, Is.EqualTo(expected.AttachedColliderCount), "attachedColliderCount"); Assert.That(actual.AttachedColliderQueryCount, Is.EqualTo(expected.AttachedColliderQueryCount), "GetAttachedColliders count"); Assert.That(actual.AttachedColliderCount, Is.EqualTo(actual.AttachedColliderQueryCount), "attachedColliderCount/query agreement"); Assert.That(actual.AttachedColliderCount, Is.EqualTo(actual.AttachedColliders.Length), "attached collider query count"); Assert.That(actual.AttachedColliders.Length, Is.EqualTo(expected.AttachedColliders.Length), "attached collider identity count"); for (var i = 0; i < actual.AttachedColliders.Length; i++) Assert.That(actual.AttachedColliders[i], Is.SameAs(expected.AttachedColliders[i]), "attached collider identity " + i); Assert.That(actual.AttachedColliders, Has.Member(actual.ColliderReference), "attached collider identity");
            Assert.That(actual.HitReference, Is.SameAs(expected.HitReference), "hit reference"); Assert.That(actual.HitToken, Is.EqualTo(expected.HitToken), "hit token"); Assert.That(actual.HurtReference, Is.SameAs(expected.HurtReference), "hurt reference"); Assert.That(actual.HurtToken, Is.EqualTo(expected.HurtToken), "hurt token"); Assert.That(actual.AimReference, Is.SameAs(expected.AimReference), "aim reference"); Assert.That(actual.AimToken, Is.EqualTo(expected.AimToken), "aim token"); Assert.That(actual.TransferReference, Is.SameAs(expected.TransferReference), "transfer reference"); Assert.That(actual.TransferToken, Is.EqualTo(expected.TransferToken), "transfer token");
            Assert.That(actual.Attack, Is.EqualTo(expected.Attack), "attack stat"); Assert.That(actual.Defense, Is.EqualTo(expected.Defense), "defense stat"); Assert.That(actual.HitPoints, Is.EqualTo(expected.HitPoints), "HP"); Assert.That(actual.CooldownTicks, Is.EqualTo(expected.CooldownTicks), "cooldown"); CollectionAssert.AreEqual(expected.AbilityIds, actual.AbilityIds, "ability set");
            Assert.That(actual.TargetReference, Is.SameAs(expected.TargetReference), "target reference"); Assert.That(actual.TargetToken, Is.EqualTo(expected.TargetToken), "target token"); Assert.That(actual.Phase, Is.EqualTo(expected.Phase), "phase"); Assert.That(actual.AiState, Is.EqualTo(expected.AiState), "AI state"); Assert.That(actual.SimulationCounter, Is.EqualTo(expected.SimulationCounter), "simulation counter"); Assert.That(actual.FrameCounter, Is.EqualTo(expected.FrameCounter), "frame counter");
        }

        private sealed class MechanicsReference
        {
            public MechanicsReference(string token) { Token = token; }
            public string Token;
        }

        // R3-P1-004 private test-only rig: it is intentionally not given to the adapter, projection, or CIO.
        private sealed class MechanicsProbe : IDisposable
        {
            private readonly GameObject _owner;
            private readonly PhysicsMaterial2D _material;
            private MechanicsProbe(GameObject owner, PhysicsMaterial2D material, Rigidbody2D body, BoxCollider2D collider)
            {
                _owner = owner; _material = material; Body = body; Collider = collider;
                Hit = new MechanicsReference("hit-token"); Hurt = new MechanicsReference("hurt-token"); Aim = new MechanicsReference("aim-token"); Transfer = new MechanicsReference("transfer-token"); Target = new MechanicsReference("target-token");
                Attack = 37; Defense = 19; HitPoints = 83; CooldownTicks = 11; AbilityIds = new HashSet<string>(StringComparer.Ordinal) { "dash", "parry", "roll" }; Phase = "combat"; AiState = "seek"; SimulationCounter = 701; FrameCounter = 313;
            }

            public Rigidbody2D Body { get; }
            public BoxCollider2D Collider { get; }
            public GameObject Owner { get { return _owner; } }
            public MechanicsReference Hit;
            public MechanicsReference Hurt;
            public MechanicsReference Aim;
            public MechanicsReference Transfer;
            public MechanicsReference Target;
            public int Attack;
            public int Defense;
            public int HitPoints;
            public int CooldownTicks;
            public HashSet<string> AbilityIds;
            public string Phase;
            public string AiState;
            public long SimulationCounter;
            public long FrameCounter;

            public static MechanicsProbe Create()
            {
                var owner = new GameObject("cua-r6-independent-mechanics-probe");
                var body = owner.AddComponent<Rigidbody2D>(); var collider = owner.AddComponent<BoxCollider2D>(); var material = new PhysicsMaterial2D("cua-r6-independent-material");
                owner.transform.position = new Vector3(31, -8, 4); owner.transform.rotation = Quaternion.Euler(13, 27, 41); owner.transform.localScale = new Vector3(2, 3, 1);
                body.bodyType = RigidbodyType2D.Dynamic; body.simulated = true; body.useAutoMass = false; body.useFullKinematicContacts = true; body.gravityScale = 2.5f; body.mass = 3.75f; body.linearDamping = 0.375f; body.angularDamping = 0.625f; body.centerOfMass = new Vector2(0.25f, -0.5f); body.inertia = 8.25f; body.position = new Vector2(31, -8); body.rotation = 41f; body.linearVelocity = new Vector2(7.5f, -4.25f); body.angularVelocity = 31.5f; body.constraints = RigidbodyConstraints2D.FreezePositionX; body.freezeRotation = true; body.collisionDetectionMode = CollisionDetectionMode2D.Continuous; body.interpolation = RigidbodyInterpolation2D.Interpolate; body.sleepMode = RigidbodySleepMode2D.NeverSleep; body.includeLayers = 1 << 8; body.excludeLayers = 1 << 10; body.sharedMaterial = material;
                collider.enabled = true; collider.isTrigger = true; collider.offset = new Vector2(-2.5f, 6.75f); collider.size = new Vector2(7, 9); collider.sharedMaterial = material;
                return new MechanicsProbe(owner, material, body, collider);
            }

            public void Dispose()
            {
                UnityEngine.Object.DestroyImmediate(_owner);
                UnityEngine.Object.DestroyImmediate(_material);
            }
        }

        private sealed class MechanicsProbeSnapshot
        {
            public GameObject OwnerReference; public Transform TransformReference; public Rigidbody2D BodyReference; public BoxCollider2D ColliderReference;
            public Vector3 Position; public Quaternion Rotation; public Vector3 LocalScale;
            // IsSleeping/IsAwake and accumulated force/torque are physics-step transients, not configuration; this synchronous test never advances simulation and deliberately excludes them. Unity 6 legacy aliases (velocity, drag, angularDrag, isKinematic) add no independent state beyond the source-of-truth settings captured below.
            public RigidbodyType2D BodyType; public bool Simulated; public bool UseAutoMass; public bool UseFullKinematicContacts; public float GravityScale; public float Mass; public float LinearDamping; public float AngularDamping; public Vector2 CenterOfMass; public Vector2 WorldCenterOfMass; public float Inertia; public Vector2 BodyPosition; public float BodyRotation; public Vector2 LinearVelocity; public float AngularVelocity; public RigidbodyConstraints2D Constraints; public bool FreezeRotation; public bool FreezeRotationConstraint; public CollisionDetectionMode2D CollisionDetectionMode; public RigidbodyInterpolation2D Interpolation; public RigidbodySleepMode2D SleepMode; public int IncludeLayers; public int ExcludeLayers; public PhysicsMaterial2D BodyMaterial;
            public bool ColliderEnabled; public bool ColliderTrigger; public Vector2 ColliderOffset; public Vector2 ColliderSize; public PhysicsMaterial2D ColliderMaterial; public int AttachedColliderCount; public int AttachedColliderQueryCount; public Collider2D[] AttachedColliders;
            public MechanicsReference HitReference; public string HitToken; public MechanicsReference HurtReference; public string HurtToken; public MechanicsReference AimReference; public string AimToken; public MechanicsReference TransferReference; public string TransferToken;
            public int Attack; public int Defense; public int HitPoints; public int CooldownTicks; public string[] AbilityIds;
            public MechanicsReference TargetReference; public string TargetToken; public string Phase; public string AiState; public long SimulationCounter; public long FrameCounter;

            public static MechanicsProbeSnapshot Capture(MechanicsProbe probe)
            {
                var abilities = new string[probe.AbilityIds.Count]; probe.AbilityIds.CopyTo(abilities); Array.Sort(abilities, StringComparer.Ordinal);
                var attached = new List<Collider2D>(); var attachedCount = probe.Body.GetAttachedColliders(attached);
                return new MechanicsProbeSnapshot
                {
                    OwnerReference = probe.Owner, TransformReference = probe.Owner.transform, BodyReference = probe.Body, ColliderReference = probe.Collider,
                    Position = probe.Body.transform.position, Rotation = probe.Body.transform.rotation, LocalScale = probe.Body.transform.localScale,
                    BodyType = probe.Body.bodyType, Simulated = probe.Body.simulated, UseAutoMass = probe.Body.useAutoMass, UseFullKinematicContacts = probe.Body.useFullKinematicContacts, GravityScale = probe.Body.gravityScale, Mass = probe.Body.mass, LinearDamping = probe.Body.linearDamping, AngularDamping = probe.Body.angularDamping, CenterOfMass = probe.Body.centerOfMass, WorldCenterOfMass = probe.Body.worldCenterOfMass, Inertia = probe.Body.inertia, BodyPosition = probe.Body.position, BodyRotation = probe.Body.rotation, LinearVelocity = probe.Body.linearVelocity, AngularVelocity = probe.Body.angularVelocity, Constraints = probe.Body.constraints, FreezeRotation = probe.Body.freezeRotation, FreezeRotationConstraint = (probe.Body.constraints & RigidbodyConstraints2D.FreezeRotation) != 0, CollisionDetectionMode = probe.Body.collisionDetectionMode, Interpolation = probe.Body.interpolation, SleepMode = probe.Body.sleepMode, IncludeLayers = probe.Body.includeLayers, ExcludeLayers = probe.Body.excludeLayers, BodyMaterial = probe.Body.sharedMaterial,
                    ColliderEnabled = probe.Collider.enabled, ColliderTrigger = probe.Collider.isTrigger, ColliderOffset = probe.Collider.offset, ColliderSize = probe.Collider.size, ColliderMaterial = probe.Collider.sharedMaterial, AttachedColliderCount = probe.Body.attachedColliderCount, AttachedColliderQueryCount = attachedCount, AttachedColliders = attached.ToArray(),
                    HitReference = probe.Hit, HitToken = probe.Hit.Token, HurtReference = probe.Hurt, HurtToken = probe.Hurt.Token, AimReference = probe.Aim, AimToken = probe.Aim.Token, TransferReference = probe.Transfer, TransferToken = probe.Transfer.Token,
                    Attack = probe.Attack, Defense = probe.Defense, HitPoints = probe.HitPoints, CooldownTicks = probe.CooldownTicks, AbilityIds = abilities,
                    TargetReference = probe.Target, TargetToken = probe.Target.Token, Phase = probe.Phase, AiState = probe.AiState, SimulationCounter = probe.SimulationCounter, FrameCounter = probe.FrameCounter
                };
            }
        }

        private static byte[] MutateSourceManifestBytes(byte[] canonicalBytes, string mutation)
        {
            if (mutation == "utf8Bom")
            {
                var result = new byte[canonicalBytes.Length + 3]; result[0] = 0xef; result[1] = 0xbb; result[2] = 0xbf;
                Array.Copy(canonicalBytes, 0, result, 3, canonicalBytes.Length); return result;
            }
            var text = new UTF8Encoding(false, true).GetString(canonicalBytes);
            if (mutation == "leadingWhitespace") return Utf8(" " + text);
            if (mutation == "trailingWhitespace") return Utf8(text + " ");
            if (mutation == "interiorWhitespace") return Utf8(ReplaceExactOnce(text, ",\"costumeId\"", ", \"costumeId\""));
            if (mutation == "reorderActorCostume") return Utf8(ReplaceExactOnce(text, "\"actorId\":\"fixture\",\"costumeId\":\"fixture.costume.synthetic2.v1\"", "\"costumeId\":\"fixture.costume.synthetic2.v1\",\"actorId\":\"fixture\""));
            if (mutation == "reorderPortraitGameplaySet") return Utf8(ReplaceExactOnce(text, "\"portraitSetId\":\"fixture.portrait.v1\",\"gameplaySetId\":\"fixture.gameplay.v1\"", "\"gameplaySetId\":\"fixture.gameplay.v1\",\"portraitSetId\":\"fixture.portrait.v1\""));
            if (mutation == "reorderCatalogPresentationRevision") return Utf8(ReplaceExactOnce(text, "\"catalogRevision\":9,\"presentationRevision\":5", "\"presentationRevision\":5,\"catalogRevision\":9"));
            if (mutation == "unknownProperty") return Utf8(ReplaceExactOnce(text, "\"actorId\":\"fixture\",", "\"actorId\":\"fixture\",\"unknown\":0,"));
            if (mutation == "duplicateProperty") return Utf8(ReplaceExactOnce(text, "\"actorId\":\"fixture\",", "\"actorId\":\"fixture\",\"actorId\":\"fixture\","));
            if (mutation == "actorId") return Utf8(ReplaceExactOnce(text, "\"actorId\":\"fixture\"", "\"actorId\":\"other\""));
            if (mutation == "costumeId") return Utf8(ReplaceExactOnce(text, "\"costumeId\":\"fixture.costume.synthetic2.v1\"", "\"costumeId\":\"other.costume\""));
            if (mutation == "portraitSetId") return Utf8(ReplaceExactOnce(text, "\"portraitSetId\":\"fixture.portrait.v1\"", "\"portraitSetId\":\"other.portrait\""));
            if (mutation == "gameplaySetId") return Utf8(ReplaceExactOnce(text, "\"gameplaySetId\":\"fixture.gameplay.v1\"", "\"gameplaySetId\":\"other.gameplay\""));
            if (mutation == "catalogRevision") return Utf8(ReplaceExactOnce(text, "\"catalogRevision\":9", "\"catalogRevision\":10"));
            if (mutation == "presentationRevision") return Utf8(ReplaceExactOnce(text, "\"presentationRevision\":5", "\"presentationRevision\":6"));
            if (mutation == "portraitHash") return Utf8(ReplaceQuotedFieldValue(text, "portraitHash", new string('b', 64)));
            if (mutation == "atlasHash") return Utf8(ReplaceQuotedFieldValue(text, "atlasHash", new string('b', 64)));
            if (mutation == "clipMapHash") return Utf8(ReplaceQuotedFieldValue(text, "clipMapHash", new string('b', 64)));
            if (mutation == "requiredActionIdsMissing") return Utf8(ReplaceExactOnce(text, "\"requiredActionIds\":[\"airborne\",", "\"requiredActionIds\":["));
            if (mutation == "requiredActionIdsExtra") return Utf8(ReplaceExactOnce(text, "\"requiredActionIds\":[\"airborne\",", "\"requiredActionIds\":[\"extra\",\"airborne\","));
            if (mutation == "requiredActionIdsUnsorted") return Utf8(ReplaceExactOnce(text, "\"airborne\",\"bowdraw\"", "\"bowdraw\",\"airborne\""));
            if (mutation == "requiredActionIdsDuplicate") return Utf8(ReplaceExactOnce(text, "\"requiredActionIds\":[\"airborne\",", "\"requiredActionIds\":[\"airborne\",\"airborne\","));
            if (mutation == "requiredFacingIdsMissingLeft") return Utf8(ReplaceExactOnce(text, "\"requiredFacingIds\":[\"left\",\"right\"]", "\"requiredFacingIds\":[\"right\"]"));
            if (mutation == "requiredFacingIdsMissingRight") return Utf8(ReplaceExactOnce(text, "\"requiredFacingIds\":[\"left\",\"right\"]", "\"requiredFacingIds\":[\"left\"]"));
            if (mutation == "requiredFacingIdsUnsorted") return Utf8(ReplaceExactOnce(text, "\"requiredFacingIds\":[\"left\",\"right\"]", "\"requiredFacingIds\":[\"right\",\"left\"]"));
            if (mutation == "requiredFacingIdsExtra") return Utf8(ReplaceExactOnce(text, "\"requiredFacingIds\":[\"left\",\"right\"]", "\"requiredFacingIds\":[\"left\",\"right\",\"up\"]"));
            if (mutation == "requiredFacingIdsDuplicate") return Utf8(ReplaceExactOnce(text, "\"requiredFacingIds\":[\"left\",\"right\"]", "\"requiredFacingIds\":[\"left\",\"left\",\"right\"]"));
            if (mutation == "clipMapSchemaVersion") return Utf8(ReplaceExactOnce(text, "\"clipMapSchemaVersion\":1", "\"clipMapSchemaVersion\":2"));
            throw new ArgumentOutOfRangeException(nameof(mutation));
        }

        private static byte[] MutateClipMapBytes(byte[] canonicalBytes, string mutation)
        {
            if (mutation == "utf8Bom")
            {
                var result = new byte[canonicalBytes.Length + 3]; result[0] = 0xef; result[1] = 0xbb; result[2] = 0xbf;
                Array.Copy(canonicalBytes, 0, result, 3, canonicalBytes.Length); return result;
            }
            var text = new UTF8Encoding(false, true).GetString(canonicalBytes);
            if (mutation == "leadingWhitespace") return Utf8(" " + text);
            if (mutation == "trailingWhitespace") return Utf8(text + " ");
            if (mutation == "interiorWhitespace") return Utf8(ReplaceExactOnce(text, ",\"actorId\"", ", \"actorId\""));
            if (mutation == "reorderSchemaActor") return Utf8(ReplaceExactOnce(text, "\"schemaVersion\":1,\"actorId\":\"fixture\"", "\"actorId\":\"fixture\",\"schemaVersion\":1"));
            if (mutation == "reorderActorCostume") return Utf8(ReplaceExactOnce(text, "\"actorId\":\"fixture\",\"costumeId\":\"fixture.costume.synthetic2.v1\"", "\"costumeId\":\"fixture.costume.synthetic2.v1\",\"actorId\":\"fixture\""));
            if (mutation == "reorderPpuCell") return Utf8(ReplaceExactOnce(text, "\"ppu\":1,\"cellWidth\":4", "\"cellWidth\":4,\"ppu\":1"));
            if (mutation == "unknownProperty") return Utf8(ReplaceExactOnce(text, "\"schemaVersion\":1,", "\"schemaVersion\":1,\"unknown\":0,"));
            if (mutation == "duplicateProperty") return Utf8(ReplaceExactOnce(text, "\"schemaVersion\":1,", "\"schemaVersion\":1,\"schemaVersion\":1,"));
            if (mutation == "schemaVersion") return Utf8(ReplaceExactOnce(text, "\"schemaVersion\":1", "\"schemaVersion\":2"));
            if (mutation == "actorId") return Utf8(ReplaceExactOnce(text, "\"actorId\":\"fixture\"", "\"actorId\":\"other\""));
            if (mutation == "costumeId") return Utf8(ReplaceExactOnce(text, "\"costumeId\":\"fixture.costume.synthetic2.v1\"", "\"costumeId\":\"other.costume\""));
            if (mutation == "gameplaySetId") return Utf8(ReplaceExactOnce(text, "\"gameplaySetId\":\"fixture.gameplay.v1\"", "\"gameplaySetId\":\"other.gameplay\""));
            if (mutation == "ppu") return Utf8(ReplaceExactOnce(text, "\"ppu\":1", "\"ppu\":2"));
            if (mutation == "cellWidth") return Utf8(ReplaceExactOnce(text, "\"cellWidth\":4", "\"cellWidth\":5"));
            if (mutation == "cellHeight") return Utf8(ReplaceExactOnce(text, "\"cellHeight\":2", "\"cellHeight\":3"));
            if (mutation == "pivotX") return Utf8(ReplaceExactOnce(text, "\"pivotXQ1000\":500,\"pivotYQ1000\":0,\"baselineY\":0,\"clips\"", "\"pivotXQ1000\":501,\"pivotYQ1000\":0,\"baselineY\":0,\"clips\""));
            if (mutation == "pivotY") return Utf8(ReplaceExactOnce(text, "\"pivotXQ1000\":500,\"pivotYQ1000\":0,\"baselineY\":0,\"clips\"", "\"pivotXQ1000\":500,\"pivotYQ1000\":1,\"baselineY\":0,\"clips\""));
            if (mutation == "baseline") return Utf8(ReplaceExactOnce(text, "\"pivotXQ1000\":500,\"pivotYQ1000\":0,\"baselineY\":0,\"clips\"", "\"pivotXQ1000\":500,\"pivotYQ1000\":0,\"baselineY\":1,\"clips\""));
            if (mutation == "clipsUnsorted") return Utf8(SwapFirstTwoObjects(text, "\"clips\":["));
            if (mutation == "duplicateActionFacing") return Utf8(ReplaceExactOnce(text, "\"actionId\":\"airborne\",\"facing\":\"right\"", "\"actionId\":\"airborne\",\"facing\":\"left\""));
            if (mutation == "missingAction") return Utf8(RemoveObject(RemoveObject(text, "\"clips\":[", 0), "\"clips\":[", 0));
            if (mutation == "missingLeft") return Utf8(RemoveObject(text, "\"clips\":[", 0));
            if (mutation == "missingRight") return Utf8(RemoveObject(text, "\"clips\":[", 1));
            if (mutation == "unknownFacing") return Utf8(ReplaceExactOnce(text, "\"facing\":\"left\"", "\"facing\":\"up\""));
            if (mutation == "frameOrderSwap") return Utf8(SwapFirstTwoObjects(text, "\"frames\":["));
            if (mutation == "frameX") return Utf8(ReplaceExactOnce(text, "\"x\":0", "\"x\":1"));
            if (mutation == "frameY") return Utf8(ReplaceExactOnce(text, "\"y\":0", "\"y\":1"));
            if (mutation == "frameWidth") return Utf8(ReplaceExactOnce(text, "\"width\":1", "\"width\":2"));
            if (mutation == "frameHeight") return Utf8(ReplaceExactOnce(text, "\"height\":1", "\"height\":2"));
            if (mutation == "framePivotX") return Utf8(ReplaceExactOnce(text, "\"x\":0,\"y\":0,\"width\":1,\"height\":1,\"pivotXQ1000\":500", "\"x\":0,\"y\":0,\"width\":1,\"height\":1,\"pivotXQ1000\":499"));
            if (mutation == "framePivotY") return Utf8(ReplaceExactOnce(text, "\"x\":0,\"y\":0,\"width\":1,\"height\":1,\"pivotXQ1000\":500,\"pivotYQ1000\":0", "\"x\":0,\"y\":0,\"width\":1,\"height\":1,\"pivotXQ1000\":500,\"pivotYQ1000\":1"));
            if (mutation == "frameBaseline") return Utf8(ReplaceExactOnce(text, "\"x\":0,\"y\":0,\"width\":1,\"height\":1,\"pivotXQ1000\":500,\"pivotYQ1000\":0,\"baselineY\":0", "\"x\":0,\"y\":0,\"width\":1,\"height\":1,\"pivotXQ1000\":500,\"pivotYQ1000\":0,\"baselineY\":1"));
            if (mutation == "frameDuration") return Utf8(ReplaceExactOnce(text, "\"x\":0,\"y\":0,\"width\":1,\"height\":1,\"pivotXQ1000\":500,\"pivotYQ1000\":0,\"baselineY\":0,\"durationTicks\":1", "\"x\":0,\"y\":0,\"width\":1,\"height\":1,\"pivotXQ1000\":500,\"pivotYQ1000\":0,\"baselineY\":0,\"durationTicks\":2"));
            throw new ArgumentOutOfRangeException(nameof(mutation));
        }

        private static byte[] Utf8(string value) { return new UTF8Encoding(false, true).GetBytes(value); }

        private static string ReplaceQuotedFieldValue(string value, string field, string replacement)
        {
            var prefix = "\"" + field + "\":\"";
            var start = value.IndexOf(prefix, StringComparison.Ordinal);
            if (start < 0) throw new ArgumentException("Missing canonical field.");
            start += prefix.Length;
            var end = value.IndexOf('"', start);
            if (end < 0) throw new ArgumentException("Unclosed canonical field.");
            return value.Substring(0, start) + replacement + value.Substring(end);
        }

        private static string ReplaceExactOnce(string value, string before, string after)
        {
            var index = value.IndexOf(before, StringComparison.Ordinal);
            if (index < 0) throw new ArgumentException("Missing canonical byte sequence.");
            return value.Substring(0, index) + after + value.Substring(index + before.Length);
        }

        private static string SwapFirstTwoObjects(string value, string marker)
        {
            var firstStart = MarkerEnd(value, marker); var firstEnd = ObjectEnd(value, firstStart); var secondStart = firstEnd + 2; var secondEnd = ObjectEnd(value, secondStart);
            return value.Substring(0, firstStart) + value.Substring(secondStart, secondEnd - secondStart + 1) + "," + value.Substring(firstStart, firstEnd - firstStart + 1) + value.Substring(secondEnd + 1);
        }

        private static string RemoveObject(string value, string marker, int ordinal)
        {
            var start = MarkerEnd(value, marker);
            for (var i = 0; i < ordinal; i++) start = ObjectEnd(value, start) + 2;
            var end = ObjectEnd(value, start);
            if (ordinal == 0 && end + 1 < value.Length && value[end + 1] == ',') return value.Remove(start, end - start + 2);
            return value.Remove(start - (ordinal == 0 ? 0 : 1), end - start + 1 + (ordinal == 0 ? 0 : 1));
        }

        private static int MarkerEnd(string value, string marker)
        {
            var markerStart = value.IndexOf(marker, StringComparison.Ordinal);
            if (markerStart < 0) throw new ArgumentException("Missing canonical array.");
            return markerStart + marker.Length;
        }

        private static int ObjectEnd(string value, int start)
        {
            if (start >= value.Length || value[start] != '{') throw new ArgumentException("Missing canonical object.");
            var depth = 0;
            for (var i = start; i < value.Length; i++)
            {
                if (value[i] == '{') depth++;
                else if (value[i] == '}' && --depth == 0) return i;
            }
            throw new ArgumentException("Unclosed canonical object.");
        }

        // B1a preservation oracle: every constructible rejected candidate keeps the old complete tuple and all view/port state exact.
        private static void AssertB1aPublishedPairPreserved(CostumeUnityPresentationAdapterV1 adapter, MemoryPort port, Projection projection, PublishedCostumePresentationV1 oldPublished, CostumeUnityMediaPackageV1 oldMedia, CostumePresentationBindingV1 oldBinding, DeepViewSnapshot beforeView, int moves, int replaces, int projections, CostumeUnityPersistenceStatusV1 status)
        {
            Assert.That(adapter.Published, Is.SameAs(oldPublished));
            Assert.That(adapter.Published.Media, Is.SameAs(oldMedia));
            Assert.That(adapter.Published.Binding, Is.EqualTo(oldBinding));
            Assert.That(port.MoveCalls, Is.EqualTo(moves));
            Assert.That(port.ReplaceCalls, Is.EqualTo(replaces));
            Assert.That(projection.Count, Is.EqualTo(projections));
            Assert.That(adapter.PersistenceStatus, Is.EqualTo(status));
            DeepViewSnapshot.Capture(adapter.SnapshotView()).AssertEqual(beforeView);
        }

        private static CostumeUnityPresentationAdapterV1 PublishedAdapter(AcceptedFixture fixture, MemoryPort port, Projection projection)
        {
            var adapter = fixture.NewAdapter(port, projection);
            Assert.That(adapter.CompletedSnapshots.Observe(new CompletedActorSnapshotV1(10, "fixture", "idle", 500, "left", 3, 4)), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
            Assert.That(adapter.TrySelect(fixture.Package), Is.EqualTo(CostumeUnityAdapterRejectionV1.None));
            return adapter;
        }
        private static void AssertTerminalAfterCorruption(CostumeUnityPresentationAdapterV1 adapter, AcceptedFixture fixture, MemoryPort port, Projection projection, string mutation, bool observe)
        {
            var malformed = Corrupt(adapter.Published, mutation);
            typeof(CostumeUnityPresentationAdapterV1).GetField("_published", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(adapter, malformed);
            Assert.That(adapter.Highlight(fixture.PackageTwo.CostumeId), Is.EqualTo(CostumeUnityAdapterRejectionV1.HighlightOnly));
            var view = adapter.SnapshotView(); var moves = port.MoveCalls; var replaces = port.ReplaceCalls; var projections = projection.Count;
            var outcome = observe ? adapter.CompletedSnapshots.Observe(new CompletedActorSnapshotV1(11, "fixture", "idle", 500, "left", 3, 4)) : adapter.TrySelect(fixture.PackageTwo);
            Assert.That(outcome, Is.EqualTo(CostumeUnityAdapterRejectionV1.ReloadRequired), mutation);
            Assert.That(adapter.PersistenceStatus, Is.EqualTo(CostumeUnityPersistenceStatusV1.ReloadRequired));
            Assert.That(adapter.Published, Is.SameAs(malformed)); Assert.That(port.MoveCalls, Is.EqualTo(moves)); Assert.That(port.ReplaceCalls, Is.EqualTo(replaces)); Assert.That(projection.Count, Is.EqualTo(projections));
            var after = adapter.SnapshotView(); Assert.That(after.CurrentId, Is.EqualTo(view.CurrentId)); Assert.That(after.TransientPreviewId, Is.EqualTo(view.TransientPreviewId));
            Assert.That(adapter.TrySelect(fixture.PackageTwo), Is.EqualTo(CostumeUnityAdapterRejectionV1.ReloadRequired)); Assert.That(adapter.CompletedSnapshots.Observe(new CompletedActorSnapshotV1(12, "fixture", "idle", 0, "left", 3, 4)), Is.EqualTo(CostumeUnityAdapterRejectionV1.ReloadRequired)); Assert.That(adapter.Highlight(fixture.PackageTwo.CostumeId), Is.EqualTo(CostumeUnityAdapterRejectionV1.ReloadRequired));
        }
        private static PublishedCostumePresentationV1 Corrupt(PublishedCostumePresentationV1 good, string kind)
        {
            var state = good.State; var binding = good.Binding; var media = good.Media; var snapshot = good.Snapshot; var frame = good.Frame;
            if (kind == "mediaActor") { typeof(CostumeUnityMediaPackageV1).GetField("<ActorId>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(media, "other"); }
            else if (kind == "snapshotActor") snapshot = new CompletedActorSnapshotV1(snapshot.Tick, "other", snapshot.ActionId, snapshot.ActionAgeQ1000, snapshot.Facing, snapshot.AnchorX, snapshot.AnchorY);
            else if (kind == "stateAbsent") state = new CostumeStateV1(state.Revision, state.UnlockedCostumeIds, new KeyValuePair<string, string>[0]);
            else if (kind == "stateDifferent") state = new CostumeStateV1(state.Revision, state.UnlockedCostumeIds, new[] { new KeyValuePair<string, string>("fixture", "fixture.costume.synthetic2.v1") });
            else if (kind == "frameX") frame = new CostumeUnityFrameV1(frame.X + 1, frame.Y, frame.Width, frame.Height, frame.PivotXQ1000, frame.PivotYQ1000, frame.BaselineY, frame.DurationTicks);
            else if (kind == "frameY") frame = new CostumeUnityFrameV1(frame.X, frame.Y + 1, frame.Width, frame.Height, frame.PivotXQ1000, frame.PivotYQ1000, frame.BaselineY, frame.DurationTicks);
            else if (kind == "frameWidth") frame = new CostumeUnityFrameV1(frame.X, frame.Y, frame.Width + 1, frame.Height, frame.PivotXQ1000, frame.PivotYQ1000, frame.BaselineY, frame.DurationTicks);
            else if (kind == "frameHeight") frame = new CostumeUnityFrameV1(frame.X, frame.Y, frame.Width, frame.Height + 1, frame.PivotXQ1000, frame.PivotYQ1000, frame.BaselineY, frame.DurationTicks);
            else if (kind == "framePivotX") frame = new CostumeUnityFrameV1(frame.X, frame.Y, frame.Width, frame.Height, frame.PivotXQ1000 + 1, frame.PivotYQ1000, frame.BaselineY, frame.DurationTicks);
            else if (kind == "framePivotY") frame = new CostumeUnityFrameV1(frame.X, frame.Y, frame.Width, frame.Height, frame.PivotXQ1000, frame.PivotYQ1000 + 1, frame.BaselineY, frame.DurationTicks);
            else if (kind == "frameBaseline") frame = new CostumeUnityFrameV1(frame.X, frame.Y, frame.Width, frame.Height, frame.PivotXQ1000, frame.PivotYQ1000, frame.BaselineY + 1, frame.DurationTicks);
            else if (kind == "frameDuration") frame = new CostumeUnityFrameV1(frame.X, frame.Y, frame.Width, frame.Height, frame.PivotXQ1000, frame.PivotYQ1000, frame.BaselineY, frame.DurationTicks + 1);
            else binding = MutateBinding(binding, kind);
            var ctor = typeof(PublishedCostumePresentationV1).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(CostumeStateV1), typeof(CostumePresentationBindingV1), typeof(CostumeUnityMediaPackageV1), typeof(CompletedActorSnapshotV1), typeof(CostumeUnityFrameV1) }, null);
            return (PublishedCostumePresentationV1)ctor.Invoke(new object[] { state, binding, media, snapshot, frame });
        }
        private static CostumePresentationBindingV1 MutateBinding(CostumePresentationBindingV1 value, string kind)
        {
            var actor = value.ActorId; var costume = value.CostumeId; var portraitSet = value.PortraitSetId; var atlasSet = value.GameplayAtlasSetId; var portrait = value.PortraitHash; var atlas = value.GameplayAtlasHash; var clip = value.ClipMapHash; var presentation = value.PresentationRevision; var catalog = value.CatalogRevision; var actions = new List<string>(value.ClipIds);
            if (kind == "bindingActor") { actor = "other"; costume = "other.costume.synthetic.v1"; }
            else if (kind == "bindingCostume") costume = "fixture.costume.unknown.v1";
            else if (kind == "portraitSet" || kind == "bindingPortraitSet") portraitSet = "other.portrait.v1"; else if (kind == "atlasSet" || kind == "bindingAtlasSet") atlasSet = "other.gameplay.v1";
            else if (kind == "catalogRevision" || kind == "bindingCatalogRevision") catalog++; else if (kind == "presentationRevision" || kind == "bindingPresentationRevision") presentation++; else if (kind == "portraitHash" || kind == "bindingPortraitHash") portrait = new string('c', 64); else if (kind == "atlasHash" || kind == "bindingAtlasHash") atlas = new string('d', 64); else if (kind == "clipMapHash" || kind == "bindingClipHash") clip = new string('e', 64); else if (kind == "missingAction" || kind == "bindingMissingAction") actions.RemoveAt(0); else if (kind == "extraAction" || kind == "bindingExtraAction") actions.Add("extra"); else if (kind == "bindingEmpty") actions.Clear();
            return new CostumePresentationBindingV1(actor, costume, portraitSet, portrait, atlasSet, atlas, clip, presentation, catalog, actions);
        }
        private static bool IsBindingOnlyNonJsonMutation(string mutation)
        {
            return mutation == "bindingCatalogRevision" || mutation == "bindingMissingAction" || mutation == "bindingExtraAction" || mutation == "bindingEmpty";
        }

        private static bool IsCatalogIdentityNonJsonMutation(string mutation)
        {
            return mutation == "definitionActor" || mutation == "definitionCostume" || mutation == "definitionPending" || mutation == "definitionRejected" || mutation == "bindingActor" || mutation == "bindingCostume";
        }

        private static void AssertNonJsonPreserved(CostumeUnityPresentationAdapterV1 adapter, MemoryPort port, Projection projection, PublishedCostumePresentationV1 oldPublished, CostumeUnityMediaPackageV1 oldMedia, CostumePresentationBindingV1 oldBinding, DeepViewSnapshot expectedView, int expectedReplaceCalls)
        {
            Assert.That(adapter.Published, Is.SameAs(oldPublished)); Assert.That(adapter.Published.Media, Is.SameAs(oldMedia)); Assert.That(adapter.Published.Binding, Is.EqualTo(oldBinding)); Assert.That(port.MoveCalls, Is.EqualTo(1)); Assert.That(port.ReplaceCalls, Is.EqualTo(expectedReplaceCalls)); Assert.That(projection.Count, Is.EqualTo(1)); Assert.That(adapter.PersistenceStatus, Is.EqualTo(CostumeUnityPersistenceStatusV1.Ready)); DeepViewSnapshot.Capture(adapter.SnapshotView()).AssertEqual(expectedView);
        }

        private static void AssertNoPublicBindingIngress()
        {
            var type = typeof(CostumeUnityPresentationAdapterV1);
            foreach (var constructor in type.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)) foreach (var parameter in constructor.GetParameters()) Assert.That(TypeAcceptsBinding(parameter.ParameterType), Is.False, constructor.Name);
            foreach (var method in type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)) foreach (var parameter in method.GetParameters()) Assert.That(TypeAcceptsBinding(parameter.ParameterType), Is.False, method.Name);
            foreach (var field in type.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)) Assert.That(TypeAcceptsBinding(field.FieldType), Is.False, field.Name);
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)) if (property.SetMethod != null && property.SetMethod.IsPublic) Assert.That(TypeAcceptsBinding(property.PropertyType), Is.False, property.Name);
            foreach (var @event in type.GetEvents(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly)) Assert.That(EventAcceptsBinding(@event), Is.False, @event.Name);
        }

        private static bool EventAcceptsBinding(EventInfo @event)
        {
            if (TypeAcceptsBinding(@event.EventHandlerType)) return true;
            var invoke = @event.EventHandlerType == null ? null : @event.EventHandlerType.GetMethod("Invoke");
            if (invoke == null) return false;
            foreach (var parameter in invoke.GetParameters()) if (TypeAcceptsBinding(parameter.ParameterType)) return true;
            return false;
        }

        private static bool TypeAcceptsBinding(Type type)
        {
            if (type == null) return false;
            if (type.IsByRef || type.IsArray || type.IsPointer) return TypeAcceptsBinding(type.GetElementType());
            if (type == typeof(CostumePresentationBindingV1)) return true;
            if (!type.IsGenericType) return false;
            foreach (var argument in type.GetGenericArguments()) if (TypeAcceptsBinding(argument)) return true;
            return false;
        }

        private static CostumeUnityPackageRejectionV1 ExpectedValidation(string mutation) { if (mutation == "syntheticFalse") return CostumeUnityPackageRejectionV1.Synthetic; if (mutation.Contains("Revision")) return CostumeUnityPackageRejectionV1.Revision; if (mutation.Contains("Hash")) return CostumeUnityPackageRejectionV1.Hash; if (mutation.Contains("Action") || mutation == "bindingEmpty") return CostumeUnityPackageRejectionV1.Clips; return CostumeUnityPackageRejectionV1.Identity; }
        private static CostumeUnityAdapterRejectionV1 ExpectedAdapter(string mutation) { return mutation == "definitionPending" || mutation == "definitionRejected" || mutation == "definitionActor" || mutation == "definitionCostume" || mutation == "bindingActor" || mutation == "bindingCostume" ? CostumeUnityAdapterRejectionV1.Selection : CostumeUnityAdapterRejectionV1.Package; }
        private sealed class DeepViewSnapshot
        {
            private readonly CostumeUnityViewV1 _view; private DeepViewSnapshot(CostumeUnityViewV1 view) { _view = view; }
            public static DeepViewSnapshot Capture(CostumeUnityViewV1 view) { return new DeepViewSnapshot(view); }
            public void AssertEqual(DeepViewSnapshot other) { Assert.That(_view.CatalogRevision, Is.EqualTo(other._view.CatalogRevision)); Assert.That(_view.StateRevision, Is.EqualTo(other._view.StateRevision)); Assert.That(_view.ActorId, Is.EqualTo(other._view.ActorId)); Assert.That(_view.CurrentId, Is.EqualTo(other._view.CurrentId)); Assert.That(_view.TransientPreviewId, Is.EqualTo(other._view.TransientPreviewId)); Assert.That(_view.Availability, Is.EqualTo(other._view.Availability)); Assert.That(_view.PersistenceStatus, Is.EqualTo(other._view.PersistenceStatus)); Assert.That(_view.Rows.Count, Is.EqualTo(other._view.Rows.Count)); for(var i=0;i<_view.Rows.Count;i++){var a=_view.Rows[i];var b=other._view.Rows[i];Assert.That(a.CostumeId,Is.EqualTo(b.CostumeId));Assert.That(a.IsUnlocked,Is.EqualTo(b.IsUnlocked));Assert.That(a.IsCurrent,Is.EqualTo(b.IsCurrent));Assert.That(a.Disposition,Is.EqualTo(b.Disposition));Assert.That(a.CanPrepareSelection,Is.EqualTo(b.CanPrepareSelection));} }
        }
        private static CostumeDefinitionV1 DefinitionFromBinding(CostumePresentationBindingV1 binding, CostumeDisposition disposition) { return new CostumeDefinitionV1(binding.ActorId, binding.CostumeId, "synthetic", new string('a',64), "synthetic", disposition, true, binding.PortraitSetId, binding.GameplayAtlasSetId, binding.PortraitHash, binding.GameplayAtlasHash, binding.ClipMapHash, binding.PresentationRevision); }
        private static void ReplaceDefinition(CostumeCatalogV1 catalog, string oldId, CostumeDefinitionV1 replacement) { var field=typeof(CostumeCatalogV1).GetField("_definitions", BindingFlags.Instance|BindingFlags.NonPublic); var rows=(CostumeDefinitionV1[])field.GetValue(catalog); var copy=(CostumeDefinitionV1[])rows.Clone(); for(var i=0;i<copy.Length;i++) if(copy[i].CostumeId==oldId){copy[i]=replacement;break;} field.SetValue(catalog,copy); }

        private static CostumeUnityClipV1[] Clips()
        {
            var idle = new CostumeUnityFrameV1(0, 0, 1, 1, 500, 0, 0, 1);
            var run = new CostumeUnityFrameV1(1, 0, 1, 1, 500, 0, 0, 1);
            return new[] {
                new CostumeUnityClipV1("idle", "left", false, new[] { idle, run }), new CostumeUnityClipV1("idle", "right", false, new[] { idle, run }),
                new CostumeUnityClipV1("locomotion", "left", true, new[] { run, idle }), new CostumeUnityClipV1("locomotion", "right", true, new[] { run, idle }),
                new CostumeUnityClipV1("airborne", "left", false, new[] { idle, run }), new CostumeUnityClipV1("airborne", "right", false, new[] { idle, run }),
                new CostumeUnityClipV1("landing", "left", false, new[] { run, idle }), new CostumeUnityClipV1("landing", "right", false, new[] { run, idle }),
                new CostumeUnityClipV1("bowdraw", "left", false, new[] { idle, run }), new CostumeUnityClipV1("bowdraw", "right", false, new[] { idle, run }),
                new CostumeUnityClipV1("bowrelease", "left", false, new[] { run, idle }), new CostumeUnityClipV1("bowrelease", "right", false, new[] { run, idle }),
                new CostumeUnityClipV1("bowrecovery", "left", false, new[] { idle, run }), new CostumeUnityClipV1("bowrecovery", "right", false, new[] { idle, run }) };
        }
        private static string[] ActionIds() { return new[] { "airborne", "bowdraw", "bowrecovery", "bowrelease", "idle", "landing", "locomotion" }; }
        private static string[] ActionIds(CostumeUnityMediaPackageV1 media) { var actions = new SortedSet<string>(StringComparer.Ordinal); foreach (var clip in media.Clips) actions.Add(clip.ActionId); var result = new string[actions.Count]; actions.CopyTo(result); return result; }

        private sealed class Projection : ICostumeUnityProjectionPortV1
        {
            public bool Throw { get; set; } public int Count { get; private set; } public Action<PublishedCostumePresentationV1> OnProject { get; set; } public PublishedCostumePresentationV1 LastSuccessful { get; private set; }
            public void Project(PublishedCostumePresentationV1 published) { Assert.That(published, Is.Not.Null); Count++; if (OnProject != null) OnProject(published); if (Throw) throw new InvalidOperationException("synthetic projection fault"); LastSuccessful = published; }
        }

        private sealed class AcceptedFixture : IDisposable
        {
            private readonly Texture2D _portrait;
            private readonly Texture2D _atlas;
            private AcceptedFixture(CostumeCatalogV1 catalog, CostumeStateV1 state, CostumeUnityMediaPackageV1 package, CostumeUnityMediaPackageV1 packageTwo, Texture2D portrait, Texture2D atlas) { Catalog = catalog; State = state; Package = package; PackageTwo = packageTwo; _portrait = portrait; _atlas = atlas; }
            public CostumeCatalogV1 Catalog { get; } public CostumeStateV1 State { get; } public CostumeUnityMediaPackageV1 Package { get; } public CostumeUnityMediaPackageV1 PackageTwo { get; }
            public static AcceptedFixture Create()
            {
                var portrait = new Texture2D(2, 2, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
                var atlas = new Texture2D(4, 2, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
                CostumeDefinitionV1 firstDefinition; var package = Build("fixture.costume.synthetic.v1", "a", portrait, atlas, out firstDefinition);
                CostumeDefinitionV1 secondDefinition; var packageTwo = Build("fixture.costume.synthetic2.v1", "b", portrait, atlas, out secondDefinition);
                return new AcceptedFixture(new CostumeCatalogV1(9, new[] { firstDefinition, secondDefinition }), new CostumeStateV1(3, new[] { firstDefinition.CostumeId, secondDefinition.CostumeId }, new KeyValuePair<string, string>[0]), package, packageTwo, portrait, atlas);
            }
            private static CostumeUnityMediaPackageV1 Build(string costumeId, string sourceFill, Texture2D portrait, Texture2D atlas, out CostumeDefinitionV1 definition)
            {
                var seed = new CostumeUnityMediaPackageV1("fixture", costumeId, "fixture.portrait.v1", "fixture.gameplay.v1", 9, 5, true, portrait, atlas, new byte[] { 1, 2 }, new byte[] { 3, 4 }, new byte[0], new byte[0], CostumeUnityMediaPackageV1.SyntheticApprovedPpu, Clips());
                var clipMap = seed.CanonicalClipMapBytes();
                definition = new CostumeDefinitionV1("fixture", costumeId, "synthetic", new string(sourceFill[0], 64), "synthetic-hair", CostumeDisposition.Accepted, costumeId.EndsWith("synthetic.v1", StringComparison.Ordinal), "fixture.portrait.v1", "fixture.gameplay.v1", CostumeUnityMediaPackageV1.Hash(seed.PortraitBytes), CostumeUnityMediaPackageV1.Hash(seed.AtlasBytes), CostumeUnityMediaPackageV1.Hash(clipMap), 5);
                return new CostumeUnityMediaPackageV1("fixture", costumeId, "fixture.portrait.v1", "fixture.gameplay.v1", 9, 5, true, portrait, atlas, seed.PortraitBytes, seed.AtlasBytes, clipMap, seed.CanonicalSourceManifestBytes(definition), CostumeUnityMediaPackageV1.SyntheticApprovedPpu, Clips());
            }
            public CostumeUnityPresentationAdapterV1 NewAdapter(MemoryPort port, Projection projection) { return new CostumeUnityPresentationAdapterV1(Catalog, State, "fixture", new CostumeFileAdapterV1(Path.Combine(Path.GetTempPath(), "cua-isolated-memory-port"), port), projection); }
            public void Dispose() { UnityEngine.Object.DestroyImmediate(_portrait); UnityEngine.Object.DestroyImmediate(_atlas); }
        }

        private sealed class MemoryPort : ICostumeFilePortV1
        {
            private readonly Dictionary<string, byte[]> _files = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
            public bool FailOpen { get; set; } public bool FailMove { get; set; } public bool CorruptPrimary { get; set; } public bool CorruptPrimaryWithSameRevisionDifferentBytes { get; set; } public int MoveCalls { get; private set; } public int ReplaceCalls { get; private set; } public Action BeforeMove { get; set; } public Action BeforeReplace { get; set; } public List<string> CommitTrace { get; } = new List<string>();
            public bool IsExistingLocalNtfsDirectoryWithoutReparseAncestor(string normalizedDirectory) { return true; }
            public CostumeFilePortReadResultV1 ReadOnce(string path) { byte[] bytes; return _files.TryGetValue(path, out bytes) ? new CostumeFilePortReadResultV1(CostumeFileReadDispositionV1.Read, bytes, FileAttributes.Normal) : new CostumeFilePortReadResultV1(CostumeFileReadDispositionV1.Missing, null, FileAttributes.Normal); }
            public CostumeFilePathInfoV1 Inspect(string path) { return _files.ContainsKey(path) ? new CostumeFilePathInfoV1(CostumeFilePathKindV1.File, FileAttributes.Normal) : new CostumeFilePathInfoV1(CostumeFilePathKindV1.Missing, FileAttributes.Normal); }
            public ICostumeFileWriteSessionV1 OpenTempForWrite(string path) { if (FailOpen) throw new IOException("synthetic pre-commit fault"); return new MemoryWrite(bytes => _files[path] = bytes); }
            public byte[] BytesForFileName(string fileName) { foreach (var entry in _files) if (StringComparer.Ordinal.Equals(Path.GetFileName(entry.Key), fileName)) return (byte[])entry.Value.Clone(); return null; }
            public void MoveNoOverwrite(string sourcePath, string destinationPath) { MoveCalls++; CommitTrace.Add("Move"); if (BeforeMove != null) BeforeMove(); if (FailMove) throw new IOException("synthetic commit uncertainty"); _files[destinationPath] = CorruptPrimary ? new byte[] { 0 } : _files[sourcePath]; _files.Remove(sourcePath); }
            public void Replace(string sourcePath, string destinationPath, string backupPath)
            {
                MoveCalls++; ReplaceCalls++;
                CommitTrace.Add("Replace"); if (BeforeReplace != null) BeforeReplace();
                if (FailMove || !_files.ContainsKey(sourcePath) || !_files.ContainsKey(destinationPath) || _files.ContainsKey(backupPath)) throw new IOException("synthetic replace uncertainty");
                _files[backupPath] = _files[destinationPath];
                if (CorruptPrimary) _files[destinationPath] = new byte[] { 0 };
                else if (CorruptPrimaryWithSameRevisionDifferentBytes)
                {
                    CostumeStateV1 staged; if (!CostumeCanonicalCodecV1.TryDecode(_files[sourcePath], out staged)) throw new IOException("Synthetic staged decode failure");
                    _files[destinationPath] = CostumeCanonicalCodecV1.Encode(new CostumeStateV1(staged.Revision, staged.UnlockedCostumeIds, new[] { new KeyValuePair<string, string>("fixture", "fixture.costume.synthetic.v1") }));
                }
                else _files[destinationPath] = _files[sourcePath];
                _files.Remove(sourcePath);
            }
        }

        private sealed class MemoryWrite : ICostumeFileWriteSessionV1
        {
            private readonly Action<byte[]> _complete; private byte[] _bytes;
            public MemoryWrite(Action<byte[]> complete) { _complete = complete; }
            public void WriteExact(byte[] bytes) { _bytes = (byte[])bytes.Clone(); } public void FlushManaged() { } public void FlushDurable() { } public void Dispose() { _complete(_bytes); }
        }
    }
}
