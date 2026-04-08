using UnityEngine;
using UnityEditor;
using System.IO;

public class AnimationGenerator : EditorWindow
{
    private string animationName = "NewAnimation";
    private string folderPath = "Assets/Animations";
    private string suffix = "";
    private int sampleRate = 24;

    [MenuItem("Tools/Animation Generator (8-Directions)")]
    public static void ShowWindow()
    {
        GetWindow<AnimationGenerator>("Anim Gen");
    }

    private void OnGUI()
    {
        GUILayout.Label("Paramètres de l'animation", EditorStyles.boldLabel);
        
        animationName = EditorGUILayout.TextField("Nom de base", animationName);
        
        EditorGUILayout.Space();
        
        suffix = EditorGUILayout.TextField("Suffix (optionnel)", suffix);
        
        sampleRate = EditorGUILayout.IntField("Sample Rate", sampleRate);
        
        EditorGUILayout.Space();
        GUILayout.Label("Dossier de destination : " + folderPath);
        
        if (GUILayout.Button("Choisir le dossier"))
        {
            string path = EditorUtility.OpenFolderPanel("Choisir le dossier de destination", "Assets", "");
            if (!string.IsNullOrEmpty(path))
            {
                if (path.Contains(Application.dataPath))
                {
                    folderPath = "Assets" + path.Replace(Application.dataPath, "");
                }
            }
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Générer les 8 Directions", GUILayout.Height(40)))
        {
            GenerateAnimations();
        }
    }

    private void GenerateAnimations()
    {
        string[] directions = { "N", "S", "E", "W", "NE", "NW", "SE", "SW" };

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        foreach (string dir in directions)
        {
            AnimationClip animClip = new AnimationClip
            {
                frameRate = sampleRate
            };

            string fileName = "";
            
            if (suffix == "")
            {
                fileName = $"{animationName}_{dir}.anim";
            }
            else
            {
                fileName = $"{animationName}_{dir}_{suffix}.anim";
            }
            
            
            string fullPath = Path.Combine(folderPath, fileName);

            AssetDatabase.CreateAsset(animClip, fullPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        EditorUtility.DisplayDialog("Succès", $"Les 8 animations pour '{animationName}' ont été créées !", "Super !");
    }
}
