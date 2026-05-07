using UnityEditor;
using UnityEngine;
using Enum;

[CustomEditor(typeof(CameraTrigger))] 
public class CameraTriggerEditor : Editor 
{
    private int nodeIndex = 0;

    public override void OnInspectorGUI()
    {
        if (target == null) return;

        serializedObject.Update();
        
        SerializedProperty settings = serializedObject.FindProperty("settings");
        if (settings == null)
        {
            EditorGUILayout.HelpBox("Erreur: Propriété 'settings' introuvable dans CameraTrigger.", MessageType.Error);
            return;
        }

        SerializedProperty onlyOnceProp = serializedObject.FindProperty("triggerOnlyOnce");
        SerializedProperty stateProp = settings.FindPropertyRelative("CameraPlayerState");
        SerializedProperty camTransProp = settings.FindPropertyRelative("CameraTargetTransform"); 
        SerializedProperty railProp = settings.FindPropertyRelative("ActiveRail");
        SerializedProperty smoothProp = settings.FindPropertyRelative("transitionSmoothTime");
        SerializedProperty holdProp = settings.FindPropertyRelative("holdDuration");
        SerializedProperty targetCinematicProp = settings.FindPropertyRelative("targetCinematic");

        if (stateProp == null)
        {
            EditorGUILayout.HelpBox("Erreur: 'CameraPlayerState' introuvable dans CameraSettings.", MessageType.Error);
            return;
        }

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField(" CONFIGURATION CAMÉRA", EditorStyles.boldLabel);
        
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        
        if (onlyOnceProp != null) 
        {
            EditorGUILayout.PropertyField(onlyOnceProp, new GUIContent("Usage Unique", "Si coché, ne se déclenchera qu'une fois par partie."));
            EditorGUILayout.Space(5);
        }

        if (smoothProp != null) EditorGUILayout.PropertyField(smoothProp);
        EditorGUILayout.PropertyField(stateProp);
        
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        CameraPlayerState state = (CameraPlayerState)stateProp.enumValueIndex;

        switch (state)
        {
            case CameraPlayerState.Fix:
            case CameraPlayerState.Cinematic:
                if (camTransProp != null)
                {
                    EditorGUILayout.PropertyField(camTransProp, new GUIContent("Point de Vue (Transform)"));
                    
                    if (state == CameraPlayerState.Cinematic)
                    {
                        if (holdProp != null) EditorGUILayout.PropertyField(holdProp, new GUIContent("Durée (sec)"));
                        if (targetCinematicProp != null) EditorGUILayout.PropertyField(targetCinematicProp, new GUIContent("Cible Regardée"));
                    }

                    if (camTransProp.objectReferenceValue != null)
                    {
                        Transform t = (Transform)camTransProp.objectReferenceValue;
                        DrawTpButton(t.position);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Assignez un Transform pour définir la position de la caméra.", MessageType.Warning);
                        if (GUILayout.Button("Créer et assigner un point de vue"))
                        {
                            CreateAndAssignPoint(camTransProp);
                        }
                    }
                }
                break;

            case CameraPlayerState.Rail:
                if (railProp != null)
                {
                    EditorGUILayout.PropertyField(railProp);
                    Rail rail = railProp.objectReferenceValue as Rail;
                    if (rail != null) DrawRailTools(rail);
                    else EditorGUILayout.HelpBox("Veuillez assigner un script Rail.", MessageType.Warning);
                }
                break;

            case CameraPlayerState.FollowPlayer:
                EditorGUILayout.HelpBox("Retour au mode Follow classique (Offset Manager).", MessageType.Info);
                break;
        }

        EditorGUILayout.Space(5);
        EditorGUILayout.EndVertical();
        
        serializedObject.ApplyModifiedProperties();
    }

    private void CreateAndAssignPoint(SerializedProperty prop)
    {
        GameObject newPoint = new GameObject("CamPos_" + target.name);
        Undo.RegisterCreatedObjectUndo(newPoint, "Create Cam Point"); 
        
        CameraTrigger script = (CameraTrigger)target;
        newPoint.transform.SetParent(script.transform);
        newPoint.transform.localPosition = new Vector3(0, 5, -10);
        
        prop.objectReferenceValue = newPoint.transform;
    }

    private void DrawTpButton(Vector3 targetPos)
    {
        if (Camera.main == null) return;

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
            Debug.Log("Reset visuel effectué.");
        }
        EditorGUILayout.EndHorizontal();
    }

    private void DrawRailTools(Rail rail)
    {
        if (rail == null || rail.Nodes == null || rail.Nodes.Length == 0) return;

        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Outils de Prévisualisation Rail", EditorStyles.miniBoldLabel);
        nodeIndex = EditorGUILayout.IntSlider("Node Cible", nodeIndex, 0, rail.Nodes.Length - 1);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("< Précédent")) nodeIndex = Mathf.Clamp(nodeIndex - 1, 0, rail.Nodes.Length - 1);
        if (GUILayout.Button("Suivant >")) nodeIndex = Mathf.Clamp(nodeIndex + 1, 0, rail.Nodes.Length - 1);
        EditorGUILayout.EndHorizontal();

        if (Camera.main != null)
        {
            GUI.color = Color.cyan;
            if (GUILayout.Button($"TP Camera au Node {nodeIndex}", GUILayout.Height(30)))
            {
                Transform camRoot = Camera.main.transform.parent != null ? Camera.main.transform.parent : Camera.main.transform;
                Undo.RecordObject(camRoot, "Snap Camera to Rail");
                camRoot.position = rail.Nodes[nodeIndex];
            }
            GUI.color = Color.white;
        }
    }
}