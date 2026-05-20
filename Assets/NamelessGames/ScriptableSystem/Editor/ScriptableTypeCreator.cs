using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ScriptableTypeCreator : EditorWindow
{
    [MenuItem("Tools/Nameless Games/Scriptable System/Scriptable Type Creator")]
    protected static void Init()
    {
        ScriptableTypeCreator window = GetWindow<ScriptableTypeCreator>(false, "Scriptable Type Creator");
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 650, 450);
        window.Show();
    }

    bool _createVariable = false, _createEvent = false;
    string _usingDirectives = string.Empty;
    string _varTypeName = string.Empty;

    private void OnGUI()
    {
        _usingDirectives = EditorGUILayout.TextField("Using", _usingDirectives);
        _varTypeName = EditorGUILayout.TextField("Var Type", _varTypeName);

        _createVariable = EditorGUILayout.Toggle("Variable", _createVariable);
        _createEvent = EditorGUILayout.Toggle("Event", _createEvent || _createVariable);

        if (GUILayout.Button("Create"))
        {
            CreateEvent();
        }
    }

    private void CreateEvent()
    {
        if(_createVariable)
        {
            CreateScriptableType("Variable", "Variables");
        }
        if (_createEvent)
        {
            CreateScriptableType("Event", "Events/");
            CreateScriptableType("EventsListener", "Events/Listeners/");
        }
    }

    private void CreateScriptableType(string scriptableType, string folder)
    {
        string template = ReadFile(GetFrameworkBasePath() + "Editor/Templates/", scriptableType + "Template.txt");

        string fileDirectives = template.Split(new string[] { "namespace" }, StringSplitOptions.RemoveEmptyEntries)[0];
        string usingDirectives = FormatUsingDirectives(_usingDirectives, fileDirectives);

        string varTypeName = _varTypeName.Replace("<", "").Replace(">", "");
        varTypeName = varTypeName.Substring(0, 1).ToUpper() + varTypeName.Substring(1);

        string newContent = usingDirectives;
        newContent += template.Replace("{VAR_TYPE}", _varTypeName).Replace("{VAR_TYPE_NAME}", varTypeName);

        WriteFile(Application.dataPath + "/Scripts/ScriptableSystemCustom/" + folder,  varTypeName + scriptableType, newContent);
    }

    private string FormatUsingDirectives(string customUsingDirectives, string fileUsingDirectives)
    {
        string finalUsingDirective = "";

        List<string> usingDirectivesSplitted = customUsingDirectives.Split(';').ToList();
        List<string> fileUsingDirectivesSplitted = fileUsingDirectives.Split(';').ToList();
        HashSet<string> usingDirectives = new HashSet<string>();
        for (int i = 0; i < fileUsingDirectivesSplitted.Count; i++)
        {
            usingDirectives.Add(fileUsingDirectivesSplitted[i] + ";");
        }

        for (int i = 0; i < usingDirectivesSplitted.Count; i++)
        {
            string usingToCheck = ClearString(usingDirectivesSplitted[i]);
            if (string.IsNullOrWhiteSpace(usingToCheck) || string.IsNullOrEmpty(usingToCheck))
                continue;

            usingToCheck += ";";
            if (!usingToCheck.Contains("using "))
                usingToCheck = "using " + usingToCheck;

            if (usingDirectives.Contains(usingToCheck))
                continue;

            usingDirectives.Add(usingToCheck);
            finalUsingDirective += usingToCheck + "\n";
        }
        return finalUsingDirective;
    }
    private string ClearString(string stringToClear)
    {
        stringToClear = stringToClear.Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace('\u00A0'.ToString(), "").Replace('\u200C'.ToString(), "").Replace('\u2063'.ToString(), "").Trim(' ');
        if (string.IsNullOrWhiteSpace(stringToClear) || string.IsNullOrEmpty(stringToClear))
        {
            stringToClear = string.Empty;
        }
        return stringToClear;
    }

    private string GetFrameworkBasePath()
    {
        string frameworkPath = Directory.GetFiles(Application.dataPath, "ScriptableTypeCreator.cs", SearchOption.AllDirectories)[0];
        frameworkPath = frameworkPath.Replace("Editor\\ScriptableTypeCreator.cs", "");
        int searchIndex = frameworkPath.IndexOf("Assets\\");
        frameworkPath = frameworkPath.Substring(searchIndex);
        return frameworkPath;
    }

    private string ReadFile(string path, string fileName)
    {
        string filePath = Path.Combine(path, fileName);
        string fileContent = File.ReadAllText(filePath);
        return fileContent;
    }

    protected void WriteFile(string folder, string fileName, string fileContent)
    {
        string filePath = Path.Combine(folder, fileName + ".cs");
        string fileDirectory = Path.GetDirectoryName(filePath);
        Debug.Log("Would write file " + filePath);
        Debug.Log("Would write in " + fileDirectory);
        if (!Directory.Exists(fileDirectory))
        {
            Directory.CreateDirectory(fileDirectory);
        }
        if (!File.Exists(filePath))
        {
            File.WriteAllText(filePath, fileContent);
            AssetDatabase.ImportAsset(filePath);
            AssetDatabase.Refresh();
        }
    }

}
