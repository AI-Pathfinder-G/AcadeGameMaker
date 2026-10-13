# C4 미반환 부모 전체 이름 국소 재검사

- 추적: REQ-M5D7QC4-003/005/007, AC-M5D7QC4-005/008.
- Approved 개정 SHA `D99B4306B9CDECB74E76415954ECF5E3CA28C17514D36AACB5DFDBE0FA45CDF8`에 따른 두 공개 부모의 정확 NUnit 전체 이름만 허용했다.
- `pwsh -NoProfile -File docs/verification/2026-09-30-c4-issued-unreturned-parent-busy-fullname-terra-localcheck.ps1`: 종료값 0, `Checks=179 Matched=179`.
- 원시 결과 `docs/verification/2026-09-30-c4-issued-unreturned-terra-localcheck-r6.json` SHA `F4461502341D53A50C3E01978EF0342D1DD78C10EB05448EFB003757AFE5A554`.
- 검증기 SHA `E946A1C40ABC0B29C0524D6366E3D16DF30CF5CF70FC6F61212C26B531DF3F59`; 국소 스크립트 SHA `05AF59E3DA85A0A86CC2D144D4EE804CFEFC258A9389ECF2E1E4ABF4EECD7D7E`.
- 다른 이름 공간과 다른 시험 클래스에서 동일 메서드명을 쓴 네 반례를 거절했다. 합성 국소 결과이며 Unity·실제 CWT·독립 통합 수용은 아니다. `WholeAccepted=false`.
