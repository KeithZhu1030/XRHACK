using UnityEngine;

public class CarMover : MonoBehaviour
{
    private Transform target;  // Target point (Point B)
    private float speed;       // Movement speed
    private bool isInitialized = false;

    public void Initialize(Transform targetPoint, float moveSpeed)
    {
        target = targetPoint;
        speed = moveSpeed;
        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized) return;

        // Move the car toward the target
        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        // Destroy the car once it reaches the target
        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            Destroy(gameObject);
        }
    }
}
