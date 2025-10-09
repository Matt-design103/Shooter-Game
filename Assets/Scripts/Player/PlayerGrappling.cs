using UnityEngine;
using System.Collections;

public class PlayerGrappling : MonoBehaviour
{

    public float grappleSpeed = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            HandleGrapple();
        }
    }

    public void HandleGrapple()
    {
        RaycastHit hit;
        Camera playerCam = Camera.main;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, 100f))
        {
            Vector3 grapplePoint = hit.point;
            StartCoroutine(GrappleToPoint(grapplePoint));
        }
    }

    IEnumerator GrappleToPoint(Vector3 point)
    {
        while (Vector3.Distance(transform.position, point) > 1f)
        {
            Vector3 direction = (point - transform.position).normalized;
            transform.position += direction * grappleSpeed * Time.deltaTime;
            yield return null;
        }
    }
}
