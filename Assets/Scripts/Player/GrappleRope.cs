using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleRope : MonoBehaviour
{
    [Header("Rope Settings")]
    public LineRenderer lineRenderer;
    public int segmentCount = 20;
    public float ropeWidth = 0.1f;
    public Material ropeMaterial;
    
    [Header("Wave Settings")]
    public float waveSpeed = 2f;
    public float waveHeight = 0.3f;
    public AnimationCurve waveIntensity;
    
    private Transform startPoint;
    private Vector3 endPoint;
    private bool isActive = false;
    
    void Awake()
    {
        // Setup LineRenderer if not assigned
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }
        
        ConfigureLineRenderer();
        lineRenderer.enabled = false;
    }
    
    void ConfigureLineRenderer()
    {
        lineRenderer.positionCount = segmentCount;
        lineRenderer.startWidth = ropeWidth;
        lineRenderer.endWidth = ropeWidth * 0.5f;
        
        if (ropeMaterial != null)
        {
            lineRenderer.material = ropeMaterial;
        }
        
        // Make it look nice
        lineRenderer.numCapVertices = 5;
        lineRenderer.numCornerVertices = 5;
        lineRenderer.useWorldSpace = true;
    }
    
    void Update()
    {
        if (isActive && startPoint != null)
        {
            DrawRope();
        }
    }
    
    public void StartGrapple(Transform start, Vector3 end)
    {
        startPoint = start;
        endPoint = end;
        isActive = true;
        lineRenderer.enabled = true;
    }
    
    public void UpdateGrappleTarget(Vector3 newEnd)
    {
        endPoint = newEnd;
    }
    
    public void EndGrapple()
    {
        isActive = false;
        lineRenderer.enabled = false;
    }
    
    void DrawRope()
    {
        Vector3 start = startPoint.position;
        Vector3 end = endPoint;
        
        for (int i = 0; i < segmentCount; i++)
        {
            float t = i / (float)(segmentCount - 1);
            Vector3 position = Vector3.Lerp(start, end, t);
            
            // Add wave effect for rope physics simulation
            float waveAmount = waveIntensity.Evaluate(t) * waveHeight;
            float wave = Mathf.Sin(Time.time * waveSpeed + t * 5f) * waveAmount;
            
            // Apply wave perpendicular to rope direction
            Vector3 ropeDirection = (end - start).normalized;
            Vector3 perpendicular = Vector3.Cross(ropeDirection, Vector3.up);
            if (perpendicular.magnitude < 0.1f)
            {
                perpendicular = Vector3.Cross(ropeDirection, Vector3.right);
            }
            perpendicular = perpendicular.normalized;
            
            position += perpendicular * wave;
            lineRenderer.SetPosition(i, position);
        }
    }
}