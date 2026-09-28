# AST-UI-SHADER-001 — uGUI 2.6.0 TMP Mobile SDF shader pair

- Verified: 2026-09-27
- Implementer: Terra (`gpt-5.6-terra`)
- Scope: `REQ-M5D7QA-007`, `REQ-M5D7QA-009`; `AC-M5D7QA-002`,
  `AC-M5D7QA-008`, `AC-M5D7QA-010`
- Status: exact source and license bytes installed; generated SDF/material and
  Unity identity evidence remain implementation-verification work.

## Provenance

The installed package is `com.unity.ugui` `2.6.0`, resolved at package
fingerprint `23caec89ae2780ae2aa9b14f95a19c03e3dcdf9e` by
`Packages/manifest.json` and `Packages/packages-lock.json`. Its embedded
`Package Resources/TMP Essential Resources.unitypackage` is not imported or
stored in this repository; it is 804,874 bytes, SHA-256
`26CDEE2072683CB25CEAFA4FAB23C93C35B2F692E5DAF9CA0F33D05C4E163274`.

Only these two upstream `asset` members were copied byte-for-byte:

| Upstream member | Upstream GUID | Project path | Bytes | SHA-256 |
|---|---|---|---:|---|
| `Assets/TextMesh Pro/Shaders/TMP_SDF-Mobile.shader` | `fe393ace9b354375a9cb14cdbbc28be4` | `Assets/UI/Shaders/Hub/TMP_SDF-Mobile.shader` | 8,074 | `44C39AABC7E88E7E1FEFFC1880DF754F4ADF86B62E9512AA89E9D2D65171AFF4` |
| `Assets/TextMesh Pro/Shaders/TMPro_Properties.cginc` | `3997e2241185407d80309a82f9148466` | `Assets/UI/Shaders/Hub/TMPro_Properties.cginc` | 2,707 | `66DB1F03E8D7A413EBA79BCB6602FDB2DE710B586F34A6F2584ED6F68F028E90` |

The shader declares exactly `TextMeshPro/Mobile/Distance Field` and includes
the admitted local `TMPro_Properties.cginc`; `UnityCG.cginc` and
`UnityUI.cginc` remain engine includes. No other TMP shader or Essential
Resources member is introduced.

## Fixed project identity

| Path | GUID | SHA-256 |
|---|---|---|
| `Assets/UI/Shaders.meta` | `070c509b184d2bca114677820f278a46` | `142F6FE56DA2CF181D4E55B0C86F0A10ECFD0A1CE266BFAAF1FCFC0ABB5C9BC8` |
| `Assets/UI/Shaders/Hub.meta` | `12819597576562f134f4225299f186fe` | `0441D34AA04ED1622960D9B115A249E452A1E75B97DB0E1DCF8D8A861EAAE5A9` |
| `TMP_SDF-Mobile.shader.meta` | `5d56ff4c414a9d872126e38007c1d57d` | `94D3127E88D565DE1A4D71F6FB8F4E08A2CAEAC3D2FD6D8BE0277C033E9BF91C` |
| `TMPro_Properties.cginc.meta` | `f0cdd71d926cc071f345d0b51adef566` | `E8B9FD72E432CE711D125188E548A461263E1A74FE673A01D736984BCFCCDAF8` |

## License notice

The exact 431-byte uGUI `LICENSE.md` is retained as
`third_party/unity-ugui-2.6.0-LICENSE.md`, SHA-256
`3F8833F9736C0B5DB5076663BA5ABB0A33606712FA9128456AC834FC9D43FDD6`.
It preserves the uGUI copyright and Unity Companion License notice. The
official license is [Unity Companion License](https://unity.com/legal/licenses/unity-companion-license).
The copied source is used only as the direct project-owned TMP Mobile SDF
surface for this Unity-dependent project; the notice remains distributed with
the copied work.

## Required post-generation evidence

The builder/validator must still prove package identity, `Shader.Find`
reference equality, zero shader compiler errors, immutable source/meta/notice
bytes, no Essential Resources import, and canonical shader references from both
generated static SDF materials. Luna independently verifies those results
before any `Verified` transition.
