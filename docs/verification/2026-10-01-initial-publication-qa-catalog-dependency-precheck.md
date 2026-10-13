# QA 목록 검사기의 후보 밖 직접 입력 예비 대조

- 범위: 최초 코드 게시 501후보 안의 `qa/tools/Test-QaCatalog.ps1` 현재 바이트 SHA-256 `E13E91B6967520D6ADC357E03C7A32D3B6A07F061A1FF0CA1F2A85228FF1B835`와 직접 읽기 네 파일만 조사했다. 현재 원격 기준은 [변경 요청 #6 병합 개정](https://github.com/AI-Pathfinder-G/AcadeGameMaker/commit/e95bb5169ad2ed08106bd342915d4637a1241263)이다. 이 문서는 501 전체 의존 폐쇄가 아니다.
- 검사기 246~249·252~255줄에서 네 파일을 실제로 읽는다. 네 경로 모두 v2의 501후보 **밖**에 있고 로컬에는 존재한다. [Approved QA 규격](../specs/vertical-demo/11-pre-unity-qa-artifacts.md)의 `AC-PLAT-010..012`는 현재 열린 차단 ID 원장을 입력으로 명시한다.

| 직접 입력 | 현재 로컬 SHA-256 | 원격 상태 | 게시 경계 |
| --- | --- | --- | --- |
| `qa/schema/qa-scenario-catalog.schema.json` | `64946C4DA36EE3F88432E15AB6391C06F2445ADF09A321CF3A16B75DB824729E` | 존재, Git 객체 `3e01efa05530fba97c3d29cce4875e5469530d37`; 현재는 1행 추가·1행 삭제 | 차단 ID 허용 방식을 고정 열거에서 형식 패턴으로 바꾼 실제 내용 차이다. |
| `qa/coverage/required-ac-ids.json` | `AAF5AE2ED3821C5883BE58D0D318E19CFF180C6D98A6115EF48F8D912CBFA98E` | 존재, Git 객체 `26dd77ea3b3c1bb2cc29eb536f41bfa69ba8e738`; 현재 Git 기록 바이트와 동일 | 이미 원격에 있는 직접 입력이다. |
| `qa/coverage/allowed-blocker-ids.json` | `F26C362E25C27E84A47213A3D24A233A774DC1DA034A8A07DC6D1CD55C76FF35` | 원격 `main`에 부재 | 현재 검사기가 필수로 읽으므로 도구만 게시하면 파일 부재로 실행할 수 없다. 자동 포함하지 않는다. |
| `qa/catalog/vertical-demo.json` | `657D48AD29AA2F35513F84A8CB0D31C587B470ACBAB10D3C3454DDD7111EAF6E` | 존재, Git 객체 `f32b4c960d1cf44f8b57113566d8b8f2fb59f100`; 현재는 2행 추가·2행 삭제 | 첫 시나리오의 차단 상태·차단 ID가 바뀐 실제 내용 차이다. |

현재 검사기 자체도 원격 대비 32행 추가·7행 삭제이며 새 차단 ID 원장을 필수로 읽는다. 로컬에서 일반 실행은 `PASS scenarios=13 coveredAC=68`, 자체 음성 검사는 `PASS selfTest=7 scenarios=13 coveredAC=68`로 각각 종료코드 0이었다. 이는 네 로컬 입력을 갖춘 상태의 제한 검증이다. 원격에서 입력이 빠진 상태의 실행 성공이나 네 파일의 게시 승인을 뜻하지 않는다.

최초 게시 후보를 정할 때 이 도구와 네 입력의 정확 경로·바이트·요구 근거를 함께 검토해야 한다. 501 밖 파일의 추가 여부와 원격의 두 실제 내용 차이는 별도 범위 결정·독립 검수로 남긴다. `PublicationApproved=false`, `DependencyClosure=false`, `WholeAccepted=false`.
