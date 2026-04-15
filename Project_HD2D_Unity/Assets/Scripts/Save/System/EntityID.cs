using UnityEngine;

[DisallowMultipleComponent]
public class EntityID : MonoBehaviour
{
    [SerializeField] private string id;
    public string ID => id;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(id))
        {
            id = System.Guid.NewGuid().ToString();
        }
    }

    [ContextMenu("Generate New ID")]
    private void GenerateGuid()
    {
        id = System.Guid.NewGuid().ToString();
    }
}