using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManagement : MonoBehaviour
{
    public Transform weaponParent;          // where weapons will be attached
    public List<Weapon> weapons = new List<Weapon>();
    private int currentIndex = 0;

    public Weapon CurrentWeapon => weapons.Count > 0 ? weapons[currentIndex] : null;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) { EquipWeapon(0); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { EquipWeapon(1); }
        if (Input.GetAxis("Mouse ScrollWheel") > 0f) { NextWeapon(); }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f) { PreviousWeapon(); }
    }

    public void EquipWeapon(int index)
    {
        if (index >= 0 && index < weapons.Count)
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].gameObject.SetActive(i == index);
            }
            currentIndex = index;
        }
    }

    public void NextWeapon()
    {
        int next = (currentIndex + 1) % weapons.Count;
        EquipWeapon(next);
    }

    public void PreviousWeapon()
    {
        int prev = (currentIndex - 1 + weapons.Count) % weapons.Count;
        EquipWeapon(prev);
    }

    public void AddWeapon(Weapon newWeapon)
    {
        newWeapon.transform.SetParent(weaponParent, false);
        newWeapon.transform.localPosition = Vector3.zero;
        newWeapon.transform.localRotation = Quaternion.identity;
        newWeapon.gameObject.SetActive(false);

        weapons.Add(newWeapon);

        if (weapons.Count == 1)  // if it’s the first weapon, auto-equip
            EquipWeapon(0);
    }
}
