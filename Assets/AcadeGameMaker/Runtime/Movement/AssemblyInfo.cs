using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AcadeGameMaker.Movement.EditMode.Tests")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Movement.Unity")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Movement.PlayMode.Tests")]
// REQ-MOV-001, REQ-MOV-005. The boss integration assembly may consume only
// the internal directive/receipt contract; no public movement API is widened.
[assembly: InternalsVisibleTo("AcadeGameMaker.Combat.Unity")]
[assembly: InternalsVisibleTo("AcadeGameMaker.Combat.Unity.EditMode.Tests")]
// REQ-COM-004. M2B2 PlayMode overflow evidence seeds the existing deterministic motor at an exact global tick.
[assembly: InternalsVisibleTo("AcadeGameMaker.Combat.Unity.PlayMode.Tests")]
// REQ-COM-001. Editor-only M3E1 authored-graph construction; no runtime reference is added.
[assembly: InternalsVisibleTo("AcadeGameMaker.Combat.Authoring.Editor")]
