using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraTriggerBase), true)]
public class TriggerCameraEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SerializedProperty camPosProp = serializedObject.FindProperty("cameraPosition");

        if (camPosProp != null)
        {
            EditorGUILayout.Space();
            GUI.color = Color.cyan;

            if (GUILayout.Button("Place Main Camera at Position", GUILayout.Height(30)))
            {
                if (Camera.main.transform.parent != null)
                {
                    Undo.RecordObject(Camera.main.transform.parent, "Move Main Camera");
                    Camera.main.transform.parent.position = camPosProp.vector3Value;
                }
                else
                {
                    Debug.LogWarning("Aucune caméra tagguée 'MainCamera' n'a été trouvée !");
                }
            }
            
            GUI.color = Color.white;
        }
    }
}