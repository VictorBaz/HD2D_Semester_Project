using UnityEditor;
using UnityEngine;
using Enum;

[CustomEditor(typeof(CameraTrigger))] 
public class CameraTriggerEditor : Editor 
{
    private int nodeIndex = 0;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        SerializedProperty settings = serializedObject.FindProperty("settings");
        SerializedProperty onlyOnceProp = serializedObject.FindProperty("triggerOnlyOnce");

        SerializedProperty stateProp = settings.FindPropertyRelative("CameraPlayerState");
        SerializedProperty posProp = settings.FindPropertyRelative("CameraPosition");
        SerializedProperty railProp = settings.FindPropertyRelative("ActiveRail");
        SerializedProperty smoothProp = settings.FindPropertyRelative("transitionSmoothTime");
        SerializedProperty holdProp = settings.FindPropertyRelative("holdDuration");
        SerializedProperty targetCinematicProp = settings.FindPropertyRelative("targetCinematic");

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField(" CONFIGURATION CAMÉRA", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        if (onlyOnceProp != null) 
        {
            EditorGUILayout.PropertyField(onlyOnceProp, new GUIContent("Usage Unique", "Si coché, ne se déclenchera qu'une fois par partie."));
            EditorGUILayout.Space(5);
        }

        EditorGUILayout.PropertyField(stateProp);
        EditorGUILayout.PropertyField(smoothProp);
        
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        CameraPlayerState state = (CameraPlayerState)stateProp.enumValueIndex;

        switch (state)
        {
            case CameraPlayerState.Fix:
            case CameraPlayerState.Cinematic:
                EditorGUILayout.PropertyField(posProp, new GUIContent("Position de Vue"));
                if (state == CameraPlayerState.Cinematic)
                {
                    EditorGUILayout.PropertyField(holdProp, new GUIContent("Durée (sec)"));
                    EditorGUILayout.PropertyField(targetCinematicProp, new GUIContent("Target Cinematic"));
                }
                
                DrawTPButton(posProp.vector3Value);
                break;

            case CameraPlayerState.Rail:
                EditorGUILayout.PropertyField(railProp);
                Rail rail = railProp.objectReferenceValue as Rail;
                
                if (rail != null)
                {
                    DrawRailTools(rail);
                }
                break;

            case CameraPlayerState.FollowPlayer:
                EditorGUILayout.HelpBox("Ce trigger ramènera la caméra au mode Follow classique.", MessageType.Info);
                break;
        }

        EditorGUILayout.Space(5);
        EditorGUILayout.EndVertical();
        
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawTPButton(Vector3 targetPos)
    {
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();
        
        GUI.color = Color.cyan;
        if (GUILayout.Button("TP Caméra ici", GUILayout.Height(30)))
        {
            Transform camRoot = Camera.main.transform.parent != null ? Camera.main.transform.parent : Camera.main.transform;
            Undo.RecordObject(camRoot, "Move Camera");
            camRoot.position = targetPos;
        }

        GUI.color = Color.white;
        if (GUILayout.Button("Reset Pos", GUILayout.Height(30), GUILayout.Width(80)))
        {
            CameraManager manager = FindFirstObjectByType<CameraManager>();
            if (manager != null)
            {
                Transform camRoot = Camera.main.transform.parent != null ? Camera.main.transform.parent : Camera.main.transform;
                Undo.RecordObject(camRoot, "Reset Camera Pos");
                Debug.Log("Position de la caméra réinitialisée visuellement.");
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawRailTools(Rail rail)
    {
        if (rail.Nodes == null || rail.Nodes.Length == 0) return;

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Outils de Prévisualisation Rail", EditorStyles.miniBoldLabel);
        
        nodeIndex = EditorGUILayout.IntSlider("Node Cible", nodeIndex, 0, rail.Nodes.Length - 1);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("< Précédent")) nodeIndex = Mathf.Clamp(nodeIndex - 1, 0, rail.Nodes.Length - 1);
        if (GUILayout.Button("Suivant >")) nodeIndex = Mathf.Clamp(nodeIndex + 1, 0, rail.Nodes.Length - 1);
        EditorGUILayout.EndHorizontal();

        GUI.color = Color.cyan;
        if (GUILayout.Button($"TP Camera au Node {nodeIndex}", GUILayout.Height(30)))
        {
            Transform camRoot = Camera.main.transform.parent != null ? Camera.main.transform.parent : Camera.main.transform;
            Undo.RecordObject(camRoot, "Snap Camera to Rail");
            camRoot.position = rail.Nodes[nodeIndex];
            
            if (nodeIndex < rail.Nodes.Length - 1)
                camRoot.LookAt(rail.Nodes[nodeIndex + 1]);
        }
        GUI.color = Color.white;
    }
}