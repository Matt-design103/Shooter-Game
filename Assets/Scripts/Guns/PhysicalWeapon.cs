using UnityEngine;

public class PhysicalWeapon : Weapon
{

    public float range = 100f;
    public GameObject target;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Fire()
    {
        Instantiate(bulletPrefab, bulletSpawnPos.position, Quaternion.identity);
    }
}
