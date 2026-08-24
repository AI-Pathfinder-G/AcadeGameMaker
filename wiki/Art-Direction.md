# Art Direction

고해상도 픽셀 아트에 URP 2D 조명, 파티클, 다층 패럴랙스를 결합한다. 황동 계량기, 배관, 거대한 추, 붉은 체납 도장, 청회색 콘크리트가 관료주의 디젤펑크 산업도시의 반복 시각 언어다.

환경 공통 격자는 18 PPU이며 18×18px 타일을 1×1 world unit으로 사용한다. gameplay 내부 pixel canvas는 640×360이고 최종 16:9 2560×1440 출력에서 nearest-neighbor 4배로 확대한다. 비16:9 화면에서도 같은 world framing을 유지하고 중앙 frame 밖은 letterbox/pillarbox로 처리한다. 최소 창은 640×360이다.

일반 카메라는 고정 배율·dead-zone 방식으로 이동 방향과 상승·낙하를 예측해 따라간다. mouse pan과 gameplay dynamic zoom은 사용하지 않고 room bounds를 지키며 보스·선택·컷신에서만 authored anchor를 사용한다.

dead zone은 6×4u, 수평 선행은 최대 3u, 상승은 +1.5u, 빠른 낙하는 -3u이며 offset은 12 tick에 전환된다. 일반 camera follow는 틱당 각 축 최대 0.5u이고 room 진입·respawn·teleport·anchor 경계에서만 즉시 snap한다.

UI는 640×360 logical safe frame을 사용한다. pixel frame·icon은 정수 point scale, text는 final-output SDF이며 logical font size는 최소 12px·본문 14px·제목 18px, hit area 24×24px, edge margin 12px 이상이다.

팔레트는 world 21색과 semantic accent 11색으로 나눈다. 냉청회색·먹색이 주조이고 황동·녹·콘크리트가 보조다. 청록=전이 가능, 주황=쿨다운, 적색=차단이며 수탈 2색·연대 2색·히로인 정체 3색까지 독립 slot으로 분리한다. 연대의 상아색과 히로인의 따뜻한 상아색도 공유하지 않으며, 히로인 색은 보상·상호작용 표시에 재사용하지 않는다.

세계색 HEX는 `#090D12 #101820 #17232C #20323C #2C424B #3B555D #526B70 #6C8081 #899693 #A9B0A7 #CAC9B8 #211916 #37241E #503027 #704031 #92563A #7B6035 #A47E43 #203D39 #3B6258 #394035`다. 의미색은 전이 `#20E0D0`, 조준 포착·전이 활성 공통 백색 `#F7FFFC`, 쿨다운 `#FFAA2B`, 차단 `#FF3B45`, 수탈 `#C92350 #632E83`, 연대 `#91F3E3 #E7E7C5`, 히로인 `#F0D7B2 #C9858D #D7AE68`로 고정한다.

인물과 핵심 충돌 실루엣은 1px 먹색, 일반 조준 포착은 1px 백색 단일선, 활성 전이는 청록 안쪽선+백색 바깥선의 지속 2px 이중선을 사용한다. 우선순위는 활성 전이, 조준 포착, 정상 순이며 점멸·타일 내부 이음매·비충돌 장식 전체선·벽 너머 투시를 금지한다.

무료 에셋은 수량보다 라이선스 추적과 통합 가능성을 먼저 본다. 출처, 제작자, 라이선스, 표기 의무, 수정 가능 여부, 검증일이 없는 후보는 프로젝트에 도입하지 않는다.

## Acquisition status

2026-08-24 기준 첫 CC0 원본 7종을 Git 제외 격리 영역에 확보했다. 산업 타일, 픽셀 UI, 입력 프롬프트, 파티클·연기, 충돌음·인터페이스음이며 각 ZIP의 SHA-256과 포함 라이선스를 [에셋 등록부](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/assets/asset-register.md)에 기록했다. VD-08 Approved 및 OD-ART-001 해결 전에는 Unity 임포트와 파생 작업을 하지 않는다.

**권위 문서:** [미술 캐논](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/canon/art-direction.md), [미술·에셋 스펙](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/specs/vertical-demo/08-art-and-asset-integration.md), [에셋 등록부](https://github.com/AI-Pathfinder-G/AcadeGameMaker/blob/main/docs/assets/asset-register.md)
