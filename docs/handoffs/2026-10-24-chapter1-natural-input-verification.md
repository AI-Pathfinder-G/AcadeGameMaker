# 2026-10-24 — 정상 부팅과 자연 입력 갱신의 대체 검사

## 사용자 위임

사용자 중간 승인·버튼·키 해제·경로 입력 요청 없이 개발과 검증을 수행한다. 앱 접근 시간초과만 반복 보고하고 중단하는 방식 대신 소유 프로그램 내부의 공개 Unity 입력 기능으로 빠진 검사를 보강했다. 보안·앱 권한 설정과 승인창은 조작하지 않았다.

## 실제로 새로 확인한 결과

`AC-AUTOCH1-BOOTINPUT-001`은 별도 진단 빌드에서 통과했고 Luna가 독립 검수했다. 기존 시험의 `InputTestFixture`, 추가 장면 적재, 수동 `InputSystem.Update()`를 사용하지 않았다. CH1 단독 첫 장면, `BuildOptions.None`, 자연 Dynamic 입력 갱신, 새 가상 Keyboard/Mouse의 공개 상태 큐를 사용했다. 시험 기호·시험 저장 재정의·생산 상태 주입은 없다.

- frame2: 실제 준비 완료를 관측했지만 흐름은 `PreparingHub`, 시작 개정0·실행 키 없음이었다.
- frame3: 거점 준비와 중립 영수증을 확인한 뒤 Enter를 발행했다.
- frame6: 실제 첫 원정·새 실행 키·세대1·저장 생성을 확인했다.
- tick33: 소유 D 입력의 실제 이동 값 `(1,0)`과 위치 `0→0.2915039`를 확인했다.
- tick36: 해제 후 실제 이동 값 `(0,0)`을 확인했다.
- tick41: 완료 틱3회 동안 위치가 안정된 것을 확인했다.

Editor42600과 Player15900은 각각 자연 종료0, 강제종료0이다. 관측 자식·핸들·로그 폐쇄, 복제본634 복원, 원본630 불변과 기존 Ship 지문 불변을 확인했다. 빌드205파일의 실행 후 지문 불일치는0이며 프로젝트 시험 조립체·NUnit·TestRunner는 없다. 생성된 시험 저장3파일은 진단 빌드 안에만 있다.

## 원시와 보존

기준 루트: `output/chapter1-boot-input-probe-20261024`.

- `evidence/round-3/boot-input-probe.raw.json`: 전체2047바이트, 관측13행의 문자열1484바이트, Dynamic16회. SHA256 `4C50740B8427868299F58E284CAE2558B94B85A9158AAD468264DFC28AEF1578`.
- `evidence/round-3/closure.json`: SHA256 `796E0CC499F1933FCDBE77559255372579EF3BDFC0AD32FEEE8F31D6FB0164C7`.
- `verification-summary.v1.json`: Root의 빌드205파일·저장·종료·원본 불변 대조.
- `implementation-inputs.v2.json`: 최종 진단 소스와 실행기 입력 지문.

첫 빌드는 실행기의 짧은 자식 프로세스 종료 경쟁상태 때문에 중단됐다. 원문과 미완성201파일은 `evidence/interrupted-r1`, `evidence/round-1-build-preserved`에 보존했다. 패키지가 빌드 전 생성하고 빌드 후 정리하는 정확5자료도 별도 보존했다.

두 번째 실행은 Enter 이전의 검사 상태 단언에서 실패했다. 기본 실행 순서의 관찰자가 흐름 소유자보다 먼저 실행되는 정상 준비 전환을 기다리도록 한정 보정했다. 두 번째 실행의 실제 상태값은 기록되지 않았으므로 과거 원인을 확정하지 않는다. 실패 원문 `evidence/boot-input-probe.raw.json`의 SHA256은 `2B4EE71EAC5AD8D84D73CBF329CEC68803572030A4D277B11F627C21F5D285B6`으로 보존됐다. 해당 빌드와 저장은 `build/completed-r2`에 있다.

## 사용자용 파일과 남은 경계

사용자용 실행파일은 기존 `output/chapter1-autonomous-20261021/build/windows/AcadeGameMakerChapter1.exe`, 압축은 같은 상위 폴더의 `AcadeGameMaker-CH1-Windows.zip`이다. 진단용 `AcadeGameMakerChapter1BootProbe.exe`는 검사 후 자동 종료하므로 사용자 플레이 파일로 안내하지 않는다.

기존 양분기·실패/재시도 자동 시험과 이번 정상 첫 부팅 입력 검사는 분리된 증거다. 이번 결과는 별도 진단 바이너리의 합성 입력이며 실제 배포 실행본의 운영체제 키 전달·창 초점 복귀·화면 조작·사람 완주를 입증하지 않는다. `actualWindowAcceptance=false`, `finalAcceptance=false`를 유지한다. 실행파일의 Unity 공통 실행기 지문이 같아도 전체 게임 바이너리가 같다고 해석하지 않는다.

앱 접근 승인은 여전히 확인되지 않았다. 공개 설정의 읽기 전용 조사에서 게임의 영구 허용 일치0이며 권한 변경0·보호된 창 호출 반복0이다. 사용자가 다시 승인하거나 버튼을 누르도록 요구하지 않는다. 같은 대체 검사를 무변경 반복하거나 기존 실패를 덮지 않는다.
