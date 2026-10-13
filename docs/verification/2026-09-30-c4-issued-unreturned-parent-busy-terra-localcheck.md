# C4 미반환 부모와 Busy 출처 국소 검사

- 추적: REQ-M5D7QC4-001/003/005/007, AC-M5D7QC4-003/005/008/009/010.
- 근거: Approved `docs/specs/work-contracts/2026-09-30-c4-unreturned-parent-and-busy-source-amendment.md` SHA `D99B4306B9CDECB74E76415954ECF5E3CA28C17514D36AACB5DFDBE0FA45CDF8`.
- 실행: `pwsh -NoProfile -File docs/verification/2026-09-30-c4-issued-unreturned-parent-busy-terra-localcheck.ps1`, 종료값 0, `Checks=175 Matched=175`.
- 원시 결과: `docs/verification/2026-09-30-c4-issued-unreturned-terra-localcheck-r5.json` SHA `D977EC2F04824AAA6104F6B1491C1FB8C0561EC5E68F9502FF9B16DBAFB75B1A`.
- 검사 스크립트 SHA `F85A1DC76E740BA944319C96689A21C10765019D090A033ED28BD36AB69DE31F`; 검증기 SHA `E1D99FEDBA0538A0C3D92EEF3DDB219FF497F46CAD49C62129B23E1A821EF572`.
- 신규 정상 사례는 두 번째 AC005 부모와 C4 17의 순서 있는 실제 lease·Busy 출처 진단이다. 위조 부모, 빠진 lease, 잘못된 lease 값·확실성, Busy의 잘못된 확실성을 거절했다.
- 두 PowerShell 파일 구문 오류 0. 합성 국소 검사이며 실제 CWT, Unity, NUnit 실행이나 독립 수용을 뜻하지 않는다. `WholeAccepted=false`.
