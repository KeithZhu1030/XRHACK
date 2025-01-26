using UnityEngine;

public class CarSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs; // Array of car prefabs
    public Transform pointA;       // Start point
    public Transform pointB;       // End point
    public float spawnInterval = 2f; // Time between spawns
    public float carSpeed = 5f;    // Speed of the car

    private int currentCarIndex = 0; // Tracks which car to spawn next

    void Start()
    {
        InvokeRepeating(nameof(SpawnCar), 0f, spawnInterval); // Spawn cars in a loop
    }

    void SpawnCar()
    {
        // Get the current car prefab from the array
        GameObject carPrefab = carPrefabs[currentCarIndex];

        // Instantiate the car at Point A
        GameObject car = Instantiate(carPrefab, pointA.position, pointA.rotation);

        // Add the CarMover component (if not already on the prefab)
        CarMover carMover = car.AddComponent<CarMover>();
        carMover.Initialize(pointB, carSpeed);

        // Update the car index to loop through the array
        currentCarIndex = (currentCarIndex + 1) % carPrefabs.Length;
    }
}
