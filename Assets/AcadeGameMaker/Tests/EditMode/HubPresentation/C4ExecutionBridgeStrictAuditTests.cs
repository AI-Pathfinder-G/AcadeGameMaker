using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;

namespace AcadeGameMaker.Tests.EditMode.HubPresentation
{
    // REQ-M5D7QC4-007 / AC-M5D7QC4-009/010. Structure 증거만 발행한다.
    internal sealed class C4ExecutionBridgeStrictAuditTests
    {
        // C4_EXPECTED_ROWS_BEGIN
        private const string ExpectedRowsJson = @"{""SchemaVersion"":1,""Rows"":[{""Id"":""C4_HUB_STRICT_01"",""Order"":183,""RequirementIds"":[""REQ-M5D7QC4-007""],""AcceptanceIds"":[""AC-M5D7QC4-009"",""AC-M5D7QC4-010""],""ExpectedCase"":""AcadeGameMaker.Tests.EditMode.HubPresentation.C4ExecutionBridgeStrictAuditTests.AC009_LowerExecutorHasNoHubTypeOrPublicAuthority"",""EvidenceKind"":""Structure"",""CheckpointFamily"":""None"",""CheckpointName"":null,""CheckpointValue"":null,""Outcome"":null,""Phase"":null,""Applicability"":""Required"",""ExpectedReach"":""RequiredReached"",""RequiredFacts"":[{""Name"":""authorityCorrelation.confirmed"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.owner"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.adapter"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.router"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.root"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.proof"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.c2Result"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.receipt"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.executionGeneration"",""ExpectedValue"":{""Rule"":""Unobserved""},""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.memoryGeneration"",""ExpectedValue"":{""Rule"":""Unobserved""},""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.nextEpoch"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.checked"",""ExpectedValue"":""false"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""guard.adapter"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""guard.router"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""guard.permanentFaultRecorded"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""barrier.state"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""barrier.ordinaryWriterBlocked"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""barrier.evidenceReference"",""ExpectedValue"":{""Rule"":""Absent""},""RequiredCertainty"":""SourceEstablished""},{""Name"":""history.originalCommitPreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""history.originalEpochPreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""history.oldTakePreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""history.oldCursorPreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""history.receiptReferencePreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""cleanup.required"",""ExpectedValue"":""false"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.releaseSignaled"",""ExpectedValue"":""NotApplicable"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.workerLeaseReleased"",""ExpectedValue"":""NotApplicable"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.workerJoined"",""ExpectedValue"":""NotApplicable"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.originalThreadCleanup"",""ExpectedValue"":""NotApplicable"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.exception"",""ExpectedValue"":{""Rule"":""Absent""},""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.c1Begin"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.c2Finalize"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.freshReserve"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.freshComplete"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.preparedObserver"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.c2Observer"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.nativeCallback"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""diagnostic.generationUnobservedReason"",""ExpectedValue"":""StructureGenerationOutsideScope"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""diagnostic.barrierEvidenceAbsentReason"",""ExpectedValue"":""NoBarrierEvidenceRequiredForThisPreEntryOrPureRow"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""diagnostic.cleanupNotRequiredReason"",""ExpectedValue"":""StructureAuditHasNoRuntimeCleanup"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""diagnostic.strictAuditCompleted"",""ExpectedValue"":""Yes"",""RequiredCertainty"":""SourceEstablished""}],""ExpectedCallCounts"":{""c1Begin"":0,""c2Finalize"":0,""freshReserve"":0,""freshComplete"":0,""preparedObserver"":0,""c2Observer"":0,""nativeCallback"":0},""ExpectedAuthorityCorrelation"":{""confirmed"":""Unknown"",""owner"":""Unknown"",""adapter"":""Unknown"",""router"":""Unknown"",""root"":""Unknown"",""proof"":""Unknown"",""c2Result"":""Unknown"",""receipt"":""Unknown"",""executionGeneration"":{""Rule"":""Unobserved""},""memoryGeneration"":{""Rule"":""Unobserved""},""nextEpoch"":""Unknown"",""checked"":false},""ExpectedGuard"":{""adapter"":""Unknown"",""router"":""Unknown"",""permanentFaultRecorded"":""Unknown""},""ExpectedBarrier"":{""state"":""Unknown"",""ordinaryWriterBlocked"":""Unknown"",""evidenceReference"":{""Rule"":""Absent""}},""ExpectedHistory"":{""originalCommitPreserved"":""Unknown"",""originalEpochPreserved"":""Unknown"",""oldTakePreserved"":""Unknown"",""oldCursorPreserved"":""Unknown"",""receiptReferencePreserved"":""Unknown""},""ExpectedCleanup"":{""required"":false,""releaseSignaled"":""NotApplicable"",""workerLeaseReleased"":""NotApplicable"",""workerJoined"":""NotApplicable"",""originalThreadCleanup"":""NotApplicable"",""exception"":{""Rule"":""Absent""}}},{""Id"":""C4_HUB_STRICT_02"",""Order"":184,""RequirementIds"":[""REQ-M5D7QC4-007""],""AcceptanceIds"":[""AC-M5D7QC4-009"",""AC-M5D7QC4-010""],""ExpectedCase"":""AcadeGameMaker.Tests.EditMode.HubPresentation.C4ExecutionBridgeStrictAuditTests.AC009_QBFreshAmendmentKeepsGameplayAndDurableTokensForbidden"",""EvidenceKind"":""Structure"",""CheckpointFamily"":""None"",""CheckpointName"":null,""CheckpointValue"":null,""Outcome"":null,""Phase"":null,""Applicability"":""Required"",""ExpectedReach"":""RequiredReached"",""RequiredFacts"":[{""Name"":""authorityCorrelation.confirmed"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.owner"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.adapter"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.router"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.root"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.proof"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.c2Result"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.receipt"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.executionGeneration"",""ExpectedValue"":{""Rule"":""Unobserved""},""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.memoryGeneration"",""ExpectedValue"":{""Rule"":""Unobserved""},""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.nextEpoch"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.checked"",""ExpectedValue"":""false"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""guard.adapter"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""guard.router"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""guard.permanentFaultRecorded"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""barrier.state"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""barrier.ordinaryWriterBlocked"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""barrier.evidenceReference"",""ExpectedValue"":{""Rule"":""Absent""},""RequiredCertainty"":""SourceEstablished""},{""Name"":""history.originalCommitPreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""history.originalEpochPreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""history.oldTakePreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""history.oldCursorPreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""history.receiptReferencePreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""cleanup.required"",""ExpectedValue"":""false"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.releaseSignaled"",""ExpectedValue"":""NotApplicable"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.workerLeaseReleased"",""ExpectedValue"":""NotApplicable"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.workerJoined"",""ExpectedValue"":""NotApplicable"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.originalThreadCleanup"",""ExpectedValue"":""NotApplicable"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.exception"",""ExpectedValue"":{""Rule"":""Absent""},""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.c1Begin"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.c2Finalize"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.freshReserve"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.freshComplete"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.preparedObserver"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.c2Observer"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.nativeCallback"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""diagnostic.generationUnobservedReason"",""ExpectedValue"":""StructureGenerationOutsideScope"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""diagnostic.barrierEvidenceAbsentReason"",""ExpectedValue"":""NoBarrierEvidenceRequiredForThisPreEntryOrPureRow"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""diagnostic.cleanupNotRequiredReason"",""ExpectedValue"":""StructureAuditHasNoRuntimeCleanup"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""diagnostic.strictAuditCompleted"",""ExpectedValue"":""Yes"",""RequiredCertainty"":""SourceEstablished""}],""ExpectedCallCounts"":{""c1Begin"":0,""c2Finalize"":0,""freshReserve"":0,""freshComplete"":0,""preparedObserver"":0,""c2Observer"":0,""nativeCallback"":0},""ExpectedAuthorityCorrelation"":{""confirmed"":""Unknown"",""owner"":""Unknown"",""adapter"":""Unknown"",""router"":""Unknown"",""root"":""Unknown"",""proof"":""Unknown"",""c2Result"":""Unknown"",""receipt"":""Unknown"",""executionGeneration"":{""Rule"":""Unobserved""},""memoryGeneration"":{""Rule"":""Unobserved""},""nextEpoch"":""Unknown"",""checked"":false},""ExpectedGuard"":{""adapter"":""Unknown"",""router"":""Unknown"",""permanentFaultRecorded"":""Unknown""},""ExpectedBarrier"":{""state"":""Unknown"",""ordinaryWriterBlocked"":""Unknown"",""evidenceReference"":{""Rule"":""Absent""}},""ExpectedHistory"":{""originalCommitPreserved"":""Unknown"",""originalEpochPreserved"":""Unknown"",""oldTakePreserved"":""Unknown"",""oldCursorPreserved"":""Unknown"",""receiptReferencePreserved"":""Unknown""},""ExpectedCleanup"":{""required"":false,""releaseSignaled"":""NotApplicable"",""workerLeaseReleased"":""NotApplicable"",""workerJoined"":""NotApplicable"",""originalThreadCleanup"":""NotApplicable"",""exception"":{""Rule"":""Absent""}}},{""Id"":""C4_HUB_STRICT_03"",""Order"":185,""RequirementIds"":[""REQ-M5D7QC4-007""],""AcceptanceIds"":[""AC-M5D7QC4-009"",""AC-M5D7QC4-010""],""ExpectedCase"":""AcadeGameMaker.Tests.EditMode.HubPresentation.C4ExecutionBridgeStrictAuditTests.AC009_QAFreshAmendmentPreservesOriginalTransferAudit"",""EvidenceKind"":""Structure"",""CheckpointFamily"":""None"",""CheckpointName"":null,""CheckpointValue"":null,""Outcome"":null,""Phase"":null,""Applicability"":""Required"",""ExpectedReach"":""RequiredReached"",""RequiredFacts"":[{""Name"":""authorityCorrelation.confirmed"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.owner"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.adapter"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.router"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.root"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.proof"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.c2Result"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.receipt"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.executionGeneration"",""ExpectedValue"":{""Rule"":""Unobserved""},""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.memoryGeneration"",""ExpectedValue"":{""Rule"":""Unobserved""},""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.nextEpoch"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.checked"",""ExpectedValue"":""false"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""guard.adapter"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""guard.router"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""guard.permanentFaultRecorded"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""barrier.state"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""barrier.ordinaryWriterBlocked"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""barrier.evidenceReference"",""ExpectedValue"":{""Rule"":""Absent""},""RequiredCertainty"":""SourceEstablished""},{""Name"":""history.originalCommitPreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""history.originalEpochPreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""history.oldTakePreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""history.oldCursorPreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""history.receiptReferencePreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""cleanup.required"",""ExpectedValue"":""false"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.releaseSignaled"",""ExpectedValue"":""NotApplicable"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.workerLeaseReleased"",""ExpectedValue"":""NotApplicable"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.workerJoined"",""ExpectedValue"":""NotApplicable"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.originalThreadCleanup"",""ExpectedValue"":""NotApplicable"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.exception"",""ExpectedValue"":{""Rule"":""Absent""},""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.c1Begin"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.c2Finalize"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.freshReserve"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.freshComplete"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.preparedObserver"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.c2Observer"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.nativeCallback"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""diagnostic.generationUnobservedReason"",""ExpectedValue"":""StructureGenerationOutsideScope"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""diagnostic.barrierEvidenceAbsentReason"",""ExpectedValue"":""NoBarrierEvidenceRequiredForThisPreEntryOrPureRow"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""diagnostic.cleanupNotRequiredReason"",""ExpectedValue"":""StructureAuditHasNoRuntimeCleanup"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""diagnostic.strictAuditCompleted"",""ExpectedValue"":""Yes"",""RequiredCertainty"":""SourceEstablished""}],""ExpectedCallCounts"":{""c1Begin"":0,""c2Finalize"":0,""freshReserve"":0,""freshComplete"":0,""preparedObserver"":0,""c2Observer"":0,""nativeCallback"":0},""ExpectedAuthorityCorrelation"":{""confirmed"":""Unknown"",""owner"":""Unknown"",""adapter"":""Unknown"",""router"":""Unknown"",""root"":""Unknown"",""proof"":""Unknown"",""c2Result"":""Unknown"",""receipt"":""Unknown"",""executionGeneration"":{""Rule"":""Unobserved""},""memoryGeneration"":{""Rule"":""Unobserved""},""nextEpoch"":""Unknown"",""checked"":false},""ExpectedGuard"":{""adapter"":""Unknown"",""router"":""Unknown"",""permanentFaultRecorded"":""Unknown""},""ExpectedBarrier"":{""state"":""Unknown"",""ordinaryWriterBlocked"":""Unknown"",""evidenceReference"":{""Rule"":""Absent""}},""ExpectedHistory"":{""originalCommitPreserved"":""Unknown"",""originalEpochPreserved"":""Unknown"",""oldTakePreserved"":""Unknown"",""oldCursorPreserved"":""Unknown"",""receiptReferencePreserved"":""Unknown""},""ExpectedCleanup"":{""required"":false,""releaseSignaled"":""NotApplicable"",""workerLeaseReleased"":""NotApplicable"",""workerJoined"":""NotApplicable"",""originalThreadCleanup"":""NotApplicable"",""exception"":{""Rule"":""Absent""}}},{""Id"":""C4_HUB_STRICT_04"",""Order"":186,""RequirementIds"":[""REQ-M5D7QC4-007""],""AcceptanceIds"":[""AC-M5D7QC4-009"",""AC-M5D7QC4-010""],""ExpectedCase"":""AcadeGameMaker.Tests.EditMode.HubPresentation.C4ExecutionBridgeStrictAuditTests.AC009_Q0SelectsOnlyExactChangedCurrentSuccessor"",""EvidenceKind"":""Structure"",""CheckpointFamily"":""None"",""CheckpointName"":null,""CheckpointValue"":null,""Outcome"":null,""Phase"":null,""Applicability"":""Required"",""ExpectedReach"":""RequiredReached"",""RequiredFacts"":[{""Name"":""authorityCorrelation.confirmed"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.owner"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.adapter"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.router"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.root"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.proof"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.c2Result"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.receipt"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.executionGeneration"",""ExpectedValue"":{""Rule"":""Unobserved""},""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.memoryGeneration"",""ExpectedValue"":{""Rule"":""Unobserved""},""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.nextEpoch"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.checked"",""ExpectedValue"":""false"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""guard.adapter"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""guard.router"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""guard.permanentFaultRecorded"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""barrier.state"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""barrier.ordinaryWriterBlocked"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""barrier.evidenceReference"",""ExpectedValue"":{""Rule"":""Absent""},""RequiredCertainty"":""SourceEstablished""},{""Name"":""history.originalCommitPreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""history.originalEpochPreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""history.oldTakePreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""history.oldCursorPreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""history.receiptReferencePreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""cleanup.required"",""ExpectedValue"":""false"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.releaseSignaled"",""ExpectedValue"":""NotApplicable"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.workerLeaseReleased"",""ExpectedValue"":""NotApplicable"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.workerJoined"",""ExpectedValue"":""NotApplicable"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.originalThreadCleanup"",""ExpectedValue"":""NotApplicable"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.exception"",""ExpectedValue"":{""Rule"":""Absent""},""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.c1Begin"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.c2Finalize"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.freshReserve"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.freshComplete"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.preparedObserver"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.c2Observer"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.nativeCallback"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""diagnostic.generationUnobservedReason"",""ExpectedValue"":""StructureGenerationOutsideScope"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""diagnostic.barrierEvidenceAbsentReason"",""ExpectedValue"":""NoBarrierEvidenceRequiredForThisPreEntryOrPureRow"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""diagnostic.cleanupNotRequiredReason"",""ExpectedValue"":""StructureAuditHasNoRuntimeCleanup"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""diagnostic.strictAuditCompleted"",""ExpectedValue"":""Yes"",""RequiredCertainty"":""SourceEstablished""}],""ExpectedCallCounts"":{""c1Begin"":0,""c2Finalize"":0,""freshReserve"":0,""freshComplete"":0,""preparedObserver"":0,""c2Observer"":0,""nativeCallback"":0},""ExpectedAuthorityCorrelation"":{""confirmed"":""Unknown"",""owner"":""Unknown"",""adapter"":""Unknown"",""router"":""Unknown"",""root"":""Unknown"",""proof"":""Unknown"",""c2Result"":""Unknown"",""receipt"":""Unknown"",""executionGeneration"":{""Rule"":""Unobserved""},""memoryGeneration"":{""Rule"":""Unobserved""},""nextEpoch"":""Unknown"",""checked"":false},""ExpectedGuard"":{""adapter"":""Unknown"",""router"":""Unknown"",""permanentFaultRecorded"":""Unknown""},""ExpectedBarrier"":{""state"":""Unknown"",""ordinaryWriterBlocked"":""Unknown"",""evidenceReference"":{""Rule"":""Absent""}},""ExpectedHistory"":{""originalCommitPreserved"":""Unknown"",""originalEpochPreserved"":""Unknown"",""oldTakePreserved"":""Unknown"",""oldCursorPreserved"":""Unknown"",""receiptReferencePreserved"":""Unknown""},""ExpectedCleanup"":{""required"":false,""releaseSignaled"":""NotApplicable"",""workerLeaseReleased"":""NotApplicable"",""workerJoined"":""NotApplicable"",""originalThreadCleanup"":""NotApplicable"",""exception"":{""Rule"":""Absent""}}},{""Id"":""C4_HUB_STRICT_05"",""Order"":187,""RequirementIds"":[""REQ-M5D7QC4-007""],""AcceptanceIds"":[""AC-M5D7QC4-009"",""AC-M5D7QC4-010""],""ExpectedCase"":""AcadeGameMaker.Tests.EditMode.HubPresentation.C4ExecutionBridgeStrictAuditTests.AC010_PredecessorBodiesAndRequiredSelectionNamesRemainExact"",""EvidenceKind"":""Structure"",""CheckpointFamily"":""None"",""CheckpointName"":null,""CheckpointValue"":null,""Outcome"":null,""Phase"":null,""Applicability"":""Required"",""ExpectedReach"":""RequiredReached"",""RequiredFacts"":[{""Name"":""authorityCorrelation.confirmed"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.owner"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.adapter"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.router"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.root"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.proof"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.c2Result"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.receipt"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.executionGeneration"",""ExpectedValue"":{""Rule"":""Unobserved""},""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.memoryGeneration"",""ExpectedValue"":{""Rule"":""Unobserved""},""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.nextEpoch"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""authorityCorrelation.checked"",""ExpectedValue"":""false"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""guard.adapter"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""guard.router"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""guard.permanentFaultRecorded"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""barrier.state"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""barrier.ordinaryWriterBlocked"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""barrier.evidenceReference"",""ExpectedValue"":{""Rule"":""Absent""},""RequiredCertainty"":""SourceEstablished""},{""Name"":""history.originalCommitPreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""history.originalEpochPreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""history.oldTakePreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""history.oldCursorPreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""history.receiptReferencePreserved"",""ExpectedValue"":""Unknown"",""RequiredCertainty"":""Unknown""},{""Name"":""cleanup.required"",""ExpectedValue"":""false"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.releaseSignaled"",""ExpectedValue"":""NotApplicable"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.workerLeaseReleased"",""ExpectedValue"":""NotApplicable"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.workerJoined"",""ExpectedValue"":""NotApplicable"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.originalThreadCleanup"",""ExpectedValue"":""NotApplicable"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""cleanup.exception"",""ExpectedValue"":{""Rule"":""Absent""},""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.c1Begin"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.c2Finalize"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.freshReserve"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.freshComplete"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.preparedObserver"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.c2Observer"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""callCounts.nativeCallback"",""ExpectedValue"":""0"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""diagnostic.generationUnobservedReason"",""ExpectedValue"":""StructureGenerationOutsideScope"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""diagnostic.barrierEvidenceAbsentReason"",""ExpectedValue"":""NoBarrierEvidenceRequiredForThisPreEntryOrPureRow"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""diagnostic.cleanupNotRequiredReason"",""ExpectedValue"":""StructureAuditHasNoRuntimeCleanup"",""RequiredCertainty"":""SourceEstablished""},{""Name"":""diagnostic.strictAuditCompleted"",""ExpectedValue"":""Yes"",""RequiredCertainty"":""SourceEstablished""}],""ExpectedCallCounts"":{""c1Begin"":0,""c2Finalize"":0,""freshReserve"":0,""freshComplete"":0,""preparedObserver"":0,""c2Observer"":0,""nativeCallback"":0},""ExpectedAuthorityCorrelation"":{""confirmed"":""Unknown"",""owner"":""Unknown"",""adapter"":""Unknown"",""router"":""Unknown"",""root"":""Unknown"",""proof"":""Unknown"",""c2Result"":""Unknown"",""receipt"":""Unknown"",""executionGeneration"":{""Rule"":""Unobserved""},""memoryGeneration"":{""Rule"":""Unobserved""},""nextEpoch"":""Unknown"",""checked"":false},""ExpectedGuard"":{""adapter"":""Unknown"",""router"":""Unknown"",""permanentFaultRecorded"":""Unknown""},""ExpectedBarrier"":{""state"":""Unknown"",""ordinaryWriterBlocked"":""Unknown"",""evidenceReference"":{""Rule"":""Absent""}},""ExpectedHistory"":{""originalCommitPreserved"":""Unknown"",""originalEpochPreserved"":""Unknown"",""oldTakePreserved"":""Unknown"",""oldCursorPreserved"":""Unknown"",""receiptReferencePreserved"":""Unknown""},""ExpectedCleanup"":{""required"":false,""releaseSignaled"":""NotApplicable"",""workerLeaseReleased"":""NotApplicable"",""workerJoined"":""NotApplicable"",""originalThreadCleanup"":""NotApplicable"",""exception"":{""Rule"":""Absent""}}}]}";
        // C4_EXPECTED_ROWS_END
        private const string HubRoot = "Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/";
        private const string InputRoot = "Assets/AcadeGameMaker/Runtime/Input/Unity/";
        private const string AuditRoot = "docs/evidence/c4-audit-predecessor/";
        private const string PreviousRoot = "docs/evidence/c4-runtime-predecessor/";
        private static readonly string[] AuditNames = {
            "HubMenuIntentHandoffEditModeTests.cs", "HubMenuIntentHandoffC3AuditSuccessorTests.cs",
            "HubPresentationScopeAuditTests.cs", "HubUiOnlyQ0ScopeAuditEditModeTests.cs" };
        private static readonly string[] AuditHashes = {
            "57E7EFB17307BFB156D338F7BDCE25104E3192748FD6C159F7BB7B4A7483DAD6",
            "1B0F8D0919770CD1FBD06F908EC23E370A18E911EC5AAC09938FAF0BFDA174E6",
            "1DB3012C82DD6E0A1348ADEAF954EF0056EB28E925108716B6317289AAA93D60",
            "C27116AC35BF83D4AB8794FA9D3E379AD6F127B0E52B48C51399F49ADDF862D4" };

        private static readonly Dictionary<string, string> RuntimePredecessors = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            {"Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileNewGameConfirmationV1.cs", "ABD4B8CC79D0C24F16EB053B98460C68A466B03AABE0B2CFA01C7862BA027CB1"},
            {"Assets/AcadeGameMaker/Runtime/Input/Unity/DesktopProfileLaunchAdapterV1.cs", "1798F51244FCAFD8EE4B0D847AABBC6DA89EC8EA7138A12826CB37810264DCFB"},
            {"Assets/AcadeGameMaker/Runtime/Input/Unity/InputRouter.cs", "66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB"},
            {"Assets/AcadeGameMaker/Runtime/Input/Unity/ProfileResetMemoryCutoverV1.cs", "60DFD415C0425F88046C47F51142749F230FF39CA7C81AEBA90D4C778993F275"},
            {"Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/NewGameConfirmationOwnerV1.cs", "CAEBD7B5DA99AF94A25C5C300789AA48CA87908CA6D8894B99AE49C2391D7043"},
            {"Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuIntentHandoffOwnerV1.cs", "F47C61A043DAF4695CC53E41C6BE9BB67CA147C722F7B830459AA6304B45B3B5"},
            {"Assets/AcadeGameMaker/Runtime/HubPresentation/Unity/HubMenuPresenterV1.cs", "393B325BA5B8F6D79CA983F1A91098215E7A6993DE37E9F5D98BA775760E4900"},
            {"Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs", "C27116AC35BF83D4AB8794FA9D3E379AD6F127B0E52B48C51399F49ADDF862D4"},
            {"Assets/AcadeGameMaker/Runtime/Profile/ProfileResetDiskTransactionV1.cs", "BE0F16834F96272AD68C15F5004E9C37491EEF6F82D7CA728A5E1D1D60CB71E2"},
        };

        private static string Read(string path) { return File.ReadAllText(path); }

        [SetUp]
        public void BeforeAudit()
        {
            Emit("Planned", false, null);
            Emit("Reached", true, null);
        }

        [TearDown]
        public void AfterAudit()
        {
            var passed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed;
            Emit(passed ? "Passed" : "Failed", true, passed ? null : "엄격 구조 감사 부모 사례 실패");
        }

        private static void Emit(string status, bool reached, string failure)
        {
            var fullName = TestContext.CurrentContext.Test.FullName;
            var definitions = Object(new JsonReader(ExpectedRowsJson).Read());
            var expected = Array(definitions["Rows"]).Select(Object).Single(r => (string)r["ExpectedCase"] == fullName);
            var authority = Object(new JsonReader(@"{""confirmed"":""Unknown"",""owner"":""Unknown"",""adapter"":""Unknown"",""router"":""Unknown"",""root"":""Unknown"",""proof"":""Unknown"",""c2Result"":""Unknown"",""receipt"":""Unknown"",""executionGeneration"":null,""memoryGeneration"":null,""nextEpoch"":""Unknown"",""checked"":false}").Read());
            var guard = Object(new JsonReader(@"{""adapter"":""Unknown"",""router"":""Unknown"",""permanentFaultRecorded"":""Unknown""}").Read());
            var barrier = Object(new JsonReader(@"{""state"":""Unknown"",""ordinaryWriterBlocked"":""Unknown"",""evidenceReference"":null}").Read());
            var history = Object(new JsonReader(@"{""originalCommitPreserved"":""Unknown"",""originalEpochPreserved"":""Unknown"",""oldTakePreserved"":""Unknown"",""oldCursorPreserved"":""Unknown"",""receiptReferencePreserved"":""Unknown""}").Read());
            var cleanup = Object(new JsonReader(@"{""required"":false,""releaseSignaled"":""NotApplicable"",""workerLeaseReleased"":""NotApplicable"",""workerJoined"":""NotApplicable"",""originalThreadCleanup"":""NotApplicable"",""exception"":null}").Read());
            var calls = Object(new JsonReader(@"{""c1Begin"":0,""c2Finalize"":0,""freshReserve"":0,""freshComplete"":0,""preparedObserver"":0,""c2Observer"":0,""nativeCallback"":0}").Read());
            var facts = new List<object>();
            foreach (var pair in new[] { new KeyValuePair<string, Dictionary<string, object>>("authorityCorrelation",authority),
                new KeyValuePair<string, Dictionary<string, object>>("guard",guard),new KeyValuePair<string, Dictionary<string, object>>("barrier",barrier),
                new KeyValuePair<string, Dictionary<string, object>>("history",history),new KeyValuePair<string, Dictionary<string, object>>("cleanup",cleanup),
                new KeyValuePair<string, Dictionary<string, object>>("callCounts",calls) })
                foreach (var key in CanonicalKeys(pair.Key))
                    facts.Add(Fact(pair.Key + "." + key, Canonical(pair.Value[key]),
                        key == "executionGeneration" || key == "memoryGeneration" || Canonical(pair.Value[key]) == "Unknown" ? "Unknown" : "SourceEstablished"));
            facts.Add(Fact("diagnostic.generationUnobservedReason","StructureGenerationOutsideScope","SourceEstablished"));
            facts.Add(Fact("diagnostic.barrierEvidenceAbsentReason","NoBarrierEvidenceRequiredForThisPreEntryOrPureRow","SourceEstablished"));
            facts.Add(Fact("diagnostic.cleanupNotRequiredReason","StructureAuditHasNoRuntimeCleanup","SourceEstablished"));
            facts.Add(Fact("diagnostic.strictAuditCompleted", status == "Passed" ? "Yes" : "No", "SourceEstablished"));
            var record = new Dictionary<string, object> {
                {"id",expected["Id"]},{"schemaVersion",1L},{"requirementIds",expected["RequirementIds"]},{"acceptanceIds",expected["AcceptanceIds"]},
                {"expectedCase",fullName},{"status",status},{"actualEvidenceKind","Structure"},{"checkpointFamily","None"},
                {"checkpointName",null},{"checkpointValue",null},{"outcome",null},{"phase",null},{"applicability","Required"},{"reached",reached},
                {"before",new Dictionary<string,object>{{"snapshotSha256",null},{"facts",new List<object>()}}},
                {"after",new Dictionary<string,object>{{"snapshotSha256",null},{"facts",facts}}},
                {"callCounts",calls},{"authorityCorrelation",authority},{"guard",guard},{"barrier",barrier},{"history",history},{"cleanup",cleanup},{"failureReason",failure} };
            TestContext.Out.WriteLine(WriteJson(record));
        }
        private static Dictionary<string, object> Fact(string name, string value, string certainty)
        { return new Dictionary<string, object>{{"name",name},{"value",value},{"certainty",certainty},{"evidenceReference",null}}; }
        private static string[] CanonicalKeys(string group)
        {
            switch (group)
            {
                case "authorityCorrelation": return "confirmed,owner,adapter,router,root,proof,c2Result,receipt,executionGeneration,memoryGeneration,nextEpoch,checked".Split(',');
                case "guard": return "adapter,router,permanentFaultRecorded".Split(',');
                case "barrier": return "state,ordinaryWriterBlocked,evidenceReference".Split(',');
                case "history": return "originalCommitPreserved,originalEpochPreserved,oldTakePreserved,oldCursorPreserved,receiptReferencePreserved".Split(',');
                case "cleanup": return "required,releaseSignaled,workerLeaseReleased,workerJoined,originalThreadCleanup,exception".Split(',');
                case "callCounts": return "c1Begin,c2Finalize,freshReserve,freshComplete,preparedObserver,c2Observer,nativeCallback".Split(',');
                default: throw new InvalidOperationException("정규 사실 그룹 오류");
            }
        }
        private static string Canonical(object value)
        { return value == null ? "null" : value is bool ? ((bool)value ? "true" : "false") : value is long ? ((long)value).ToString(CultureInfo.InvariantCulture) : (string)value; }
        private static string WriteJson(object value)
        {
            if (value == null) return "null";
            var text = value as string;
            if (text != null) { var output = new StringBuilder("\""); foreach(var c in text) { if(c == '"' || c == '\\') output.Append('\\').Append(c); else if(c < 32) output.Append("\\u").Append(((int)c).ToString("x4",CultureInfo.InvariantCulture)); else output.Append(c); } return output.Append('"').ToString(); }
            if (value is bool || value is long) return Canonical(value);
            var dictionary = value as Dictionary<string,object>;
            if (dictionary != null) return "{" + string.Join(",",dictionary.Keys.OrderBy(key => key == "id" ? 0 : 1)
                .ThenBy(key => key, StringComparer.Ordinal).Select(key => WriteJson(key)+":"+WriteJson(dictionary[key])).ToArray()) + "}";
            var array = value as List<object>;
            if (array != null) return "["+string.Join(",",array.Select(WriteJson).ToArray())+"]";
            throw new InvalidOperationException("지원하지 않는 JSON 값");
        }
        private static string Hash(string path)
        {
            using (var sha = SHA256.Create())
                return BitConverter.ToString(sha.ComputeHash(File.ReadAllBytes(path))).Replace("-", "");
        }

        // 문자열이나 주석 안 중괄호가 메서드 경계를 바꾸지 못한다.
        private static string Method(string source, string signature)
        {
            var start = source.IndexOf(signature, StringComparison.Ordinal);
            Assert.That(start, Is.GreaterThanOrEqualTo(0), signature);
            Assert.That(source.IndexOf(signature, start + signature.Length, StringComparison.Ordinal), Is.EqualTo(-1), signature);
            var open = source.IndexOf('{', start);
            Assert.That(open, Is.GreaterThan(start), signature);
            var depth = 0; var state = 0;
            for (var i = open; i < source.Length; i++)
            {
                var c = source[i]; var next = i + 1 < source.Length ? source[i + 1] : '\0';
                if (state == 1) { if (c == '\n') state = 0; continue; }
                if (state == 2) { if (c == '*' && next == '/') { state = 0; i++; } continue; }
                if (state == 3 || state == 4) { if (c == '\\') { i++; continue; } if (c == (state == 3 ? '"' : '\'')) state = 0; continue; }
                if (state == 5) { if (c == '"') { if (next == '"') i++; else state = 0; } continue; }
                if (c == '/' && next == '/') { state = 1; i++; continue; }
                if (c == '/' && next == '*') { state = 2; i++; continue; }
                if (c == '@' && next == '"') { state = 5; i++; continue; }
                if (c == '"') { state = 3; continue; }
                if (c == '\'') { state = 4; continue; }
                if (c == '{') depth++;
                if (c == '}' && --depth == 0) return source.Substring(start, i - start + 1);
            }
            Assert.Fail("닫히지 않은 감사 메서드: " + signature);
            return null;
        }

        private static string Transfer(string source)
        {
            const string begin = "TryTakeRetainedIntent"; const string end = "private void Awake";
            var first = source.IndexOf(begin, StringComparison.Ordinal);
            var last = source.IndexOf(end, StringComparison.Ordinal);
            Assert.That(first, Is.GreaterThanOrEqualTo(0)); Assert.That(last, Is.GreaterThan(first));
            return source.Substring(first, last - first);
        }

        private static void Forbidden(string source, params string[] tokens)
        { foreach (var token in tokens) Assert.That(source, Does.Not.Contain(token), token); }

        private static void AuditPredecessors()
        {
            for (var i = 0; i < AuditNames.Length; i++)
                Assert.That(Hash(AuditRoot + AuditNames[i] + ".txt"), Is.EqualTo(AuditHashes[i]), AuditNames[i]);
            for (var i = 0; i < 3; i++)
                Assert.That(File.ReadAllBytes("Assets/AcadeGameMaker/Tests/EditMode/HubPresentation/" + AuditNames[i]),
                    Is.EqualTo(File.ReadAllBytes(AuditRoot + AuditNames[i] + ".txt")), AuditNames[i]);
        }

        [Test] [Timeout(180000)]
        public void AC009_LowerExecutorHasNoHubTypeOrPublicAuthority()
        {
            var source = Read(InputRoot + "ProfileResetExecutionBridgeV1.cs");
            Forbidden(source, "AcadeGameMaker.Hub", "HubMenuPresenter", "HubMenuIntentHandoffOwner", "NewGameConfirmationOwner",
                "SceneManager", "Application.Quit", "UnityEvent", "InputSystem", "DateTime.Now", "DateTime.UtcNow", "UnityEngine.Random",
                "System.Net", "HttpClient", "WebRequest", "File.Delete", "Directory.Delete");
            var assembly = Type.GetType("AcadeGameMaker.Input.Unity.InputRouter, AcadeGameMaker.Input.Unity", true).Assembly;
            var factory = assembly.GetType("AcadeGameMaker.Input.Unity.ProfileResetExecutionTestControlFactoryV1", true);
            Assert.That(factory.IsPublic, Is.False);
            var create = factory.GetMethods(System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
            Assert.That(create.Length, Is.EqualTo(1)); Assert.That(create[0].Name, Is.EqualTo("Create"));
            var checkpoint = assembly.GetType("AcadeGameMaker.Input.Unity.ProfileResetExecutionCheckpointV1", true);
            Assert.That(create[0].GetParameters().Select(p => p.ParameterType).ToArray(),
                Is.EqualTo(new[] { typeof(Action<>).MakeGenericType(checkpoint), typeof(Action), typeof(Action<object>), typeof(Action<object>) }));
            Assert.That(create[0].ReturnType.FullName, Is.EqualTo("AcadeGameMaker.Input.Unity.IProfileResetExecutionTestControlV1"));
            var body = Method(source, "internal static IProfileResetExecutionTestControlV1 Create(");
            var approvedFactory = "internal static IProfileResetExecutionTestControlV1 Create(Action<ProfileResetExecutionCheckpointV1> checkpoint, Action postDiskPreparedValidatedPreC2, Action<object> actualPreparedProof, Action<object> actualC2Result) { if (checkpoint == null || postDiskPreparedValidatedPreC2 == null || actualPreparedProof == null || actualC2Result == null) throw new ArgumentNullException(); return new Control(checkpoint,postDiskPreparedValidatedPreC2,actualPreparedProof,actualC2Result); }";
            Assert.That(Compact(body), Is.EqualTo(Compact(approvedFactory)));
            var factorySource = Method(source, "internal static class ProfileResetExecutionTestControlFactoryV1");
            var forwarding = new[] {
                "public void Checkpoint(ProfileResetExecutionCheckpointV1 checkpoint) { _checkpoint(checkpoint); }",
                "public void PostDiskPreparedValidatedPreC2() { _prepared(); }",
                "public void ObserveActualPreparedProofBeforeRowValidation(ProfileResetDiskPreparedProofV1 actual) { _proof(actual); }",
                "public void ObserveActualC2ResultBeforeComposition(ProfileResetMemoryResultV1 actual) { _memory(actual); }" };
            foreach (var exact in forwarding)
                Assert.That(Compact(Method(factorySource, exact.Substring(0, exact.IndexOf('{')).TrimEnd())), Is.EqualTo(Compact(exact)));
            Forbidden(body, "ConditionalWeakTable", "UnityEngine", "ExecuteConfirmedReset(", "new ProfileResetDiskPreparedProof", "new ProfileResetExecutionResult");
            var normal = Method(source, "internal static ProfileResetExecutionResultV1 ExecuteConfirmedReset(ConfirmedProfileResetRequestV1 confirmed,DesktopProfileLaunchAdapterV1 adapter,InputRouter router)");
            Assert.That(normal, Does.Contain("ProfileResetExecutionNoOpControlV1.Instance"));
            Forbidden(normal, "Factory", "Action", "Control(");
            foreach (var type in assembly.GetTypes().Where(t => t.Name.StartsWith("ProfileResetExecution", StringComparison.Ordinal)
                || t.Name.StartsWith("ProfileResetFresh", StringComparison.Ordinal)))
            {
                Assert.That(type.IsPublic || type.IsNestedPublic, Is.False, type.FullName);
                foreach (var field in type.GetFields(System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Static |
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
                    Assert.That(typeof(Delegate).IsAssignableFrom(field.FieldType), Is.False, field.Name + " 전역 제어 금지");
                foreach (var method in type.GetMethods(System.Reflection.BindingFlags.DeclaredOnly |
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static |
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic))
                {
                    Assert.That(method.ReturnType.FullName ?? "", Does.Not.Contain("AcadeGameMaker.Hub."));
                    foreach (var parameter in method.GetParameters())
                        Assert.That(parameter.ParameterType.FullName ?? "", Does.Not.Contain("AcadeGameMaker.Hub."));
                }
            }
        }

        [Test] [Timeout(180000)]
        public void AC009_QBFreshAmendmentKeepsGameplayAndDurableTokensForbidden()
        {
            AuditPredecessors();
            var current = Read(HubRoot + "HubMenuIntentHandoffOwnerV1.cs");
            HubMenuIntentHandoffC3AuditInputV1.AssertForbiddenBody(current);
            Forbidden(current, "Action<", "Action ", "Func<", "event ", "System.Net", "DateTime.Now", "DateTime.UtcNow", "UnityEngine.Random");
            var previous = Read(PreviousRoot + "HubMenuIntentHandoffOwnerV1.cs.txt");
            Assert.That(Method(current, "internal bool TryTakeRequest("),
                Is.EqualTo(Method(previous, "internal bool TryTakeRequest(")));
            foreach (var name in new[] { "ReserveExecutionFresh", "CommitExecutionFresh", "MatchesExecutionFreshReservation", "HasLiveExecutionFreshPermission" })
            {
                Assert.That(current, Does.Contain(name + "("));
                Assert.That(current, Does.Not.Contain("public " + name));
            }
            Assert.That(current, Does.Contain("_takenRequest"));
            Assert.That(current, Does.Contain("ConditionalWeakTable<"));
            var reserve = Method(current, "internal HubNewGameExecutionFreshReservationV1 ReserveExecutionFresh(");
            var commit = Method(current, "internal void CommitExecutionFresh(");
            Assert.That(reserve, Does.Contain("IsExecutionFreshInProgress("));
            Assert.That(commit, Does.Contain("MatchesPreparedExecutionFresh("));
            Forbidden(reserve + commit, "TryTakeRequest(", "TryTakeRetainedIntent(");
            Assert.That(System.Text.RegularExpressions.Regex.IsMatch(reserve + commit, @"\b_takenRequest\s*="), Is.False);
            OriginalTail(current, previous, "private void LateUpdate(");
        }

        [Test] [Timeout(180000)]
        public void AC009_QAFreshAmendmentPreservesOriginalTransferAudit()
        {
            AuditPredecessors();
            var current = Read(HubRoot + "HubMenuPresenterV1.cs");
            var previous = Read(PreviousRoot + "HubMenuPresenterV1.cs.txt");
            Assert.That(Transfer(current), Is.EqualTo(Transfer(previous)));
            Forbidden(Transfer(current), "SceneManager", "Application.Quit", "new InputRouter", "new HubMenuPresentationControllerV1", "UnityEvent", "delegate");
            Forbidden(current, "SceneManager", "Application.Quit", "new InputRouter", "new HubMenuPresentationControllerV1", "UnityEvent", "delegate", "ProfileReset", "System.Net", "DateTime.Now", "UnityEngine.Random");
            Forbidden(current, "Action<", "Action ", "Func<", "event ", "DateTime.UtcNow");
            foreach (var name in new[] { "PrepareExecutionFresh", "MatchesPreparedExecutionFresh", "TryTakeExecutionFreshIntent" })
                Assert.That(current, Does.Contain(name + "("));
            var prepare = Method(current, "internal void PrepareExecutionFresh(");
            Assert.That(prepare, Does.Contain("MatchesExecutionFreshReservation("));
            var take = Method(current, "internal bool TryTakeExecutionFreshIntent(");
            Assert.That(take, Does.Contain("ValidateExecutionFreshSlot();"));
            var validate = Method(current, "private void ValidateExecutionFreshSlot(");
            Assert.That(validate, Does.Contain("HasLiveExecutionFreshPermission("));
            Assert.That(take.IndexOf("ValidateExecutionFreshSlot();", StringComparison.Ordinal),
                Is.LessThan(take.IndexOf("request=new HubMenuIntentRequestV1", StringComparison.Ordinal)));
            Forbidden(prepare + take, "_confirmationInteractionClosed=false", "_confirmationInteractionClosed = false",
                "_consumedTransfer=false", "_consumedTransfer = false", "_retainedIntent=default", "_retainedIntent = default");
            Assert.That(System.Text.RegularExpressions.Regex.IsMatch(prepare + take,
                @"\b_(?:confirmationInteractionClosed|controller|cursor|retainedIntent|retainedIntentProof|consumedTransferIntent|consumedTransferIntentProof)\s*="), Is.False);
            Forbidden(take, "TryTakeRetainedIntent(", "TryTakeSuccessorIntent(");
            OriginalTail(current, previous, "private void Update(");
            OriginalTail(current, previous, "private void FixedUpdate(");
        }

        [Test] [Timeout(180000)]
        public void AC009_Q0SelectsOnlyExactChangedCurrentSuccessor()
        {
            const string path = "docs/evidence/c4-audit-current-source-pins-v2.json";
            var pins = Object(new JsonReader(Read(path)).Read()); Keys(pins, "SchemaVersion", "Trace", "RuntimePins");
            Assert.That(pins["SchemaVersion"], Is.TypeOf<long>()); Assert.That(pins["SchemaVersion"], Is.EqualTo(1L));
            Assert.That(Array(pins["Trace"]).All(v => v is string), Is.True);
            var rows = Array(pins["RuntimePins"]); Assert.That(rows.Count, Is.EqualTo(2));
            var required = new[] { InputRoot + "DesktopProfileLaunchAdapterV1.cs", InputRoot + "InputRouter.cs" };
            var predecessors = new[] { "1798F51244FCAFD8EE4B0D847AABBC6DA89EC8EA7138A12826CB37810264DCFB", "66CD817B322565753A3533EA93FFA2BD4E347B24C9A61D5F9C4A1A1169EC54AB" };
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var value in rows)
            {
                var row = Object(value); Keys(row, "Path", "Sha256", "Predecessor", "ChangeApproval", "StaticReview");
                var index = System.Array.IndexOf(required, row["Path"] as string);
                Assert.That(index, Is.GreaterThanOrEqualTo(0)); Assert.That(seen.Add((string)row["Path"]), Is.True);
                Assert.That(Hash(required[index]), Is.EqualTo(row["Sha256"]));
                var previous = Object(row["Predecessor"]); Keys(previous, "Path", "Sha256");
                Assert.That(previous["Path"], Is.EqualTo(PreviousRoot + Path.GetFileName(required[index]) + ".txt"));
                Assert.That(previous["Sha256"], Is.EqualTo(predecessors[index])); Binding(previous);
                foreach (var name in new[] { "ChangeApproval", "StaticReview" })
                {
                    var binding = Object(row[name]); Keys(binding, "Path", "Sha256"); Binding(binding);
                    var prefix = name == "ChangeApproval" ? "docs/specs/work-contracts/" : "docs/verification/";
                    Assert.That((string)binding["Path"], Does.StartWith(prefix));
                    if (name == "StaticReview") Assert.That(Read((string)binding["Path"]), Does.Contain((string)row["Sha256"]), name + " 원문 지문 결속");
                    else Assert.That(Read((string)binding["Path"]), Does.Contain("Approved"));
                }
            }
            var q0 = Read("Assets/AcadeGameMaker/Tests/EditMode/InputUnity/HubUiOnlyQ0ScopeAuditEditModeTests.cs");
            Assert.That(q0, Does.Contain(path)); Assert.That(q0, Does.Contain(Hash(path)));
            var previousQ0 = Read(AuditRoot + "HubUiOnlyQ0ScopeAuditEditModeTests.cs.txt");
            foreach (var signature in new[] { "private static readonly EvidenceHash[] EvidenceHashedSources", "private static readonly EvidenceHash[] HistoricalCurrentHashedSources",
                "private static readonly string[] RequiredCurrentQ0Paths", "private static IEnumerable<string> CurrentRepositoryChangedOrUntrackedPaths(" })
                Assert.That(Method(q0, signature), Is.EqualTo(Method(previousQ0, signature)), signature);
            var originalSuccessors = Method(previousQ0, "private static readonly EvidenceHash[] CurrentSuccessorHashedSources");
            Assert.That(Method(q0, "private static readonly EvidenceHash[] CurrentSuccessorHashedSources"), Is.EqualTo(originalSuccessors), "원래 C2/C2R successor 역사 두 행");
            const string callerSignature="public void AC011_EvidenceBackedHistoricalSourcesAndPinnedC2C2RSuccessorsMatchTheirSeparateManifests(";
            var expectedCaller=Method(previousQ0,callerSignature)
                .Replace("Assert.That(Hash(path), Is.EqualTo(entry.Hash), entry.Path + \" fixed successor hash\");",string.Empty)
                .Replace("foreach (var entry in CurrentSuccessorHashedSources)","AssertC4CurrentSuccessorPins(root); foreach (var entry in CurrentSuccessorHashedSources)");
            Assert.That(Compact(Method(q0,callerSignature)),Is.EqualTo(Compact(expectedCaller)),"한정된 current successor hash 대체 경계");
            Assert.That(Method(q0,"private static void AssertSuccessorProvenance("),Is.EqualTo(Method(previousQ0,"private static void AssertSuccessorProvenance(")));
            var helper=Method(q0,"private static void AssertC4CurrentSuccessorPins(string root)");
            var bindingHelper=Method(q0,"private static void AssertC4PinBinding(string root,C4PinBinding binding,string exactPath,string exactSha256)");
            var resourceHash=helper.IndexOf("Assert.That(Hash(resourcePath),Is.EqualTo(C4CurrentSuccessorPinSha256)",StringComparison.Ordinal);
            var parse=helper.IndexOf("JsonUtility.FromJson<C4PinResource>",StringComparison.Ordinal);
            Assert.That(resourceHash,Is.GreaterThanOrEqualTo(0)); Assert.That(parse,Is.GreaterThan(resourceHash));
            Forbidden(helper,"return;","catch");
            StringAssert.Contains("private const string C4CurrentSuccessorPinPath = \""+path+"\";",q0);
            StringAssert.Contains("private const string C4CurrentSuccessorPinSha256 = \""+Hash(path)+"\";",q0);
            StringAssert.Contains("C8FD2B74EF842F45B8D202B1DE3DC408F09F20F69C1BD030FE589E43B8DDD7F7",helper);
            StringAssert.Contains("predecessor.Files.Single(entry=>entry.SourcePath==original.Path)",helper);
            StringAssert.Contains("AssertC4PinBinding(root,pin.Predecessor,preserved.PreservedPath,original.Hash);",helper);
            StringAssert.Contains("AssertC4PinBinding(root,pin.ChangeApproval,",helper);
            StringAssert.Contains("AssertC4PinBinding(root,pin.StaticReview,",helper);
            Assert.That(Compact(bindingHelper),Is.EqualTo(Compact("private static void AssertC4PinBinding(string root,C4PinBinding binding,string exactPath,string exactSha256) { Assert.That(binding,Is.Not.Null); Assert.That(binding.Path,Is.EqualTo(exactPath)); Assert.That(binding.Sha256,Is.EqualTo(exactSha256)); Assert.That(Hash(Path.Combine(root,binding.Path.Replace('/',Path.DirectorySeparatorChar))),Is.EqualTo(binding.Sha256)); }")));
        }

        [Test] [Timeout(180000)]
        public void AC010_PredecessorBodiesAndRequiredSelectionNamesRemainExact()
        {
            AuditPredecessors();
            var manifest = Object(new JsonReader(Read("docs/evidence/c4-runtime-predecessor-manifest.json")).Read());
            var originals = Array(manifest["Files"]); Assert.That(originals.Count, Is.EqualTo(9));
            var unique = new HashSet<string>(StringComparer.Ordinal);
            foreach (var value in originals)
            {
                var row = Object(value); Keys(row, "Sha256", "SourcePath", "PreservedPath");
                Assert.That(unique.Add((string)row["SourcePath"]), Is.True);
                Assert.That(RuntimePredecessors.ContainsKey((string)row["SourcePath"]), Is.True);
                Assert.That(row["Sha256"], Is.EqualTo(RuntimePredecessors[(string)row["SourcePath"]]));
                Assert.That((string)row["PreservedPath"], Is.EqualTo(PreviousRoot + Path.GetFileName((string)row["SourcePath"]) + ".txt"));
                Assert.That(Hash((string)row["PreservedPath"]), Is.EqualTo(row["Sha256"]));
            }
            Assert.That(File.ReadAllBytes(PreviousRoot + "ProfileResetDiskTransactionV1.cs.txt"),
                Is.EqualTo(File.ReadAllBytes("Assets/AcadeGameMaker/Runtime/Profile/ProfileResetDiskTransactionV1.cs")));
            // 선택 원본의 전체 바이트 지문은 이름·순서·선택 문자열·개수를 함께 결속한다.
            var paths = new[] { "c3-r11-editmode-focused-selection.json", "c3-r11-playmode-focused-selection.json",
                "c3l-edit-regression-selection.json", "c2-r36-selection-preflight.json", "c3l-play-regression-selection.json",
                "c3-r11-edit-matrix-selection.json", "c3-r11-edit-remaining-selection.json" };
            var hashes = new[] { "66B62A29BC1626A72D60607CA92DA6C880D09870D16CE0D16201DC117CAC4CF6",
                "C63B2946E83FF4F4862197E8C5EDFDC25B488C3AACC5C0628F612553A485CA01",
                "FBD1B5A6FE6AD6E96CA5CDAD4103C9DFA129B3F5F622859E11D0B43FD4D649F8",
                "B2CF18E155D98BA7ECAC1E8CABE5EAF90CF6C48CC0419D1ECC800758220120E3",
                "FFFF443666F58B8518A6E89C206AF8BF23A0EF559888AAC8D528E3111591FB9E",
                "B04192B8627B3BC03F934B102ADA35D3CD095757C88DC4ED2AD9FD476F2ACBED",
                "F8CCD04044B67A76E3033CD3D6A5A169B98BA5B25107801B26C667520B125EAF" };
            for (var i = 0; i < paths.Length; i++) Assert.That(Hash("artifacts/" + paths[i]), Is.EqualTo(hashes[i]), paths[i]);
        }

        private static Dictionary<string, object> Object(object value)
        { Assert.That(value, Is.TypeOf<Dictionary<string, object>>()); return (Dictionary<string, object>)value; }
        private static void OriginalTail(string current, string previous, string signature)
        {
            var original = Method(previous, signature);
            var tail = original.Substring(original.IndexOf('{') + 1);
            Assert.That(Method(current, signature).EndsWith(tail, StringComparison.Ordinal), Is.True, signature + " 원래 본문 꼬리");
        }
        private static string Compact(string value) { return System.Text.RegularExpressions.Regex.Replace(value, @"\s+", ""); }
        private static List<object> Array(object value)
        { Assert.That(value, Is.TypeOf<List<object>>()); return (List<object>)value; }
        private static void Keys(Dictionary<string, object> value, params string[] keys)
        { Assert.That(value.Keys.OrderBy(k => k, StringComparer.Ordinal).ToArray(), Is.EqualTo(keys.OrderBy(k => k, StringComparer.Ordinal).ToArray())); }
        private static void Binding(Dictionary<string, object> value)
        {
            var path = value["Path"] as string; var sha = value["Sha256"] as string;
            Assert.That(path, Is.Not.Null); Assert.That(sha, Does.Match("^[A-F0-9]{64}$"));
            Assert.That(path, Does.Not.Contain("\\")); Assert.That(path.Split('/'), Does.Not.Contain(".."));
            Assert.That(path, Does.Match("^[A-Za-z0-9_./-]+$"));
            Assert.That(path.Split('/').Any(segment => segment == "" || segment == "."), Is.False);
            Assert.That(Path.IsPathRooted(path), Is.False); Assert.That(Hash(path), Is.EqualTo(sha));
        }

        // 중복 키 및 정수 강제 변환을 거절하는 입력 전용 파서다.
        private sealed class JsonReader
        {
            private readonly string _text; private int _position; private int _depth;
            internal JsonReader(string text) { _text = text; }
            internal object Read()
            { var value = Value(); White(); if (_position != _text.Length) throw new InvalidOperationException("여분 JSON 입력"); return value; }
            private void White() { while (_position < _text.Length && " \t\r\n".IndexOf(_text[_position]) >= 0) _position++; }
            private char Peek() { White(); if (_position >= _text.Length) throw new InvalidOperationException("JSON 입력 끝"); return _text[_position]; }
            private void Need(char c) { if (Peek() != c) throw new InvalidOperationException("JSON 구분자 오류"); _position++; }
            private object Value()
            {
                if (++_depth > 64) throw new InvalidOperationException("JSON 중첩 한도 초과");
                try { return ValueCore(); } finally { _depth--; }
            }
            private object ValueCore()
            {
                var c = Peek();
                if (c == '{')
                {
                    _position++; var result = new Dictionary<string, object>(StringComparer.Ordinal);
                    if (Peek() == '}') { _position++; return result; }
                    while (true) { var key = String(); Need(':'); if (result.ContainsKey(key)) throw new InvalidOperationException("중복 JSON 키"); result.Add(key, Value()); if (Peek() == '}') { _position++; return result; } Need(','); }
                }
                if (c == '[')
                {
                    _position++; var result = new List<object>(); if (Peek() == ']') { _position++; return result; }
                    while (true) { result.Add(Value()); if (Peek() == ']') { _position++; return result; } Need(','); }
                }
                if (c == '"') return String();
                foreach (var literal in new[] { "true", "false", "null" })
                    if (_text.Substring(_position).StartsWith(literal, StringComparison.Ordinal)) { _position += literal.Length; return literal == "null" ? null : (object)(literal == "true"); }
                var start = _position; if (c == '-') _position++;
                if (_position >= _text.Length || !char.IsDigit(_text[_position])) throw new InvalidOperationException("JSON 정수 오류");
                if (_text[_position] == '0') _position++; else while (_position < _text.Length && _text[_position] >= '0' && _text[_position] <= '9') _position++;
                return long.Parse(_text.Substring(start, _position - start), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture);
            }
            private string String()
            {
                Need('"'); var output = new StringBuilder();
                while (_position < _text.Length)
                {
                    var c = _text[_position++]; if (c == '"') return output.ToString();
                    if (c < 32) throw new InvalidOperationException("JSON 문자열 제어 문자");
                    if (c != '\\') { output.Append(c); continue; }
                    if (_position >= _text.Length) throw new InvalidOperationException("JSON 이스케이프 끝");
                    c = _text[_position++];
                    switch (c) { case '"': case '\\': case '/': output.Append(c); break; case 'b': output.Append('\b'); break; case 'f': output.Append('\f'); break; case 'n': output.Append('\n'); break; case 'r': output.Append('\r'); break; case 't': output.Append('\t'); break;
                        case 'u': if (_position + 4 > _text.Length) throw new InvalidOperationException("JSON 유니코드 끝"); output.Append((char)int.Parse(_text.Substring(_position, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture)); _position += 4; break;
                        default: throw new InvalidOperationException("JSON 이스케이프 오류"); }
                }
                throw new InvalidOperationException("닫히지 않은 JSON 문자열");
            }
        }
    }
}
