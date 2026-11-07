using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AcadeGameMaker.Movement.PlayMode.Tests")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Movement.EditMode.Tests")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Movement.Authoring.Editor")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Transfer.Unity")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Transfer.Unity.PlayMode.Tests")]
// REQ-COM-001, REQ-WT-006. The approved -190 combat adapter reads only the completed movement snapshot and exact next tick.
[assembly: InternalsVisibleTo("AcadeGameMaker.Combat.Unity")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Combat.Unity.PlayMode.Tests")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Input.Unity")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Input.Unity.PlayMode.Tests")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Camera.Unity")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Camera.Unity.PlayMode.Tests")]
// REQ-COM-001. Editor-only M3E1 authored-graph construction; no runtime reference is added.
[assembly: InternalsVisibleTo("AcadeGameMaker.Combat.Authoring.Editor")]
#if UNITY_EDITOR || UNITY_INCLUDE_TESTS
[assembly: InternalsVisibleTo("AcadeGameMaker.Run.Unity.PlayMode.Tests")]
#endif
