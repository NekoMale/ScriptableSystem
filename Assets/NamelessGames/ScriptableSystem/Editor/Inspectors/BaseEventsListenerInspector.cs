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
    class BaseEventsListenerInspector : Editor
    {
        [SerializeField] VisualTreeAsset _inspectorTemplate;
        [SerializeField] VisualTreeAsset _eventDataTemplate;

        SerializedProperty _eventsDatas;
        VisualElement _root;

        public override VisualElement CreateInspectorGUI()
        {
            _root = _inspectorTemplate.CloneTree();
            _eventsDatas = serializedObject.FindProperty("_baseEventsDatas");

            _root.Q<ListView>().bindItem += BindItem;
            _root.Q<ListView>().Q<Button>("unity-list-view__add-button").text = "+ Add Event";

            return _root;
        }

        private void BindItem(VisualElement element, int index)
        {
            SerializedProperty eventData = _eventsDatas.GetArrayElementAtIndex(index);
            SerializedProperty eventElement = eventData.FindPropertyRelative("Event");

            element.Q<Foldout>("event-data-item").userData = index;
            element.Q<Foldout>("event-data-item").value = eventData.isExpanded;
            element.Q<Foldout>("event-data-item").RegisterValueChangedCallback(ChangeIsExpanded);
            
            Toggle headerToggle = element.Q<Toggle>();

            ChangeElementName(headerToggle.Q<Label>(), eventElement);

            Button removeButton = new Button();
            removeButton.AddToClassList("event-data-remove-button");
            removeButton.Add(new VisualElement());
            headerToggle.Add(removeButton);
            removeButton.RegisterCallback<ClickEvent>(RemoveEventData);


            element.Q<PropertyField>("event-property-field").BindProperty(eventData.FindPropertyRelative("Event"));
            element.Q<PropertyField>("event-property-field").RegisterValueChangeCallback(ChangeElementName);

            element.Q<Toggle>("subscribe-slide-toggle-field").BindProperty(eventData.FindPropertyRelative("SubscribeOnEnable"));
            element.Q<Toggle>("unsubscribe-slide-toggle-field").BindProperty(eventData.FindPropertyRelative("UnsubscribeOnDisable"));
            element.Q<Toggle>("ask-last-event-slide-toggle-field").BindProperty(eventData.FindPropertyRelative("AskLastEventOnSubscribe"));
            
            element.Q<PropertyField>("response-property-field").BindProperty(eventData.FindPropertyRelative("Response"));
        }

        private void RemoveEventData(ClickEvent evt)
        {
            Foldout foldout = (evt.target as VisualElement).parent.parent as Foldout;
            int indexToRemove = (int)foldout.userData;

            _eventsDatas.DeleteArrayElementAtIndex(indexToRemove);
            serializedObject.ApplyModifiedProperties();
        }

        private void ChangeElementName(SerializedPropertyChangeEvent evt)
        {
            Foldout foldout = (evt.target as VisualElement).parent as Foldout;
            ChangeElementName(foldout.Q<Toggle>().Q<Label>(), evt.changedProperty);
        }

        private void ChangeElementName(Label label, SerializedProperty property)
        {
            if (property.objectReferenceValue == null)
            {
                label.text = "No event assigned";
                return;
            }

            label.BindProperty(new SerializedObject(property.objectReferenceValue).FindProperty("m_Name"));
        }

        private void ChangeIsExpanded(ChangeEvent<bool> evt)
        {
            Foldout foldout = evt.target as Foldout;
            
            if (foldout == null) return;
            
            SerializedProperty eventData = _eventsDatas.GetArrayElementAtIndex((int)foldout.userData);

            eventData.isExpanded = evt.newValue;
        }
    }
}
