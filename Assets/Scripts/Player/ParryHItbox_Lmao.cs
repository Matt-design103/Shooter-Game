using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryHItbox_Lmao : MonoBehaviour
{
    public float parryDuration = 0.5f;
    public GameObject player; // Reference to the player object

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(DestroyAfterTime(parryDuration));
        player = GameObject.FindWithTag("Player");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("EnemyAttack"))
        {
            Debug.Log("Parried an attack!");
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.ParryPogo(); // Call the ParryPogo method on the player
            }
            Destroy(other.gameObject); // Destroy the enemy attack object
        }
    }

    IEnumerator DestroyAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        Destroy(gameObject);
        Debug.Log("You did it bro");
        
    }
}
