using System;
using AcadeGameMaker.Camera;
using AcadeGameMaker.Camera.Authoring;
using AcadeGameMaker.Camera.Unity;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Movement;
using AcadeGameMaker.Transfer.Unity;
using AcadeGameMaker.Combat.Unity;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
namespace AcadeGameMaker.Combat.Authoring
{
    // REQ-C1B-ENTRY-001/INPUT-001: preserve the existing regular encounter producers.
    internal static class Chapter1RegularEncounterAuthoringBuilder
    {
        internal const string PrefabPath="Assets/Prefabs/Chapter1/Chapter1RegularEncounter.prefab";
        internal const string HostPrefabPath="Assets/Prefabs/Chapter1/Chapter1RegularEncounterHost.prefab";
        // REQ-C1B-ENTRY-001 / contract §14: host-only recovery of the exact first partial output.
        public static void BuildHostFromPreservedGraph()
        {
            if(System.IO.File.Exists(HostPrefabPath) || System.IO.File.Exists(HostPrefabPath+".meta"))
                throw new InvalidOperationException("Host recovery requires both outputs absent; inspect any partial output.");
            ValidatePreservedGraphBytes();
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);scene.name="Chapter1RegularHostRecovery";
            var graph=AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if(graph==null)throw new InvalidOperationException("Preserved regular graph could not be loaded.");
            Chapter1RegularEncounterAuthoringValidator.Validate(graph);
            var host=new GameObject("Chapter1RegularEncounterHost");host.SetActive(false);
            try
            {
                host.AddComponent<Chapter1EncounterLifetimeOwner>().ConfigureInitialPrefabForAuthoring(graph);
                host.SetActive(true);Chapter1RegularEncounterAuthoringValidator.ValidateHost(host);
                var saved=PrefabUtility.SaveAsPrefabAsset(host,HostPrefabPath);
                if(saved==null)throw new InvalidOperationException("Host-only save failed.");
                Chapter1RegularEncounterAuthoringValidator.ValidateHost(saved);
                // SaveAsPrefabAsset saves this host directly; never flush unrelated dirty assets.
            }
            finally{UnityEngine.Object.DestroyImmediate(host);ValidatePreservedGraphBytes();}
        }
        private static void ValidatePreservedGraphBytes()
        {
            var paths=new[]{PrefabPath,PrefabPath+".meta"};var lengths=new long[]{31532,154};
            var hashes=new[]{"B16B013217C342805509DCF5C34BFD1B43A3EBA1F16DBDCE386BA465AFA9A2DC","C2F76DD8A3E5B58D599FF9B8B2C8C0A7D3C22958446B46A8A9C7A0F05E679DAF"};
            for(var index=0;index<paths.Length;index++)
            {
                if(!System.IO.File.Exists(paths[index]) || new System.IO.FileInfo(paths[index]).Length!=lengths[index])
                    throw new InvalidOperationException("Preserved graph length is not the approved partial output.");
                using(var sha=System.Security.Cryptography.SHA256.Create())
                using(var stream=System.IO.File.OpenRead(paths[index]))
                    if(BitConverter.ToString(sha.ComputeHash(stream)).Replace("-",string.Empty)!=hashes[index])
                        throw new InvalidOperationException("Preserved graph fingerprint changed; recovery is blocked.");
            }
        }
        public static void Build()
        {
            if(System.IO.File.Exists(PrefabPath) || System.IO.File.Exists(HostPrefabPath))throw new InvalidOperationException("Inspect existing authoring output before retry.");
            var source=AssetDatabase.LoadAssetAtPath<GameObject>(RegularEnemyEncounterAuthoringBuilder.GraphPrefabPath);
            if(source==null)throw new InvalidOperationException("Approved regular graph input is missing.");
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);scene.name="Chapter1RegularAuthoring";
            var graph=(GameObject)PrefabUtility.InstantiatePrefab(source,scene);
            PrefabUtility.UnpackPrefabInstance(graph,PrefabUnpackMode.OutermostRoot,InteractionMode.AutomatedAction);
            graph.name="CombatEncounterGraph";graph.SetActive(true);
            var stage=graph.GetComponentsInChildren<RegularEnemyEncounterHandoffAdapter>(true).Length==1?RegularEnemyEncounterAuthoringStage.B1:RegularEnemyEncounterAuthoringStage.B0;
            RegularEnemyEncounterAuthoringValidator.Validate(graph,stage,false);
            var player=graph.GetComponentInChildren<PlayerMovementController>(true);
            var cameraObject=new GameObject("Camera");cameraObject.transform.SetParent(graph.transform,false);cameraObject.transform.localPosition=new Vector3(0,4.5f,-10);
            var camera=cameraObject.AddComponent<UnityEngine.Camera>();var pixel=cameraObject.AddComponent<PixelPerfectCamera>();
            var cameraDriver=cameraObject.AddComponent<SimulationCameraDriver>();SimulationCameraProfileAuthoring.ApplyApprovedProfile(camera,pixel);
            cameraDriver.ConfigureForAuthoring(player,camera,pixel,0,81,new CameraCenterBounds(0,0,81,81));
            var router=graph.transform.Find("Systems").gameObject.AddComponent<InputRouter>();
            router.ConfigureChapter1GameplayForAuthoring(player,graph.GetComponentInChildren<TransferSimulationDriver>(true),graph.GetComponentInChildren<CombatSimulationDriver>(true),cameraDriver);
            graph.SetActive(false);Chapter1RegularEncounterAuthoringValidator.Validate(graph);
            if(!AssetDatabase.IsValidFolder("Assets/Prefabs/Chapter1"))AssetDatabase.CreateFolder("Assets/Prefabs","Chapter1");
            var graphPrefab=PrefabUtility.SaveAsPrefabAsset(graph,PrefabPath);
            if(graphPrefab==null)throw new InvalidOperationException("Regular prefab save failed.");
            UnityEngine.Object.DestroyImmediate(graph);
            var host=new GameObject("Chapter1RegularEncounterHost");host.SetActive(false);
            try
            {
                host.AddComponent<Chapter1EncounterLifetimeOwner>().ConfigureInitialPrefabForAuthoring(graphPrefab);
                host.SetActive(true);Chapter1RegularEncounterAuthoringValidator.ValidateHost(host);
                if(PrefabUtility.SaveAsPrefabAsset(host,HostPrefabPath)==null)throw new InvalidOperationException("Regular host prefab save failed.");
            }
            finally{UnityEngine.Object.DestroyImmediate(host);}
            AssetDatabase.SaveAssets();
        }
    }
}