using UnityEngine;

public class Rock : MonoBehaviour
{
    // Prefab to be generated
    public GameObject rockPrefab;

    [Header("Spawn Area")]
    public GameObject spawnAreaObject; 

    // Spawn area range
    private Vector3 spawnAreaMin;
    private Vector3 spawnAreaMax;

    // Spawn interval time
    public float spawnInterval = 2.0f;

    // Timer
    // private float timer;

    // Whether the generator has started
    private bool isGenerating = false;

    void Start()
    {
        if (spawnAreaObject != null)
        {
            CalculateSpawnArea();
        }
        else
        {
            Debug.LogError("Spawn area object is not assigned!");
        }
    }

    void Update()
    {

    }

    void CalculateSpawnArea()
    {
        // Attempt to get a Collider from the GameObject
        Collider collider = spawnAreaObject.GetComponent<Collider>();

        if (collider != null)
        {
            Bounds bounds = collider.bounds;
            spawnAreaMin = bounds.min;
            spawnAreaMax = bounds.max;
        }
        else
        {
            Debug.LogError("The spawn area object must have a Collider component!");
        }
    }

    void GenerateRock()
    {
        // Randomly generate position within the specified range
        Vector3 spawnPosition = new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y),
            Random.Range(spawnAreaMin.z, spawnAreaMax.z)
        );
        
        // Instantiate the prefab
        GameObject rock = Instantiate(rockPrefab, spawnPosition, Quaternion.identity);

        // Ensure the prefab has a Rigidbody component
        if (rock.GetComponent<Rigidbody>() == null)
        {
            rock.AddComponent<Rigidbody>();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the player enters the trigger
        if (other.CompareTag("Player"))
        {
            // isGenerating = true;
            InvokeRepeating(nameof(GenerateRock), 0, spawnInterval);
        }
    }
}
