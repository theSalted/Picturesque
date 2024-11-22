using UnityEngine;

public class RockGenerator : MonoBehaviour
{
    // Prefab to be generated
    public GameObject rockPrefab;

    // Spawn area range
    public Vector3 spawnAreaMin;
    public Vector3 spawnAreaMax;

    // Spawn interval time
    public float spawnInterval = 2.0f;

    // Timer
    private float timer;

    // Whether the generator has started
    private bool isGenerating = false;

    void Start()
    {
        timer = spawnInterval;
    }

    void Update()
    {
        if (isGenerating)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                GenerateRock();
                timer = spawnInterval;
            }
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
            isGenerating = true;
        }
    }
}
