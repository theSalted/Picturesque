using UnityEngine;

public class Hammer : MonoBehaviour
{
    
    [SerializeField] private GameObject brokenColumnPrefab;
    private int hitCount = 0; 
    [SerializeField] private float darknessFactor = 0.8f;
    private float initDark;

    void Start()
    {
        initDark = darknessFactor;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Breakable"))
        {
            hitCount++;

            if (hitCount < 3)
            {
                darknessFactor -= 0.4f; 
                // Ensure it stays between 0 and 1
                darknessFactor = Mathf.Clamp01(darknessFactor); 

                // Apply the new color to the material SO IT APPEARS TO BE HIT HAHAHA
                collision.gameObject.GetComponent<Renderer>().material.color = new Color(
                collision.gameObject.GetComponent<Renderer>().material.color.r * darknessFactor, 
                collision.gameObject.GetComponent<Renderer>().material.color.g * darknessFactor, 
                collision.gameObject.GetComponent<Renderer>().material.color.b * darknessFactor,
                collision.gameObject.GetComponent<Renderer>().material.color.a);
            }
            else 
            {
                hitCount = 0;

                // Get the position and rotation of columnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnnn
                Vector3 columnPosition = collision.transform.position;
                Quaternion columnRotation = collision.transform.rotation;

                // Instantiate the broken column prefab at the same position and rotation
                GameObject brokenColumn = Instantiate(brokenColumnPrefab, columnPosition, columnRotation);

                foreach (Rigidbody piece in brokenColumn.GetComponentsInChildren<Rigidbody>())
                {
                    Vector3 randomForce = Random.insideUnitSphere * 2f; // Random force magnitude
                    piece.AddForce(randomForce, ForceMode.Impulse);
                }

                // Destroy the og column GameObject
                Destroy(collision.gameObject);
                darknessFactor = initDark;
            }
        }
    }
}
