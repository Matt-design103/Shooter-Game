using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 100f;

    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void takeDamage(float damageAmount)
    {
        health = health - damageAmount;
        if (health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
