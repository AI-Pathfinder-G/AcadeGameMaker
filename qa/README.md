# Pre-Unity QA Artifacts

VD-11의 machine-readable scenario catalog와 정적 validator다. Unity project·runtime test·scene은 포함하지 않는다.

```powershell
pwsh -NoProfile -File qa/tools/Test-QaCatalog.ps1
pwsh -NoProfile -File qa/tools/Test-QaCatalog.ps1 -SelfTest
```

정상 실행은 scenario 수와 68개 AC coverage를 출력하고 exit 0을 반환한다. SelfTest는 invalid JSON, duplicate ID, missing field, invalid enum, unknown/uncovered AC와 unauthorized blocker를 메모리에서 변이해 모두 거부되는지 검사한다. 파일은 수정하지 않는다.

Scenario는 기존 `REQ-*`·`AC-*`만 참조한다. 현재 `OD-SCENE-001`로 결정되지 않은 시나리오는 `blocked`이며 Pass 증적이 아니다. MiniMax M3가 schema·manifest 구현안을 냈고 validator·catalog 구현도 시도했지만 완결된 출력을 만들지 못했다. GPT가 그 결과를 VD-11 계약에 맞게 교정·완성하며, Luna만 독립 검증 판정을 남긴다.
