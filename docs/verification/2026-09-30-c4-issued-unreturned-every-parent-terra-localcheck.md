# C4 일곱 미반환 경계 부모 한정 검사

- 추적: REQ-M5D7QC4-001/003/005/007, AC-M5D7QC4-003/005/008/009/010.
- 근거: Approved `docs/specs/work-contracts/2026-09-30-c4-issued-unreturned-boundaries-amendment.md` SHA `0AEC1429EB90B924218C070DF2C07816E4B1C9CE1F9B0E648F71D6F912AD1B6D`, 부모·Busy 보정 SHA `D99B4306B9CDECB74E76415954ECF5E3CA28C17514D36AACB5DFDBE0FA45CDF8`.
- `pwsh -NoProfile -File docs/verification/2026-09-30-c4-issued-unreturned-every-parent-terra-localcheck.ps1`: 종료값 0, `Checks=190 Matched=190`.
- 원시 결과 `docs/verification/2026-09-30-c4-issued-unreturned-terra-localcheck-r8.json` SHA `A984FC6858A567E3D8D7C0315511819B9985E8DF94310728CAA1B8627B2CD1AC`.
- 검증기 SHA `20418B63A488C6ED8D3EF2088BE8FAE719E7E142315B92EE029CFA3AB9C3CD69`; 국소 스크립트 SHA `ADC55C1BB4890C914D4A21447623F96EADD7CBEC693535268388D6A3143E523D`.
- projection 두 부모, Play lifecycle, AC005 두 부모, C4 12/13/17의 정확 NUnit 전체 이름·대소문자를 강제했다. 다른 이름 공간·클래스·대소문자 변형 반례를 거절했다. 합성 검사이며 실제 Unity·CWT 또는 독립 수용은 아니다. `WholeAccepted=false`.
