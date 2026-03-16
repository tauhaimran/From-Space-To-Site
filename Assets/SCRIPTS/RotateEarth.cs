using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateEarth : MonoBehaviour
{[Header("Rotation Settings")]
    [Tooltip("Speed of the Earth's rotation.")]
    public float rotationSpeed = 2f;

    void Update()
    {
        // Space.Self ensures it respects your realistic axial tilt!
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
    }
}
