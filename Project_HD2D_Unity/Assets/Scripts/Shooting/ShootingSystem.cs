using UnityEngine;

public class ShootingSystem
{
    private const string ProjectilePoolKey = "Projectile";

    public void SpawnProjectile(Vector3 directionShoot, Transform origin)
    {
        ProjectileBase projectile = ObjectPooler.DequeueObject<ProjectileBase>(ProjectilePoolKey);

        projectile.transform.position = origin.position;
        
        projectile.gameObject.SetActive(true);

        Vector2 finalDirection = new Vector2(directionShoot.x, directionShoot.z);
        
        if(finalDirection != Vector2.zero) finalDirection.Normalize();
            
        projectile.Initialize(finalDirection);
    }

    
}