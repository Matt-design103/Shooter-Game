using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weaponPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("valid collision");
        }
        WeaponManagement weaponManager = other.GetComponent<WeaponManagement>(); //change to GetComponentInChildren if u change the hierarchy :))))
        if (weaponManager != null)
        {
            // Spawn a copy of the weapon into the holder
            GameObject newWeaponObj = Instantiate(weaponPrefab);
            Weapon newWeapon = newWeaponObj.GetComponent<Weapon>();

            weaponManager.AddWeapon(newWeapon);

            Destroy(gameObject); // remove pickup from world
        }
    }
}

