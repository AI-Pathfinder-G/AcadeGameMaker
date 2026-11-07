using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("AcadeGameMaker.Transfer.EditMode.Tests")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Transfer.Unity")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Transfer.Unity.PlayMode.Tests")]
// REQ-COM-004, REQ-WT-005. M2B2's internal next-tick death-removal seam remains unavailable to public consumers.
[assembly: InternalsVisibleTo("AcadeGameMaker.Combat.Unity")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Combat.Unity.PlayMode.Tests")]
// REQ-COM-001, REQ-WT-003. Editor-only M3E1 authored-graph construction.
[assembly: InternalsVisibleTo("AcadeGameMaker.Combat.Authoring.Editor")]
