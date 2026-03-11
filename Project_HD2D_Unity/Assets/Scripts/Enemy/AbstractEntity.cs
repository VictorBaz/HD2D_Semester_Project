using UnityEngine;

public class AbstractEntity : MonoBehaviour
{
    public float Life { get; private  set; }
    public float MaxLife { get; private set; }

    public void TakeDamage(float damage)
    {
        Life -= damage;
        
        Life = Mathf.Clamp(Life, 0, MaxLife);

        if (Life <= 0)
        {
            
        }
    }

    private void HandleTakeDown()
    {
        
    }
}
