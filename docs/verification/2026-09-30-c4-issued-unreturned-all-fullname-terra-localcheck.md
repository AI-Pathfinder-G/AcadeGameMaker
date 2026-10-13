# C4 미반환 부모 전체 이름 한정 재검사

- 추적: REQ-M5D7QC4-003/005/007, AC-M5D7QC4-005/008/010.
- Approved 개정 `docs/specs/work-contracts/2026-09-30-c4-unreturned-parent-and-busy-source-amendment.md` SHA `D99B4306B9CDECB74E76415954ECF5E3CA28C17514D36AACB5DFDBE0FA45CDF8`.
- `pwsh -NoProfile -File docs/verification/2026-09-30-c4-issued-unreturned-all-fullname-terra-localcheck.ps1`: 종료값 0, `Checks=184 Matched=184`.
- 원시 결과 `docs/verification/2026-09-30-c4-issued-unreturned-terra-localcheck-r7.json` SHA `9A1DE731AE749C017E200058CDA7596C7624F281014D487C979ECE96C72245B3`.
- 검증기 SHA `0F4C91B2622F097CEED4DBE23D020C0D26D4F7A68535D3EBD88C3A76CC4E2326`; 스크립트 SHA `CA087378347A1E4A3EB0F3AAFC7ECFCEE32F6320D39B3DA94CB0316B58B4F730`.
- C4 17 부모는 정확 NUnit 전체 이름으로 비교한다. 두 AC005 부모의 비교는 대소문자를 구분한다. 다른 이름 공간·시험 클래스·대소문자 변형 다섯 반례를 추가해 거절했다. 합성 검사이며 실제 Unity·CWT 또는 독립 수용은 아니다. `WholeAccepted=false`.
