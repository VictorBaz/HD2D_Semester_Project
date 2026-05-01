using UnityEngine;

public class RemoveWater : MonoBehaviour
{
    public GameObject[] objectBeforeWaters;
    public GameObject[] objectAfterWaters;
    void Start()
    {
        foreach (GameObject g in objectAfterWaters)
        {
            g.SetActive(false);
        }
    }

    public void Remove()
    {
        foreach (GameObject g in objectBeforeWaters)
        {
            g.SetActive(false);
        }
        foreach (GameObject g in objectAfterWaters)
        {
            g.SetActive(true);
        }
    }
}
