using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace NamelessGames.ScriptableSystem.ScriptableSystemEditor
{
    public class VariableDetailViewer : NGSODetailViewer
    {
        public VariableDetailViewer(VisualTreeAsset template, VisualElement root) : base(template, root) { }

        protected override void OnBind()
        {
            TabView tabView = _root.Q<TabView>("selected-item-detail-tab-view");

            PropertyField valueChangedField = tabView.Q<PropertyField>("selected-variable-empty-event-property");
            valueChangedField.UnregisterCallback<SerializedPropertyChangeEvent>(VariableEventAssigned);
            valueChangedField.RegisterCallback<SerializedPropertyChangeEvent>(VariableEventAssigned);

            tabView.Q<Button>("edit-button").RegisterCallback<ClickEvent>(EditValueChangedEvent);
        }

        private void VariableEventAssigned(SerializedPropertyChangeEvent evt)
        {
            PropertyField eventPropertyField = evt.target as PropertyField;

            BindEvent(eventPropertyField.parent.Q<VisualElement>("selected-variable-change-event-assigned"), eventPropertyField, evt.changedProperty);
        }

        private static void BindEvent(VisualElement changeEventElement, PropertyField valueChangedField, SerializedProperty onChangeEventSP)
        {
            if (onChangeEventSP.objectReferenceValue == null)
            {
                changeEventElement.style.display = DisplayStyle.None;
                valueChangedField.style.display = DisplayStyle.Flex;
                changeEventElement.Unbind();
                Debug.Log("No event assigned");
            }
            else
            {
                changeEventElement.style.display = DisplayStyle.Flex;
                valueChangedField.style.display = DisplayStyle.None;

                SerializedObject onChangeEventSO = new SerializedObject(onChangeEventSP.objectReferenceValue);
                changeEventElement.Bind(onChangeEventSO);
                changeEventElement.Q<Label>("event-type-label").text = onChangeEventSO.targetObject.GetType().Name;
            }
        }

        private void EditValueChangedEvent(ClickEvent evt)
        {
            TabView tabView = _root.Query<TabView>("selected-item-detail-tab-view");
            Tab tab = tabView.Query<Tab>().Where(x => x.tabIndex == tabView.selectedTabIndex);

            VisualElement changeEventElement = tab.Q<VisualElement>("selected-variable-change-event-assigned");

            changeEventElement.style.display = DisplayStyle.None;
            tab.Q<PropertyField>("selected-variable-empty-event-property").style.display = DisplayStyle.Flex;
        }
    }
}
