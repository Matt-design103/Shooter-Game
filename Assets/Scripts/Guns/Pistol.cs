using UnityEngine;
using System.Collections;

public class Pistol : Weapon
{

    public float range = 100f;
    private Camera playerCam;

    // coroutine reference for charging
    private Coroutine chargeCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCam = Camera.main;
        heatManager = GetComponentInParent<HeatManager>();
        canFire = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Fire()
    {
        if (!canFire) return;

        heatManager?.AddHeat(heatPerShot);
        RaycastHit hit;

        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, range))
        {
            if (muzzleFlash != null && muzzleFlashSpawnPos != null)
                Instantiate(muzzleFlash, muzzleFlashSpawnPos.position, playerCam.transform.rotation);

            GameObject target = hit.collider.gameObject;
            EnemyHealth enemy = target.GetComponent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.takeDamage(damage);
            }
        }
    }

    // keep for interface compatibility; pistol uses Start/Stop hooks instead
    public override void SustainedFire()
    {
        // no-op (pistol handles charge via coroutine)
    }

    // start charging when held
    public override void StartSustainedFire()
    {
        if (chargeCoroutine == null)
            chargeCoroutine = StartCoroutine(ChargeShot());
    }

    // stop charging when released
    public override void StopSustainedFire()
    {
        if (chargeCoroutine != null)
        {
            StopCoroutine(chargeCoroutine);
            chargeCoroutine = null;
            canFire = true; // reset state if needed
        }
    }

    IEnumerator ChargeShot()
    {
        canFire = false;
        yield return new WaitForSeconds(1f);
        if (chargedShotPrefab != null && bulletSpawnPos != null)
            Instantiate(chargedShotPrefab, bulletSpawnPos.position, playerCam.transform.rotation);
        canFire = true;
        chargeCoroutine = null;
    }


}
