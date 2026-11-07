# R01 영상 바이트 스트림 완료와 실제 폐쇄의 한정 호환

- 상태: Draft — 솔 `gpt-6.1-sol`의 후속 설계. 구현·빌드·새 영상 실행 승인 없음.
- 사용자 산출물: 실제 프레임 제출 뒤 영상 파일 마무리·쓰기 버퍼 반영·하위 스트림 폐쇄와 원래 프로세스 종료를 각각 입증하는 준비 경로.
- 담당: 아스트라 계약·정확 실행 승인·통합, 솔 승인 뒤 한정 구현·국소 검사, 루나 독립 차이·실패 경로 검수, Root 실제 입력·원시·원래 Job/handle·실행 종료, 사용자 게임 입력.
- 기준: 기존 10월16일 R01 계약, 10월18일 재개·시간/정리 진단 및 합성기 시간 호환 계약. 명시 차이 외 원래 REQ/AC·역할·단회·소스590+추가1·시간/영상 ABI·원시 소유·종료/회수는 유지한다.
- 이번 허용 작업: 정확 r4 영상/관리자 원시와 현재 v3 C++의 BoundedStream/정리 구간, Microsoft 공식 Flush/Close/Finalize 문서 판독1묶음·본 Draft1. 코드·빌드·새 Unity/WGC 실행·큰 원장 검색0.

## 실제 분기와 확정 원인 범위

REQ-R01STREAM-001. `evidence/r01-user-play-entry-20261018-r4/video-stdout.jsonl` 7742바이트와 같은 제어 경로의 `startup.jsonl` 7949바이트를 기준으로 한다. 최초 프레임은 receivedQPC `1785749952695`, frameTime `1785750058043`, acceptedQPC `1785750110275`이며 실제 보류 수용과 WriteSample1이 관측됐다. Finalize 단계는 성공 반환했고, writer release 뒤 Flush가 `ByteStream-Flush-state`/HRESULT `3222093445`(`0xC00D3E85`)로 실패했다. 그 뒤 Close는 성공 반환했으나 `writerClosed=false`, `byteStreamClosed=true`, 비동기 미완료false다. 파일 extent33376·실제 쓰기량33384는 재작성 포함 서로 다른 수치다. Root는 실제 native.mp4 33376바이트 존재를 보고했으며 이번 조사에서 파일 본문을 판독하지 않았다.

관리자 원시는 영상 자식 종료값1과 원래 Job 종료·stdout/stderr EOF·파이프 및 process/Job/event/Root handle 폐쇄를 기록한다. 관리자 `firstFailure=null`은 자식 영상 실패가 없었다는 뜻이 아니다. 영상의 firstFailure는 별도 원시의 Flush 상태 오류다. r4 권위·신원·시계·단회는 소진되어 재사용하지 않는다. SourceReady=false·G3/B1 FINAL·CH1 완주 미완료를 유지한다.

현재 `BoundedStream::Flush`는 `closed_`이면 `inner_->Flush()`를 호출하지 않고 상태 오류를 반환한다. 현재 `Close`는 미완료 작업을 거절한 뒤 `inner_->Close()`가 성공한 경우에만 `closed_=true`로 만든다. 이미 closed이면 실제 inner Close를 다시 호출하지 않고 S_OK다. 따라서 이번 Flush 거절 시점에 wrapper는 앞서 성공한 inner Close를 알고 있었으나, 이 원시는 그 Close가 Finalize 내부인지 writer release 내부인지 구분하지 못한다. 정확 호출자를 추정해서 확정하지 않는다.

현재 정리 순서는 Finalize→writer release→별도 Flush→Close이고, `writerClosed_`는 별도 Flush의 성공 여부와 Close 반환에 결속된다. 이미 닫힌 wrapper에 뒤늦게 Flush를 요구하는 순서는 그 분기에서 항상 상태 거절을 만든다. Close 성공과 실제 Flush 성공은 서로 대체할 수 없다. closed=true만으로 과거 Flush 성공을 알 수 없다.

AC-R01STREAM-001. 실제 프레임·파일 마무리·상태 거절·실제/멱등 Close·자식 종료/Root 회수를 구분한다. 과거 firstFailure나 writerClosed=false를 바꾸지 않고 현재 wrapper 코드가 보장하는 사실만 추론한다. 파일 존재/크기만으로 디코딩·연속성·최종 게임 수용을 선언하지 않는다.

## 공식 API 의미와 변경 근거

REQ-R01STREAM-002. Microsoft 문서에서 Flush는 내부 쓰기 버퍼를 하위 파일/장치에 반영하고, Close는 스트림과 관련 파일 handle 등의 자원을 닫으며 미완료 비동기 I/O를 취소한다. Finalize는 모든 쓰기 동작을 마무리하며, 입력 샘플을 모두 보낸 뒤 호출한다. 비동기 callback을 제공하지 않는 경로에서는 완료까지 동기 대기한다. Finalize는 지원되는 미디어 sink의 최종화 동작도 호출한다.

이 문서들만으로 특정 MP4 sink가 wrapper Close를 호출하는 정확 단계나 writer release 이후 wrapper가 계속 열려 있다는 보장을 얻지 못한다. 따라서 wrapper가 열려 있음을 전제로 writer release 뒤 별도 Flush를 강제하는 대신, **실제 inner Close가 일어나기 직전에 같은 wrapper에서 inner Flush 결과를 확보하는 순서**로 호환하는 것이 최소 수정 후보다. API가 Close에 Flush 성공을 암묵 보장한다는 주장이나 이미 닫힌 스트림의 Flush 실패를 무시하는 변경이 아니다.

AC-R01STREAM-002. Finalize 성공·하위 Flush 성공·하위 Close 성공·포인터 해제·MFShutdown을 독립 증거로 유지한다. Finalize가 언제 Close를 호출하는지 추가 진단 실행으로 먼저 알아내야 한다는 새 직렬 선행을 만들지 않는다. 현재 수명주기에서 어느 호출자가 Close를 시작해도 같은 실제 순서가 적용되도록 설계한다.

## 최소 동작 보정 후보

REQ-R01STREAM-003. 승인 뒤 `BoundedStream::Close`의 최초 실제 폐쇄 경로를 같은 기존 mutex 안에서 다음 순서로 닫는다.

1. 이미 closed이면 기존 멱등 S_OK를 유지한다. 이 반환은 실제 Flush/Close를 새로 수행한 증거로 세지 않는다.
2. 기존 미완료 작업 거절은 유지한다. 미완료false를 추정하거나 비동기 동작을 강제 취소하는 새 구현은 추가하지 않는다.
3. `inner_->Flush()`를 실제 호출하여 HRESULT·호출/성공 수를 저장한다.
4. Flush가 실패해도 `inner_->Close()`를 독립적으로 실제 시도하여 HRESULT·호출/성공 수를 저장한다. 실제 Close 성공 시에만 closed=true다.
5. 최초 Flush 실패는 즉시 기존 최초 실패 보존 경로에 기록하며, 뒤 Close 결과로 덮어쓰지 않는다. 반환값은 Flush 실패가 있으면 그 실패, 그 외 Close 실패 또는 둘 다 성공한 S_OK다. 하위 Flush 성공만으로 closed나 파일 완료를 만들지 않는다.

각 실제 폐쇄 시도의 Flush→Close 결과를 한 쌍으로 결속한다. 과거 Flush 성공 뒤 후속 쓰기가 있었을 수 있으므로 마지막 폐쇄 시도의 실제 Flush 성공을 입증해야 한다. 단순 전역 성공 횟수>0으로 성공 판정하지 않는다. 실제 하위 Close가 실패하면 closed=false를 유지하고 오류/첫 실패를 보존한다. 후속 기존 정리의 독립 Close 시도가 있어도 그것을 자동 복구 성공으로 승격하지 않는다.

일반 `IMFByteStream::Flush`의 열린 상태·미완료 작업 검사·실제 하위 Flush 실패 전파는 유지한다. closed 상태의 직접 Flush를 S_OK로 바꾸지 않는다. 기존 읽기/쓰기/Seek/범위/extent·동시성·비동기 End/소유/한도 로직과 최초 실패 동작은 그대로다.

AC-R01STREAM-003. 정적 실패 경로 판독은 최초 Close의 Flush성공/Close성공, Flush실패/Close성공, Flush성공/Close실패, 양쪽실패, 이미closed 멱등, 비동기 미완료 거절을 구분한다. 앞 실패에도 뒤 실제 Close 시도가 도달 가능하며 실패가 보존되는지 확인한다. 실제 발생하지 않은 조합은 동적 미관측으로 남기며 별도 유발 도구·실행을 만들지 않는다.

## 정리 순서·증거·성공 기준

REQ-R01STREAM-004. capture 자원 정리→Finalize 시도→writer release→wrapper 종료 확인/필요 시 독립 Close→실측 snapshot→bounded/file 포인터 해제→MFShutdown→아파트 해제의 독립 단계 순서를 유지한다. 새 wrapper Close가 실제 Flush를 포함하므로 writer release 뒤 별도 `bounded_->Flush()` 단계는 제거한다. 이 제거는 Flush 요구의 삭제가 아니라 그 실제 호출을 최초 inner Close 이전으로 옮기는 호환이다.

writer release 뒤 snapshot에서 이미closed라면 Close에서 보존한 해당 실제 inner Flush·Close 한 쌍의 결과와 비동기 미완료false를 확인한다. 아직 open이면 기존 독립 Close 단계에서 위 새 실제 Flush→Close를 수행한다. 이미closed만을 이유로 성공 처리하지 않는다. Finalize 실패가 있더라도 원래 해제·Close·회수 시도를 계속하되 전체 실패는 보존한다. 같은 객체 포인터 해제 뒤 새 객체를 열어 Flush하거나 파일을 다시 작성하는 경로는 0이다.

snapshot은 실제 하위 Flush/Close 호출 수·성공 수·마지막 폐쇄 시도의 각 HRESULT·해당 시도의 실제 QPC와 Flush→Close 완료 상태·closed/pending/extent/실제 쓰기량을 담는다. HRESULT 미호출은 null로 구분하며 0을 채워 성공처럼 보이지 않는다. 매 호출/매 프레임 무제한 로그 대신 기존 정리 snapshot 한 묶음에 이 유한 사실을 결속한다. 원래 한 줄32768바이트·stdout/stderr 각1MiB 상한과 원시 쓰기 실패 전파를 유지한다.

`writerFinalized`는 실제 Finalize 성공인 경우만 참이다. `writerClosed`의 새 의미는 **해당 최종 폐쇄 시도의 실제 inner Flush와 inner Close 성공 및 pending=false·closed=true를 모두 확인**한 경우만 참으로 명시한다. 포인터 Reset·멱등 Close 반환·과거 다른 Flush·파일 존재만으로 참이 되지 않는다. 전체 영상 성공은 여기에 기존 실제 제출>0·firstFailure 없음·필수 자원 해제/MF·아파트 반환·자식 정상 종료와 후속 파일 검증을 함께 요구한다. 실패값을 이름만 바꾸거나 상태 오류를 허용하는 변경이 아니다.

AC-R01STREAM-004. 독립 검수는 새 호출 순서와 snapshot의 미호출/성공/실패 표현·멱등 경로·최초 실패 보존·객체 수명·원래 기한을 확인한다. 실제 단회에서는 actual Finalize와 마지막 actual inner Flush/Close 한 쌍·필수 해제·자식 종료를 확인한다. 정상 영상 파일의 최종 크기/핀·내용/연속성 수용은 기존 별도 요구를 유지하며 한 프레임 진단으로 대체하지 않는다.

## 한정 파일·검사·실행 예산 제안

REQ-R01STREAM-005. 아래는 향후 승인 요청이며 현재 실행 권한이 아니다. 새 `b1-window-video-capture.v4.cpp`, `b1-window-video-capture-r7.exe`, `chapter1-r01-user-play-entry-manager.v6.ps1`만 구현 후보로 제안한다. 원래 v3/r6/manager5·M1 핀·시간 pending 정책·PTS·입력 감독·정리·소스590+1·외부7핀·Prepare20/Capture21키·신원3+표준입출력3·실행 인수5·고정 최종 입력4는 유지한다. 관리자는 새 cpp/exe 경로·실측핀·제어접두사 `r01-user-play-entry-20261018-r5`만 바꾼다. 사례 기본 경로 후보는 `b1-unity-g3-owned-20261018-r5`다.

한정 구현1·전체 차이/정적/PowerShell 구문 검사1·기존 도구체인 빌드1(컴파일1과 성공 뒤 링크1), 같은 범위 보정1·관련 재검사/재빌드 각1, 루나 독립1·보정 재판독1을 제안한다. 부수 산출물은 새 `project/Library/b1-video-cpp-build-r7` 안 obj/의존 자료/링크 응답/각 stdout·stderr/정확 명령·결과로 닫되, 승인 시 정확 이름·작성자·부재/비링크를 먼저 고정한다. 제품/장면/큰 원장 변경·전체 검색·추가 도구·설치·선행 진단 전용 실행은 0이다.

실제 새 진단은 독립 수용·실측핀·현재 호스트/소스/쓰기 종료·새 신원/입력·아스트라 정확 승인·정식 도구 심사 뒤 관리자1·Unity1·같은 영상 자식 준비/최종 소비1로 제안한다. 전체18000초·컴파일900초·사례1800초·종료5초/회수5초를 유지한다. collector/Play·게임 입력 요청0이며 실제 프레임 제출 뒤 기존 STOP과 새 완료·폐쇄 증거를 한 사례에서 같이 얻는다. 별도 Close 호출자 규명용 진단을 선행 반복하지 않는다. 첫 실패/중단이면 원시·부분 파일과 원래 Job 종료/파이프/handle 폐쇄로 끝내며 자동 재시도하지 않는다.

AC-R01STREAM-005. 종료 인계는 실제 프레임·Finalize·폐쇄 시도 한 쌍·모든 실패·부분/완성 파일의 실제 상태·원래 자식 종료/회수·실소비 예산을 한 묶음으로 제출한다. r4 소진과 파일/실패 기록은 그대로 보존한다. 진단 성공이 즉시 직접 플레이 준비·B1 FINAL·CH1 완주 수용이 되지 않는다. 승인 전 코드/빌드/새 실행0이다.

## 공식 근거

- [Microsoft: IMFByteStream Flush](https://learn.microsoft.com/en-us/windows/win32/api/mfobjects/nf-mfobjects-imfbytestream-flush)
- [Microsoft: IMFByteStream Close](https://learn.microsoft.com/en-us/windows/win32/api/mfobjects/nf-mfobjects-imfbytestream-close)
- [Microsoft: IMFSinkWriter Finalize](https://learn.microsoft.com/en-us/windows/win32/api/mfreadwrite/nf-mfreadwrite-imfsinkwriter-finalize)

## 실제 폐쇄 직전 버퍼 반영 호환·구현 승인 — Approved

2026-10-18 아스트라 `gpt-6-astra`는 위 초안 전체를 판독하고 REQ/AC-R01STREAM-001~005를 한정 구현 계약으로 승인한다. 최초 실제 inner Close 직전 같은 mutex에서 실제 inner Flush를 수행하고, 그 실패에도 inner Close를 독립 시도하며, 마지막 실제 폐쇄 쌍의 결과를 보존하는 안을 선택한다. 작성 당시 Draft/후보 표현은 역사이며 이 절이 현재 권위다. 새 실제 실행은 아직0이다.

정확 호환 범위는 기존 영상 cleanup의 writer release 뒤 별도 Flush 강제를 **실제 최초 inner Close 이전 Flush**로 이동하고, writerClosed 판정을 해당 마지막 실제 Flush/Close 쌍의 성공 및 closed=true/pending=false로 바꾸는 부분이다. REQ/AC-R01ENTRY-003/006/007과 REQ/AC-R01VIDDIAG-004/005의 정리/실제 폐쇄 의미를 이 순서로 한정 호환한다. 일반 Flush의 closed 거절·Finalize 성공 요구·첫 실패 보존·독립 해제·시간 pending·입력 감독·원래 기한은 유지한다. 이미closed의 멱등 반환을 실제 호출 성공으로 세거나 실패 HRESULT를 묵살하지 않는다.

최초 폐쇄 시도의 Flush 호출 직전/반환 직후·Close 호출 직전/반환 직후 QPC와 실제 HRESULT/호출수/성공수를 같은 시도에 결속한다. 미호출과 실패·성공을 구별한다. 하위 API가 실패하면 기존 State의 최초 실패를 보존하고 뒤 단계는 계속 시도한다. snapshot/통계 접근은 같은 기존 동기화 정책을 따르며 COM 경계 밖으로 예외를 누출하거나 기록 실패 때문에 inner Close가 생략되는 경로를 새로 만들지 않는다. inner Close가 실제 성공해 닫혔으나 Flush가 실패한 경우에도 closed=true·전체실패를 동시에 정확히 보존한다. 이후 멱등 Close로 그 실패를 성공 처리하지 않는다. 원래 성공 횟수만으로 최종 쌍의 성공을 추정하지 않는다.

솔에게 정확 새 tools의 v4.cpp/r7.exe/manager.v6.ps1 구현1묶음·정적 전체 차이/PowerShell 구문1·기존 도구체인 빌드1·같은 범위 보정1/관련 재검사·재빌드 각1, 루나 독립1·보정 재판독1을 승인한다. 기존 v3.cpp76403/`04FBC75011835360FF00D566F828599AB2B6FD9EC3E731255F6AB2226119CBF7`, r6.exe461824/`9B74652F261BF21A227546C402D26AC4CF37EBEE6161CC3FE82FE2E580706C8C`, manager5 43767/`741B497B206A4FEE17472AABF4118DD880BB706A277F933B35B9BDBD26C6E0B4`는 보존한다. 관리자 변경은 새 영상 경로/실측핀·r5 제어접두사뿐이다. 형식/인수/원래 M1핀·기한·소스590+1·제품/장면은 변경하지 않는다.

기존 빌드1의 부수 출력은 새 `project/Library/b1-video-cpp-build-r7` 안 `capture.obj`, `compile-dependencies.json`, `link-inputs.rsp`, `compile.stdout.log`, `compile.stderr.log`, `link.stdout.log`, `link.stderr.log`, `build-commands.json`, `build-result.json`으로 닫는다. 컴파일러는 obj/의존자료, 링커는 응답파일/tools의r7.exe, 솔 부모는 로그/명령/결과를 소유한다. 기존 도구체인·옵션을 유지하고 부재/비링크 확인 뒤 컴파일1·성공뒤링크1을 수행한다. 같은 허용 보정 재빌드 시 최초 로그/결과는 `.initial`을 붙인 같은 폴더 보존본으로 남기고 최종 기록을 구분한다. 추가 설치·예비 영상 실행·다른 경유0이다.

Root에게 현재590+추가1/쓰기 종료·정확핀·호스트 대상/다른 Editor0·경로 상태의 읽기 대조, 새 `evidence/r01-user-play-entry-20261018-r5` 제어폴더1과 그 아래 startup후보1 작성을 허용한다. 새 manager6 최종핀·현재 관측·새 계약핀·과거 실패/소진을 정확히 결속한다. 새 caseBase `evidence/b1-unity-g3-owned-20261018-r5`는 실제 예약 준비까지 부재로 유지한다. project/evidence 기준은 `output/chapter1-playable-r1`이다.

정확 `project/Temp/UnityLockfile`이 남아 있으면 직전 호스트 Editor0·0바이트/배타읽기폐쇄·조상비링크·보존목표 부재 확인 후 `UnityLockfile.r01-user-play-entry-20261018-r5.preserved`로 이름변경1을 허용한다. 부재이면 변경0이다. 삭제/강제잠금해제/기존보존본덮어쓰기/실패재시도0이다. 솔/루나 허용보정 때문에 미소비 startup 핀이 바뀌면 기존 정확 원문을 `startup-approval.pre-correction.json`에 신규 보존·핀확인 뒤 새 핀과 의존 명령/인수만1회 수정·flush/close·재판독할 수 있다. 실행에 소비한 원문 수정0이다. 종결오류·공유허용원시읽기·실제바이트해시·UInt64 정밀도 원칙을 유지한다.

루나 r4 실제 판독 P0/P1=0/0 및 Root 종료요약6377바이트/SHA256 `8E785B998E45E60D2535E4D55ADCE8F82B6061AED27DD21CA6E6B1C18199F863`를 이전 한정 근거로 참조한다. 첫수용/제출·Finalize 반환·후속Flush실패·부분파일33376바이트·child1·Root회수는 각각 보존한다. MP4패킷PTS/내용 직접검사는 미실시이며 이 문서로 전체 영상 수용을 발급하지 않는다.

새 실제 진단은 구현·빌드·독립 판독·최종핀·현재입력·아스트라 정확 승인·정식 도구 심사 뒤1회다. 별도 Close 호출자 규명 진단을 선행하지 않는다. 첫 실제 제출 뒤 원래 STOP하여 새 실제 Flush/Close쌍·Finalize/파일·해제/종료를 한 사례에서 확인한다. 첫 실패도 원래 정리로 끝내며 자동재시도0이다. collector/Play/게임입력0·SourceReady=false·G3/B1 FINAL/CH1완주 미완료와 사용자 직접 플레이 목표를 유지한다.
