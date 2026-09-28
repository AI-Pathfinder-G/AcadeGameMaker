# VD-09 M5D7Q C3L 시험 파일 실행 전 독립 검토

- 검수자: Luna
- 일자: 2026-09-28
- 대상: `Assets/AcadeGameMaker/Tests/EditMode/InputUnity/ProfileNewGameObservationLeaseV1Tests.cs`
- 범위: 읽기 전용 fixture·컴파일 위험·AC 누락 검토

## 판정

현재 시험 파일은 실제 lifecycle fixture와 기본 contention/root-file/lock-directory/foreign receipt·actions/reflection 결과 행을 포함한다. 다만 실행 전 필수 행이 빠져 있어 C3L 수용용 시험으로는 불충분하다. P0=0, **P1=2**이다.

## P1 및 보강 단위

1. **AC-M5D7QC3L-002/003 — provenance 변조 행 부재:** 현재 `AC002_ActualHeldLockTimesOutOnlyThroughLowerBusy`는 실제 held lock과 5초 elapsed 및 Busy만 확인한다. timeout wrapper 직접 생성/반사 생성, CWT 미등록 timeout, 원인 HRESULT `0x80070020→0x80070021` 변조, wrapper HRESULT 변조를 verifier에 각각 넣어 모두 Unreadable이 되는 단일 행 묶음이 없다. early release 행도 실제 lease 획득을 확인하지만, release 전후 timeout witness가 Busy로 오인되지 않는지 확인하지 않는다.
2. **AC-M5D7QC3L-003/004 — barrier·unsafe path·3-leaf agreement 행 부재:** root-as-file와 lock-directory만 있고 `profile.reset.json` present, barrier directory, filesystem-root/relative/reparse unsafe root 행이 없다. 성공 capture 후 identity leaf, role/name/presence/classification/revision/hash 또는 decoded projection을 반사 변조하여 `capture.Validate()`와 lower result 검증이 모두 닫히는 행도 없다. 현재 reflection 시험은 결과 classification 한 필드만 바꾼다.

## 현재 fixture 확인

- `Fixture.Create`는 실제 `GameObject`·`InputRouter`·`DesktopProfileLaunchAdapterV1`를 만들고 `ConfigureForTests`, 수동 `Awake` 보정, `Start`, 실제 receipt 존재 확인을 수행한다. adapter의 `ConfigureForTests`, router의 `PreparedLaunchAwakeEntered`, 현재 private field 이름은 런타임 소스와 일치하여 이 범위의 컴파일 위험은 발견하지 않았다.
- held-lock 행은 실제 `FileStream(FileShare.None)`을 별도 스레드에서 보유하고, early-release 행은 실제 해제 후 동일 호출이 deadline 전에 Captured인지 확인한다.
- foreign receipt 행은 adapter의 두 receipt witness를 foreign receipt로 바꾸고, foreign actions 행은 router `_actions`를 foreign actions로 바꾼다. 두 행 모두 원래 root 반환 거부를 확인한다.
- predecessor reconstruction 행은 `artifacts/c2-r36-source-before.json`에서 두 SHA 문자열의 존재만 확인한다. 이는 소스 재구성 일치나 현재 legacy 본문 보존의 실행 증거가 아니다.

## 미검증 범위

실제 Unity/EditMode 실행, XML 결과, fixture 보정 및 AC-M5D7QC3L-005는 미검증이다. 위 P1 두 묶음이 보강되고 실행 결과가 확인되기 전에는 C3L 또는 C3-010 통과를 주장할 수 없다.
