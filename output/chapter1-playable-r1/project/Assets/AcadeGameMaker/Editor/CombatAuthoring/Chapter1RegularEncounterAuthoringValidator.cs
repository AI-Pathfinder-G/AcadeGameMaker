using System;
using AcadeGameMaker.Camera.Unity;
using AcadeGameMaker.Input.Unity;
using AcadeGameMaker.Combat.Unity;
using UnityEngine;
using UnityEditor;
namespace AcadeGameMaker.Combat.Authoring
{
    // REQ-C1B-ENTRY-001: validate additions and then the complete original producer graph.
    internal static class Chapter1RegularEncounterAuthoringValidator
    {
        internal static void ValidateHost(GameObject host)
        {
            if(host==null || !host.activeSelf || host.name!="Chapter1RegularEncounterHost" || host.transform.childCount!=0 ||
                host.GetComponents<Component>().Length!=2 || host.GetComponentsInChildren<Chapter1EncounterLifetimeOwner>(true).Length!=1)
                throw new InvalidOperationException("Authored host requires exactly one external owner and no embedded graph.");
            var owner=host.GetComponent<Chapter1EncounterLifetimeOwner>();
            if(owner==null || !owner.enabled || owner.InitialPrefabForAuthoring==null ||
                AssetDatabase.GetAssetPath(owner.InitialPrefabForAuthoring)!=Chapter1RegularEncounterAuthoringBuilder.PrefabPath)
                throw new InvalidOperationException("Authored host initial graph reference is not the exact saved regular prefab.");
            Validate(owner.InitialPrefabForAuthoring);
        }
        internal static void Validate(GameObject graph)
        {
            if(graph==null || graph.activeSelf)throw new InvalidOperationException("Seed entry requires an inactive graph.");
            var persistent=EditorUtility.IsPersistent(graph);
            if(persistent ? AssetDatabase.GetAssetPath(graph)!=Chapter1RegularEncounterAuthoringBuilder.PrefabPath || graph.name!="Chapter1RegularEncounter"
                : !graph.scene.IsValid() || graph.name!="CombatEncounterGraph")
                throw new InvalidOperationException("Saved graph asset and scene instance identities must be exact.");
            var routers=graph.GetComponentsInChildren<InputRouter>(true);var cameras=graph.GetComponentsInChildren<SimulationCameraDriver>(true);
            var combat=graph.GetComponentsInChildren<CombatSimulationDriver>(true);
            if(routers.Length!=1 || cameras.Length!=1 || combat.Length!=1 || !routers[0].IsChapter1Regular ||
                routers[0].BoundChapter1Combat!=combat[0] || routers[0].BoundPlayerForAuthoring!=combat[0].BoundPlayer ||
                routers[0].BoundTransferForAuthoring!=combat[0].BoundTransferDriver ||
                graph.GetComponentsInChildren<Chapter1EncounterLifetimeOwner>(true).Length!=0 ||
                graph.GetComponentsInChildren<OrdanBossTerminalTransitionRequester>(true).Length!=0)
                throw new InvalidOperationException("Regular sink/producer/external owner boundaries are invalid.");
            var copy=UnityEngine.Object.Instantiate(graph);copy.name="CombatEncounterGraph";
            try
            {
                UnityEngine.Object.DestroyImmediate(copy.GetComponentInChildren<InputRouter>(true));
                var camera=copy.transform.Find("Camera");if(camera==null)throw new InvalidOperationException("Authored camera is missing.");
                UnityEngine.Object.DestroyImmediate(camera.gameObject);copy.SetActive(true);
                var stage=copy.GetComponentsInChildren<RegularEnemyEncounterHandoffAdapter>(true).Length==1?RegularEnemyEncounterAuthoringStage.B1:RegularEnemyEncounterAuthoringStage.B0;
                RegularEnemyEncounterAuthoringValidator.Validate(copy,stage,false);
            }
            finally{UnityEngine.Object.DestroyImmediate(copy);}
        }
    }
}