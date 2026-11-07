using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AcadeGameMaker.Combat.Unity.EditMode.Tests")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Combat.Unity.PlayMode.Tests")]
// REQ-COM-001, REQ-COM-004. Editor-only M3E1 authored-graph construction.
[assembly: InternalsVisibleTo("AcadeGameMaker.Combat.Authoring.Editor")]
// REQ-M5D1-002/003. The terminal requester reads only immutable handoff
// evidence; it receives no mutation or simulation authority.
[assembly: InternalsVisibleTo("AcadeGameMaker.Input.Unity")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Input.Unity.PlayMode.Tests")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Run.Unity.PlayMode.Tests")]
