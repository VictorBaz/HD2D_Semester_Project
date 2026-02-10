using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

public class ToolsEditorGrid : EditorWindow
{

    #region Variables

    private GameObject cellGameObject;
    
    private GameObject[] assets;
    private string[] names;
    private int index = -1;
    
    private Vector3 origin = Vector3.zero;
    
    private readonly string pathPrefabs = "Assets/Prefabs/Cell";
    
    List<GameObject> objs = new List<GameObject>();

    private float offset = 1f;
    private int linecount = 100;

    #endregion

    #region Show in Menu

    [MenuItem("Tools/Tools Editor Grid")]
    public static void ShowWindow()
    {
        GetWindow<ToolsEditorGrid>("Tools Editor Grid");
    }

    #endregion

    #region Display

    private void OnEnable()
    {
        GetObjs();
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI()
    {
        index = EditorGUILayout.Popup("Prefab", index, names);

        if (GUILayout.Button("Refresh"))
        {
            GetObjs();
        }
        
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        DrawXZGrid(sceneView);
    }

    private void GetObjs()
    {
        objs.Clear();
        
        assets = AssetDatabase.FindAssets("t:Prefab", new[] { pathPrefabs })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(p => System.IO.Path.GetDirectoryName(p)?.Replace("\\", "/") == pathPrefabs)
            .Select(AssetDatabase.LoadAssetAtPath<GameObject>)
            .ToArray();

        names = assets.Select(a => a.name).ToArray();
    }
    #endregion
    
    #region Handles Methods

    
    
    private void DrawXZGrid(SceneView sceneView)
    {
        Handles.color = Color.green;
        float extent = linecount * offset;

        for (int i = -linecount; i <= linecount; i++)
        {
            float p = i * offset;
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            Handles.DrawLine(new Vector3(p, 0, -extent), new Vector3(p, 0, extent));
            Handles.DrawLine(new Vector3(-extent, 0, p), new Vector3(extent, 0, p));
        }
    }

    #endregion
}
