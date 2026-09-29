# C3 R11 Play 집중 실행 독립 결과 검토

실제 실행 XML SHA-256 `6E8C427027138E1034F820EEE30DBEA9901B5469245579B34AFBDB574D6D18C7`, 비교 검증 JSON `85C038941F678EC0E01000A2CA05321974E7BC58238EBDFA7AE3F1CC1DE4A246`, QA 반환 JSON `ECA75CEA9106C7A67CF68589D095E2F679C9E5044C78C99E0C7E8B0A5372A3DE`를 확인했다. 실제 Unity 결과는 15/15 통과, 실패·건너뜀·판정 불가 0, runner/QA 종료 코드 모두 0이다. 예상 선택과 XML 완전 수식 이름이 15개 모두 일치하고 이름 차이와 중복은 0이다. 입력 파일은 전후 각 884개이며 지문 비교 차이는 0이다.

R11 동결 manifest `artifacts/c3-upper-r11-frozen-source-manifest.json`의 14개 파일을 재해시했고 불일치는 0이다. Play 시험 지문은 `2FA406515CEB7C885A5F41EC6438ACC85123714AB00302B520A58B36066C9E23`, Edit 시험 지문은 `A0A1928CE83A22D324F785476B38DCDE595B71E53122139F10535C5B3AF6F9D6`다. 두 `AC005_SameActualCancelPreservesCompleteObservedFixtureSnapshot(False/True)` 사례가 실제 선택돼 통과했다. 각각의 XML 출력은 183개 관찰 필드, `currentMemoryAbsent=true`, `wholeGameplaySessionClaimed=false`를 기록한다. 따라서 이는 해당 시험 fixture의 실제 memory cell 부재 상태 스냅샷 결과이며, 전체 gameplay session 부재를 주장하지 않는다.

이 제한된 15개 Play 회귀는 초기 실제 입력/요청, 취소 snapshot 비교, 정상 disable 후 늦은 intake 거절, successor 첫 Submit 폐기 후 다음 입력 처리 등 선택된 경로의 실행 근거다. 이전 R6/R7/R8/R9/R10 실패 기록은 보존되며 R11 통과가 과거 실행을 소급 변경하지 않는다.

**제한 Play 실행 수용 근거로는 적합하다. 전체 C3 또는 C4 수용은 아니다.** Edit 240개, 예상 내부 행 377개/91개 matrix case, 필수 562/51/610 회귀는 이 실행에서 검증하지 않았다. 본 독립 검토는 기존 실행 자료만 대조했으며 소스 변경이나 재실행은 하지 않았다.
