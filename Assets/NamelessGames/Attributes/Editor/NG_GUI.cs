using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class NG_GUI
{
    public static readonly GUIStyle header = new GUIStyle("RL Header");
    public static readonly GUIStyle centeredLabel = new GUIStyle() { alignment = TextAnchor.MiddleCenter };
    public static readonly GUIStyle centeredSubHeader = new GUIStyle() { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };

    static NG_GUI()
    {
        header.padding.top = 3;
        header.fontStyle = FontStyle.Bold;
        header.fontSize = 12;
        centeredSubHeader.normal.textColor = new Color(0.68f, 0.68f, 0.68f);
    }

    public static void VerticalSeparator()
    {
        Rect scrollRect = EditorGUILayout.BeginVertical(GUILayout.Width(2));
        EditorGUILayout.GetControlRect(GUILayout.Width(2), GUILayout.Height(Screen.height - 25));

        EditorGUI.DrawRect(scrollRect, NG_COLOR.UnityBackgroundColor);
        EditorGUILayout.EndVertical();
    }

    public static int GetLocalID(Object obj)
    {
        PropertyInfo inspectorModeInfo = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);

        SerializedObject serializedObject = new SerializedObject(obj);
        inspectorModeInfo.SetValue(serializedObject, InspectorMode.Debug, null);

        SerializedProperty localIdProp = serializedObject.FindProperty("m_LocalIdentfierInFile");   //note the misspelling!

        return localIdProp.intValue;
    }

    public static void DrawRowAndColumns(ref Rect[] columnRects)
    {
        Rect rowRect = EditorGUILayout.BeginHorizontal();
        EditorGUI.DrawRect(rowRect, NG_COLOR.UnityBackgroundColor);

        for (int columnIndex = 0; columnIndex < columnRects.Length; columnIndex++)
        {
            columnRects[columnIndex] = EditorGUILayout.GetControlRect();
            if (columnIndex % 2 == 0)
            {
                columnRects[columnIndex].x += 1;
                columnRects[columnIndex].width += 1;
            }
            EditorGUI.DrawRect(columnRects[columnIndex], NG_COLOR.MediumGray);

            columnRects[columnIndex].x += 2;
            columnRects[columnIndex].width -= 4;
        }
        EditorGUILayout.EndHorizontal();
    }
}