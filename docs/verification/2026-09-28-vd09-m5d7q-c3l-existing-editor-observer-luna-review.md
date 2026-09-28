# VD-09 M5D7Q C3L 기존 Editor 재관찰기 독립 검토

- 검수자: Luna
- 대상: `artifacts/c3l-observe-existing-editor.ps1`, 승인된 `qa/tools/Invoke-UnityQa.ps1` 상태 기계
- 범위: 읽기 전용 정적 검토. 새 Editor 실행·중단 없음.

## 판정

재관찰기는 기존 소유 PID만 attach하는 범위에서 적합하다. `ExpectedPid`를 후보 snapshot의 PID와 다시 대조하고, 후보 선택은 Unity executable 경로, command line의 정확한 project/results/log 경로, 단일 `-runTests`, `AssetWorkerCount=0`, 완전한 process inventory를 검사한다. `Attach-OwnedEditor`는 실제 `Process` handle을 열고 `Process.StartTime`과 snapshot creation time을 마이크로초 단위로 비교한다. 따라서 PID만 믿는 우회가 아니다.

관찰 중에는 launch port, test invocation, kill, source mutation이 없다. 기존 Editor가 살아 있으면 관찰 timeout으로 분류하고 죽이지 않는다. 종료 후에는 실제 `ExitCode`, XML 존재와 XML counters를 읽어 `actual exit=0`, total=passed, failed/skipped/inconclusive=0일 때만 Completed로 닫는다.

단, watcher 자체는 qualified name 집합의 exact equality나 before/after source manifest를 검증하지 않는다. 따라서 최종 증거는 watcher의 Completed/actual exit 0에 더해 별도 XML expected-name 비교와 source manifest 경로·SHA 비교를 함께 기록해야 한다. 그 조합이면 장기 회귀에서 결과 XML과 실제 종료 코드를 허용할 수 있다.

`R35 613` 및 `R36 worker 51`의 과거 실행 시간은 관찰 한계 산정 자료일 뿐이며, 기존 PID 35344에 대한 현재 재관찰은 새 실행이나 과거 결과 재사용이 아니다. C3L 범위와 C3 최종 회귀 범위는 별도로 기록한다.
