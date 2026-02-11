using System;
using System.Linq;
using Grid;
using UnityEditor;
using UnityEngine;

public class ToolsEditorGrid : EditorWindow
{
    #region Variables

    private const string PREFABS_PATH = "Assets/Prefabs/Cell";
    
    private float gridCellSize = 1f;
    private int gridLineCount = 100;
    private int floorCount = 0;
    
    private GameObject[] availablePrefabs;
    private int selectedPrefabIndex = -1;
    
    private Vector2 scrollPosition;
    
    private bool placementMode = false;
    private Vector3Int previewGridPosition;
    private bool isValidPlacement = false;
    
    private GameObject gameObjectSelected;
    private bool snappingGameobjectSelected = false;
    
    private Vector3 lastKnownPosition;

    #endregion

    #region Window Management

    [MenuItem("Tools/Grid Editor")]
    public static void ShowWindow()
    {
        GetWindow<ToolsEditorGrid>("Grid Editor");
    }

    private void OnEnable()
    {
        LoadPrefabs();
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    #endregion

    #region GUI

    /// <summary>
    /// -Alors dans l'ordre création d'un label (title).
    /// -Puis mise en place d'un bouton permettant de refresh le contenu de la palette de prefabs au cas où
    /// on rajoute un élément dans le dossier.
    /// - On affiche la palette que je vais un peu plus préciser en dessous.
    /// - Mise en place de la grid settings comportant un floatField et un intField qui nous permettent
    /// de récupérer ainsi depuis l'inspecteur unity des valeurs et de les réinjecter dans mes deux
    /// variables.
    /// </summary>
    private void OnGUI()
    {
        EditorGUILayout.Space(10);
        
        EditorGUILayout.LabelField("Prefab Palette", EditorStyles.boldLabel);
        
        if (GUILayout.Button("Refresh Prefabs", GUILayout.Height(25)))
        {
            LoadPrefabs();
        }
        
        EditorGUILayout.Space(5);
        
        
        EditorGUILayout.LabelField("Placement", EditorStyles.boldLabel);
        placementMode = EditorGUILayout.Toggle("Placement Mode", placementMode);
        
        
        EditorGUILayout.Space(10);
        
        EditorGUILayout.LabelField("Grid Settings", EditorStyles.boldLabel);
        gridCellSize = EditorGUILayout.FloatField("Cell Size", gridCellSize);
        gridLineCount = EditorGUILayout.IntField("Grid Extent", gridLineCount);
        floorCount = EditorGUILayout.IntField("Grid Floor", floorCount);
        
        EditorGUILayout.Space(10);
        
        DrawPrefabPalette();
        EditSelectedGameObject();
        
    }
    
    /// <summary>
    /// -Dans l'ordre si pas de prefabs ou array vide on utilise HelpBox qui est une petite fenêtre en bas de
    /// notre tools qui permet de notifier l'utilisateur.
    /// -Puis utilisation de BeginScrollView qui concrètement nous permet de scroll dans la palette.
    /// -Par la suite on itère sur tous les prefabs disponibles en créant 3 boutons par ligne (avec BeginHorizontal
    /// tous les 3 éléments).
    /// -Enfin on vérifie si l'index est bien supérieur à -1 ce qui correspond qu'on a bien sélectionné un
    /// item puis si l'index est bien inférieur au nombre de prefabs disponibles puis on affiche le nom de
    /// l'asset sélectionné.
    /// </summary>
    private void DrawPrefabPalette()
    {
        if (availablePrefabs == null || availablePrefabs.Length == 0)
        {
            EditorGUILayout.HelpBox("No prefabs found in " + PREFABS_PATH, MessageType.Warning);
            return;
        }

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
        
        for (int i = 0; i < availablePrefabs.Length; i++)
        {
            if (i % 3 == 0) EditorGUILayout.BeginHorizontal();
            
            DrawPrefabButton(i);
            
            if (i % 3 == 2 || i == availablePrefabs.Length - 1)
                EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.EndScrollView();
        
        if (selectedPrefabIndex >= 0 && selectedPrefabIndex < availablePrefabs.Length)
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField($"Selected: {availablePrefabs[selectedPrefabIndex].name}");
        }
        
        
    }
    
    /// <summary>
    /// On récupère le gameobject puis créons une texture 2D comprenant le visuel de l'asset.
    /// Puis on dessine le bouton avec le nom du prefab (entre crochets si sélectionné).
    /// Le isSelected est vérifié en comparant l'index de l'objet sélectionné et celui sur lequel on itère.
    /// </summary>
    /// <param name="index"></param>
    private void DrawPrefabButton(int index)
    {
        GameObject prefab = availablePrefabs[index];
        Texture2D preview = AssetPreview.GetAssetPreview(prefab);
        
        string buttonText = (index == selectedPrefabIndex) ? $"[{prefab.name}]" : prefab.name;
        GUIContent content = new GUIContent(preview, buttonText);
        
        if (GUILayout.Button(content, GUILayout.Width(80), GUILayout.Height(80)))
        {
            selectedPrefabIndex = index;
        }
    }

    #endregion

    #region Scene GUI
    

    private void OnSceneGUI(SceneView sceneView)
    {
        DrawGrid();
    
        if (placementMode && selectedPrefabIndex >= 0)
        {
            HandlePlacement(sceneView);
        }

        if (snappingGameobjectSelected && gameObjectSelected != null)
        {
            Vector3 actualPositionSelectedObject = gameObjectSelected.transform.position;

            if (lastKnownPosition == Vector3.zero)
            {
                lastKnownPosition = actualPositionSelectedObject;
                return; 
            }
            
            if (Vector3.Distance(actualPositionSelectedObject,lastKnownPosition) > 0.01f)
            {
                Vector3Int gridPosition = GridHelper.WorldToGrid(actualPositionSelectedObject, gridCellSize);
                Vector3 newPosition = GridHelper.GridToWorld(gridPosition, gridCellSize);
                Undo.RecordObject(gameObjectSelected.transform, "Snap to Grid");
                gameObjectSelected.transform.position = newPosition;
                lastKnownPosition = newPosition;
            }
        }
        
    }

    private void DrawGrid()
    {
        Handles.color = Color.green;
        float extent = gridLineCount * gridCellSize;

        for (int i = -gridLineCount; i <= gridLineCount; i++)
        {
            float position = i * gridCellSize;
            Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
            
            Handles.DrawLine(new Vector3(position, floorCount * gridCellSize, -extent),
                new Vector3(position, floorCount * gridCellSize, extent));
            Handles.DrawLine(new Vector3(-extent, floorCount * gridCellSize, position),
                new Vector3(extent, floorCount * gridCellSize, position));
        }
    }

    #endregion

    #region Prefab Loading

    private void LoadPrefabs()
    {
        availablePrefabs = AssetDatabase.FindAssets("t:Prefab", new[] { PREFABS_PATH })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(path => System.IO.Path.GetDirectoryName(path)?.Replace("\\", "/") == PREFABS_PATH)
            .Select(AssetDatabase.LoadAssetAtPath<GameObject>)
            .Where(prefab => prefab != null)
            .ToArray();
        
        if (selectedPrefabIndex >= availablePrefabs.Length)
        {
            selectedPrefabIndex = -1;
        }
    }

    #endregion

    private void HandlePlacement(SceneView sceneView)
    {
        Event e = Event.current;
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, floorCount * gridCellSize, 0f));
        
        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPos = ray.GetPoint(distance);
            
            worldPos = new Vector3(worldPos.x, floorCount * gridCellSize, worldPos.z);
            
            previewGridPosition = GridHelper.WorldToGrid(worldPos,gridCellSize);
            isValidPlacement = true;
            
            DrawPlacementPreview();
            
            if (e.type == EventType.MouseDown && e.button == 0)
            {
                PlacePrefab();
                e.Use(); 
            }
            
            sceneView.Repaint();
        }
        
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
    }
    
    private void DrawPlacementPreview()
    {
        Vector3 worldPos = GridHelper.GridToWorld(previewGridPosition,gridCellSize);
            
        Handles.color = isValidPlacement ? new Color(0, 1, 0, 0.1f) : new Color(1, 0, 0, 0.1f);
        Handles.CubeHandleCap(0, worldPos, Quaternion.identity, gridCellSize * 0.9f, EventType.Repaint);
    }

    private void PlacePrefab()
    {
        if (selectedPrefabIndex < 0 || selectedPrefabIndex >= availablePrefabs.Length)
            return;
        
        GameObject prefab = availablePrefabs[selectedPrefabIndex];
        Vector3 worldPos = GridHelper.GridToWorld(previewGridPosition,gridCellSize);
        
        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        instance.transform.position = worldPos;
        
        Undo.RegisterCreatedObjectUndo(instance, "Place Prefab");
    }
    
    private void EditSelectedGameObject()
    {
        gameObjectSelected = Selection.activeGameObject;
    
        if (gameObjectSelected == null) return;
        
        snappingGameobjectSelected = EditorGUILayout.Toggle("Snapping Mode", snappingGameobjectSelected);
    
        if (GUILayout.Button("Rotate 90°", GUILayout.Height(25)))
        {
            gameObjectSelected.transform.Rotate(90, PuzzleHelpers.RotationAxis.Y);
        }
        
        
    }
    
}