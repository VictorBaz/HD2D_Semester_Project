using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Root))]
public class RootEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Root rootScript = (Root)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("ROOT VISUAL GENERATION", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginHorizontal();

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("BAKE VISUALS", GUILayout.Height(35)))
        {
            Undo.RecordObject(rootScript, "Bake Root Visuals");
            rootScript.BakeVisuals();
        }

        GUI.backgroundColor = new Color(1f, 0.4f, 0.4f); 
        if (GUILayout.Button("CLEAR ALL", GUILayout.Height(35)))
        {
            Undo.RecordObject(rootScript, "Clear Root Visuals");
            rootScript.ClearAllKnots();
        }

        EditorGUILayout.EndHorizontal();
        
        GUI.backgroundColor = Color.white;
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); 
        EditorGUILayout.Space();

        DrawDefaultInspector();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(rootScript);
        }
    }
}