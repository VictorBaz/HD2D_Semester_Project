using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootingSystem : MonoBehaviour
{
    #region Variables

    public event Action OnStartChargingShoot;
    public event Action<float> OnChargeTick;

    [SerializeField] private ProjectileBase[] projectilePrefab;
    [SerializeField] private Transform origin;

    private Vector3 shootDirection  = Vector3.zero;

    private float shootPressTime  = 0f;
    private bool isCharging      = false;
    private float chargeRatio     = 0f;
    
    private PlayerDataInstance playerData;
    
    private Vector2 finalDirection = Vector2.zero;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        for (int i = 0; i < projectilePrefab.Length; i++)
            ObjectPooler.SetupPool(projectilePrefab[i], 3, $"Projectile_{i}");
    }

    private void Update()
    {
        Tick();
    }

    #endregion

    #region Try
    

    public void HandleStartTryShoot()
    {
        if (isCharging) return;
        
        isCharging     = true;
        shootPressTime = Time.time;
        OnStartChargingShoot?.Invoke();
    }

    public void HandleStopTryShoot(Transform toPosition = null)
    {
        if (!isCharging) return;
        
        isCharging = false;

        float holdDuration = Time.time - shootPressTime;


        if (toPosition != null)
        {
            SpawnProjectile(SelectProjectile(),toPosition);
            
            chargeRatio = 0f;
            OnChargeTick?.Invoke(chargeRatio);
            
            return;
        }
        
        
        
        if (holdDuration < playerData.ChargeThreshold)
        {
            SpawnProjectile(SelectProjectile());
        }
        else
        {
            chargeRatio = Mathf.Clamp01(holdDuration / playerData.MaxChargeTime);
            SpawnProjectile(SelectProjectile(chargeRatio));
        }

        chargeRatio = 0f;
        OnChargeTick?.Invoke(chargeRatio);
    }
    
    

    #endregion

    public void SetShootDirection(Vector3 direction)
    {
        shootDirection = direction;
    }

    public void Tick()
    {
        if (!isCharging) return;

        chargeRatio = Mathf.Clamp01((Time.time - shootPressTime) / playerData.MaxChargeTime);
        OnChargeTick?.Invoke(chargeRatio);
    }

    #region Projectile Handling

    private ProjectileBase SelectProjectile()
    {
        print("Projectile Easy");
        return projectilePrefab[0];
    }

    private ProjectileBase SelectProjectile(float charge)
    {
        print(charge < playerData.MediumHeavyThreshold ? "Projectile Medium" : "Projectile Heavy");
        return charge < playerData.MediumHeavyThreshold ? projectilePrefab[1] : projectilePrefab[2];
    }

    #endregion
    

    private void SpawnProjectile(ProjectileBase prefab)
    {
        ProjectileBase projectile = ObjectPooler.DequeueObject<ProjectileBase>(prefab.PoolKey);

        projectile.transform.position = origin.position;
        projectile.gameObject.SetActive(true);

        finalDirection = new Vector2(shootDirection.x, shootDirection.z);
        if (finalDirection != Vector2.zero) finalDirection.Normalize();
        
        projectile.Initialize(finalDirection);
    }
    
    private void SpawnProjectile(ProjectileBase prefab,Transform toPosition)
    {
        ProjectileBase projectile = ObjectPooler.DequeueObject<ProjectileBase>(prefab.PoolKey);

        projectile.transform.position = origin.position;
        projectile.gameObject.SetActive(true);
        
        projectile.Initialize(origin,toPosition);
    }

    public void InitData(PlayerDataInstance data)
    {
        playerData = data;
    }
}