using NamelessGames.ScriptableSystem.Events;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;


namespace NamelessGames.ScriptableSystem.ScriptableSystemEditor
{
    [CustomEditor(typeof(BaseEventsListener), true)]
    public class BaseEventsListenerInspector : Editor
    {
        [SerializeField] VisualTreeAsset _inspectorTemplate;
        [SerializeField] VisualTreeAsset _drawerTemplate;

        SerializedProperty _eventsDatas;
        VisualElement _root;

        public override VisualElement CreateInspectorGUI()
        {
            _root = _inspectorTemplate.CloneTree();
            _eventsDatas = serializedObject.FindProperty("_baseEventsDatas");


            if (_eventsDatas.arraySize == 0)
            {
                CreateEmptyGUI();
            }
            else
            {
                CreatePopulatedGUI();
            }

            _root.Q<ListView>().bindItem += ElementBinded;
            _root.Q<ListView>().BindProperty(_eventsDatas);

            _root.Q<Button>("add-event-button").clicked += AddNewEvent;
            return _root;
        }

        private void CreateEmptyGUI()
        {
            _root.Q<VisualElement>("empty-events-container").style.display = DisplayStyle.Flex;
            _root.Q<VisualElement>("populated-events-container").style.display = DisplayStyle.None;
        }

        private void CreatePopulatedGUI()
        {
            _root.Q<VisualElement>("empty-events-container").style.display = DisplayStyle.None;
            _root.Q<VisualElement>("populated-events-container").style.display = DisplayStyle.Flex;

            _root.Q<Button>("show-events-button").clicked += () =>
            {
                _eventsDatas.isExpanded = !_eventsDatas.isExpanded;
                ShowEvents();
            };

            ShowEvents();
        }

        private void ShowEvents()
        {
            _root.Q<Button>("show-events-button").text = _eventsDatas.isExpanded ? "Hide" : "Show";
            _root.Q<ListView>().style.display = _eventsDatas.isExpanded ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void AddNewEvent()
        {
            if (_eventsDatas.arraySize == 0)
            {
                CreatePopulatedGUI();
            }

            _eventsDatas.InsertArrayElementAtIndex(_eventsDatas.arraySize);
            serializedObject.ApplyModifiedProperties();
        }

        Dictionary<VisualElement, Action[]> _lambdas = new();
        private void ElementBinded(VisualElement element, int arrayIndex)
        {
            SerializedProperty eventDatas = _eventsDatas.GetArrayElementAtIndex(arrayIndex);

            element.Q<PropertyField>().BindProperty(eventDatas.FindPropertyRelative("Event"));
            element.Q<Toggle>("on-enable-field").BindProperty(eventDatas.FindPropertyRelative("SubscribeOnEnable"));
            element.Q<Toggle>("on-disable-field").BindProperty(eventDatas.FindPropertyRelative("UnsubscribeOnDisable"));
            element.Q<Toggle>("ask-last-event-field").BindProperty(eventDatas.FindPropertyRelative("AskLastEventOnSubscribe"));

            if (_lambdas.TryGetValue(element, out Action[] lambdas))
            {
                element.Q<Button>("event-details-button").clicked -= lambdas[0];
                element.Q<Button>("remove-event-button").clicked -= lambdas[1];
            }
            else
            {
                lambdas = new Action[2];
            }

            lambdas[0] = () =>
            {
                eventDatas.isExpanded = !eventDatas.isExpanded;
                ShowEventDetails(element, eventDatas.isExpanded);
            };
            element.Q<Button>("event-details-button").clicked += lambdas[0];

            lambdas[1] = () => RemoveEventListened(arrayIndex);
            element.Q<Button>("remove-event-button").clicked += lambdas[1];

            _lambdas[element] = lambdas;

            element.Q<PropertyField>("event-responses-field").BindProperty(eventDatas.FindPropertyRelative("Response"));

            ShowEventDetails(element, eventDatas.isExpanded);
        }

        private void ShowEventDetails(VisualElement element, bool isExpanded)
        {
            element.Q<VisualElement>("event-listener-mode-container").style.display = isExpanded ? DisplayStyle.None : DisplayStyle.Flex;
        }

        private void RemoveEventListened(int arrayIndex)
        {
            SerializedProperty serializedEvent = _eventsDatas.GetArrayElementAtIndex(arrayIndex).FindPropertyRelative("Event");

            if (serializedEvent.objectReferenceValue != null)
            {
                //(serializedEvent.objectReferenceValue as BaseEvent).RemoveReferral(serializedObject.targetObject);
            }

            _eventsDatas.DeleteArrayElementAtIndex(arrayIndex);
            serializedObject.ApplyModifiedProperties();
        }
    }
}