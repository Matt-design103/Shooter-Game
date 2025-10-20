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
    public Transform muzzleFlashSpawnPos;
    public ParticleSystem muzzleFlash;
    public HeatManager heatManager;
    public float heatPerShot;
  


    public float nextFireTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
     
    }

    public abstract void Fire();
    
    public void AddHeat(float amount)
    {
        heatManager.AddHeat(amount);
    }
}
