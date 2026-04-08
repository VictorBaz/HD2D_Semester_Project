using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraTriggerRail))]
public class TriggerRailCameraEditor : Editor
{
    private int nodeIndex = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        CameraTriggerRail script = (CameraTriggerRail)target;

        if (script.RailToUse == null || script.RailToUse.Nodes == null || script.RailToUse.Nodes.Length == 0)
        {
            EditorGUILayout.HelpBox("Veuillez assigner un Rail avec des points pour activer les outils.", MessageType.Info);
            return;
        }

        int nodeCount = script.RailToUse.Nodes.Length;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Outils de Prévisualisation du Rail", EditorStyles.boldLabel);

        nodeIndex = EditorGUILayout.IntSlider("Node Cible", nodeIndex, 0, nodeCount - 1);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("< Précédent"))
        {
            nodeIndex = Mathf.Clamp(nodeIndex - 1, 0, nodeCount - 1);
        }
        if (GUILayout.Button("Suivant >"))
        {
            nodeIndex = Mathf.Clamp(nodeIndex + 1, 0, nodeCount - 1);
        }
        EditorGUILayout.EndHorizontal();

        GUI.color = Color.cyan;
        
        if (GUILayout.Button($"TP Main Camera au Node {nodeIndex}", GUILayout.Height(30)))
        {
            if (Camera.main.transform.parent != null)
            {
                Undo.RecordObject(Camera.main.transform.parent, "Snap Camera to Rail Node");
                Camera.main.transform.parent.position = script.RailToUse.Nodes[nodeIndex];
                
                if (nodeIndex < nodeCount - 1)
                {
                    Camera.main.transform.LookAt(script.RailToUse.Nodes[nodeIndex + 1]);
                }
            }
            else
            {
                Debug.LogWarning("Aucune MainCamera trouvée dans la scène !");
            }
        }
        GUI.color = Color.white;
    }
}