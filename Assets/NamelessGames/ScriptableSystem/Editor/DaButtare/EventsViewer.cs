using NamelessGames.ScriptableSystem.Events;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NamelessGames.ScriptableSystem.ScriptableSystemEditor
{
    public class EventsViewer : EditorWindow
    {
        [SerializeField] VisualTreeAsset _toolTemplate;
        [SerializeField] VisualTreeAsset _eventItemTemplate;
        [SerializeField] VisualTreeAsset _eventReferralTemplate;

        string[] _guids;
        VisualElement _eventButtonActive;
        BaseEvent _eventActive;

        [MenuItem("Tools/Nameless Games/Scriptable System/Scriptable Events Viewer")]
        public static EventsViewer Init()
        {
            EventsViewer window = GetWindow<EventsViewer>(false, "Scriptable Events Viewer");
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 650, 450);
            window.Show();
            return window;
        }

        public static void Init(BaseEvent baseEvent)
        {
            EventsViewer viewer = Init();
            viewer.ShowEvent(baseEvent);
        }

        private void CreateGUI()
        {
            _guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(BaseEvent)));

            rootVisualElement.Add(_toolTemplate.CloneTree());

            string guid, assetPath;
            for (int guidIndex = 0; guidIndex < _guids.Length; guidIndex++)
            {
                guid = _guids[guidIndex];
                assetPath = AssetDatabase.GUIDToAssetPath(guid);
                BaseEvent baseEvent = AssetDatabase.LoadAssetAtPath<BaseEvent>(assetPath);

                VisualElement eventItem = _eventItemTemplate.CloneTree();
                eventItem.Q<Button>("event-button").userData = guid;
                eventItem.Q<Button>("event-button").text = $"{baseEvent.name} ({baseEvent.GetType().Name.ToString().Replace("Event", "")})";
                eventItem.Q<Button>("event-button").clicked += () => ShowEvent(eventItem, baseEvent);
                eventItem.Q<Button>("event-ping").clicked += () => EditorGUIUtility.PingObject(baseEvent);
                rootVisualElement.Q<ScrollView>("events-scroll-view").Add(eventItem);
            }

            rootVisualElement.Q<ListView>("event-referrals-list-view").makeItem = () => _eventReferralTemplate.CloneTree();
            rootVisualElement.Q<ListView>("event-referrals-list-view").bindItem += BindReferral;

            rootVisualElement.Q<Button>("event-raise-button").clicked += RaiseEvent;
        }

        private void BindReferral(VisualElement element, int index)
        {
            ReferralEntry entry = _eventActive.Referrals[index];
            Label l = element.Q<Label>("event-referral-label");

            l.text = entry.Label;

            if (_pingActions.TryGetValue(element, out Action pingAction))
                element.Q<Button>("event-referral-ping-button").clicked -= pingAction;

            //pingAction = () => _eventActive.PingReferral(entry);
            //_pingActions[element] = pingAction;
            //element.Q<Button>("event-referral-ping-button").clicked += pingAction;
        }

        private void RaiseEvent()
        {
            if (_eventActive == null) return;
            //_eventActive.SendFakeArg();
        }

        private void ShowEvent(BaseEvent baseEvent)
        {
            VisualElement eventItem = rootVisualElement.Q<ScrollView>("events-scroll-view").Query<Button>("event-button").Where(elem => (string)elem.userData == NG_ASSETS.GetObjectGUID(baseEvent));
            ShowEvent(eventItem, baseEvent);
        }

        private void ShowEvent(VisualElement eventItem, BaseEvent baseEvent)
        {
            if (_eventButtonActive != null)
            {
                _eventButtonActive.Q<Button>("event-button").RemoveFromClassList("event-button--active");
                //rootVisualElement.Q<Button>("event-raise-button").clicked -= _eventActive.SendFakeArg;
            }

            SerializedObject serializedEvent = new SerializedObject(baseEvent);
            _eventButtonActive = eventItem;
            _eventActive = baseEvent;

            rootVisualElement.Q<VisualElement>("event-details").Bind(serializedEvent);

            eventItem.Q<Button>("event-button").AddToClassList("event-button--active");
            _eventActive.CheckReferrals();

            var listView = rootVisualElement.Q<ListView>("event-referrals-list-view");
            listView.itemsSource = baseEvent.Referrals;
            listView.Rebuild();
        }

        Dictionary<VisualElement, Action> _pingActions = new Dictionary<VisualElement, Action>();
    }
}