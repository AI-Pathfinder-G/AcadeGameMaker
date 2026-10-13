# Q0 외부 권위 준비 보정 v2 — 테라 구현 결과

상태: 키 없는 준비 구현과 합성 검증 완료. 루나 독립 검수 및 아스트라 운영 시작 판정 전이다. 운영키·운영 권위·원격 변경·운영 계획·운영 서명은 발급하지 않았다. 근거는 원 승인 계약 SHA `451998AB32068DF1879E76B0A4CEC2ACB13619CDE7D48F25CB1E0E25A88AADEC`, 입력 기한 보정 승인 SHA `974DE3DE893DF84AB44BDB988B34F69AA8706D25730FAD5E96D2744A00776E09`, 준비 결과 계획 결속 승인 SHA `8520094B77DDFBDF1B482EAABC844C553E7444E44A031D338D1C888B33748537`이다. 소유 `REQ-PLAT-068/069/070`, `AC-PLAT-066/067/068` 범위이며 전체 실행 수용 `AC-PLAT-059/060`은 아니다.

최초 준비 기록의 `preparation.json` SHA `1E96EA32DD1A08AED2A4F737F15864AE200928BBE0689FE953DC9C278E0D9AEA`, `negative-tests.json` SHA `2B74D4B051CA0AE1CE5629D51D00098E683A8E9357BE84B049A5AB61C3D07A24`, 최초 보고 SHA `70275187A9D9C3B4DA845259622FE789A61B2C0EAC0C11B6418074E097CCB555`를 다시 확인하고 그대로 보존했다. 최초 보고의 준비 성공 표기는 .NET `Console.In` 동기 래퍼에서 열린 무응답 입력의 기한을 입증하지 못해 철회한 역사다. 보정 시작 때 관측한 진행자 SHA `9F0CB93BA7576B4DFB8740A67B551A4FE774951048A07D9A8577CA60BCA86D8E`, 합성 시험 SHA `6E990E2C317880F0E6E45022C678A1F9620468C430109AA3829405C01B91531B`도 최종 지문과 구별한다.

새 공개 준비 결과는 최초 경로를 덮어쓰지 않고 `output/q0-external-authority-evidence-r1/preparation-v2.json` SHA `12AF0D043B6AF0B1D6895736D080D108D1BBB6673FF8D5B1C097343AB8478608`와 `negative-tests-v2.json` SHA `C3C270D1A4590FDE0D9DB1E71F1D1097976CFF684F18D007ADC707697755EF81`로 `CreateNew` 생성했다. 두 파일은 동일 준비 식별자 `8ba93947e3624e6f8f30bbf0c40a1de2` 및 키 없는 현재 소스 20경로/각 원시 SHA를 공유하며 자기 SHA나 서로의 미래 SHA를 담지 않는다. 최초 실패 증거의 경로·SHA·사유를 각각 `SupersedesPreparationEvidence`에 기록했다. 새 음성 증거는 25건의 실제 합성 결과와 각 자식 프로세스 표준출력·표준오류·경과시간·종료코드를 포함한다. 새 두 SHA는 키 생성 전 진행자가 다시 읽어 검증하며, 이후 계획의 필수 `CurrentPreparationEvidence` 두 행에 실제 바이트 SHA로 들어간다.

| 최종 허용 소스 | SHA-256 |
|---|---|
| `qa/tools/Test-Q0FixedPlanAuthority.ps1` | `5F337B208D58988C4AB43B91EC58ACAA90E48EE43E2B0A7C09CB11FE9DAF0E9E` |
| `qa/tools/Invoke-Q0ExternalAuthorityIssuance.ps1` | `9607242503F5F6C16445356B0D4716E37F89F1C9716B2D29A0539A97A8161836` |
| `qa/tools/Test-Q0ExternalAuthorityIssuance.ps1` | `B560E9F18405D5C4295F0E2CED2FAFBCD7727FCD2E0DB9515810C8A92C476DB5` |
| `artifacts/c4-canonical-verify-context.ps1` | `9D550A579BA1419A261C2E1877828E51F3B6BA7B2A7D30AF3AD4EFA20B40103E` |
| `artifacts/c4-canonical-authority-plan.schema.json` | `F35D4CDCB2C1702D6532BF14463295B6516D08E5CEF28609C0320FA60EAF0F6B` |

새 복제는 정확 20파일이고 준비본 대비 변경은 위 다섯 경로뿐이며 추가/누락 파일은 0개다. 네 C#·각 메타 등 보호된 준비본 15개는 이전 SHA와 같고 수정 허용 helper 한 개만 달라졌다. 네 PowerShell 소스 구문 오류는 0개였다. 기존 V2 발급 모듈 SHA `0F6937BD52732C49E979C8F8A3F897C81733DD21028BE6EEB194AC0B381D012A`, 기존 준비 원장 SHA `4B2C0F3D0B5D9F0F5465AEB22324792566A2F3D1CA7877C5FD94C9AEBAB69342`, 원시 계보 원장 SHA `E3B86595404638BC15F3CB1281057044ACC731B3C6602542ADA6FAF8126BEAEA`를 재확인했다. 원문 534·LF 전임 534·LF 입력 505는 이 원장들의 보호 참조이며 현재 20파일 복제의 구성원으로 표시하지 않는다. 원래 Git에 쓰지 않았다.

`AC-PLAT-067/068`: `Console.OpenStandardInput()`에서 만든 엄격 UTF-8 reader의 동기 읽기를 .NET 작업 스레드에 격리했다. 단조 기한을 감시하는 PowerShell 스레드는 열린 입력에도 블록되지 않는다. 단일 읽기만 허용하며 기한 초과·EOF·문자 오류·형식 오류가 구별되어 실패한다. `finally`는 발급 세션과 reader/stream을 닫는다. 키 없는 별도 자식 프로세스에서 표준입력을 **열어 두고 데이터 없이** 200밀리초로 시험했으며 실제 읽기 경과 234밀리초, 전체 자식 경과 916밀리초, 강제종료 없음, 종료 2를 관측했다. EOF는 읽기 28밀리초/종료 2, 정상 공개 한 줄은 26밀리초/종료 0, 형식 오류는 29밀리초/종료 2, 잘못된 UTF-8은 26밀리초/종료 2였다. 다섯 경우 모두 reader 폐기 상태와 폐기 후 재호출 거절을 관측했다. 이는 실제 운영키 폐기 관측이 아니다.

`AC-PLAT-066/068`: 합성 RSA 공개키를 검증기의 정확 네 대입문에만 고정한 임시 복제에서 양성 서명과 저장소·권위·계획·스키마·현재 소스 변조, 복제 추가 파일, 다른 키 세대, 중복 JSON, 거짓의 문자열 대체, 신뢰 문맥 부재, 일반 명령행 거절을 확인했다. 추가로 v2 증거 누락·원시 한 바이트 변조·역할 바꿈/중복·v1 선택·검증기 공개 네 자리 밖 변경을 거절했다. 전체 실제 합성 결과는 25/25 통과했다. 검증기는 v2 원장 두 행의 정확 역할·절대 경로·실제 원시 SHA, 동일 준비 ID/20소스 SHA·실패 이력, 음성 결과 25개 및 입력 관측 다섯 개를 확인한다. 최종 검증기 한 파일은 고정 공개값 네 대입만 준비본으로 역치환했을 때 v2의 키 없는 검증기 SHA와 같아야 하고, 다른 19파일은 v2 SHA와 같아야 한다. 현재 계획의 원시 SHA를 서명하므로 두 v2 결과 SHA도 기존 RSA 서명에 결속된다.

실제 주 조정자는 루나의 독립 검수와 아스트라 시작 판정 뒤 단일 생존 PowerShell 프로세스에서 `Invoke-Q0ManualProtocol`을 호출한다. `Q0_PUBLIC_READY` 공개 정보로 원격 정확 두 문서를 게시·병합하고 인증 조회한 공개 커밋·blob SHA-1·원시 SHA-256·원시 Base64·UTC 관측시각 다섯 줄을 기한 내 전달한다. 이 표준입력 자체는 인증 수단이 아니며 주 조정자의 인증 연결 도구 조회와 직접 비교가 신뢰 경계다. 진행자는 현재 계획 파일을 먼저 기록·해시해 `Q0_PLAN_READY` 경로/SHA/nonce를 보여주고, 아스트라가 원시 파일을 확인하여 정확 SHA 한 줄을 돌려준 뒤 v2 두 파일·스키마·최종20소스를 다시 읽고 한 번 서명한다. 세션은 서명 직후 닫고 공개 서명을 재검증한다. 일반 명령행과 기존 자식 C# 호출은 종료 2로 남으며 `ExecutionAuthorized`, `WholeAccepted`, `PublicationApproved`는 거짓이다. 독립 검수·실제 원격 인증·운영 발급 결과는 이 보고로 대신하지 않는다.
