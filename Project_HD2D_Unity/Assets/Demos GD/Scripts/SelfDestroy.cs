using UnityEngine;

public class SelfDestroy : MonoBehaviour
{
    public void KillThySelf()
    {
        Destroy(gameObject);
    }
}
