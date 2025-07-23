using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

public abstract class BaseEvent : ScriptableObject
{
#if UNITY_EDITOR
    public abstract void SendFakeArg();

    [System.Serializable]
    public struct Referral
    {
        public string GUID;
        public string ContainerGUID;

        public Referral(string referralGUID, string containerGUID)
        {
            GUID = referralGUID;
            ContainerGUID = containerGUID;
        }
    }

    public Referral[] ListenersReferrals = new Referral[0];
    public Referral[] InvokersReferrals = new Referral[0];

    static Dictionary<string, string[]> sceneFiles = new Dictionary<string, string[]>();
    public void CheckReferences()
    {
        string myGUID = AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(this));
        sceneFiles = new Dictionary<string, string[]>();

        for (int referralIndex = ListenersReferrals.Length - 1; referralIndex > -1; referralIndex--)
        {
            Referral listenerReferral = ListenersReferrals[referralIndex];
            if (IsReferenceStillPresent(myGUID, listenerReferral)) continue;
            RemoveListenerReferral(listenerReferral.GUID);
        }

        for (int referralIndex = InvokersReferrals.Length - 1; referralIndex > -1; referralIndex--)
        {
            Referral invokerReferral = InvokersReferrals[referralIndex];
            if (IsReferenceStillPresent(myGUID, invokerReferral)) continue;
            RemoveInvokerReferral(invokerReferral.GUID);
        }
    }

    public string GetListenerReferralLabel(int index)
    {
        return GetReferralLabel(index, true);
    }

    public string GetInvokerReferralLabel(int index)
    {
        return GetReferralLabel(index, false);
    }

    private string GetReferralLabel(int index, bool isListener)
    {
        Referral referral = isListener ? ListenersReferrals[index] : InvokersReferrals[index];
        if (!string.IsNullOrEmpty(referral.ContainerGUID))
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(referral.ContainerGUID);
            string referralName = System.IO.Path.GetFileName(scenePath) + " > ";

            string[] lines = GetContainerFileLines(scenePath);

            if (lines.Length == 0)
                return referralName;

            string searchingLine = "&" + referral.GUID;
            int lineIndex = GetLineIndexOfText(lines, searchingLine);
            lineIndex = GetLineIndexOfText(lines, "m_Name", lineIndex + 1);
            return referralName + lines[lineIndex].Replace("m_Name:", string.Empty).Trim();
        }
        return AssetDatabase.GUIDToAssetPath(referral.GUID);
    }

    public Object GetListenerReferralObject(int index)
    {
        return GetReferralObject(index, true);
    }

    public Object GetInvokerReferralObject(int index)
    {
        return GetReferralObject(index, false);
    }

    private Object GetReferralObject(int index, bool isListener)
    {
        Referral referral = isListener ? ListenersReferrals[index] : InvokersReferrals[index];
        if (!string.IsNullOrEmpty(referral.ContainerGUID))
        {
            string containerPath = AssetDatabase.GUIDToAssetPath(referral.ContainerGUID);
            string[] lines = GetContainerFileLines(containerPath);
            string searchingLine = "&" + referral.GUID;
            int lineIndex = GetLineIndexOfText(lines, searchingLine);
            lineIndex = GetLineIndexOfText(lines, "m_Name", lineIndex + 1);

            if (System.IO.Path.GetExtension(containerPath).Trim().ToLower() == ".prefab")
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(containerPath);
                PrefabStage ps = PrefabStageUtility.GetCurrentPrefabStage();
                if (ps == null || ps.assetPath != containerPath)
                {
                    ps = PrefabStageUtility.OpenPrefab(containerPath);
                }
                List<Transform> transformsToCheck = new() { ps.prefabContentsRoot.transform };
                int transformIndex = 0;
                Transform referralTransform;
                do
                {
                    Transform parent = transformsToCheck[transformIndex];
                    transformIndex++;
                    referralTransform = parent.Find(lines[lineIndex].Replace("m_Name:", string.Empty).Trim());
                    if (referralTransform == null)
                    {
                        if (parent.childCount > 0)
                        {
                            foreach (Transform child in parent)
                            {
                                transformsToCheck.Add(child);
                            }
                        }
                        continue;
                    }
                } while (transformIndex < transformsToCheck.Count && referralTransform == null);
                return referralTransform.gameObject;

            }
            else
            {
                UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                if (scene.path != containerPath)
                {
                    scene = EditorSceneManager.OpenScene(containerPath);
                }
                return GameObject.Find(lines[lineIndex].Replace("m_Name:", string.Empty).Trim());
            }
        }

        return AssetDatabase.LoadAssetAtPath<Object>(AssetDatabase.GUIDToAssetPath(referral.GUID));
    }

    private int GetLineIndexOfText(string[] lines, string searchingLine, int startingIndex = 0)
    {
        int lineIndex;
        for (lineIndex = startingIndex; lineIndex < lines.Length; lineIndex++)
        {
            string line = lines[lineIndex];
            if (line.IndexOf(searchingLine) > -1)
                return lineIndex;
        }
        return -1;
    }

    private string[] GetContainerFileLines(string containerPath)
    {
        if (!sceneFiles.TryGetValue(containerPath, out string[] lines))
        {
            if (!System.IO.File.Exists(containerPath)) return new string[0];
            lines = System.IO.File.ReadAllLines(containerPath);
            sceneFiles[containerPath] = lines;
        }
        return lines;
    }

    private bool IsReferenceStillPresent(string myGUID, Referral referral)
    {
        if (string.IsNullOrEmpty(referral.ContainerGUID))
            return System.IO.File.Exists(AssetDatabase.GUIDToAssetPath(referral.GUID));

        string containerPath = AssetDatabase.GUIDToAssetPath(referral.ContainerGUID);

        if (!System.IO.File.Exists(containerPath)) return false;


        string[] lines = GetContainerFileLines(containerPath);
        string searchingLine = $"m_GameObject: {{fileID: {referral.GUID}}}";
        int lineIndex = 0;

        if (System.IO.Path.GetExtension(containerPath).Trim().ToLower() == ".prefab")
        {
            PrefabStage ps = PrefabStageUtility.GetCurrentPrefabStage();
            if (ps != null && ps.assetPath == containerPath && EditorUtility.IsDirty(ps))
            {
                return true;
            }
        }
        else
        {
            UnityEngine.SceneManagement.Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (scene.path == containerPath && scene.isDirty)
            {
                return true;
            }
        }

        do
        {
            lineIndex = GetLineIndexOfText(lines, searchingLine, lineIndex);
            while (lineIndex != -1 && lineIndex < lines.Length && !lines[lineIndex].StartsWith("--- !u!"))
            {
                if (lines[lineIndex].IndexOf(myGUID) != -1)
                    return true;
                lineIndex++;
            }
        } while (lineIndex != -1);
        return false;
    }

    public void AddReferral(Object referralToAdd)
    {
        bool isListener = false;

        if (referralToAdd is MonoBehaviour monoBehaviour)
        {
            isListener = referralToAdd is BaseEventsListener;
            referralToAdd = monoBehaviour.gameObject;
        }

        GlobalObjectId goid = GlobalObjectId.GetGlobalObjectIdSlow(referralToAdd);

        string referralGUID, containerGUID;
        if (goid.identifierType == 0)
        {
            PrefabStage ps = PrefabStageUtility.GetCurrentPrefabStage();
            if (ps == null || !ps.IsPartOfPrefabContents(Selection.activeGameObject)) return;

            PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);

            SerializedObject serializedObject = new SerializedObject(Selection.activeGameObject);
            inspectorModeInfo.SetValue(serializedObject, InspectorMode.Debug, null);
            SerializedProperty localIdProp = serializedObject.FindProperty("m_LocalIdentfierInFile");

            referralGUID = ps.prefabContentsRoot == Selection.activeGameObject ? AssetDatabase.AssetPathToGUID(ps.assetPath) : localIdProp.longValue.ToString();
            containerGUID = ps.prefabContentsRoot == Selection.activeGameObject ? string.Empty : AssetDatabase.AssetPathToGUID(ps.assetPath);
        }
        else
        {
            referralGUID = goid.identifierType == 1 || goid.identifierType == 3 ? goid.assetGUID.ToString() : goid.targetObjectId.ToString();
            containerGUID = goid.identifierType == 2 ? goid.assetGUID.ToString() : string.Empty;
        }

        if (isListener)
        {
            AddListenerReferral(referralGUID, containerGUID);
        }
        else
        {
            AddInvokerReferral(referralGUID, containerGUID);
        }
        EditorUtility.SetDirty(this);
    }

    private void AddListenerReferral(string referralGUID, string sceneGUID)
    {
        if (!ListenersReferrals.FirstOrDefault(referral => referral.GUID == referralGUID).Equals(default(Referral))) return;

        System.Array.Resize(ref ListenersReferrals, ListenersReferrals.Length + 1);
        ListenersReferrals[ListenersReferrals.Length - 1] = new Referral(referralGUID, sceneGUID);
    }

    public void AddInvokerReferral(string referralGUID, string sceneGUID)
    {
        if (!InvokersReferrals.FirstOrDefault(referral => referral.GUID == referralGUID).Equals(default(Referral))) return;

        System.Array.Resize(ref InvokersReferrals, InvokersReferrals.Length + 1);
        InvokersReferrals[InvokersReferrals.Length - 1] = new Referral(referralGUID, sceneGUID);
    }

    public void RemoveReferral(Object referralToRemove)
    {
        string referralType = referralToRemove.GetType().AssemblyQualifiedName;
        bool isListener = false;

        if (referralToRemove is MonoBehaviour monoBehaviour)
        {
            isListener = referralToRemove is BaseEventsListener;
            referralToRemove = monoBehaviour.gameObject;
        }

        GlobalObjectId goid = GlobalObjectId.GetGlobalObjectIdSlow(referralToRemove);

        string referralGUID;
        if (goid.identifierType == 0)
        {
            PrefabStage ps = PrefabStageUtility.GetCurrentPrefabStage();
            if (ps == null || !ps.IsPartOfPrefabContents(Selection.activeGameObject)) return;

            PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);

            SerializedObject serializedObject = new SerializedObject(Selection.activeGameObject);
            inspectorModeInfo.SetValue(serializedObject, InspectorMode.Debug, null);
            SerializedProperty localIdProp = serializedObject.FindProperty("m_LocalIdentfierInFile");

            referralGUID = ps.prefabContentsRoot == Selection.activeGameObject ? AssetDatabase.AssetPathToGUID(ps.assetPath) : localIdProp.longValue.ToString();
        }
        else
        {
            referralGUID = goid.identifierType == 1 || goid.identifierType == 3 ? goid.assetGUID.ToString() : goid.targetObjectId.ToString();
        }


        if (isListener)
        {
            RemoveListenerReferral(referralGUID);
        }
        else
        {
            RemoveInvokerReferral(referralGUID);
        }
        EditorUtility.SetDirty(this);
    }

    private void RemoveListenerReferral(string referralGUID)
    {
        if (ListenersReferrals.FirstOrDefault(referral => referral.GUID == referralGUID).Equals(default(Referral))) return;

        bool found = false;
        for (int guidIndex = 0; guidIndex < ListenersReferrals.Length - 1; guidIndex++)
        {
            if (!found && ListenersReferrals[guidIndex].GUID != referralGUID) continue;
            found = true;
            ListenersReferrals[guidIndex] = ListenersReferrals[guidIndex + 1];
        }
        System.Array.Resize(ref ListenersReferrals, ListenersReferrals.Length - 1);
    }


    private void RemoveInvokerReferral(string referralGUID)
    {
        if (InvokersReferrals.FirstOrDefault(referral => referral.GUID == referralGUID).Equals(default(Referral))) return;

        bool found = false;
        for (int guidIndex = 0; guidIndex < InvokersReferrals.Length - 1; guidIndex++)
        {
            if (!found && InvokersReferrals[guidIndex].GUID != referralGUID) continue;
            found = true;
            InvokersReferrals[guidIndex] = InvokersReferrals[guidIndex + 1];
        }
        System.Array.Resize(ref InvokersReferrals, InvokersReferrals.Length - 1);
    }
#endif
}