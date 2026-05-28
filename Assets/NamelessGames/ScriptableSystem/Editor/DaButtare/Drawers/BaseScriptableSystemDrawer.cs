using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseScriptableSystemDrawer_old : PropertyDrawer
{
    [SerializeField] protected VisualTreeAsset _drawerTemplate;
    protected SerializedProperty _property;
    protected VisualElement _root;
    protected ObjectField _field;
    protected TextField _newNameField;

    ~BaseScriptableSystemDrawer_old()
    {
        Debug.Log("Destroying " + _property.serializedObject.targetObject);
    }

    protected abstract string _path { get; }

    public sealed override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        _root = _drawerTemplate.CloneTree();
        _property = property;

        _field = _root.Q<ObjectField>();
        _field.BindProperty(property);
        _field.label = property.displayName;
        _field.objectType = fieldInfo.FieldType;
        _field.AddToClassList(ObjectField.alignedFieldUssClassName);

        _newNameField = _root.Q<TextField>();
        _newNameField.label = property.displayName;
        _newNameField.value = property.displayName.Replace(" ", "");
        _newNameField.AddToClassList(BaseField<TextField>.alignedFieldUssClassName);

        _root.Q<Button>("CreateButton").clicked += StartCreation;
        _root.Q<Button>("ConfirmButton").clicked += ConfirmCreation;
        _root.Q<Button>("CancelButton").clicked += CancelCreation;

        if (property.objectReferenceValue != null)
        {
            _root.Q<Button>("CreateButton").AddToClassList("hidden");
        }
        _field.RegisterValueChangedCallback(ObjectValueChanged);

        InitializeGUI();

        return _root;
    }

    protected abstract void InitializeGUI();

    private void ObjectValueChanged(ChangeEvent<Object> evt)
    {
        if (evt.newValue == null)
        {
            _root.Q<Button>("CreateButton").RemoveFromClassList("hidden");
        }
        else
        {
            _root.Q<Button>("CreateButton").AddToClassList("hidden");
        }
        ValueChanged(evt);
    }

    protected abstract void ValueChanged(ChangeEvent<Object> evt);

    private void StartCreation()
    {
        _field.AddToClassList("hidden");
        _root.Q<Button>("CreateButton").AddToClassList("hidden");

        _newNameField.RemoveFromClassList("hidden");
        _root.Q<Button>("CancelButton").RemoveFromClassList("hidden");
        _root.Q<Button>("ConfirmButton").RemoveFromClassList("hidden");
    }

    private void CancelCreation()
    {
        _newNameField.AddToClassList("hidden");
        _root.Q<Button>("CancelButton").AddToClassList("hidden");
        _field.RemoveFromClassList("hidden");
        _root.Q<Button>("CreateButton").RemoveFromClassList("hidden");
    }

    private void ConfirmCreation()
    {
        _root.Q<TextField>().AddToClassList("hidden");
        _root.Q<Button>("CancelButton").AddToClassList("hidden");
        _root.Q<Button>("ConfirmButton").AddToClassList("hidden");

        _field.RemoveFromClassList("hidden");

        if (string.IsNullOrEmpty(_newNameField.value) || string.IsNullOrWhiteSpace(_newNameField.value)
            || string.IsNullOrEmpty(_newNameField.value.Trim()) || _newNameField.value.Trim().Length < 2)
        {
            _root.Q<Button>("CreateButton").RemoveFromClassList("hidden");
            return;
        }

        CreateNewObject(_newNameField.value);
    }

    protected void CreateNewObject(string newEventName)
    {
        ScriptableObject newEvent = ScriptableObject.CreateInstance(fieldInfo.FieldType);
        if (!Directory.Exists(_path)) Directory.CreateDirectory(_path);

        int nameIndex = 1;
        while (File.Exists(_path + newEventName + ".asset"))
        {
            string suffix = nameIndex.ToString("00");
            newEventName = _path + suffix;
            nameIndex++;
        }

        AssetDatabase.CreateAsset(newEvent, _path + newEventName + ".asset");
        _field.value = newEvent;

        Undo.RegisterCreatedObjectUndo(newEvent, newEventName + " event created");
        Undo.RecordObject(_property.serializedObject.targetObject, newEventName + " assigned to " + _property.serializedObject.targetObject.name);
    }

    protected void CheckPath(string path)
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);
    }

    protected void GetFileName(string path, string fileName)
    {
        int nameIndex = 1;
        while (File.Exists(path + fileName + ".asset"))
        {
            string suffix = nameIndex.ToString("00");
            fileName = path + suffix;
            nameIndex++;
        }
    }

    protected void SaveNewObjectAndAssign(Object newObject, string path, string fileName)
    {
        AssetDatabase.CreateAsset(newObject, path + fileName + ".asset");
        _field.value = newObject;

        Undo.RegisterCreatedObjectUndo(newObject, fileName + " created");
        Undo.RecordObject(_property.serializedObject.targetObject, fileName + " assigned to " + _property.serializedObject.targetObject.name);

    }
}