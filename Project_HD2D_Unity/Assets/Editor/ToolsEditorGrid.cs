using System;
using UnityEditor;
using UnityEngine;

public class ToolsEditorGrid : EditorWindow
{
    #region Variables

    private float gridCellSize = 1f;
    private int gridLineCount = 100;
    private int floorCount = 0;
    
    private GameObject[] availablePrefabs;
    private int selectedPrefabIndex = -1;
    
    private Vector2 scrollPosition;
    
    private bool placementMode = false;
    
    private GameObject gameObjectSelected;
    private bool snappingGameobjectSelected = false;
    
    private Vector3 lastKnownPosition;
    private float gridOpacity = 1f;

    #endregion

    #region Handlers

    private PrefabLoader prefabLoader = new PrefabLoader();
    private GridRenderer gridRenderer = new GridRenderer();
    private PrefabPaletteDrawer paletteDrawer = new PrefabPaletteDrawer();
    private PlacementHandler placementHandler = new PlacementHandler();
    private SnappingHandler snappingHandler = new SnappingHandler();

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

    private void OnGUI()
    {
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Placement", EditorStyles.boldLabel);
        placementMode = EditorGUILayout.Toggle("Placement Mode", placementMode);
        
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Grid Settings", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();
        
        gridCellSize = EditorGUILayout.FloatField("Cell Size", gridCellSize);
        gridLineCount = EditorGUILayout.IntField("Grid Extent", gridLineCount);
        floorCount = EditorGUILayout.IntField("Grid Floor", floorCount);
        gridOpacity = EditorGUILayout.Slider("Grid Opacity", gridOpacity, 0f, 1f);

        if (EditorGUI.EndChangeCheck())
        {
            SceneView.RepaintAll();
        }

        EditorGUILayout.Space(10);
        EditSelectedGameObject();
        
        EditorGUILayout.Space(10);
        if (GUILayout.Button("Refresh Prefabs", GUILayout.Height(25)))
        {
            LoadPrefabs();
        }
        
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("Prefab Palette", EditorStyles.boldLabel);
        
        paletteDrawer.DrawPrefabPalette(availablePrefabs, ref selectedPrefabIndex, ref scrollPosition);
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

    #endregion

    #region Scene GUI

    private void OnSceneGUI(SceneView sceneView)
    {
        gridRenderer.DrawGrid(gridCellSize, gridLineCount, floorCount, gridOpacity);
    
        if (placementMode && selectedPrefabIndex >= 0)
        {
            placementHandler.HandlePlacement(sceneView, availablePrefabs, selectedPrefabIndex, 
                                            gridCellSize, floorCount);
        }

        snappingHandler.SnappingGameObjectSelected(gameObjectSelected, snappingGameobjectSelected, 
                                                   gridCellSize, ref lastKnownPosition);

        if (gameObjectSelected != null)
        {
            gridRenderer.DrawGridY(gridCellSize, gridLineCount, gameObjectSelected.transform.position, gridOpacity);
        }
    }

    #endregion

    #region Prefab Loading

    private void LoadPrefabs()
    {
        availablePrefabs = prefabLoader.LoadPrefabs();
        
        if (selectedPrefabIndex >= availablePrefabs.Length)
        {
            selectedPrefabIndex = -1;
        }
    }

    #endregion

    #region Selection

    private void OnSelectionChange()
    {
        snappingHandler.ResetLastKnownPosition(ref lastKnownPosition);
        placementMode = false;
        Repaint();
    }

    #endregion
}