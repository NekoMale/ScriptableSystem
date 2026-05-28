using NamelessGames.ScriptableSystem.Events;
using NamelessGames.ScriptableSystem.Variables;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NamelessGames.ScriptableSystem.ScriptableSystemEditor
{
    public class ScriptableSystemViewer_old : EditorWindow
    {
        [SerializeField] VisualTreeAsset _toolTemplate;

        //static ScriptableSystemViewer _window;

        //[MenuItem("Tools/Nameless Games/Scriptable System/Scriptable System Viewer")]
        //public static ScriptableSystemViewer Init()
        //{
        //    //if (_window != null) return _window;
        //
        //    _window = GetWindow<ScriptableSystemViewer>(false, "Scriptable System Viewer");
        //    _window.position = new Rect(Screen.width / 2, Screen.height / 2, 650, 450);
        //    _window.Show();
        //    return _window;
        //}


        private void CreateGUI()
        {
            rootVisualElement.Add(_toolTemplate.CloneTree());

            TabView tabView = rootVisualElement.Q<TabView>("scriptable-system-tab-view");

            tabView.activeTabChanged += ChangeTab;
            foreach (Tab tab in tabView.Query<Tab>().ToList())
            {
                if(tab.tabIndex == 0) tab.selected += OnFirstTabClicked;
            }
        }

        private void OnFirstTabClicked(Tab tab)
        {
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(BaseVariable)));
            string guid, assetPath;

            Tab newTab = rootVisualElement.Q<Tab>("variables-viewer-tab");

            List<Button> variableButtons = tab.Q<ScrollView>("existing-variables-scroll-view").Query<Button>().ToList();

            int minSize = Mathf.Min(variableButtons.Count, guids.Length);

            for (int guidIndex = 0; guidIndex < minSize; guidIndex++)
            {
                guid = guids[guidIndex];
                assetPath = AssetDatabase.GUIDToAssetPath(guid);
                BaseVariable variable = AssetDatabase.LoadAssetAtPath<BaseVariable>(assetPath);

                variableButtons[guidIndex].style.display = DisplayStyle.Flex;
                variableButtons[guidIndex].text = variable.name;
            }

            for (int guidIndex = minSize; guidIndex < guids.Length; guidIndex++)
            {
                guid = guids[guidIndex];
                assetPath = AssetDatabase.GUIDToAssetPath(guid);
                BaseVariable variable = AssetDatabase.LoadAssetAtPath<BaseVariable>(assetPath);

                newTab.Q<ScrollView>("existing-variables-scroll-view").Add(new Button() { text = variable.name });
            }

            for (int buttonIndex = minSize; buttonIndex < variableButtons.Count; buttonIndex++)
            {
                variableButtons[buttonIndex].style.display = DisplayStyle.None;
            }

            SerializedObject sv = new SerializedObject(AssetDatabase.LoadAssetAtPath<BaseVariable>(AssetDatabase.GUIDToAssetPath(guids[0])));
            tab.Q<VisualElement>(className: "scriptable-system-datas-container").Bind(sv);

            
        }

        private void ChangeTab(Tab previousTab, Tab newTab)
        {
            string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(BaseVariable<,>)));
            string guid, assetPath;
            if(newTab.tabIndex == 0)
            {
                for (int guidIndex = 0; guidIndex < guids.Length; guidIndex++)
                {
                    guid = guids[guidIndex];
                    assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    NGScriptableObject variable = AssetDatabase.LoadAssetAtPath<NGScriptableObject>(assetPath);
                    newTab.Q<ScrollView>("existing-variables-scroll-view").Add(new Button() { text = variable.name });
                }
            }
        }
    }
}