using UnityEngine;

public class Rotation : MonoBehaviour
{
    // Speed of rotation in degrees per second
    public float rotationSpeed = 10f;

    void Update()
    {
        // Rotate the object around its Y-axis at the specified speed
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
