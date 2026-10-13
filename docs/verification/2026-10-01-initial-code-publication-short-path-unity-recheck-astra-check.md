# 501파일 짧은 경로 Unity 재진단 아스트라 기계 대조

- 승인 계약: [짧은 경로 한 번 재현](../specs/work-contracts/2026-10-01-initial-code-publication-short-path-unity-recheck.md) SHA-256 `9DCF5118B9E46741EB01D3578B35A0AFB34F93127D1BE768B68D26AEE49E947F`, `REQ-PLAT-035..037` / `AC-PLAT-033..035`.
- 테라 결과: JSON（`../../artifacts/c4-initial-publication-short-path-unity-recheck-v1.json`；로컬 보존 자료） SHA-256 `6F7CC1EFB8759D9B05FF20992A0F22295CE19601113A31E7E73E192CADD29CF9`; 판독 문서 SHA-256 `3142470431DBE72D6DAFB7F6D1F8F65536CBB9E3A2D5B81FAEF8121275BFD875`; [보고](2026-10-01-initial-code-publication-short-path-unity-recheck-terra-report.md) SHA-256 `CB0574AB5D6A2292EBE19D101FB86B10A5E89E820F319D5653DBCD0C27A486A6`; 로그 SHA-256 `04FF56A60EF4BDB3C05AFE49F20108258E3987C430EE745715D2BDD5B9DE333A`. 로그는 공유 잠금이 있었으나 공유 읽기 방식으로 바이트 지문을 대조했다.
- `AC-PLAT-033`: 결과 JSON의 501개 행에 원본·복사본 실행 전후 지문 일치, 초기 후보 밖 복사 0, 첫 복사본 501/501, 첫 결과·로그 SHA 불변이 기록됐다. 복사본은 `output/i501-r2`에 남겨 뒀다.
- `AC-PLAT-034`: Unity `6000.6.0f1` 호출은 한 번이며 약 31초 뒤 종료코드 1이었다. 새 로그 6·80~81줄에 라이선스 연결·전역 잠금 실패, 100~101줄에 패키지 관리자 연결 실패가 있다. 자산 가져오기와 C# 컴파일에 도달하지 못했으므로 첫 로그의 `DirectoryNotFoundException` 40줄·`Asset import failed` 13줄이 이번에 0줄인 것은 오류 해소 근거가 아니다. 이번 실행에서 해석된 패키지 버전도 미관측이다.
- `AC-PLAT-035`: 전체·501행의 `PublicationApproved`, `DependencyClosure`, `WholeAccepted`, `LunaReviewed`, `CleanCloneExecuted`가 모두 거짓이다. Renderer2D 일곱 GUID의 미해석 P1은 유지한다.

이 확인은 출력과 로그의 기계적 일관성에 한정한다. 루나 독립 검수나 Unity 가져오기 수용이 아니다. 이미 실행 중인 라이선스 클라이언트가 관측됐지만 중단·재시작하지 않았다. 같은 계약으로 Unity를 다시 실행하지 않는다.
