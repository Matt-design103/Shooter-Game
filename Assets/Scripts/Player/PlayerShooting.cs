using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public WeaponManagement weaponManagement;

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && weaponManagement.CurrentWeapon != null)
        {
            weaponManagement.CurrentWeapon.Fire();
        }
    }
}
