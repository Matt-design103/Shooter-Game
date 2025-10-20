using UnityEngine;

public class Pistol : Weapon
{

    public float range = 100f;
    private Camera playerCam;
    


    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCam = Camera.main;
        heatManager = GetComponentInParent<HeatManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Fire()
    {

        heatManager.AddHeat(heatPerShot);
        RaycastHit hit;

        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, range))
        {
             Instantiate(muzzleFlash, muzzleFlashSpawnPos.position, playerCam.transform.rotation);

           
            GameObject target = hit.collider.gameObject;
            EnemyHealth enemy = target.GetComponent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.takeDamage(damage);
            }
        }
    }
}
