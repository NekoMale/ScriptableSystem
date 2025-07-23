using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(BaseVariable<,>), true)]
public class BaseVariableDrawer : BaseScriptableSystemDrawer
{
    SerializedObject _variableObject;

    protected override string _path => "Assets/ScriptableObjects/Variables/";

    protected override void InitializeGUI()
    {
        _root.Q<Foldout>("BaseVariableFoldout").Q<Toggle>().Q<VisualElement>(className: "unity-foldout__input").style.flexGrow = 0;
        _root.Q<Foldout>("BaseVariableFoldout").Q<Toggle>().Add(_root.Q("PropertyTopContainer"));

        if (_property.objectReferenceValue != null)
        {
            _variableObject = new SerializedObject(_property.objectReferenceValue);

            _root.Q<PropertyField>("StartingValue").BindProperty(_variableObject.FindProperty("_startingValue"));
            _root.Q<PropertyField>("CurrentValue").BindProperty(_variableObject.FindProperty("_value"));
            _root.Q<PropertyField>("ValueChangedEvent").BindProperty(_variableObject.FindProperty("ValueChanged"));
        }
        else
        {
            _root.Q<VisualElement>("unity-checkmark").AddToClassList("hidden");
            _root.Q<VisualElement>("PropertyBottomContainer").AddToClassList("hidden");
        }
    }

    protected override void ValueChanged(ChangeEvent<Object> evt)
    {
        if (evt.newValue == null)
        {
            _root.Q<VisualElement>("unity-checkmark").AddToClassList("hidden");
            _root.Q<VisualElement>("PropertyBottomContainer").AddToClassList("hidden");

            _root.Q<PropertyField>("StartingValue").Unbind();
            _root.Q<PropertyField>("CurrentValue").Unbind();
            _root.Q<PropertyField>("ValueChangedEvent").Unbind();

        }
    }
}
