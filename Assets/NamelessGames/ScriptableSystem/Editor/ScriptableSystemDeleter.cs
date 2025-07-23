using UnityEditor;
using UnityEngine;

public class ScriptableSystemDeleter : AssetModificationProcessor
{
    //static void OnWillCreateAsset(string assetName)
    //{
    //    Debug.Log(AssetDatabase.LoadAssetAtPath<ScriptableObject>(assetName) != null);
    //    Debug.Log(AssetDatabase.LoadMainAssetAtPath(assetName));
    //}

    static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions opt)
    {
        //if (AssetDatabase.LoadAssetAtPath<ScriptableEventManager>(path) != null)
        //{
        //    return AssetDeleteResult.FailedDelete;
        //}
        return AssetDeleteResult.DidNotDelete;
    }
}