using UnityEngine;
using System.Collections;

public class PlayerShooting : MonoBehaviour
{
    public WeaponManagement weaponManagement;

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && weaponManagement.CurrentWeapon != null)
        {
            weaponManagement.CurrentWeapon.Fire();
            //get rotation of muzzle flash spawn pos
            Debug.Log(weaponManagement.CurrentWeapon.muzzleFlashSpawnPos.rotation);
        }
    }
}
