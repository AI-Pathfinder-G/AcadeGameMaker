# M5B3 — Approved device action asset and generated wrapper

- Status: Verified
- Integrated by: Astra, 2026-09-08 after Luna independent AC-M5B3-001..005 PASS; full EditMode 440/440 and PlayMode 435/435.
- Approved by: Astra, 2026-09-08, following Terra local generator impact and Luna independent pre-gate PASS (P0/P1/P2 = 0)
- Authority: Astra
- Implementation: Terra
- Independent verification: Luna
- Parents: Approved VD-07 REQ-UX-001/006/007/009, VD-09 REQ-PLAT-002/007; ADR-0027

## Scope and fixed technical decisions

Build the actual single `Assets/GameInput.inputactions` asset, generated C# wrapper and virtual-device verification using already installed Input System 1.20.0. No package installation, settings changes, runtime gameplay consumer, InputMode owner or next-tick buffering claim yet. This is the device foundation; the immediately following integration contract supplies InputRouter and mode control.

Exactly two maps: Gameplay and UI. Gameplay actions are Move (Vector2 value), Aim (Vector2 value, gamepad right stick only), Point (Vector2 pass-through, mouse position), Jump, Dash, Interact, ChoiceSkill, Attack, Transfer, Pause (buttons). Point is separate from Aim because mouse screen coordinates and gamepad direction have different meanings. UI actions Navigate (Vector2 value), Point (Vector2 pass-through), Click, ScrollWheel (Vector2 pass-through), Submit, Cancel. UI Click is a button. Do not combine raw mouse position into the Aim action.

Gameplay bindings: Move WASD 2D composite and gamepad leftStick; Point mouse position; Aim gamepad rightStick; Jump Space/gamepad buttonSouth; Dash leftShift/rightShift and gamepad buttonEast; Interact E/buttonWest; ChoiceSkill Q/rightShoulder; Attack mouse leftButton/rightTrigger; Transfer mouse rightButton/leftTrigger; Pause Escape/start. No JKL combat or click movement. UI Navigate WASD and arrow composites, gamepad dpad and leftStick; Point mouse position; Click mouse leftButton; ScrollWheel mouse scroll; Submit Enter/Space/buttonSouth; Cancel Escape/buttonEast. Pause defaults and UI defaults remain protected in later rebind work.

Control schemes: KeyboardMouse (keyboard + mouse), Gamepad (gamepad). Binding groups match those names. Every action/map/binding has a stable unique GUID stored in source JSON; repeated generation must not regenerate IDs. Asset meta GUID is a new fixed GUID chosen once by Terra and recorded in evidence; asset stable identity is that GUID (future profile inputActionsAssetId uses it). Generated wrapper class `GameInputActions`, namespace `AcadeGameMaker.Input`, source `Assets/AcadeGameMaker/Runtime/Input/GameInputActions.cs`, generated only through installed `InputActionCodeGenerator.GenerateWrapperCode` with explicit source path/class/namespace. Do not hand-maintain a second divergent binding definition.

No wrapper asset/map is automatically enabled. Runtime map enable/disable and callback subscription ownership will be InputRouter/InputMode, not this scaffold. Tests may explicitly toggle maps to verify device bindings, but do not claim production map exclusivity from that fixture.

Generation route: use Input System's built-in importer with generateWrapperCode enabled in the asset meta, explicit path/class/namespace above and the installed importer's script GUID. Importer invokes the same official code generator. The first import must exclude the not-yet-generated-wrapper-dependent test assembly until generation completes, then restore its normal unconditional test configuration before any acceptance run. Subsequent imports use the checked-in generated source. No redundant authored Editor generator is needed. Preserve the wrapper meta GUID chosen once. Independent tests may regenerate in memory using the official API and compare normalized bytes.

## Acceptance

- AC-M5B3-001: JSON imports successfully; exactly the specified maps/actions/bindings/control schemes, unique stable IDs and no forbidden/default-template map or action.
- AC-M5B3-002: generated wrapper instantiates the exact same action/binding IDs and settings as the source asset; regenerating is byte-identical after newline normalization. Wrapper initially has both maps disabled and Dispose is valid.
- AC-M5B3-003: InputTestFixture virtual keyboard/mouse/gamepad drives approved movement, pointer/stick aim, jump/dash/attack/transfer/choice/interact/pause callbacks. Schemes provide the same named meanings without JKL or click movement.
- AC-M5B3-004: virtual UI navigation/point/click/scroll/submit/cancel exercise protected defaults and gamepad focus-style inputs; no virtual mouse is created.
- AC-M5B3-005: revised bootstrap checks asset/wrapper presence while still rejecting InputSystem_Actions.inputactions and legacy template assets; package/input-handler checks and full EditMode/PlayMode regressions pass.

## Allowlist and rollback

New asset/meta, Input runtime folder/metas/asmdef referencing Unity.InputSystem, generated wrapper/meta, dedicated Input PlayMode test folder/asmdef/metas/fixture referencing Unity.InputSystem.TestFramework and runtime Input. A small Editor-only generator in new Editor/InputAuthoring folder with asmdef/metas may invoke the local official generator. Generator must touch only the named generated wrapper and must not regenerate source IDs or overwrite unrelated files. Update the obsolete asset-absence assertion in `Tests/Bootstrap/Editor/BootstrapProjectConfigurationTests.cs` only. Documentation: this contract, corresponding evidence, docs/README. No scenes/prefabs/project settings/public gameplay contracts/media modifications.

REQ comments belong in authored generator/tests; generated code retains generator headers and traces to this contract through evidence. Rollback means inverse unit hunks only; preserve dirty work/media, no reset/delete/commit/push. Terra implements; Luna independently verifies; Astra approves/integrates. No Ollama/Orca/automations or billed API.
