using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Puzzle))]
public class PuzzleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Puzzle puzzle = (Puzzle)target;

        GUILayout.Space(20);
        GUI.backgroundColor = Color.cyan;

        if (GUILayout.Button("BAKE VISUALS (Scan Zone)", GUILayout.Height(40)))
        {
            Undo.RecordObject(puzzle, "Bake Puzzle Visuals");
            
            puzzle.visuals.ScanChildren(puzzle.transform);
            
            EditorUtility.SetDirty(puzzle);
        }
        
        GUI.backgroundColor = Color.white;
    }
}