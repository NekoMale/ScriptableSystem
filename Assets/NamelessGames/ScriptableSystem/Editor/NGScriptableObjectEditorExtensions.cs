using System.Linq;
using UnityEditor;

namespace NamelessGames.ScriptableSystem.ScriptableSystemEditor
{
    public static class NGScriptableObjectEditorExtensions
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
    }
}