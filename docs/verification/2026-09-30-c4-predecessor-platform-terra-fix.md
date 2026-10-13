# C4 선행 고정 플랫폼·지도 자체 보정

- 구현자: 테라 역할, 실제 `gpt-6-sol`. 독립 수용이 아니다.
- 추적: `REQ-M5D7QC4-007`, `AC-M5D7QC4-010`, `AC-M5D7QC3-007/008`.
- 근거: Approved 선행 선택 형식 개정 SHA `26C180D05DEAA4DA59515BD891FF94638A5548E259F4BE18EC218EF90087A789`.
- 변경: `artifacts/c4-verify-required-rows.ps1`의 정확 여섯 원본 분기에서 고정 플랫폼과 지도 소유 ID를 함께 강제한다. Matrix91/Remaining149의 고정 Part도 강제한다. 새 focused schema 및 선행 파일은 변경하지 않았다.
- 검사: 원본 선택은 실제 파일로 읽었고 지도만 메모리 객체로 제공했다. runner·queue·Unity·최종 원장 생성은 수행하지 않았다. 아래는 실제 자체 검사 원시 출력이다.

```json
{
  "Positive": ["Edit562", "Worker51", "Play610"],
  "WrongPlatformRejected": ["Edit562", "Worker51", "Play610"],
  "WrongMapRejected": ["Edit562", "Worker51", "Play610"],
  "OriginalHashesBefore": [
    "FBD1B5A6FE6AD6E96CA5CDAD4103C9DFA129B3F5F622859E11D0B43FD4D649F8",
    "B2CF18E155D98BA7ECAC1E8CABE5EAF90CF6C48CC0419D1ECC800758220120E3",
    "FFFF443666F58B8518A6E89C206AF8BF23A0EF559888AAC8D528E3111591FB9E"
  ],
  "OriginalHashesAfter": [
    "FBD1B5A6FE6AD6E96CA5CDAD4103C9DFA129B3F5F622859E11D0B43FD4D649F8",
    "B2CF18E155D98BA7ECAC1E8CABE5EAF90CF6C48CC0419D1ECC800758220120E3",
    "FFFF443666F58B8518A6E89C206AF8BF23A0EF559888AAC8D528E3111591FB9E"
  ],
  "OriginalBytesUnchanged": true,
  "SyntaxErrors": 0,
  "ToolSha256": "A022A2AB896910050570DF0A54266497446738D126C22379835F788E7C55E0CC",
  "UnityExecuted": false,
  "IndependentAcceptance": false
}
```

루나의 차이 재검수와 아스트라 통합 판단은 미완료다.
