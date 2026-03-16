using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitEarth : MonoBehaviour
{
    [Header("Orbit Settings")][Tooltip("Drag the Earth GameObject here.")]
    public Transform earthTarget; 
    
    [Tooltip("How fast the satellite orbits the Earth.")]
    public float orbitSpeed = 10f;[Tooltip("The axis to orbit around. (0,1,0) is equator. Try (1,1,0) for an angled orbit!")]
    public Vector3 orbitAxis = Vector3.up;

    [Header("Visual Settings")][Tooltip("Check this if you want the bottom of the satellite to always face Earth.")]
    public bool keepFacingEarth = true;

    void Update()
    {
        if (earthTarget != null)
        {
            // 1. Orbit around the target
            transform.RotateAround(earthTarget.position, orbitAxis, orbitSpeed * Time.deltaTime);

            // 2. Keep the satellite oriented towards the Earth (optional but looks realistic)
            if (keepFacingEarth)
            {
                // Makes the satellite's forward axis (Z) point at the Earth's center
                transform.LookAt(earthTarget); 
                
                // Note: Depending on your 3D model's pivot, you might need to rotate 
                // the child mesh inside the satellite parent to make the dish face down.
            }
        }
        else
        {
            Debug.LogWarning("OrbitEarth script is missing the Earth Target!");
        }
    }
}