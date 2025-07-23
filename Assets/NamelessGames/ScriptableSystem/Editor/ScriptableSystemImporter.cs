using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

class ScriptableSystemImporter //: AssetPostprocessor
{
    //private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    //{
        //if(importedAssets.Length > 0)
        //{
        //    CheckImportEvent(importedAssets);
        //}

        //if(deletedAssets.Length > 0)
        //{
        //    CheckDeletedEvent(deletedAssets);
        //}

        //for (int i = 0; i < movedAssets.Length; i++)
        //{
        //    Debug.Log("Moved Asset: " + movedAssets[i] + " from: " + movedFromAssetPaths[i]);
        //}
    //}    

    //private static void CheckImportEvent(string[] importedAssets)
    //{
    //    string frameworkPath = Directory.GetFiles(Application.dataPath, nameof(ScriptableTypeCreator)+".cs", SearchOption.AllDirectories)[0];
    //    foreach (string str in importedAssets)
    //    {
    //        if (!str.Contains(".asset")) continue;
    //        BaseEvent pp = AssetDatabase.LoadAssetAtPath<BaseEvent>(str);
    //        if(pp != null)
    //        {
    //            string guid = AssetDatabase.AssetPathToGUID(str);

    //            ScriptableEventManager.Instance.AddEvent(guid);
    //        }
    //    }
    //}

    //private static void CheckDeletedEvent(string[] deletedAssets)
    //{
    //    string frameworkPath = Directory.GetFiles(Application.dataPath, nameof(ScriptableTypeCreator) + ".cs", SearchOption.AllDirectories)[0];
    //    foreach (string str in deletedAssets)
    //    {
    //        if (!str.Contains(".asset")) continue;

    //            string guid = AssetDatabase.AssetPathToGUID(str);

    //            ScriptableEventManager.Instance.RemoveEvent(guid);

    //    }
    //}
}