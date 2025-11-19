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

   

    IEnumerator DestroyAfterTime(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        Destroy(gameObject);
        
    }
}
