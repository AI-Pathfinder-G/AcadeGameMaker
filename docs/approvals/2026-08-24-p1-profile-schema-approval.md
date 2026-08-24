# P1 단일 프로필 저장 스키마 승인 기록

- Date: 2026-08-24
- Status: Approved by user
- Partial decision: `OD-PLAT-001`
- Owner: Sol

## Approved boundary

- 영구 상태는 version 1 단일 UTF-8 JSON `profile.json` snapshot에 기록
- `schemaVersion`, 증가하는 `profileRevision`, settings, input, tutorial, progression, integrity object 사용
- settings는 window mode, Q1000 master/music/sfx volume, gamepad aim X/Y inversion
- input은 stable input-actions asset ID, binding schema version, Input System override JSON string
- tutorial confirmed ID와 completed branch는 ordinal sorted unique array
- progression은 last offered seed, committed choice, consent state, granted skill, completed branch만 기록
- `integrity.payloadSha256`는 integrity object를 제외한 canonical payload byte의 lowercase SHA-256
- object property 순서 고정, UTF-8·LF 사용, wall-clock 저장 시각은 payload에서 제외
- 활성 원정·위치·속도·체력·방·적·보상·원정 자산·활성 전이와 runtime reference 저장 금지
- 알 수 없는 field를 gameplay 의미로 추측하지 않음

## Remaining boundary

profile 파일 경로, temporary·backup 파일을 포함한 원자 저장 순서와 손상·미지원 version 복구를 확정하기 전에는 `OD-PLAT-001`을 해결 처리하지 않는다.
