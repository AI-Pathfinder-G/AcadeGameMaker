# P1 픽셀 UI·SDF 텍스트 배율 승인 기록

- Date: 2026-08-24
- Status: Approved by user
- Partial decision: `OD-ART-001`
- Owner: Sol

## Approved boundary

- HUD·menu layout은 640×360 logical safe frame
- UI pixel frame·icon은 gameplay rectangle과 같은 integer scale·offset 및 point filtering
- TextMeshPro SDF text는 logical layout과 size를 사용하되 world upscale 뒤 final output resolution에서 render
- logical font roles: minimum body 12px, standard body 14px, heading 18px
- interactive hit area minimum 24×24 logical px
- safe-frame edge margin minimum 12 logical px
- safe frame 밖 interactive UI 금지
- 수직 데모에는 user-adjustable UI scale setting 없음

## Remaining boundary

palette role structure와 exact HEX는 후속 승인으로 확정됐다. outline·URP 2D lighting, reticle의 base/acquired pixel size·glow·animation을 확정하기 전에는 `OD-ART-001`을 해결 처리하지 않는다.
