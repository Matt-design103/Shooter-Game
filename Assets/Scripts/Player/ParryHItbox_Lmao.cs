using UnityEngine;

public class ParryHItbox_Lmao : MonoBehaviour
{
    public float parryDuration = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float timer = parryDuration;
        float elapsed = 0f; 
        if (timer > 0f)
        {
            elapsed += Time.deltaTime;
            if (elapsed >= timer)
            {
                Destroy(gameObject);

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnColliderEnter(Collider other)
    {
        if (other.CompareTag("EnemyAttack"))
        {
            Debug.Log("Parried an attack!");
            Destroy(other.gameObject); // Destroy the enemy attack object
        }
    }
}
