using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 5f;            // How fast the bullet moves
    public float lifetime = 5f;         // How long before it self-destructs
    public int damage = 10;             // How much damage it deals
    bool isDeflected;

    private void Start()
    {
        // Destroy the bullet after a few seconds to avoid clutter
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move forward constantly
             transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnCollisionEnter (Collision other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            //damage player
            Destroy(gameObject);
        }
        else if(other.gameObject.CompareTag("Parry"))
        {
            transform.Rotate(0, 180, 0);
        }
    }

}
