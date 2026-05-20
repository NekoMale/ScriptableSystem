using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace NamelessGames.ScriptableSystem.ScriptableSystemEditor
{
    public static class NekoScriptableObjectEditorExtensions
    {
        public static void AddOrUpdateReferral(this NGScriptableObject nso, ReferralEntry entry)
        {
            ReferralEntry existing = nso.Referrals.FirstOrDefault(r => r.ContainerGUID == entry.ContainerGUID && r.GameObjectFileID == entry.GameObjectFileID);

            if (existing == null)
            {
                entry.RebuildLabel();
                nso.Referrals.Add(entry);
                return;
            }

            bool changed = false;
            foreach (string component in entry.ComponentNames)
            {
                if (existing.ComponentNames.Contains(component)) continue;
                existing.ComponentNames.Add(component);
                changed = true;
            }

            if (changed) existing.RebuildLabel();
        }

        public static void CheckReferrals(this NGScriptableObject nso)
        {
            string nsoPath = AssetDatabase.GetAssetPath(nso);
            bool changed = false;

            nso.Referrals.RemoveAll(entry =>
            {
                string containerPath = AssetDatabase.GUIDToAssetPath(entry.ContainerGUID);

                // Contenitore eliminato
                if (string.IsNullOrEmpty(containerPath))
                {
                    changed = true;
                    return true;
                }

                // Controlla se il contenitore referenzia ancora questo evento
                string[] deps = AssetDatabase.GetDependencies(containerPath, false);
                bool stillReferenced = deps.Contains(nsoPath);

                if (!stillReferenced) changed = true;
                return !stillReferenced;
            });

            if (changed) EditorUtility.SetDirty(nso);
        }

        public static void RemoveAllReferralsFromContainer(this NGScriptableObject nso, string containerGUID)
        {
            nso.Referrals.RemoveAll(r => r.ContainerGUID == containerGUID);
        }

        public static void PingReferral(this NGScriptableObject nso, ReferralEntry entry)
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