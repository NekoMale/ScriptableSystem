using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NamelessGames.ScriptableSystem.ScriptableSystemEditor
{
    public static class ReferralEntryEditorExtension
    {

        public static void Ping(this ReferralEntry entry)
        {
            if (entry.Kind == ReferralKind.Asset)
            {
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(entry.ContainerGUID));
                EditorGUIUtility.PingObject(asset);
                return;
            }

            string containerPath = AssetDatabase.GUIDToAssetPath(entry.ContainerGUID);
            Object container = AssetDatabase.LoadAssetAtPath<Object>(containerPath);

            Scene scene = EditorSceneManager.GetSceneByPath(containerPath);
            if ((scene.IsValid() && scene.isLoaded) || PrefabStageUtility.GetCurrentPrefabStage()?.assetPath == containerPath)
            {
                GlobalObjectId.TryParse($"GlobalObjectId_V1-2-{entry.ContainerGUID}-{entry.GameObjectFileID}-0", out GlobalObjectId id);
                container = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id);
            }

            EditorGUIUtility.PingObject(container);
        }
    }
}
