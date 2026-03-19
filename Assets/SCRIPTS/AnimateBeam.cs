using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateBeam : MonoBehaviour
{
    [Header("Beam Animation Settings")]
    [Tooltip("How fast the data flows down. (Usually a negative Y value like -2)")]
    public float scrollSpeedY = -2.0f;
    public float scrollSpeedX = 0.0f; // Keep 0 unless you want it to twist

    private Material beamMaterial;
    private Vector2 currentOffset;

    void Start()
    {
        // Get the material attached to this cylinder
        // We use material (not sharedMaterial) so it only animates THIS specific beam
        Renderer rend = GetComponent<Renderer>();
        if (rend != null)
        {
            beamMaterial = rend.material;
        }
    }

    void Update()
    {
        if (beamMaterial != null)
        {
            // Calculate the new offset based on time
            currentOffset.y += scrollSpeedY * Time.deltaTime;
            currentOffset.x += scrollSpeedX * Time.deltaTime;

            // Apply the offset to the material's main texture
            beamMaterial.SetTextureOffset("_BaseMap", currentOffset);
        }
    }
}