using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SatelliteCommsNetwork : MonoBehaviour
{
    [Header("Network Setup")][Tooltip("Drag all your satellites into this array in the order they should connect.")]
    public Transform[] satellites;[Tooltip("Should the last satellite connect back to the first one to make a ring?")]
    public bool closeLoop = true;

    [Header("Visuals")]
    public float lineWidth = 0.02f;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        // Set how many dots the line needs to connect
        if (satellites.Length > 0)
        {
            lineRenderer.positionCount = closeLoop ? satellites.Length + 1 : satellites.Length;
        }
    }

    void Update()
    {
        if (satellites.Length < 2) return; // Need at least 2 satellites to make a line

        // Draw line to each satellite
        for (int i = 0; i < satellites.Length; i++)
        {
            if (satellites[i] != null)
            {
                lineRenderer.SetPosition(i, satellites[i].position);
            }
        }

        // Connect the very end of the line back to the start
        if (closeLoop && satellites[0] != null)
        {
            lineRenderer.SetPosition(satellites.Length, satellites[0].position);
        }
    }
}