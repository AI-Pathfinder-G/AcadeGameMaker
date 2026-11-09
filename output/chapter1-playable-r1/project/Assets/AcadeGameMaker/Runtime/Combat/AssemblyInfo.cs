using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AcadeGameMaker.Combat.EditMode.Tests")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Combat.Unity")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Combat.Unity.EditMode.Tests")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Combat.Unity.PlayMode.Tests")]
// REQ-COM-001, REQ-COM-004. Editor-only M3E1 authored-graph construction.
[assembly: InternalsVisibleTo("AcadeGameMaker.Combat.Authoring.Editor")]
// REQ-M5D1-002/003. The requester consumes only immutable terminal handoff
// values to gate a local input-mode request; it receives no Combat mutation API.
[assembly: InternalsVisibleTo("AcadeGameMaker.Input.Unity")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Input.Unity.PlayMode.Tests")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Run.Unity")]
#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
[assembly: InternalsVisibleTo("AcadeGameMaker.Run.Unity.PlayMode.Tests")]
#endif
