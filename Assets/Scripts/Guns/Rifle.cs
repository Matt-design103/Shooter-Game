using UnityEngine;

public class Rifle : Weapon
{

    private Camera playerCam;
    public float range = 200f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void Fire()
    {

        RaycastHit hit;

        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, range))
        {

            Debug.Log("hit" + hit.collider.name);
            GameObject target = hit.collider.gameObject;
            EnemyHealth enemy = target.GetComponent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.takeDamage(damage);
            }
        }
    }
}
