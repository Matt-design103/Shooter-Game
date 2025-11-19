using UnityEngine;
using System.Collections;

public class HeatManager : MonoBehaviour
{
    public float currentHeat;
    public float maxHeat = 100f;
    public float heatDissipationRate = 5f;

    public float cooldownTime = 3f;
    public Weapon weapon;
    public PlayerController player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GetComponent<PlayerController>();
         if (player != null && player.weaponManagement != null)
        {
            weapon = player.weaponManagement.CurrentWeapon;
        }
        else
        {
            weapon = GetComponentInChildren<Weapon>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHeat > 0)
        {
            currentHeat -= heatDissipationRate * Time.deltaTime;
            if (currentHeat < 0)
            {
                currentHeat = 0;
            }
            Debug.Log("Current Heat: " + currentHeat);
        }
    }

    public void AddHeat(float amount)
    {
        currentHeat += amount;
        if (currentHeat >= maxHeat)
        {
            player.Overheat();

        }
    }
    
    IEnumerator CooldownFromOverheat(float cooldownTime)
    {
        yield return new WaitForSeconds(cooldownTime);
        currentHeat = 0f;
    }
}
