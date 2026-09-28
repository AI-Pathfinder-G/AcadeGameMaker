# VD-09 M5D7Q C3L focused 11 실행 전·R1 독립 검토

- 검수자: Luna
- 대상 시험 SHA-256: `75D0974F3049A64E159F57FCD8F9821E965E6BBE2A6BD3590F3BEF238044D972`
- 동결 runtime: service `23D7248B1A603A89BC8FF0FFDBB24FA04E640A66018DDC5689F97EC80701FBE5`, disk `BE0F16834F96272AD68C15F5004E9C37491EEF6F82D7CA728A5E1D1D60CB71E2`, lower `CF03C0E9FACAFD5D8967D3B2DB5A011AF29CFB4071BF47944417C5F883DE7F8F`

## 판정

**P1=1, 실행 수용 불가.** R1 실제 Unity 컴파일에서 `GameInputActions` 네임스페이스 누락으로 `CS0246`이 발생했고 XML이 생성되지 않았다. 시험 파일에는 `AcadeGameMaker.Input` using이 필요하다. 이전 정적 컴파일 가능성 판단은 실제 compiler evidence로 대체되지 않는다.

컴파일 오류와 별개로 시험 내용은 실제 held lock 5초 timeout 및 early release, timeout 원인·wrapper HRESULT/반사 위조, root-file·lock-directory, barrier file/directory, held primary unreadable, relative/filesystem-root, capture leaf/projection 변조, foreign receipt/actions, legacy reconstruction을 포함한다. 따라서 AC-M5D7QC3L-002/003/004의 주요 fixture 의도는 보강되었다.

재parse fixture는 없다. C3L AC-M5D7QC3L-003의 명시 필수 실제 행(root-as-file, directory-valued/unsafe lock, unreadable/non-contention)은 현재 행으로 덮지만, reparse 동작 자체는 실행 증거가 아니다. `CheckRoot`/`VerifyBeforeLease`의 reparse guard는 별도 정적 근거로만 기록하며, AC-M5D7QC3L-004의 unsafe-root 실행 증명으로 과장하지 않는다.

R1 입력 before/after 차이는 0으로 보고되었으나, XML 부재와 컴파일 오류 때문에 AC-M5D7QC3L-005 및 AC-M5D7QC3-010은 미검증이다. 시험 파일 수정·Unity 재실행·runtime 수용은 이 검토에서 수행하지 않았다.
