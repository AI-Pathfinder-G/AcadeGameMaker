using AcadeGameMaker.Input.Unity;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace AcadeGameMaker.Hub.Authoring.Editor
{
    // REQ-M5D7Q0-009. Deterministic builder for the one canonical graph. It
    // owns no scene/UI/presentation behavior and assigns through typed seams.
    internal static class HubRuntimeAuthoringBuilder
    {
        internal const string FolderPath = "Assets/Prefabs/Hub";
        internal const string PrefabPath = FolderPath + "/HubRuntimeRoot.prefab";

        internal static GameObject BuildCanonicalPrefab()
        {
            EnsureFolder();
            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (existing != null)
            {
                try
                {
                    HubRuntimeAuthoringValidator.ValidateCanonicalAsset(existing);
                    return existing;
                }
                catch (System.InvalidOperationException)
                {
                    // REQ-M5D7Q0-009 / AC-M5D7Q0-008. Only an observational
                    // validation failure enters the deterministic repair path.
                }
            }
            File.WriteAllText(PrefabPath, CanonicalYaml, new UTF8Encoding(false));
            AssetDatabase.ImportAsset(PrefabPath, ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            var rebuilt = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            HubRuntimeAuthoringValidator.ValidateCanonicalAsset(rebuilt);
            return rebuilt;
        }

        // REQ-M5D7Q0-009 / AC-M5D7Q0-008. Fixed local IDs and LF UTF-8 bytes
        // make repair output independent of Unity process allocation order.
        private const string CanonicalYaml = "%YAML 1.1\n%TAG !u! tag:unity3d.com,2011:\n--- !u!1 &1001\nGameObject:\n  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n  serializedVersion: 6\n  m_Component:\n  - component: {fileID: 1002}\n  - component: {fileID: 1003}\n  - component: {fileID: 1004}\n  - component: {fileID: 1005}\n  m_Layer: 0\n  m_Name: HubRuntimeRoot\n  m_TagString: Untagged\n  m_Icon: {fileID: 0}\n  m_NavMeshLayer: 0\n  m_StaticEditorFlags: 0\n  m_IsActive: 1\n--- !u!4 &1002\nTransform:\n  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n  m_GameObject: {fileID: 1001}\n  serializedVersion: 2\n  m_LocalRotation: {x: 0, y: 0, z: 0, w: 1}\n  m_LocalPosition: {x: 0, y: 0, z: 0}\n  m_LocalScale: {x: 1, y: 1, z: 1}\n  m_ConstrainProportionsScale: 0\n  m_Children: []\n  m_Father: {fileID: 0}\n  m_LocalEulerAnglesHint: {x: 0, y: 0, z: 0}\n--- !u!114 &1003\nMonoBehaviour:\n  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n  m_GameObject: {fileID: 1001}\n  m_Enabled: 1\n  m_EditorHideFlags: 0\n  m_Script: {fileID: 11500000, guid: 222f1771cd424311ac98769d2f75f8ef, type: 3}\n  m_Name: \n  m_EditorClassIdentifier: AcadeGameMaker.Input.Unity::AcadeGameMaker.Input.Unity.InputRouter\n  _player: {fileID: 0}\n  _transfer: {fileID: 0}\n  _combat: {fileID: 0}\n  _terminalTeardown: {fileID: 0}\n  _cameraProviderBehaviour: {fileID: 0}\n  _initialMode: 1\n  _authoredHubUiOnly: 1\n  _hubUiOnlyIntentProof: 1\n--- !u!114 &1004\nMonoBehaviour:\n  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n  m_GameObject: {fileID: 1001}\n  m_Enabled: 1\n  m_EditorHideFlags: 0\n  m_Script: {fileID: 11500000, guid: 4d7c8a91b2e3456fa7091c2d3e4f5a6b, type: 3}\n  m_Name: \n  m_EditorClassIdentifier: AcadeGameMaker.Input.Unity::AcadeGameMaker.Input.Unity.DesktopProfileLaunchAdapterV1\n  _router: {fileID: 1003}\n--- !u!114 &1005\nMonoBehaviour:\n  m_ObjectHideFlags: 0\n  m_CorrespondingSourceObject: {fileID: 0}\n  m_PrefabInstance: {fileID: 0}\n  m_PrefabAsset: {fileID: 0}\n  m_GameObject: {fileID: 1001}\n  m_Enabled: 1\n  m_EditorHideFlags: 0\n  m_Script: {fileID: 11500000, guid: 6f57b008a9b1460da8cfeeb6e20f61b4, type: 3}\n  m_Name: \n  m_EditorClassIdentifier: AcadeGameMaker.Input.Unity::AcadeGameMaker.Input.Unity.HubEntryHandoffLatchV1\n  _launchAdapter: {fileID: 1004}\n  _router: {fileID: 1003}\n";

        private static void EnsureFolder()
        {
            // REQ-M5D7Q0-009/AC-M5D7Q0-011. `Assets/Prefabs` is an existing
            // project boundary, never a builder-created parent/meta side effect.
            if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
                throw new System.InvalidOperationException("Canonical hub parent folder is absent.");
            if (!AssetDatabase.IsValidFolder(FolderPath)) AssetDatabase.CreateFolder("Assets/Prefabs", "Hub");
        }
    }
}
