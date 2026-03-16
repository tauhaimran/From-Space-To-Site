using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BeamToEarth : MonoBehaviour
{
    [Header("Beam Setup")]
    [Tooltip("Drag the Zimbabwe Target point here.")]
    public Transform groundTarget;
    
    [Tooltip("How thick the main transmission beam is.")]
    public float beamWidth = 0.08f; // Thicker than the inter-satellite links

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = beamWidth;
        lineRenderer.endWidth = beamWidth;
        lineRenderer.useWorldSpace = true;
    }

    void Update()
    {
        if (groundTarget != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, groundTarget.position);
        }
    }
}