#if UNITY_EDITOR
using System;
using System.Collections;
using AcadeGameMaker.Input.Unity;
using UnityEngine.InputSystem;
using UnityEngine.TestTools;
using AcadeGameMaker.Run.Unity;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace AcadeGameMaker.Tests.PlayMode.RunUnity
{
    internal sealed class Chapter1RoomPlanTests : InputTestFixture
    {
        // AC-C1B-ROOM-001: 순수 구조 검사는 실제 이동 검수를 대신하지 않는다.
        [Test] public void FixedPlansAndSeedRotation()
        {
            Assert.That(Chapter1RoomCatalog.NextSeed(null),Is.EqualTo(101));
            Assert.That(Chapter1RoomCatalog.NextSeed(101),Is.EqualTo(303));
            Assert.That(Chapter1RoomCatalog.NextSeed(303),Is.EqualTo(202));
            Assert.That(Chapter1RoomCatalog.NextSeed(202),Is.EqualTo(404));
            Assert.That(Chapter1RoomCatalog.NextSeed(404),Is.EqualTo(101));
            Assert.Throws<ArgumentException>(()=>Chapter1RoomCatalog.NextSeed(1));
            var root=new GameObject("CatalogFixture");root.SetActive(false);
            try
            {
                var catalog=root.AddComponent<Chapter1RoomCatalog>();var rooms=new GameObject[6];var plans=new TextAsset[4];var seeds=new[]{101,202,303,404};
                for(var i=0;i<6;i++){rooms[i]=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Chapter1/R0"+(i+1)+".prefab");Assert.That(rooms[i],Is.Not.Null,"정확 저작 후보가 필요합니다.");}
                for(var i=0;i<4;i++){plans[i]=AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/Chapter1/RoomPlans/"+seeds[i]+".json");Assert.That(plans[i],Is.Not.Null);}
                catalog.ConfigureForAuthoring(rooms,plans);
                foreach(var seed in seeds){var plan=catalog.ReadPlan(seed);Assert.That(plan.Seed,Is.EqualTo(seed));Assert.That(plan.RoomAt(0),Is.EqualTo("R01"));Assert.That(plan.RoomAt(3),Is.EqualTo("R06"));Assert.That(plan.CanonicalSha,Has.Length.EqualTo(64));}
            }
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        [Test] public void RejectUnvalidatedRoom()
        {
            var root=new GameObject("UnvalidatedRoom");root.SetActive(false);
            try{var room=root.AddComponent<Chapter1RoomInstance>();room.ConfigureForAuthoring("R05");room.ValidateStructure();Assert.That(room.IsBasicMovementAccepted,Is.False);
                Assert.Throws<InvalidOperationException>(()=>room.AcceptBasicMovementForAuthoring(""));}
            finally{UnityEngine.Object.DestroyImmediate(root);}
        }
        private Chapter1AuthoredInputTrace _trace;
        public override void TearDown(){if(_trace==null)base.TearDown();}
        [UnityTearDown] public IEnumerator Close(){if(_trace==null)yield break;try{yield return _trace.Close();}finally{_trace=null;base.TearDown();}}
        [UnityTest] public IEnumerator RoomExitRequiresActualCompletion()
        {
            _trace=new Chapter1AuthoredInputTrace();yield return _trace.Open();yield return _trace.Start(101);
            var owner=_trace.Flow.Lifetime;Assert.That(owner.TryGetCompletedEvidence(out var entry),Is.True);
            Assert.That(entry.AliveEnemyCount,Is.EqualTo(2));Assert.That(entry.Position.x,Is.LessThan(30));
            yield return _trace.WalkToUnclearedExit();Assert.That(owner.TryGetCompletedEvidence(out var completed),Is.True);
            Assert.That(completed.Position.x,Is.GreaterThanOrEqualTo(30));Assert.That(completed.AliveEnemyCount,Is.GreaterThan(0));
            Assert.That(_trace.Flow.View.Phase,Is.EqualTo(Chapter1RunPhase.InRoom));Assert.That(owner.Generation,Is.EqualTo(1));
            // 실제 출구 영역에 도달해도 실제 생존 적이 있으면 다음 방이 게시되지 않는다.
        }
    }
}

#endif
