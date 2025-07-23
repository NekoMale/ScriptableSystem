using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class EventsViewer : EditorWindow
{
    [SerializeField] VisualTreeAsset _toolTemplate;
    [SerializeField] VisualTreeAsset _eventItemTemplate;
    [SerializeField] VisualTreeAsset _eventReferralTemplate;

    bool _hidden = true;
    string[] _guids;
    VisualElement _eventButtonActive;
    BaseEvent _eventActive;

    static string[] _eventGUIDs;    

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
            eventItem.Q<Button>("event-button").text = $"{baseEvent.name} ({baseEvent.GetType().ToString().Replace("Event", "")})";
            eventItem.Q<Button>("event-button").clicked += () => ShowEvent(eventItem, baseEvent);
            eventItem.Q<Button>("event-ping").clicked += () => EditorGUIUtility.PingObject(baseEvent);
            rootVisualElement.Q<ScrollView>("events-scroll-view").Add(eventItem);
        }

        rootVisualElement.Q<ListView>("event-listeners-list-view").bindItem += BindListenerReferral;
        rootVisualElement.Q<ListView>("event-invokers-list-view").bindItem += BindInvokerReferral;

        rootVisualElement.Q<Button>("event-raise-button").clicked += RaiseEvent;
    }

    private void RaiseEvent()
    {
        if(_eventActive == null) return;
        _eventActive.SendFakeArg();
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
            rootVisualElement.Q<Button>("event-raise-button").clicked -= _eventActive.SendFakeArg;
        }
        
        SerializedObject serializedEvent = new SerializedObject(baseEvent);
        _eventButtonActive = eventItem;
        _eventActive = baseEvent;

        rootVisualElement.Q<VisualElement>("event-details").Bind(serializedEvent);

        eventItem.Q<Button>("event-button").AddToClassList("event-button--active");
        _eventActive.CheckReferences();
    }

    Dictionary<VisualElement, Action> _pingActions = new Dictionary<VisualElement, Action>();

    private void BindListenerReferral(VisualElement element, int arg2)
    {
        element.Q<Label>("event-referral-label").text = _eventActive.GetListenerReferralLabel(arg2);

        if (_pingActions.TryGetValue(element, out Action pingAction))
        {
            element.Q<Button>("event-referral-ping-button").clicked -= pingAction;
        }
        pingAction = () => EditorGUIUtility.PingObject(_eventActive.GetListenerReferralObject(arg2));

        _pingActions[element] = pingAction;
        element.Q<Button>("event-referral-ping-button").clicked += _pingActions[element];
    }

    private void BindInvokerReferral(VisualElement element, int arg2)
    {
        BaseEvent.Referral referral = _eventActive.InvokersReferrals[arg2];

        element.Q<Label>("event-referral-label").text = _eventActive.GetInvokerReferralLabel(arg2);
        if (_pingActions.TryGetValue(element, out Action pingAction))
        {
            element.Q<Button>("event-referral-ping-button").clicked -= pingAction;
        }
        pingAction = () => EditorGUIUtility.PingObject(_eventActive.GetInvokerReferralObject(arg2));

        _pingActions[element] = pingAction; element.Q<Button>("event-referral-ping-button").clicked += _pingActions[element];
    }
}
