using UnityEngine;
public enum FireMode
{
    Hitscan,
    Projectile
}

public enum FiringStyle
{
    SingleShot,
    Automatic
}
public abstract class Weapon : MonoBehaviour
{
    public string weaponName;
    public float fireRate;
    public FireMode fireMode;
    public float damage;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPos;


    public float nextFireTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /*public virtual void Shoot()
    {
        if (Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }
    }*/

      public abstract void Fire();
}
