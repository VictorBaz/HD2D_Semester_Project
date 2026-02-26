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

    [SerializeField] private float chargeThreshold = 0.2f;
    [SerializeField] private float maxChargeTime   = 2f;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        for (int i = 0; i < projectilePrefab.Length; i++)
            ObjectPooler.SetupPool(projectilePrefab[i], 10, $"Projectile_{i}");
    }

    private void Update()
    {
        Tick();
    }

    #endregion

    #region Try
    

    public void HandleStartTryShoot()
    {
        //  if (!m_canShoot()) return; //TODO STATEMACHINE
        if (isCharging) return;
        
        isCharging     = true;
        shootPressTime = Time.time;
        OnStartChargingShoot?.Invoke();
    }

    public void HandleStopTryShoot()
    {
        if (!isCharging) return;
        
        isCharging = false;

        float holdDuration = Time.time - shootPressTime;

        if (holdDuration < chargeThreshold)
        {
            SpawnProjectile(SelectProjectile());
        }
        else
        {
            chargeRatio = Mathf.Clamp01(holdDuration / maxChargeTime);
            SpawnProjectile(SelectProjectile(chargeRatio));
        }

        chargeRatio = 0f;
    }

    #endregion

    public void SetShootDirection(Vector3 direction)
    {
        shootDirection = direction;
    }

    public void Tick()
    {
        if (!isCharging) return;

        chargeRatio = Mathf.Clamp01((Time.time - shootPressTime) / maxChargeTime);
        OnChargeTick?.Invoke(chargeRatio);
    }

    #region Projectile Handling

    private ProjectileBase SelectProjectile()
    {
        return projectilePrefab[0];
    }

    private ProjectileBase SelectProjectile(float charge)
    {
        return charge < 0.5f ? projectilePrefab[1] : projectilePrefab[2];
    }

    #endregion
    

    private void SpawnProjectile(ProjectileBase prefab)
    {
        ProjectileBase projectile = ObjectPooler.DequeueObject<ProjectileBase>(prefab.PoolKey);

        projectile.transform.position = origin.position;
        projectile.gameObject.SetActive(true);

        Vector2 finalDirection = new Vector2(shootDirection.x, shootDirection.z);
        if (finalDirection != Vector2.zero) finalDirection.Normalize();

        projectile.Initialize(finalDirection);
    }
}