using UnityEngine;
public enum FireMode

{
    Hitscan,
    Projectile
}
public abstract class Weapon : MonoBehaviour
{
    public string weaponName;
    public float fireRate;
    public FireMode fireMode;
    public float damage;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPos;
    public GameObject chargedShotPrefab;
    public Transform muzzleFlashSpawnPos;
    public ParticleSystem muzzleFlash;
    public HeatManager heatManager;
    public float heatPerShot;
    public bool canFire;  

    public float nextFireTime;

    // optional coroutine holder for derived classes
    protected Coroutine sustainedCoroutine;

    // Start/Update left as-is
    void Start() { }
    void Update() { }

    public abstract void Fire();

    // called when holding fire — keep abstract or noop depending on weapon
    public abstract void SustainedFire();

    // new lifecycle hooks for start/stop of sustained fire (override when needed)
    public virtual void StartSustainedFire()
    {
        // default: do nothing. Derived classes that need coroutines should override.
    }

    public virtual void StopSustainedFire()
    {
        // default: do nothing.
    }
}
