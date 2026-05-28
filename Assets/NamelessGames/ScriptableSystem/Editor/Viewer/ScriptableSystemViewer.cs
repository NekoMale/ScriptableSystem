using NamelessGames.ScriptableSystem.Collections;
using NamelessGames.ScriptableSystem.Events;
using NamelessGames.ScriptableSystem.Variables;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NamelessGames.ScriptableSystem.ScriptableSystemEditor
{
    public class ScriptableSystemViewer : EditorWindow
    {
        [SerializeField] VisualTreeAsset _toolTemplate;
        [SerializeField] VisualTreeAsset _existingItemTemplate;
        [SerializeField] VisualTreeAsset _variableDetailTemplate;
        [SerializeField] VisualTreeAsset _eventDetailTemplate;

        VisualElement _existingItemActive = null;
        string _filter = string.Empty;

        static readonly (string tabName, Type baseType)[] TabTypes = new[]
        {
            ("variables-viewer-tab",   typeof(BaseVariable)),
            ("events-viewer-tab",      typeof(BaseEvent)),
            ("arrays-viewer-tab",      typeof(BaseArray)),
            ("lists-viewer-tab",       typeof(BaseList)),
        };

        [MenuItem("Tools/Nameless Games/Scriptable System/Scriptable System Viewer")]
        public static ScriptableSystemViewer Init()
        {
            var window = GetWindow<ScriptableSystemViewer>(false, "Scriptable System Viewer");
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 900, 520);
            window.Show();
            return window;
        }

        void CreateGUI()
        {
            rootVisualElement.Add(_toolTemplate.CloneTree());

            VisualElement filtersContainer = rootVisualElement.Q<VisualElement>("filters-container");

            List<Button> filterButtons = filtersContainer.Query<Button>().Where(x => true).ToList();
            foreach(Button filterButton in filterButtons)
            {
                filterButton.RegisterCallback<ClickEvent>(FilterClicked);
            }
            filterButtons[0].SetActivePseudoState(true);

            PopulateExistingItems();
        }

        private void FilterClicked(ClickEvent evt)
        {
            Button filterButton = evt.target as Button;

            Button oldFilterButton = filterButton.parent.Query<Button>().Where(x => x.hasActivePseudoState).First();

            oldFilterButton?.SetActivePseudoState(false);
            filterButton.SetActivePseudoState(true);

            if(filterButton.name == "remove-type-filter-button")
            {
                _filter = string.Empty;
            }
            else
            {
                _filter = filterButton.name.Replace("-type-filter-button", "");
                _filter = "Base" + _filter[0..1].ToUpper() + _filter[1..];
            }

            PopulateExistingItems();
        }

        void PopulateExistingItems()
        {
            ScrollView existingItemScrollView = rootVisualElement.Q<ScrollView>("list-existing-item-scroll-view");
            existingItemScrollView.Clear();

            string[] guids = new string[0];

            if(_filter == string.Empty)
            {
                guids = AssetDatabase.FindAssets($"t:{typeof(NGScriptableObject)}");
            }
            else
            {
                guids = AssetDatabase.FindAssets($"t:{_filter}");
            }

            if (guids.Length == 0) return;

            for (int guidIndex = 0; guidIndex < guids.Length; guidIndex++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[guidIndex]);
                NGScriptableObject asset = AssetDatabase.LoadAssetAtPath<NGScriptableObject>(path);

                existingItemScrollView.Add(CreateElementAndBindTo(asset));
            }
                        
            ItemSelected(existingItemScrollView.ElementAt(0).Q<Button>());
        }

        VisualElement CreateElementAndBindTo(NGScriptableObject asset)
        {
            VisualElement element = _existingItemTemplate.CloneTree();
            element.Q<Label>("existing-item-header").text = asset.name;
            element.Q<Label>("existing-item-type-label").text = asset.GetType().Name;
            element.Q<Button>().userData = new SerializedObject(asset);
            element.Q<Button>().RegisterCallback<ClickEvent>(ExistingItemClicked);

            return element;
        }

        private void ExistingItemClicked(ClickEvent evt)
        {
            ItemSelected(evt.target as Button);
        }

        private void ItemSelected(Button selectedItem)
        {
            if (_existingItemActive != null)
            {
                _existingItemActive.SetActivePseudoState(false);
            }
            selectedItem.SetActivePseudoState(true);

            _existingItemActive = selectedItem;
            SerializedObject so = selectedItem.userData as SerializedObject;

            VisualElement rightPanel = rootVisualElement.Q<VisualElement>("right-panel");
            rightPanel.Clear();

            NGSODetailViewer detailViewer = null;
            if (so.targetObject is BaseVariable)
            {
                detailViewer = new VariableDetailViewer(_variableDetailTemplate, rightPanel);
            }
            else if (so.targetObject is BaseEvent)
            {
                detailViewer = new EventDetailViewer(_eventDetailTemplate, rightPanel);
            }
            detailViewer.Bind(so);
        }

        void RaiseSelectedEvent()
        {
            if (_existingItemActive.userData is BaseEvent evt)
                evt.SendFakeArg();
        }
    }
}
