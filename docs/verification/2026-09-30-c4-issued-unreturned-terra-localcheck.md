# C4 발급 후 미반환 경계 도구 국소 점검

- 추적: `REQ-M5D7QC4-001/003/005/006/007`, `AC-M5D7QC4-001/003/005/007/008/009/010`.
- 승인 근거: `docs/specs/work-contracts/2026-09-30-c4-issued-unreturned-boundaries-amendment.md` SHA-256 `0AEC1429EB90B924218C070DF2C07816E4B1C9CE1F9B0E648F71D6F912AD1B6D`.
- 자체 검증 결과: 실제 PowerShell 구문 오류 0건, 합성 국소 조건 166/166 일치, 명령 종료값 0. 정상 형태 일곱 사건과 부모·체크포인트·enum·소비·원본 발급 진단·결과 미발급·실제 계수·proof·장벽·가드·이력·정리·동결 입력 지문 오류를 대조했다. 손상된 동일 proof 참조를 정상 C1 typed 장벽으로 둔갑시키는 반례와 실제 marker 읽기행이 아닌 source를 가리키는 반례도 거절했다. 다섯 실제 callback 계수는 `Observed`, 미도달 완료 수 0만 `SourceEstablished`로 고정하고 여섯 leaf 각각의 반대 확실성 12건을 거절했다.
- 원시 결과: `docs/verification/2026-09-30-c4-issued-unreturned-terra-localcheck-r4.json` SHA-256 `CE4F708E304AFF9E4973F0FE1EBA844AA516742BD28CADFC0ABC6C2AE6A91BC2`. 앞선 52/52, 152/152, 154/154 국소 원시 기록은 그대로 보존한다.
- 검증 스크립트: `docs/verification/2026-09-30-c4-issued-unreturned-terra-localcheck.ps1` SHA-256 `9D124D6EEB38225E13F94EECC4A37A78088A2C759189BACA8D49B12A66777117`.
- 변경 도구: `artifacts/c4-build-required-checkpoint-rows.ps1` SHA-256 `29A9DCA4A1349519018FE7EB9F6F67ED49932B178216E8B915F53D15E2D2617D`; `artifacts/c4-verify-required-rows.ps1` SHA-256 `ABECF98EFDC2627262BE031CFEDC9D8DB05DC967632CD386C7964939238E5F6F`.

이 검사는 부분 합성 객체로 새 규칙 조건만 점검했다. 전체 36개 정규 사실, 실제 CWT, 실제 NUnit/XML, Unity 실행 및 최종 통합 수용을 검증하지 않았다. 독립 검수와 최종 실행은 별도 단계다.
