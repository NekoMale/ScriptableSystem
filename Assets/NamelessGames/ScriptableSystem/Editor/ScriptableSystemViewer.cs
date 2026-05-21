using NamelessGames.ScriptableSystem.Collections;
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
    public class ScriptableSystemViewer : EditorWindow
    {
        // ── Serialized references assigned in the Inspector of the EditorWindow asset ──
        [SerializeField] VisualTreeAsset _toolTemplate;
        [SerializeField] VisualTreeAsset _eventItemTemplate;
        [SerializeField] VisualTreeAsset _eventReferralItemTemplate;

        // ── State ──
        NGScriptableObject _selected;
        VisualElement      _activeListButton;

        // Per-tab type filters
        static readonly (string tabName, Type baseType)[] TabTypes = new[]
        {
            ("variables-viewer-tab",   typeof(BaseVariable)),
            ("events-viewer-tab",      typeof(BaseEvent)),
            ("arrays-viewer-tab",      typeof(BaseArray)),
            ("lists-viewer-tab",       typeof(BaseList)),
        };

        // Cached ping-action map (avoids lambda leak on ListView rebind)
        readonly Dictionary<VisualElement, Action> _pingActions = new();

        // ─────────────────────────────────────────────────────────────
        [MenuItem("Tools/Nameless Games/Scriptable System/Scriptable System Viewer")]
        public static ScriptableSystemViewer Init()
        {
            var window = GetWindow<ScriptableSystemViewer>(false, "Scriptable System Viewer");
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 900, 520);
            window.Show();
            return window;
        }

        // ─────────────────────────────────────────────────────────────
        void CreateGUI()
        {
            rootVisualElement.Add(_toolTemplate.CloneTree());

            // Populate each tab when it becomes active
            var tabView = rootVisualElement.Q<TabView>("scriptable-system-tab-view");
            tabView.activeTabChanged += OnTabChanged;

            // Bootstrap the first tab immediately
            PopulateTab(rootVisualElement.Q<Tab>("variables-viewer-tab"), typeof(BaseVariable));

            // Wire up the detail-panel referrals ListView (shared across tabs)
            //var referralsListView = rootVisualElement.Q<ListView>("detail-referrals-list-view");
            //if (referralsListView != null)
            //{
            //    referralsListView.makeItem = () => _eventReferralItemTemplate.CloneTree();
            //    referralsListView.bindItem += BindReferralItem;
            //}
        }

        // ─────────────────────────────────────────────────────────────
        //  Tab population
        // ─────────────────────────────────────────────────────────────

        void OnTabChanged(Tab _, Tab newTab)
        {
            // Identify which base type maps to this tab
            foreach (var (tabName, baseType) in TabTypes)
            {
                if (newTab.name == tabName)
                {
                    PopulateTab(newTab, baseType);
                    return;
                }
            }
        }

        void PopulateTab(Tab currentTab, Type baseType)
        {
            var scrollView = currentTab.Q<ScrollView>("existing-items-scroll-view");
            if (scrollView == null) return;

            // Clear stale buttons (tab may be revisited)
            scrollView.Clear();
            ClearDetailPanel(currentTab);

            string[] guids = AssetDatabase.FindAssets($"t:{baseType.Name}");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var asset   = AssetDatabase.LoadAssetAtPath<NGScriptableObject>(path);
                if (asset == null) continue;

                var btn = new Button { text = BuildButtonLabel(asset) };
                btn.AddToClassList("ssv-list-button");
                btn.clicked += () => SelectAsset(currentTab, asset, btn);
                scrollView.Add(btn);
            }
        }

        static string BuildButtonLabel(NGScriptableObject asset)
        {
            string typeSuffix = asset.GetType().Name;
            return $"{asset.name}  <color=#888888><size=10>{typeSuffix}</size></color>";
        }

        // ─────────────────────────────────────────────────────────────
        //  Selection & detail panel
        // ─────────────────────────────────────────────────────────────

        void SelectAsset(Tab currentTab, NGScriptableObject asset, Button sourceButton)
        {
            // Deactivate previous list button
            _activeListButton?.RemoveFromClassList("ssv-list-button--active");
            _activeListButton = sourceButton;
            sourceButton.AddToClassList("ssv-list-button--active");

            _selected = asset;
            asset.CheckReferrals();

            ShowDetailPanel(currentTab, asset);
        }

        void ShowDetailPanel(Tab currentTab, NGScriptableObject asset)
        {
            var detailPanel = currentTab.Q<VisualElement>("detail-panel");
            if (detailPanel == null) return;

            // Header
            detailPanel.Q<Label>("detail-name-label").text = asset.name;
            detailPanel.Q<Label>("detail-type-label").text = asset.GetType().Name;

            // NGName / NGDescription
            string ngName = asset.NGName;
            string ngDesc = asset.NGDescription;

            var nameLabel = detailPanel.Q<Label>("detail-ng-name-label");
            var descLabel = detailPanel.Q<Label>("detail-ng-desc-label");
            nameLabel.text                      = string.IsNullOrEmpty(ngName) ? string.Empty : ngName;
            nameLabel.style.display             = string.IsNullOrEmpty(ngName) ? DisplayStyle.None : DisplayStyle.Flex;
            descLabel.text                      = string.IsNullOrEmpty(ngDesc) ? string.Empty : ngDesc;
            descLabel.style.display             = string.IsNullOrEmpty(ngDesc) ? DisplayStyle.None : DisplayStyle.Flex;

            // Bind the asset so PropertyFields resolve automatically
            var serialized = new SerializedObject(asset);
            detailPanel.Bind(serialized);

            // ── Variable-specific panel ──────────────────────────────
            var varPanel = detailPanel.Q<VisualElement>("variable-values-panel");
            if (varPanel != null)
            {
                bool isVariable = asset is BaseVariable;
                varPanel.style.display = isVariable ? DisplayStyle.Flex : DisplayStyle.None;

                if (isVariable)
                {
                    detailPanel.Q<PropertyField>("detail-starting-value")?.BindProperty(
                        serialized.FindProperty("_startingValue"));
                    detailPanel.Q<PropertyField>("detail-current-value")?.BindProperty(
                        serialized.FindProperty("_value"));
                    detailPanel.Q<PropertyField>("detail-value-changed-event")?.BindProperty(
                        serialized.FindProperty("ValueChanged"));
                }
            }

            // ── Event-specific panel ─────────────────────────────────
            var evtPanel = detailPanel.Q<VisualElement>("event-raise-panel");
            if (evtPanel != null)
            {
                bool isEvent = asset is BaseEvent;
                evtPanel.style.display = isEvent ? DisplayStyle.Flex : DisplayStyle.None;

                if (isEvent)
                {
                    detailPanel.Q<PropertyField>("detail-fake-arg")?.BindProperty(
                        serialized.FindProperty("_fakeArg"));

                    var raiseBtn = detailPanel.Q<Button>("detail-raise-button");
                    if (raiseBtn != null)
                    {
                        // Re-wire to avoid stacking listeners
                        raiseBtn.clicked -= RaiseSelectedEvent;
                        raiseBtn.clicked += RaiseSelectedEvent;
                    }
                }
            }

            // ── Collection-specific panel ────────────────────────────
            var collPanel = detailPanel.Q<VisualElement>("collection-values-panel");
            if (collPanel != null)
            {
                bool isCollection = asset is BaseList || asset is BaseArray;
                collPanel.style.display = isCollection ? DisplayStyle.Flex : DisplayStyle.None;

                if (isCollection)
                {
                    detailPanel.Q<PropertyField>("detail-starting-items")?.BindProperty(
                        serialized.FindProperty("_startingItems"));
                    detailPanel.Q<PropertyField>("detail-current-items")?.BindProperty(
                        serialized.FindProperty("_items"));
                }
            }

            // ── Referrals ListView ───────────────────────────────────
            var listView = detailPanel.Q<ListView>("detail-referrals-list-view");
            if (listView != null)
            {
                listView.itemsSource = asset.Referrals;
                listView.Rebuild();
                listView.makeItem = () => _eventReferralItemTemplate.CloneTree();
                listView.bindItem += BindReferralItem;
            }
            //var referralsListView = rootVisualElement.Q<ListView>("detail-referrals-list-view");
            //if (referralsListView != null)
            //{
            //    referralsListView.makeItem = () => _eventReferralItemTemplate.CloneTree();
            //    referralsListView.bindItem += BindReferralItem;
            //}

            // ── Ping button ──────────────────────────────────────────
            var pingAssetBtn = detailPanel.Q<Button>("detail-ping-asset-button");
            if (pingAssetBtn != null)
            {
                pingAssetBtn.clicked -= PingSelectedAsset;
                pingAssetBtn.clicked += PingSelectedAsset;
            }

            detailPanel.style.display = DisplayStyle.Flex;
        }

        void ClearDetailPanel(Tab currentTab)
        {
            _selected = null;
            _activeListButton = null;
            VisualElement detailPanel = currentTab.Q<VisualElement>("detail-panel");
            if (detailPanel != null)
                detailPanel.style.display = DisplayStyle.None;
        }

        // ─────────────────────────────────────────────────────────────
        //  Referral binding
        // ─────────────────────────────────────────────────────────────

        void BindReferralItem(VisualElement element, int index)
        {
            if (_selected == null || index >= _selected.Referrals.Count) return;

            ReferralEntry entry = _selected.Referrals[index];

            element.Q<Label>("event-referral-label").text = entry.Label;

            // Kind badge
            var kindLabel = element.Q<Label>("referral-kind-label");
            if (kindLabel != null)
            {
                kindLabel.text = entry.Kind.ToString();
                kindLabel.RemoveFromClassList("referral-kind--scene");
                kindLabel.RemoveFromClassList("referral-kind--prefab");
                kindLabel.RemoveFromClassList("referral-kind--asset");
                kindLabel.AddToClassList($"referral-kind--{entry.Kind.ToString().ToLower()}");
            }

            // Ping button — clean up old listener first
            var pingBtn = element.Q<Button>("event-referral-ping-button");
            if (pingBtn != null)
            {
                if (_pingActions.TryGetValue(element, out Action old))
                    pingBtn.clicked -= old;

                Action newAction = () => _selected?.PingReferral(entry);
                _pingActions[element] = newAction;
                pingBtn.clicked += newAction;
            }
        }

        // ─────────────────────────────────────────────────────────────
        //  Actions
        // ─────────────────────────────────────────────────────────────

        void RaiseSelectedEvent()
        {
            if (_selected is BaseEvent evt)
                evt.SendFakeArg();
        }

        void PingSelectedAsset()
        {
            if (_selected != null)
                EditorGUIUtility.PingObject(_selected);
        }
    }
}
