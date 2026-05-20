using NamelessGames.ScriptableSystem.Events;
using NamelessGames.ScriptableSystem.Variables;
using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NamelessGames.ScriptableSystem.ScriptableSystemEditor
{
    public class ScriptableSystemViewer : EditorWindow
    {
        [SerializeField] VisualTreeAsset _toolTemplate;

        static ScriptableSystemViewer _window;

        [MenuItem("Tools/Nameless Games/Scriptable System/Scriptable System Viewer")]
        public static ScriptableSystemViewer Init()
        {
            //if (_window != null) return _window;

            _window = GetWindow<ScriptableSystemViewer>(false, "Scriptable System Viewer");
            _window.position = new Rect(Screen.width / 2, Screen.height / 2, 650, 450);
            _window.Show();
            return _window;
        }


        private void CreateGUI()
        {
            rootVisualElement.Add(_toolTemplate.CloneTree());

            TabView tabView = rootVisualElement.Q<TabView>("scriptable-system-tab-view");
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
            if (newTab.tabIndex == 0)
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