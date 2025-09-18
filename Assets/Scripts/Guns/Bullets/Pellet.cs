using UnityEngine;

public class Pellet : MonoBehaviour
{
    public float speed = 2000f;
    public int damage = 10;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("hit" + collision.collider.name);
        EnemyHealth enemy = collision.collider.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.takeDamage(damage);
        }
        Debug.Log("hit" + collision.collider.name);
        Destroy(gameObject);
    }
}

