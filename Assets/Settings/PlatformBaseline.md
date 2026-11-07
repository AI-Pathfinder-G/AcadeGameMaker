# Platform baseline evidence

- REQ-PLAT-001: Unity 6000.6.0f1 (revision f7f8ed4d1e24), C#, URP 2D, and a Windows x64 Mono development target are configured from the approved bootstrap template.
- REQ-PLAT-002: the baseline is 2560×1440, resizable, uses a 60 Hz fixed timestep, and sets Physics2D.gravity to (0, -9.81).
- REQ-PLAT-005: this bootstrap supplies project and package configuration only. Editor and Windows build equivalence require the approved batch and build-smoke verification.
- REQ-PLAT-007: the manifest pins com.unity.inputsystem to 1.20.0; Player Settings select Input System Package (New) only. No input-actions asset is part of this unit.

The 640×360 gameplay safe frame is a later camera/presentation contract. Standalone Player Settings do not expose a desktop minimum-window-size field; this bootstrap therefore preserves the required 640×360 baseline as a verification condition without introducing prohibited runtime UI or camera code.
