# C4 QA 증거 프로토콜 r1 독립 설계 검수

검토 대상 제안서 SHA-256: 67271D88CEBBDD4A688F36F7FB766B19DA20FEE060BECD7122A33AC1B9E9E26A. 비교한 실제 입력은 변경 금지 QA 도구 Invoke-UnityQa.ps1 SHA A4DD378FF02105345B63B40C6F446B304B04F9E8B894CCD2218806F42DD7DA85, TestListenerWrapper.cs SHA 726F0939A67520BC569F862686DDC07163E94A079D88DD7C495FE2F6477E0BDD, R11 실제 610 XML SHA F69B7F15B4C826F96CF3E68727443F45F6D4D4D4A234F72B3CB0688D26D7830A다. 검토는 읽기 전용이며 어떤 도구나 시험도 실행하지 않았다.

실제 NUnit XML에는 test-case의 fullname, result, output이 존재하며 R11 610 XML에서 610개 사례 결과와 출력 행을 확인할 수 있다. 따라서 제안의 XML fullname 대조 및 TestContext.Out 기반 행 출력은 현재 관찰 형식과 부합한다. Unity listener의 TestOutput은 비어 있지만 이 XML의 각 결과에 output이 실려 있으므로 이를 손실 근거로 오인하지 않았다.

기존 QA 도구는 정규화된 Windows 절대 경로를 이용해 Unity·프로젝트·XML·로그 인자를 결속하고, 완전 프로세스 inventory와 정확한 소유 Editor attach를 검사한다. Invoke-OwnedEditorWaitStateMachine은 실제 연결된 Process.ExitCode를 읽지만 최종 QA 출력은 분류/상세만 기록한다. 따라서 native exit 증거를 별도 실제 프로세스 관측으로 유지하고 QA 반환값을 native 증거로 대체하지 않는 제안은 적절하다. 기존 도구를 바꾸지 않고 새 하위 runner에서 소유 Editor attach와 실제 종료를 증명하는 방향도 원래 경계와 맞는다.

## P1 — 프로젝트 상호배제 규칙이 미확정

계획은 ProcessLock을 SingleProject·ProjectPath·Exclusive로만 표현하고, 잠금 저장 방식은 미결정 4번으로 남긴다. Astra가 선택한 실행 규칙인 정규화 프로젝트 경로의 SHA-256을 사용한 Local\AcadeGameMaker-C4-<hash> named mutex, 배타 획득, abandoned 상태면 실패하고 실행 중단하는 계약이 아직 제안서에 반영되지 않았다. 경로 입력은 기존 QA 함수의 GetFullPath와 드라이브 루트 외 끝 구분자 제거를 따른다. 다만 QA 경로 비교는 OrdinalIgnoreCase인데 원본 경로 문자열의 대소문자는 보존될 수 있으므로 해시 전 대소문자 canonicalization, 구분자, 인코딩 규칙까지 확정해야 대소문자만 다른 같은 Windows 경로가 서로 다른 mutex를 만들지 않는다. AbandonedMutexException을 새 획득 성공처럼 처리하지 말고, 예외 때 OS상 mutex 소유 여부를 정리한 뒤 어떤 QA 실행도 시작하지 않는 처리를 명시해야 한다. 이를 계획/잠금 evidence와 preflight에 결속하지 않으면 서로 다른 큐가 같은 프로젝트를 병행 실행할 수 있다.

## P1 — 하위 종료 관측과 불변 QA 반환 기록의 작성 경계가 열려 있음

제안의 QA 반환 키는 QaToolReturnedCode와 OuterExitCode를 분리하지만, 미결정 2번에서 작성자와 작성 시점을 아직 정하지 않았다. Astra가 정한 경계는 runner가 단일 partial QA 관측 JSON을 stdout으로 내보내고, queue가 그 runner 프로세스의 실제 종료 코드를 관측한 뒤 최종 QaReturn JSON을 CreateNew으로 한 번만 기록한 다음 verifier를 호출하는 순서다. 제안서에는 이 partial 레코드의 정확 스키마/필수 바인딩 및 단일 stdout 계약이 없고, 최종 QaReturn의 CreateNew 단일 작성 순서도 규범화되어 있지 않다. runner가 자신이 아직 종료하지 않은 OuterExitCode를 예상해 쓰거나, 기존 결과를 덮어쓰지 않아야 한다. 이 선택을 정확 계약으로 반영하고 runner stdout 부분 관측·실제 종료 관측·최종 기록·검증 호출의 순서를 고정해야 한다.

## 판정과 잔여

P0=0, P1=2. 선택 파서의 unsupported 선언 사전 실패, 정확 NUnit fullname/name 집합 대조, 고정 입력 및 before/after 비교, native/QA/outer 종료값 독립 증거, 부모 NUnit 결과와 내부 행 연결은 적절한 설계 방향이다. 다만 위 두 P1은 이미 선택된 실행 계약을 이 문서에 구체화하지 않은 부분이므로, 구현 가능한 규범으로 옮기기 전 닫아야 한다. 제안서 자체도 ContractDraft이며 구현·Unity 실행·수용 권한이 없다. 원본 QA 도구, source, selection, 증거를 변경하거나 실행하지 않았다.
