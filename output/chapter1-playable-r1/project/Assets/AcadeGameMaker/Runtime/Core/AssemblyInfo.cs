using System.Runtime.CompilerServices;

// REQ-COM-001, REQ-COM-004. Editor-only type-safe M3E1 authored-graph construction.
[assembly: InternalsVisibleTo("AcadeGameMaker.Combat.Authoring.Editor")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Input.Unity")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Transfer.Unity")]
#if UNITY_EDITOR
[assembly: InternalsVisibleTo("AcadeGameMaker.Tests.EditMode.Run")]
#endif
