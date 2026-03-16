using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CurvedCommsNetwork : MonoBehaviour
{
    [Header("Network Setup")]
    [Tooltip("Drag the Earth here so the lasers know what to curve around.")]
    public Transform earth;
    [Tooltip("Drag all your satellites into this array.")]
    public Transform[] satellites;
    public bool closeLoop = true;

    [Header("Visuals")]
    public float lineWidth = 0.02f;
    [Tooltip("How many small straight lines make up one curve? 10 is usually smooth enough for VR.")]
    public int curveResolution = 10; 

    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
    }

    void Update()
    {
        // We need at least 2 satellites and the Earth to calculate a curve
        if (satellites.Length < 2 || earth == null) return;

        // Calculate how many connections we are making
        int connections = closeLoop ? satellites.Length : satellites.Length - 1;
        
        // Total points = (number of connections * points per curve) + 1 final dot to cap it off
        int totalPoints = (connections * curveResolution) + 1;
        
        if (lr.positionCount != totalPoints)
        {
            lr.positionCount = totalPoints;
        }

        int currentPointIndex = 0;

        // Draw the curved arcs
        for (int i = 0; i < connections; i++)
        {
            Transform startSat = satellites[i];
            // If i is the last satellite and loop is closed, wrap around to 0
            Transform endSat = satellites[(i + 1) % satellites.Length]; 

            if (startSat == null || endSat == null) continue;

            // Generate the points along the curve for this specific link
            for (int j = 0; j < curveResolution; j++)
            {
                // 't' is the percentage of the way along the curve (from 0.0 to 1.0)
                float t = (float)j / curveResolution;

                // Find the direction from the Earth's center to each satellite
                Vector3 startDir = startSat.position - earth.position;
                Vector3 endDir = endSat.position - earth.position;

                // Slerp draws a perfect spherical arc between the two directions!
                Vector3 curvedPos = Vector3.Slerp(startDir, endDir, t);

                // Apply the point to the LineRenderer
                lr.SetPosition(currentPointIndex, earth.position + curvedPos);
                currentPointIndex++;
            }
        }

        // Cap off the very end of the line so there are no gaps
        if (closeLoop && satellites[0] != null)
        {
            lr.SetPosition(currentPointIndex, satellites[0].position);
        }
        else if (!closeLoop && satellites[satellites.Length - 1] != null)
        {
            lr.SetPosition(currentPointIndex, satellites[satellites.Length - 1].position);
        }
    }
}