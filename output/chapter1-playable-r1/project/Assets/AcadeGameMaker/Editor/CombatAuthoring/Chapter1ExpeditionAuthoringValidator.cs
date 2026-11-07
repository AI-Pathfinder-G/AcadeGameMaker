using System;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Run.Unity;
using AcadeGameMaker.Combat.Unity;
using AcadeGameMaker.Transfer.Unity;
using AcadeGameMaker.Hub.Presentation.Unity;
using AcadeGameMaker.Movement;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

namespace AcadeGameMaker.Combat.Authoring
{
    internal static class Chapter1ExpeditionAuthoringValidator
    {
        // REQ-C1B-ROOM-001: 신규 메타·실물 collider·생산 결속을 같이 검사한다.
        internal static void ValidateRoom(GameObject room,bool requireMovementAcceptance)
        {
            if(room==null || room.activeSelf || room.GetComponentsInChildren<Chapter1RoomInstance>(true).Length!=1 ||
                room.GetComponentsInChildren<InputRouter>(true).Length!=1 || room.GetComponentsInChildren<CombatSimulationDriver>(true).Length!=1 ||
                room.GetComponentsInChildren<Chapter1EncounterLifetimeOwner>(true).Length!=0 || room.GetComponentsInChildren<OrdanBossTerminalTransitionRequester>(true).Length!=0)
                throw new InvalidOperationException("방의 단일 생산 그래프·외부 수명 경계가 올바르지 않습니다.");
            var meta=room.GetComponent<Chapter1RoomInstance>();meta.ValidateStructure();
            if(requireMovementAcceptance && !meta.IsBasicMovementAccepted)throw new InvalidOperationException("실제 기본 이동 미검수 방입니다.");
            var environment=room.transform.Find("Environment");
            var expected=meta.Floors;var terrain=meta.Terrain;
            if(environment==null || environment.childCount!=expected.Length+terrain.Length+2)throw new InvalidOperationException("방 지형·경계 수가 다릅니다.");
            var index=0;foreach(var rect in expected)ValidateBox(environment.GetChild(index++),rect);
            foreach(var rect in terrain)ValidateBox(environment.GetChild(index++),rect);
            ValidateBox(environment.GetChild(index++),meta.LeftBound);ValidateBox(environment.GetChild(index),meta.RightBound);
            var router=room.GetComponentInChildren<InputRouter>(true);var combat=room.GetComponentInChildren<CombatSimulationDriver>(true);
            if(!router.IsChapter1Regular || router.BoundChapter1Combat!=combat || router.BoundPlayerForAuthoring!=combat.BoundPlayer ||
                router.BoundTransferForAuthoring!=combat.BoundTransferDriver)throw new InvalidOperationException("방의 실제 producer 관계가 다릅니다.");
            var targets=room.GetComponentsInChildren<TransferTarget>(true);
            if(targets.Length!=2 || room.GetComponentsInChildren<RegularEnemyTransferModifierSink>(true).Length!=2 ||
                room.GetComponentsInChildren<CombatTarget>(true).Length!=3)
                throw new InvalidOperationException("§15의 regular 등록2·sink2·combat3 경계가 다릅니다.");
        }
        private static void ValidateBox(Transform transform,Rect expected)
        {
            var collider=transform.GetComponent<BoxCollider2D>();
            if(transform.gameObject.layer!=8 || collider==null || collider.isTrigger || !collider.enabled || collider.offset!=Vector2.zero ||
                (Vector2)transform.localPosition!=expected.center || collider.size!=expected.size)
                throw new InvalidOperationException("실제 바닥·지형 AABB가 승인 표와 다릅니다.");
        }
        internal static void ValidateBoss(GameObject boss)
        {
            if(boss==null || boss.activeSelf || boss.GetComponentsInChildren<InputRouter>(true).Length!=1 ||
                boss.GetComponentsInChildren<OrdanBossTerminalTransitionRequester>(true).Length!=1 ||
                boss.GetComponentsInChildren<OrdanBossTerminalTeardown>(true).Length!=1 ||
                boss.GetComponentsInChildren<Chapter1EncounterLifetimeOwner>(true).Length!=0)throw new InvalidOperationException("보스 sole requester graph가 다릅니다.");
            var router=boss.GetComponentInChildren<InputRouter>(true);var player=router.BoundPlayerForAuthoring;
            if(player==null || router.IsChapter1Regular || player.transform.localPosition!=new Vector3(-7,.81f,0))
                throw new InvalidOperationException("보존 Ordan 로컬 player 입구가 다릅니다.");
        }
        internal static void ValidateHub(GameObject hub)
        {
            if(hub==null || hub.activeSelf || hub.GetComponentsInChildren<Chapter1HubLifetimeOwner>(true).Length!=1 ||
                hub.GetComponentsInChildren<InputRouter>(true).Length!=1 || hub.GetComponentsInChildren<DesktopProfileLaunchAdapterV1>(true).Length!=1 ||
                hub.GetComponentsInChildren<Chapter1EncounterLifetimeOwner>(true).Length!=0)
                throw new InvalidOperationException("거점 prefab의 준비·입력·수명 경계가 다릅니다.");
        }
        // AC-CHOICE-001/004, AC-CH1-004: 실제 정적 참조와 승인 기하를 검사한다.
        internal static void ValidateChoice(GameObject choice)
        {
            if(choice==null || choice.activeSelf || choice.GetComponentsInChildren<Chapter1ChoicePresenter>(true).Length!=1 ||
                choice.GetComponentsInChildren<InputRouter>(true).Length!=0 || choice.GetComponentsInChildren<Chapter1ChoicePhaseAdapter>(true).Length!=0 ||
                choice.GetComponentsInChildren<HubSafeFrameScalerV1>(true).Length!=1)throw new InvalidOperationException("선택 정적 표시 경계가 다릅니다.");
            var safe=choice.transform.Find("Canvas/SafeFrame") as RectTransform;
            if(safe==null || safe.sizeDelta!=new Vector2(640,360))throw new InvalidOperationException("선택 안전 영역이 다릅니다.");
            for(var i=0;i<5;i++){var button=safe.Find("Choice"+i) as RectTransform;if(button==null || button.sizeDelta.x<24 || button.sizeDelta.y<24)throw new InvalidOperationException("선택 버튼 영역이 없습니다.");}
        }
        internal static void ValidateBarrier(GameObject barrier)
        {
            if(barrier==null || barrier.activeSelf || barrier.GetComponentsInChildren<CombatSimulationDriver>(true).Length!=0 ||
                barrier.GetComponentsInChildren<BarrierCombatSimulationDriver>(true).Length!=1 || barrier.GetComponentsInChildren<Chapter1ChoiceEnvironmentOwner>(true).Length!=1 ||
                barrier.GetComponentsInChildren<InputRouter>(true).Length!=1 || barrier.GetComponentsInChildren<TransferTarget>(true).Length!=1 ||
                barrier.GetComponentsInChildren<CombatTarget>(true).Length!=3 || barrier.GetComponentsInChildren<Chapter1EncounterLifetimeOwner>(true).Length!=0)
                throw new InvalidOperationException("봉쇄선의 단일 생산 소유와 등록 수가 다릅니다.");
            var weight=barrier.transform.Find("Counterweight");var body=weight==null?null:weight.GetComponent<Rigidbody2D>();
            var target=weight==null?null:weight.GetComponent<TransferTarget>();var box=weight==null?null:weight.GetComponent<BoxCollider2D>();
            if(body==null || target==null || box==null || body.bodyType!=RigidbodyType2D.Dynamic || body.mass!=1 || body.gravityScale!=1 || body.constraints!=RigidbodyConstraints2D.FreezeRotation ||
                weight.localPosition!=new Vector3(4.5f,.5f,0) || box.size!=Vector2.one || box.offset!=Vector2.zero || box.isTrigger)throw new InvalidOperationException("상자 초기 물리 기하가 다릅니다.");
            var geometry=target.FreezeAuthoredGeometryForCrossValidation();
            if(geometry.TargetId!="BarrierCounterweight" || geometry.TargetCollider!=box || geometry.TargetPoseRoot!=weight ||
                geometry.LocalAimPointXQ1000!=0 || geometry.LocalAimPointYQ1000!=0 || geometry.LocalAimShapeCenterXQ1000!=0 || geometry.LocalAimShapeCenterYQ1000!=0 ||
                geometry.LocalAimShapeHalfExtentXQ1000!=500 || geometry.LocalAimShapeHalfExtentYQ1000!=500)throw new InvalidOperationException("상자의 실제 여섯 조준값이 다릅니다.");
            ValidateBox(barrier.transform.Find("LeftFloor"),new Rect(-3,-1,4,1));ValidateBox(barrier.transform.Find("RightFloor"),new Rect(4.5f,-1,4.5f,1));
            ValidateBox(barrier.transform.Find("CounterweightSupport"),new Rect(4,-1,1,1));
            var gate=barrier.transform.Find("Gate").GetComponent<BoxCollider2D>();
            if(gate.transform.localPosition!=new Vector3(5.1f,1.5f,0) || gate.size!=new Vector2(.2f,3) || gate.isTrigger)throw new InvalidOperationException("문 초기 기하가 다릅니다.");
            var bridge=barrier.transform.Find("Bridge").GetComponent<BoxCollider2D>();var platform=barrier.transform.Find("Platform").GetComponent<BoxCollider2D>();
            var effector=platform.GetComponent<PlatformEffector2D>();var player=barrier.GetComponentInChildren<PlayerMovementController>(true);
            if(bridge.enabled || bridge.size!=new Vector2(3.5f,.25f) || bridge.transform.localPosition!=new Vector3(2.75f,-.125f,0) ||
                platform.enabled || !platform.usedByEffector || effector==null || !effector.useOneWay || effector.colliderMask!=(1<<player.gameObject.layer))throw new InvalidOperationException("다리·발판 초기 충돌 규칙이 다릅니다.");
            var exit=barrier.transform.Find("Exit").GetComponent<BoxCollider2D>();var safety=barrier.transform.Find("Safety").GetComponent<BoxCollider2D>();
            if(!exit.isTrigger || exit.transform.localPosition!=new Vector3(7,1.5f,0) || exit.size!=new Vector2(1,3) ||
                !safety.isTrigger || safety.transform.localPosition!=new Vector3(3,-3,0) || safety.size!=new Vector2(12,2))throw new InvalidOperationException("출구·비치명 복귀 영역이 다릅니다.");
            barrier.GetComponentInChildren<CombatTargetRegistry>(true).ReadValidatedAuthoringRoster();
            if(((Vector2)(weight.position-player.transform.position)).sqrMagnitude>36f)throw new InvalidOperationException("최초 전이 범위를 벗어났습니다.");
            ValidateInitialBarrierSight(barrier,player.transform.position,weight.position);
        }
        private static void ValidateInitialBarrierSight(GameObject barrier,Vector2 origin,Vector2 aim)
        {
            // AC-AUTOCH1-005: 비활성 원본의 제한된 실제 기하만 검사하며 장면/물리 자원을 만들지 않는다.
            InitialSightIntersects(origin.x,origin.y,aim.x,aim.y,0,0,1,1,true);
            foreach(var source in barrier.GetComponentsInChildren<BoxCollider2D>(true))
            {
                if(source.gameObject.layer!=8 || !source.enabled || source.isTrigger)continue;
                var active=true;for(var t=source.transform;t!=barrier.transform;t=t.parent)
                {if(t==null)throw new InvalidOperationException("시야 장애물의 소유가 다릅니다.");if(!t.gameObject.activeSelf)active=false;}
                if(!active)continue;
                var m=source.transform.localToWorldMatrix;var p=source.transform.position;
                var supported=m.m00==1 && m.m11==1 && m.m22==1 && m.m33==1 &&
                    m.m01==0 && m.m02==0 && m.m10==0 && m.m12==0 && m.m20==0 && m.m21==0 &&
                    m.m30==0 && m.m31==0 && m.m32==0 && p.z==0 && source.edgeRadius==0;
                var blocked=InitialSightIntersects(origin.x,origin.y,aim.x,aim.y,(double)p.x+source.offset.x,(double)p.y+source.offset.y,source.size.x,source.size.y,supported);
                Debug.Log("[Chapter1StaticSight] origin="+origin.ToString("R")+" aim="+aim.ToString("R")+" obstacle="+source.name+" position="+p.ToString("R")+" size="+source.size.ToString("R")+" offset="+source.offset.ToString("R")+" matrix="+m.ToString("R")+" blocked="+blocked);
                if(blocked)
                    throw new InvalidOperationException("최초 상자 조준의 시야가 막혔습니다.");
            }
        }
        private static bool InitialSightIntersects(double ox,double oy,double ax,double ay,double cx,double cy,double width,double height,bool supported)
        {
            var values=new[]{ox,oy,ax,ay,cx,cy,width,height};
            foreach(var value in values)if(double.IsNaN(value)||double.IsInfinity(value))throw new InvalidOperationException("시야 기하가 유한하지 않습니다.");
            if(width<=0 || height<=0 || !supported)throw new InvalidOperationException("지원하지 않는 시야 기하입니다.");
            var dx=ax-ox;var dy=ay-oy;var lengthSquared=dx*dx+dy*dy;
            if(double.IsInfinity(lengthSquared)||lengthSquared<=0)throw new InvalidOperationException("시야 선분 길이가 올바르지 않습니다.");
            if(!supported)return false;
            var enter=0d;var leave=1d;
            return InitialSightSlab(ox,dx,cx-width/2,cx+width/2,ref enter,ref leave) &&
                InitialSightSlab(oy,dy,cy-height/2,cy+height/2,ref enter,ref leave);
        }
        private static bool InitialSightSlab(double origin,double direction,double min,double max,ref double enter,ref double leave)
        {
            if(double.IsInfinity(min)||double.IsInfinity(max))throw new InvalidOperationException("시야 경계가 유한하지 않습니다.");
            if(direction==0)return origin>=min && origin<=max;
            var first=(min-origin)/direction;var last=(max-origin)/direction;
            if(double.IsNaN(first)||double.IsInfinity(first)||double.IsNaN(last)||double.IsInfinity(last))throw new InvalidOperationException("시야 교차가 유한하지 않습니다.");
            if(first>last){var swap=first;first=last;last=swap;}enter=Math.Max(enter,first);leave=Math.Min(leave,last);
            return enter<=leave;
        }
        internal static void ValidateScene(UnityEngine.SceneManagement.Scene scene)
        {
            if(!scene.IsValid() || scene.path!=Chapter1ExpeditionAuthoringBuilder.ScenePath)throw new InvalidOperationException("지정 원정 scene이 아닙니다.");
            var flows=0;foreach(var root in scene.GetRootGameObjects())flows+=root.GetComponentsInChildren<Chapter1PlayableFlowOwner>(true).Length;
            if(flows!=1)throw new InvalidOperationException("원정 scene에는 flow owner가 정확 하나 있어야 합니다.");
        }
    }
}
