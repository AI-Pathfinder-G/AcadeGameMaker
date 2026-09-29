# R7 EditMode Submit 단독 실행 결과 독립 검토

실행 증거만 읽어 대조했으며 소스·선택·환경을 수정하거나 재실행하지 않았다. 결과는 **실패**다. 네이티브 Unity 종료값은 로그에서 2, QA 도구 반환값은 별도 기록에서 5다. XML 결과는 1개 시험 중 0 통과·1 실패·건너뜀/미확정 0이고, verifier는 `Verified=false`로 기록했다.

`AC006_ImmediateReadyCursorDiscardsActualSubmitFrameWithoutActivation` 단일 이름이 선택 및 XML에서 정확히 일치하고 이름 차이·중복은 0이다. 시험은 R7에서 successor 생성 전에 추가된 neutral released `InputSystem.Update`를 거쳤지만, 실제 다음 Enter 뒤 `CurrentUiFrame.SubmitPressed`가 참일 것이라는 426행 검증에서 여전히 거짓이 관측됐다. 따라서 해당 보정은 이 실패를 해결하지 못했다. 관측은 입력 publication 부재를 보여 주며 원인을 특정하지 않는다. InputRouter 고장이나 제품 런타임 결함으로 확정할 별도 증거는 없다.

전후 입력 캡처는 각각 884개이고 경로별 차이는 0이다. R7 동결 14개 소스 manifest SHA `F72905E1CBA99D4044B0500DF9EA78632598535245C5F88373CDDC631953DB59`가 현재와 일치하며 Edit 시험 SHA도 승인 지문 `96B763452BF8A70B8C2232E9B93449D2576C871971A16A9C9D8D9A25CA1D7CE2`와 같다.

원본 증거 지문은 XML `33C3B8CB4DA6965751EBA5B3DE499041199D1BA4BAB371D89FBA47BF79644F63`, verification JSON `4F34E364923E97AAECF8AD15B6DFA75D85F5B57619690EE584A488A5FD7EAF3E`, QA 반환 JSON `E6888CF7A754C21C6E4ACEC6DFB10A3FA2A0D79115DCDD48E32C29CF7CB9F57A`, 로그 `EC06AAE2DCF3D0FFC015D68616F9A6A52FA779CAC265B1F2AB14C863FFE403B1`이다. 시험 시간은 XML 기준 10.6689864초다.

이는 기존 R6의 AC006 입력 publication P1을 R7 코드로 다시 확인한 실패이며 P1은 계속 열린다. AC001 실제 PlayMode 비활성화와 91개 matrix 사례 및 나머지 선택은 아직 실행되지 않았다. 실패한 Submit 단독 실행이나 정적 선택 수를 전체 회귀 통과로 해석할 수 없다.
