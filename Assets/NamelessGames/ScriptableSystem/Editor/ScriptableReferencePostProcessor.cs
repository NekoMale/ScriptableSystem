using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NamelessGames.ScriptableSystem.ScriptableSystemEditor
{
    class ScriptableReferencePostProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            Dictionary<string, NGScriptableObject> eventsByGUID = LoadAllNekoScriptableObjects();

            if (eventsByGUID.Count == 0) return;

            bool save = false;
            foreach (string assetPath in importedAssets)
                ProcessAsset(assetPath, eventsByGUID);

            foreach (string assetPath in deletedAssets)
            {
                string deletedGUID = AssetDatabase.AssetPathToGUID(assetPath);
                foreach (NGScriptableObject bEvent in eventsByGUID.Values)
                {
                    if (!bEvent.Referrals.Any(r => r.ContainerGUID == deletedGUID)) continue;
                    bEvent.RemoveAllReferralsFromContainer(deletedGUID);
                    EditorUtility.SetDirty(bEvent);
                    save = true;
                }
            }

            for (int assetIndex = 0; assetIndex < movedAssets.Length; assetIndex++)
            {
                string movedGUID = AssetDatabase.AssetPathToGUID(movedAssets[assetIndex]);
                string newName = System.IO.Path.GetFileName(movedAssets[assetIndex]);

                foreach (NGScriptableObject bEvent in eventsByGUID.Values)
                {
                    bool changed = false;
                    foreach (ReferralEntry entry in bEvent.Referrals.Where(r => r.ContainerGUID == movedGUID))
                    {
                        entry.ContainerName = newName;
                        entry.RebuildLabel();
                        changed = true;
                        save = true;
                    }
                    if (changed) EditorUtility.SetDirty(bEvent);
                }
            }

            if (!save) return;
            AssetDatabase.SaveAssets();
        }

        static void ProcessAsset(string assetPath, Dictionary<string, NGScriptableObject> eventsByGuid)
        {
            string extension = System.IO.Path.GetExtension(assetPath).ToLower();

            if (extension != ".unity" && extension != ".prefab" && extension != ".asset") return;

            string[] dependencies = AssetDatabase.GetDependencies(assetPath, false);

            List<NGScriptableObject> referencedEvents = new List<NGScriptableObject>();
            foreach (string dependency in dependencies)
            {
                string dependencyGUID = AssetDatabase.AssetPathToGUID(dependency);
                if (eventsByGuid.TryGetValue(dependencyGUID, out NGScriptableObject bEvent))
                    referencedEvents.Add(bEvent);
            }

            if (referencedEvents.Count == 0) return;

            // if there are referenced events, rebuil all referrals
            string containerGUID = AssetDatabase.AssetPathToGUID(assetPath);
            string containerName = System.IO.Path.GetFileName(assetPath);

            foreach (NGScriptableObject bEvent in referencedEvents)
            {
                bEvent.RemoveAllReferralsFromContainer(containerGUID);
                EditorUtility.SetDirty(bEvent);
            }

            if (extension == ".asset")
            {
                ProcessScriptableObject(assetPath, containerGUID, containerName, referencedEvents);
            }
            else
            {
                ProcessSceneOrPrefab(assetPath, containerGUID, containerName, extension, referencedEvents);
            }
        }

        private static void ProcessScriptableObject(string assetPath, string containerGUID, string containerName, List<NGScriptableObject> referencedEvents)
        {
            Object asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            if (asset == null) return;

            foreach (NGScriptableObject bEvent in referencedEvents)
            {
                if (!ReferencesEvent(asset, bEvent)) continue;

                ReferralEntry entry = new ReferralEntry(containerGUID, System.IO.Path.GetFileNameWithoutExtension(assetPath), ReferralKind.Asset);
                entry.RebuildLabel();
                bEvent.AddOrUpdateReferral(entry);
                EditorUtility.SetDirty(bEvent);
            }
        }

        private static void ProcessSceneOrPrefab(string assetPath, string containerGUID, string containerName, string extension, List<NGScriptableObject> referencedEvents)
        {
            if (!System.IO.File.Exists(assetPath)) return;

            string[] lines = System.IO.File.ReadAllLines(assetPath);

            Dictionary<string, string> gameObjectNames = ExtractGameObjectNames(lines);

            Dictionary<string, List<(string fileID, string componentNames)>> eventToComponents = ExtractEventReferences(lines, referencedEvents);

            ReferralKind kind = extension == ".prefab" ? ReferralKind.Prefab : ReferralKind.Scene;

            foreach (NGScriptableObject bEvent in referencedEvents)
            {
                string eventGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(bEvent));

                if (!eventToComponents.TryGetValue(eventGUID, out List<(string fileID, string componentNames)> references)) continue;

                Dictionary<string, List<string>> byGameObject = references.GroupBy(r => r.fileID).ToDictionary(g => g.Key, g => g.Select(r => r.componentNames).ToList());

                foreach (KeyValuePair<string, List<string>> pair in byGameObject)
                {
                    string fileID = pair.Key;
                    List<string> components = pair.Value;

                    gameObjectNames.TryGetValue(fileID, out string goName);

                    ReferralEntry entry = new ReferralEntry(containerGUID, containerName, fileID, goName ?? $"GameObject ({fileID})", components, kind);
                    entry.RebuildLabel();
                    bEvent.AddOrUpdateReferral(entry);
                    EditorUtility.SetDirty(bEvent);
                }
            }
        }

        const string GAMEOBJECT_YAML = "--- !u!1 &";
        private static Dictionary<string, string> ExtractGameObjectNames(string[] lines)
        {
            Dictionary<string, string> results = new();

            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                string line = lines[lineIndex];

                if (!line.StartsWith(GAMEOBJECT_YAML)) continue;

                string fileID = line[GAMEOBJECT_YAML.Length..].Trim();

                for (int nextLineIndex = lineIndex + 1; nextLineIndex < lines.Length && !lines[nextLineIndex].StartsWith("---"); nextLineIndex++)
                {
                    string nextLine = lines[nextLineIndex];
                    if (!nextLine.TrimStart().StartsWith("m_Name:")) continue;
                    string name = nextLine[(nextLine.IndexOf("m_Name:") + 7)..].Trim();
                    results[fileID] = name;
                    break;
                }
            }
            return results;
        }

        private static Dictionary<string, List<(string fileID, string componentNames)>> ExtractEventReferences(string[] lines, List<NGScriptableObject> referencedEvents)
        {
            HashSet<string> targetGUIDs = new(referencedEvents.Select(e => AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(e))));

            Dictionary<string, List<(string, string)>> results = new Dictionary<string, List<(string, string)>>();

            string currentComponentFileID = string.Empty;
            string currentComponentType = string.Empty;
            string currentGameObjectFileID = string.Empty;

            for (int lineIndex = 0; lineIndex < lines.Length; lineIndex++)
            {
                string line = lines[lineIndex];

                if (line.StartsWith("--- !u!"))
                {
                    string[] parts = line.Split('&');
                    currentComponentFileID = parts.Length > 1 ? parts[1].Trim() : string.Empty;
                    currentComponentType = string.Empty;
                    currentGameObjectFileID = string.Empty;
                    continue;
                }

                if (line.TrimStart().StartsWith("m_EditorClassIdentifier:"))
                {
                    string value = line[(line.IndexOf(':') + 1)..].Trim();
                    int separatorIndex = value.IndexOf("::");
                    currentComponentType = separatorIndex >= 0
                        ? value.Substring(separatorIndex + 2).Trim()
                        : value.Trim();
                    continue;
                }

                if (line.TrimStart().StartsWith("m_GameObject:") && !string.IsNullOrEmpty(currentComponentFileID))
                {
                    int start = line.IndexOf("fileID:") + 7;
                    int end = line.IndexOf('}', start);
                    if (start > 7 && end > start)
                        currentGameObjectFileID = line[start..end].Trim();
                    continue;
                }

                if (!line.Contains("guid:")) continue;

                int guidStart = line.IndexOf("guid:") + 5;
                int guidEnd = line.IndexOf(',', guidStart);
                if (guidStart <= 5 || guidEnd <= guidStart) continue;

                string foundGUID = line[guidStart..guidEnd].Trim();
                if (!targetGUIDs.Contains(foundGUID)) continue;

                if (!results.TryGetValue(foundGUID, out List<(string, string)> list))
                {
                    list = new();
                    results[foundGUID] = list;
                }

                string componentName = string.IsNullOrEmpty(currentComponentType) ? "Component" : currentComponentType;
                list.Add((currentGameObjectFileID, componentName));
            }
            return results;
        }

        private static bool ReferencesEvent(Object asset, NGScriptableObject bEvent)
        {
            SerializedObject so = new(asset);
            SerializedProperty sp = so.GetIterator();
            while (sp.NextVisible(true))
            {
                if (sp.propertyType != SerializedPropertyType.ObjectReference) continue;
                if (sp.objectReferenceValue == bEvent) return true;
            }
            return false;
        }

        private static Dictionary<string, NGScriptableObject> LoadAllNekoScriptableObjects()
        {
            Dictionary<string, NGScriptableObject> results = new();
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(NGScriptableObject)}");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                NGScriptableObject bEvent = AssetDatabase.LoadAssetAtPath<NGScriptableObject>(path);
                if (bEvent == null) continue;
                results[guid] = bEvent;
            }
            return results;
        }
    }
}