using Grid;
using UnityEditor;
using UnityEngine;

public class PlacementHandler
{
    private Vector3Int previewGridPosition;
    private bool isValidPlacement = false;

    public void HandlePlacement(SceneView sceneView, GameObject[] availablePrefabs, int selectedPrefabIndex, 
                                float gridCellSize, int floorCount)
    {
        Event e = Event.current;
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, floorCount * gridCellSize, 0f));
        
        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPos = ray.GetPoint(distance);
            
            worldPos = new Vector3(worldPos.x, floorCount * gridCellSize, worldPos.z);
            
            previewGridPosition = GridHelper.WorldToGrid(worldPos, gridCellSize);

            isValidPlacement = true;
            
            DrawPlacementPreview(gridCellSize);
            
            if (e.type == EventType.MouseDown && e.button == 0)
            {

                if (!CanPlacePrefab(gridCellSize)) return;
                
                PlacePrefab(availablePrefabs, selectedPrefabIndex, gridCellSize);
                e.Use(); 
            }
            
            sceneView.Repaint();
        }
        
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
    }
    
    private void DrawPlacementPreview(float gridCellSize)
    {
        Vector3 worldPos = GridHelper.GridToWorld(previewGridPosition, gridCellSize);
            
        Handles.color = isValidPlacement ? new Color(0, 1, 0, 0.1f) : new Color(1, 0, 0, 0.1f);
        Handles.CubeHandleCap(0, worldPos, Quaternion.identity, gridCellSize * 0.9f, EventType.Repaint);
    }

    private void PlacePrefab(GameObject[] availablePrefabs, int selectedPrefabIndex, float gridCellSize)
    {
        if (selectedPrefabIndex < 0 || selectedPrefabIndex >= availablePrefabs.Length)
            return;
        
        GameObject prefab = availablePrefabs[selectedPrefabIndex];
        Vector3 worldPos = GridHelper.GridToWorld(previewGridPosition, gridCellSize);
        
        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        instance.transform.position = worldPos;
        
        Undo.RegisterCreatedObjectUndo(instance, "Place Prefab");
    }


    private bool CanPlacePrefab(float cellSize)
    {
        Vector3 worldPos = GridHelper.GridToWorld(previewGridPosition, cellSize);
        
        Vector3 screenPos = HandleUtility.WorldToGUIPoint(worldPos); 
        
        GameObject selectGameObject = HandleUtility.PickGameObject(
            screenPos,
            false);

        Debug.Log(selectGameObject == null ? "NO" : "YES");

        return selectGameObject == null;
    }
}