using UnityEditor;

using Object = UnityEngine.Object;

public static class NG_ASSETS
{
    public static string GetObjectGUID(Object obj)
    {
        return AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(obj));
    }

    public static string GetGameObjectLocalID(Object obj)
    {
        return GlobalObjectId.GetGlobalObjectIdSlow(obj).assetGUID.ToString();

        //PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);

        //SerializedObject serializedObject = new SerializedObject(obj);
        //inspectorModeInfo.SetValue(serializedObject, InspectorMode.Debug, null);

        //SerializedProperty localIdProp = serializedObject.FindProperty("m_LocalIdentfierInFile");   //note the misspelling!

        //return localIdProp.intValue;
    }

    public static string GetSceneObjectName(string assetPath, int instanceID)
    {
        string absolutePath = UnityEngine.Application.dataPath + assetPath.Replace("Assets", "");
        try
        {
            string text = System.IO.File.ReadAllText(absolutePath);
            string portion = text.Substring(text.IndexOf(instanceID.ToString()));
            portion = portion[(portion.IndexOf("m_Name") + 8)..];
            return portion[..portion.IndexOf("\n")];
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError("Error reading file " + assetPath + " for reference\n" + e.Message);
        }
        return "Error";
    }
}