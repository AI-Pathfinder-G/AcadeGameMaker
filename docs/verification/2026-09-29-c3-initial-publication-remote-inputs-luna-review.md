# C3 최초 게시 원격 도구 입력 비교 독립 검토

2026-09-29. 실제 모델 `gpt-6-luna`. 상위가 수집한 원격 캡처와 로컬 비교 자료만 읽었다. 네트워크·Git·Unity를 실행하거나 소스·QA·기존 원장을 수정하지 않았다.

대상 비교 원장 `artifacts/c3-initial-publication-remote-input-comparison-r1.json` SHA-256 `CDAB26186A9117B87C97546A15790C0481F7072E20FF8E00621A06F479D67F31`, 구조 차이 원장 `artifacts/c3-initial-publication-remote-input-semantic-diff-r1.json` SHA-256 `2A755C270C4B0E585A15BD596B5C529EEF22467A3892BCCF3B18403421737716`이다. 원격 ref 캡처 SHA `6AE722365DEC85AB43D180A3985CBC544E6F7912666FA9984478CBB623BA02A7`, tree JSON SHA `9B4029847F504E180653EF9B7BE8F16988804EE2B61569991C51E13CA7DFCBE7`, blob 캡처 SHA는 각각 `3D7D0795AF0D2281039725CBE4E8469155A8F4CE1DCDB69CF88C7007CFD55578`, `0392AD277FCB6D1C3D016E5ED82444790B77148ADA4D82AF1DD97DAD7B0DDB05`, `A2F6FEBEEA61D0C21E808FB930E1A646BFA29E12E47576A8A07725ACB7E01C13`이다.

7개 도구 입력의 경로별 집계를 로컬 파일과 제공된 tree/blob 캡처로 다시 대조했다. tree 목록은 905개 경로이며 `truncated=false`다. 3개 입력이 원격 경로에 있고 4개는 목록에 없었다. blob의 base64 내용을 디코딩해 크기와 Git blob SHA-1을 확인했고, 로컬 SHA-256과 비교한 결과 1개는 바이트 동일, 2개는 내용 불일치, CRLF→LF만 다른 항목은 0개였다. 불일치 두 파일은 schema의 `blockedBy.items`에서 원격 `enum: [OD-SCENE-001]` 대 로컬 `pattern` 차이, catalog 첫 시나리오에서 원격 `status=blocked`·`blockedBy` 1개 대 로컬 `status=draft`·빈 `blockedBy` 차이로 구조 원장과 일치한다. 나머지 네 입력의 원격 부재도 비교 원장 경로와 일치한다.

**P1 1건 — tree 식별 지문 필드 불일치.** ref 캡처는 main commit을 `4d864484f75939dcf9c93c055d296613cf1ba1dd`로 기록한다. tree 캡처와 비교 원장의 `RemoteTreeSHA1` 필드도 같은 값을 담고 있다. 그러나 제공 tree의 열 개 최상위 항목(mode·name·하위 tree/blob SHA)을 Git tree 객체 형식으로 재구성한 tree SHA-1은 `4e58381525ef9e933a45d61d8473278b60e896ff`였다. Git commit 객체 SHA와 tree 객체 SHA가 한 필드에 같은 값으로 적혀 있으므로 원격 tree 지문 연결은 현재 원장만으로 닫히지 않는다. 비교 생성기 `c3-verify-remote-inputs-r1.py`도 tree JSON의 `sha`를 검증하지 않고 `RemoteTreeSHA1`로 복사한다. 기존 증거를 수정하지 말고, 원격 캡처의 commit→tree 연결과 실제 tree SHA를 구분해 기록한 새 보완 증거가 필요하다.

**P0 0건.** 이 메타데이터 P1은 네트워크에서 받은 ref/tree의 재조회 결과가 아니라 현재 캡처 원장의 지문 연결에 관한 결함이다. 각 blob 자체의 SHA·크기와 7개 경로 비교 결과는 독립 재계산과 일치하지만, tree 지문 연결을 수정하기 전에는 원격 ref/tree 존재 집계를 완전히 검증된 사슬로 취급하지 않는다. 기존 원장도 `PublicationApproved=false`, `ReproductionProved=false`, 누락 입력 유지로 기록되어 있다. 원격에 존재한다는 사실이나 역사적 바이트 일치는 게시 승인·도구 재현·깨끗한 첫 게시 증거가 아니다. 도구 입력 7개 게시 차단은 그대로 열린다.
