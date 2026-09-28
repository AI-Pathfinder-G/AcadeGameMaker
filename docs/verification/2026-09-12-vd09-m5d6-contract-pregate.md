# VD-09 M5D6 계약 독립 사전 게이트

- Date: 2026-09-12
- Reviewer: Luna
- Contract: [M5D6 프로필 입력 블록·호환성 코어](../specs/work-contracts/2026-09-12-vd09-m5d6-profile-input-compatibility-core.md)
- Result: **PASS — P0=0, P1=0, P2=2**

## 판정

- 부분 복구 대상은 input `bindingSchemaVersion`이며 outer `schemaVersion != 1`은 전체 unsupported다.
- binding schema `0`, `2`, `int.MaxValue`의 구조 보존과 mismatch 분류는 VD-09와 일치한다.
- asset ID는 non-empty/surrogate-safe/NFC만 요구하며 current GUID 상수는 실제 M5B3 asset meta와 일치한다.
- nested M5D5 value를 모든 public consumption 전에 검증하고 metadata flags와 actual override apply를 분리할 수 있다.
- 새 runtime/test 파일만으로 구현 가능하며 no-engine/authority 경계가 닫혀 있다.

## P2 구현 지시

focused tests는 outer schema와 binding schema 비혼동을 정적 확인하고, exact meta GUID, ordinal/culture-independent comparison, schema 0/2/int.MaxValue, nested default/malformed override 및 모든 public getter revalidation을 포함해야 한다.

## 결론

Astra 승인 뒤 구현 가능하다. 이 PASS는 actual binding apply나 outer profile recovery를 검증하지 않는다.
