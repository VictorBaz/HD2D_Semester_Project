using UnityEditor;
using UnityEngine;
using System;

[CustomEditor(typeof(VATManager), true)]
public class VATManagerEditor : Editor
{
    private GUIStyle _tickLabelStyle;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("Visual Timeline", EditorStyles.boldLabel);
        EditorGUILayout.Space(15);

        SerializedProperty stepsProp = serializedObject.FindProperty("animationSteps");

        CheckErrors(stepsProp);
        DrawTimeline(stepsProp);

        EditorGUILayout.Space(10);
        DrawDefaultInspector();

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawTimeline(SerializedProperty stepsProp)
    {
        if (_tickLabelStyle == null)
        {
            _tickLabelStyle = new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = Color.white } };
        }

        Rect rect = GUILayoutUtility.GetRect(10, 30, GUILayout.ExpandWidth(true));
        EditorGUI.DrawRect(rect, new Color(0.15f, 0.15f, 0.15f));

        for (int i = 0; i < stepsProp.arraySize; i++)
        {
            float val = stepsProp.GetArrayElementAtIndex(i).floatValue;
            float posX = rect.x + (val * rect.width);

            Rect tickRect = new Rect(posX - 1, rect.y, 2, rect.height);
            EditorGUI.DrawRect(tickRect, Color.cyan);

            string label = val.ToString("g2");
            EditorGUI.LabelField(new Rect(posX - 5, rect.y - 15, 40, 15), label, _tickLabelStyle);
        }

        Handles.DrawSolidRectangleWithOutline(rect, Color.clear, Color.gray);
    }

    private void CheckErrors(SerializedProperty stepsProp)
    {
        int size = stepsProp.arraySize;
        if (size == 0) return;

        if (!Mathf.Approximately(stepsProp.GetArrayElementAtIndex(0).floatValue, 0f))
        {
            EditorGUILayout.HelpBox("Le premier élément doit être égal à 0", MessageType.Error);
        }

        if (!Mathf.Approximately(stepsProp.GetArrayElementAtIndex(size - 1).floatValue, 1f))
        {
            EditorGUILayout.HelpBox("Le dernier élément doit être égal à 1", MessageType.Error);
        }

        bool outOfBounds = false;
        for (int i = 0; i < size; i++)
        {
            float val = stepsProp.GetArrayElementAtIndex(i).floatValue;
            if (val < 0f || val > 1f)
            {
                outOfBounds = true;
                break;
            }
        }

        if (outOfBounds)
        {
            EditorGUILayout.HelpBox("Une de vos valeurs est en dehors de la portée [0, 1]", MessageType.Error);
        }

        EditorGUILayout.Space(5);
    }
}