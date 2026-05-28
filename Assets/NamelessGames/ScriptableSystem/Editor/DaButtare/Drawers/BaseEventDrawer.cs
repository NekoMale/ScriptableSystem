using NamelessGames.ScriptableSystem.Events;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NamelessGames.ScriptableSystem.ScriptableSystemEditor
{
    //[CustomPropertyDrawer(typeof(BaseEvent<>), true)]
    public class BaseEventDrawer_old : BaseScriptableSystemDrawer_old
    {
        protected override string _path
        {
            get => "Assets/ScriptableObjects/Events/";
        }

        protected override void InitializeGUI()
        {
            _field.BindProperty(_property);

            if (_property.objectReferenceValue == null)
            {
                _root.Q<Button>("view-event-button").AddToClassList("hidden");
                return;
            }
            _root.Q<Button>("view-event-button").RemoveFromClassList("hidden");
            _root.Q<Button>("view-event-button").clicked += () => EventsViewer.Init(_property.objectReferenceValue as BaseEvent);

            //(_property.objectReferenceValue as BaseEvent).AddReferral(_property.serializedObject.targetObject);
        }

        protected override void ValueChanged(ChangeEvent<Object> evt)
        {
            BaseEvent oldEvent = evt.previousValue as BaseEvent;
            BaseEvent newEvent = evt.newValue as BaseEvent;

            if (oldEvent == newEvent) return;

            if (oldEvent != null)
            {
                //oldEvent.RemoveReferral(_property.serializedObject.targetObject);
            }

            if (newEvent == null) return;

            //newEvent.AddReferral(_property.serializedObject.targetObject);
        }
    }
}