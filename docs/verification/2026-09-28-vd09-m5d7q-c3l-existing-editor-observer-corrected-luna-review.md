# VD-09 M5D7Q C3L 기존 Editor 재관찰기 보정 검토

- 검수자: Luna
- 대상 helper: `artifacts/c3l-observe-existing-editor.ps1`
- helper SHA-256: `D347C5508272D3C60D7149CA939D79449B9B480AA9EE480FA7859B3D3E457616`
- 승인 QA 도구 SHA-256: `A4DD378FF02105345B63B40C6F446B304B04F9E8B894CCD2218806F42DD7DA85`

## 판정

보정은 정적 P0=0/P1=0이다. 최초 watcher의 `NoOwnedActualEditor`는 시험 실패가 아니라 WMI 반환 중첩 배열을 후보 배열로 평탄화하지 못한 관찰기 결함이었다. 현재 helper는 `Get-UnityProcessSnapshots` 결과를 두 단계 `ForEach-Object`로 평탄화하여 실제 snapshot 전체를 후보 판정에 전달한다.

기존 검증은 그대로 유지된다. 후보는 완전한 process inventory, exact Unity executable, command line의 단일 `-runTests`, 정확한 project/results/log 경로, worker 0을 통과해야 하며, `ExpectedPid=35344`를 다시 대조한다. attach 시 실제 process handle을 열고 creation time을 `Process.StartTime`과 비교한다. launch·test invocation·kill·source mutation은 없다.

현재 watcher의 실제 종료 결과는 아직 대기 중이다. 따라서 XML과 actual exit 0을 아직 주장하지 않는다. 종료 뒤 watcher `Completed` 결과와 별도 XML qualified-name exact 비교, before/after 874개 경로·SHA 비교를 함께 확인해야 R4 회귀 증거가 완성된다. helper 보정은 관찰기 범위이며 production source와 frozen 874 입력에는 영향을 주지 않는다.
