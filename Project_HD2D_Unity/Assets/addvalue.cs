using UnityEngine;
using UnityEngine.UI;

public class addvalue : MonoBehaviour
{
    public Slider slider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Addvalue()
    {
        slider.value += 0.1f;
    }
    public void Decressvalue()
    {
        slider.value -= 0.1f;
    }
}
