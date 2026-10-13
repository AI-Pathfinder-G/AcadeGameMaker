# C4 R8 집중 Edit 실행 결과 독립 검토

- 계획 v12 SHA-256: `4DAD0A629965E588FCD444EC27039B20A78A0A0D3592F1CCAB4235F97D27B940`
- XML: `artifacts/c4-r8-focused-edit.xml`, SHA-256 `B427743E5E349FFDE380C64F3BDF318A82B9C31472F2FCA867A5863D934DB9FB`
- 검증 파일: `artifacts/c4-r8-focused-edit-verification.json`, SHA-256 `1982B39DC274D5BA768EDC78896A8691699BC8B61194B48FE02406B19A6AE68B`
- 행 비교: `artifacts/c4-r8-focused-edit-required-row-comparison.json`, SHA-256 `95EAE3378BB7270CFFF42CD431A27096C3492F283099244E7B897C35353CE973`
- 네이티브 종료 관측: SHA-256 `032562432BC9AF2A86CC067C9002F7CBACB2C335425FF142801B12C9DCC42648`; QA 반환 및 개별 실행 바깥 종료 기록: SHA-256 `49FE19E715230EA5BB71F9CEA3090A10BA2AD1D62C66252F310D15936B54F34A`
- 입력 전 캡처: SHA-256 `A01484FB4554853112C1CFA60F223CA5FB6045866F399D9043E7C7F2FCAA6A7E`; 입력 후 캡처: SHA-256 `0D2F904F528ED358DADAEFEB5C70575F8C7261EC39398E5FA8E8594D60E2FB47`
- 선택 v6 SHA-256: `A4D19A936CDE9974BA85FB946F6C2B571FF316F34C4B1192F7DB223C033B6F57`; 원장 v16 SHA-256: `02218AB9466E964952D5BB4D81A630D90E12F403A260410BD65CC72B1A19973F`; 입력 v16 SHA-256: `708EFBB467698A82636A4D138C4C349175B11F2345A25301494EA84BF1A4400F`

## 판정

P0 0건, P1 0건이다. R8 Edit 선택의 139개 전체 이름이 XML과 일치하며, XML 시험 결과는 139 통과, 실패·건너뜀·미결정 0, 이름 누락·초과·중복 0이다. XML에서 이름과 결과를 직접 읽어 선택 파일과 대조했다.

요구 행 비교는 계획된 148개 모두 도달·통과했고 실패·미도달 0이다. 누락·예상 밖·무효 행은 각각 0이다. 비교기에 기록된 148개 통과 증거 레코드가 XML의 실제 출력 JSON과 식별자·상태별로 모두 일치했다. 원장 `SourceMatched=true`, `EvidenceMatched=true`다. 레코드의 승인 연결은 `AC-M5D7QC4-001..009`와 `REQ-M5D7QC4-001..007`이다. 실행 증거 파일은 `AC-M5D7QC4-010`을 추적하며 계획·선택·원장·입력·행 자료의 지문에 결속된다. 이는 이번 집중 실행의 부분 증거이며 9회 전체 큐 수용을 뜻하지 않는다.

실제 종료 관측은 네이티브 Unity 프로세스 0, QA 도구 0, 개별 실행 바깥 반환 0이다. 네이티브 관측에는 계획과 정확한 프로젝트·XML·로그 경로, 단일 시험 프로세스, 종료 전 연결 관찰 및 실제 종료값이 있다. QA 반환은 계획의 QA 도구 SHA와 일치한다. `AncillaryLauncherStatus=Unknown`은 QA 규격에서 부수 정보로 허용되며 세 필수 종료값의 관측과 혼동되지 않는다. 검증 파일의 `Verified=true`는 이 개별 실행의 증거 검증값이고 `WholeAccepted=false`는 그대로 유지된다.

전후 입력 캡처는 각각 1,052개이며 입력 v16의 경로와 SHA에 결속된다. 두 캡처의 파일 배열은 경로와 지문 모두 동일하고, 현재 입력 목록과도 1,052개 전부 일치한다. 검증 파일이 가리키는 선택·원장·입력·XML·전후 캡처·네이티브 관측·QA 반환·행 비교 파일의 지문을 현재 파일과 대조해 불일치가 없었다. 원시 로그 SHA-256은 `9A34320AD642FBBF71EAC309985C2ED8BCA996939F3DAF7BEE983F072D3D21A9`다.

## 범위와 한계

관찰 시점에 큐는 다음 Play 실행을 진행 중이었고, 최종 큐 결과 v10은 아직 생성되지 않았다. 따라서 전체 큐 종료·실패 중단 여부, Play 결과, 9회 전체 비교는 판정하지 않는다. 이번 검토는 첫 Edit 실행의 결과만 다루며 Unity나 큐를 추가 실행하지 않았다.
