using UnityEngine;

public class SpinnySpinny : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
  public float rotationsPerMinute = 10f;
    void Update()
    {
        transform.Rotate(0, 6f * rotationsPerMinute * Time.deltaTime, 0f);
    }

}
