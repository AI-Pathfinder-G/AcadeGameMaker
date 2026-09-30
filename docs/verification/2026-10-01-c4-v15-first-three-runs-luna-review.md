# C4 v15 첫 세 실행 독립 중간 검수

- 범위: plan v15의 첫 세 실행만 읽기 전용으로 대조했다. 추적 범위는 `AC-M5D7QC4-009/010`이다. 이 기록은 전체 큐나 C4 수용 판정이 아니다.
- 계획·동결 결속: 세 verification 결과 모두 plan v15 SHA `43125A3558F6B1F3FDDC595FB47F602883B4861D0EF24690F1FEE27083EC9082`, manifest v19 SHA `980443316BFB2DC008B611FF948259200C3913C1B3C7F8C5DE6CE62D1511B595`, input v19 SHA `274F49B6E0D77C51636FA866DC1EB87321DF64B5AEFB4B8189D8B2DE4DC71F1`에 결속됐다. 각 선택 파일·시험 이름 집합, XML, before/after, native 종료, QA 반환, 행 비교의 파일 SHA를 현재 결과와 대조해 불일치 0개였다.

| 실행 | XML 결과 | C4 행 결과 | XML SHA-256 | verification SHA-256 | 행 비교 SHA-256 |
|---|---:|---:|---|---|---|
| `c4-r11-focused-edit` | 139/139 통과 | 148/148 도달·통과 | `114AC193B4ECF7BFA4CB88883018587D67F6EC41CE1093137602DE9A413A86E9` | `B4F3429855B937198ACFC243C584BA2A2A0ED57B7AA620F7014C1EF6C23FBB23` | `4502D1CAD2019AB28E636755385970A35E30FEA82A0C7D625805D04C147DB2E9` |
| `c4-r5-focused-play` | 32/32 통과 | 35/35 도달·통과 | `E9427463A22E42E87FD567E0D835F4BD4A5C5BB7E2BC7804D91F4EA697F755EB` | `7EA0C5A051404D7F17E07AD6A8F34B3923DD27CDA4DAA26E92940F55C8D891E2` | `9B721A2AE90514CB25C27DEE4BE8B1FB61E7788216D4543C2F0486F4E990127B` |
| `c4-r3-focused-hub` | 5/5 통과 | 5/5 도달·통과 | `6A4B0B47D834AC39D42B7DD495109DFA3B7C9A8F71140D0C02721929CF429502` | `A00F15128E9F7C556D8DFA870EA57A61B67790C789B19B59D4193CB6A5FE001C` | `4E8F8871B0BBDCB352EFBCDCC61C1986C534E2D1FA65C15994902DB201A0EC72` |

- 세 XML 모두 선택의 정확한 이름 집합과 일치하며 누락·초과·중복·실패·건너뜀·미확정은 0이다. 각각의 native 종료 코드, QA 도구 반환 코드, 바깥 종료 코드는 모두 0이다. 검증 결과는 모두 `Verified=true`, 행 비교는 `SourceMatched=true`, `EvidenceMatched=true`이고 누락·예상 밖·무효 행은 0이다.
- 각 before/after 캡처는 1,084개 입력 경로를 포함한다. 결과 비교의 입력 차이와 freeze 차이는 세 실행 모두 0이다. 전후 캡처 파일 자체의 SHA는 캡처 기록이므로 서로 다르지만, 내부 입력 목록의 경로와 파일 SHA는 각 실행 안에서 동일하다.
- 세 실행만 부분 검수했으며 네 번째 실행 이후의 결과는 포함하지 않았다. Unity/큐를 추가 실행하거나 기존 입력·출력·소스를 변경하지 않았다.
