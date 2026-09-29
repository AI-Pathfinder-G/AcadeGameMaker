# C3 R9 중립 프레임 소비 설계 독립 검토

검토 대상 제안 SHA-256 `847ADCAC9CA526A06DD6FE2A5065602879AB446F7163BCD17D35BA0E6FC283FC`. 기준 구현 지문은 Play 시험 `6A3FB8BE37AE8644E446AA04FAE63D4BF75469C4DA704D253D146E715D086FEF`, Presenter `393B325BA5B8F6D79CA983F1A91098215E7A6993DE37E9F5D98BA775760E4900`, Q-B `F47C61A043DAF4695CC53E41C6BE9BB67CA147C722F7B830459AA6304B45B3B5`, cursor `CCD7763A1103BA096DD64E02C5B91A99A3ADA21D49506CC81E5F0601E06D316F`다.

**P0 0, P1 0.** 제안은 R9에서 새로 발행한 중립 프레임을 소비하지 않고 Down 프레임까지 진행한 순서 결함 후보를 정확히 겨냥한다. `InputSystem.Update` 이후 `Publish()`가 Router frame을 Step으로 발행하고, `Presenter.FixedUpdate`가 현재 cursor의 `TryAdvance`를 한 번 수행한다. cursor는 baseline 대비 tick과 frame ordinal이 모두 정확히 +1인 다음 UI receipt를 요구한다. 중립 frame을 소비한 뒤 Down frame을 소비하는 순서는 연속성 계약과 맞으며 기존 실제 입력 경로의 authority/frame 제조를 추가하지 않는다. 기존 `PresenterFixed()` 시험 wrapper도 정확히 선언형의 현재 Presenter FixedUpdate를 호출하는 기존 사용 경로다.

후보 순서는 기존 `SelectInitialNewGame`의 맨 앞에 `Publish(); PresenterFixed();` 한 쌍만 둔다. 이후 비회복 Down→현재 frame의 NavigateChanged/음수 Y 정상 assertion→PresenterFixed→release→PresenterFixed→Enter→실제 Submit assertion→PresenterFixed/Late→RequestReady 및 exact NewGame 항목 검사를 그대로 유지한다. 회복 경로도 최초 중립 frame을 같은 방식으로 소비하고 기존 notice-dismiss·release·NewGame Enter 순서를 따른다. 새 assertion/read helper, 출력, catch, 상태 대입, cursor reset은 필요하지 않다. Cancel/Rearm 이후 AC006의 첫 successor 실제 Submit은 기존처럼 추가 빈 발행 없이 유지된다.

실제 request item assertion은 직전 구현의 exact nullable/boxed struct 선언 검증을 유지해야 한다. 다만 이번 최소 제안은 그 계측/검증 자체를 변경하지 않으므로 별도 구현 변경으로 확대하지 않는다. 기존 R9 XML은 `NavigateChanged`, 음수 Y, 실제 Submit=true 뒤 RequestReady 대신 Closed였으며 item/take에는 미도달이다. cursor 간격 불일치가 Closed를 설명할 수 있는 정적 call path 중 하나지만 실행에서 baseline/ordinal 또는 caught exception을 관측하지 않았으므로 원인 확정은 아니다. 제안 후에도 실제 probes 두 건과 helper 공유 13개 remaining Play 시험을 실행해야 한다.

제안에 적힌 R9 범위(현재 동결 소스/원장·240 Edit/15 Play·377행/91 case·180초 유지)와 successor 첫 Submit 경계는 적절하다. 이 검토는 설계 정적 판단이며 구현 승인·실제 해결·시험 통과·C3/C4 수용을 뜻하지 않는다. 코드 변경이나 Unity/컴파일 실행은 없었다.
