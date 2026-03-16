using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class OrbitPathLine : MonoBehaviour
{
    [Header("Visual Settings")]
    [Tooltip("How thick the orbit line should be.")]
    public float lineWidth = 0.02f;[Tooltip("How many points make up the circle. 60 is smooth enough for VR.")]
    public int segments = 60;

    private LineRenderer lineRenderer;
    private OrbitEarth orbitScript;

    void Start()
    {
        // 1. Get the components
        lineRenderer = GetComponent<LineRenderer>();
        orbitScript = GetComponent<OrbitEarth>();

        if (orbitScript == null || orbitScript.earthTarget == null)
        {
            Debug.LogWarning("OrbitPathLine needs the OrbitEarth script attached to the same object with an Earth Target assigned!");
            return;
        }

        // 2. Setup the Line Renderer
        lineRenderer.positionCount = segments + 1;
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.loop = true; // Connects the end to the start perfectly

        // 3. Draw the path
        DrawOrbitRing();
    }

    void DrawOrbitRing()
    {
        // Get the starting distance and direction from Earth to the Satellite
        Vector3 startOffset = transform.position - orbitScript.earthTarget.position;

        // Generate points in a circle based on the orbit axis
        for (int i = 0; i < segments + 1; i++)
        {
            // Calculate the angle for this specific point on the circle
            float angle = ((float)i / segments) * 360f;

            // Rotate our starting offset around the chosen orbit axis by that angle
            Quaternion rotation = Quaternion.AngleAxis(angle, orbitScript.orbitAxis);
            Vector3 pointOffset = rotation * startOffset;

            // Place the point in world space
            lineRenderer.SetPosition(i, orbitScript.earthTarget.position + pointOffset);
        }
    }
}