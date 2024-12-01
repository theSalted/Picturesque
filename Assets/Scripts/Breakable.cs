using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public GameObject brokenPrefab;

    public List<GameObject> neighborObjects = new List<GameObject>();

    private int hitCount = 0; 
    [SerializeField] private float darknessFactor = 0.8f;
    private float initDark;


    public void Hit()
    {
        hitCount++;

        if (hitCount < 3)
        {
            darknessFactor -= 0.4f; 
            // Ensure it stays between 0 and 1
            darknessFactor = Mathf.Clamp01(darknessFactor); 

            // Apply the new color to the material SO IT APPEARS TO BE HIT HAHAHA
            gameObject.GetComponent<Renderer>().material.color = new Color(
            gameObject.GetComponent<Renderer>().material.color.r * darknessFactor, 
            gameObject.GetComponent<Renderer>().material.color.g * darknessFactor, 
            gameObject.GetComponent<Renderer>().material.color.b * darknessFactor,
            gameObject.GetComponent<Renderer>().material.color.a);
        }
        else 
        {
            Break();
        }
    }

    public void Break()
    {
        hitCount = 0;
        GameObject brokenColumn = Instantiate(brokenPrefab, this.transform.position, this.transform.rotation);

        // brokenColumn.AddComponent<FallingController>();

        foreach (Rigidbody piece in brokenColumn.GetComponentsInChildren<Rigidbody>())
        {
            Vector3 randomForce = Random.insideUnitSphere * 50f; // Random force magnitude
            piece.AddForce(randomForce, ForceMode.Impulse);
        }

        foreach (GameObject neighbor in neighborObjects)
        {
            neighbor.AddComponent<Outline>();
            neighbor.AddComponent<Respawnable>();
            neighbor.AddComponent<Movable>();
            neighbor.AddComponent<FallingController>();
            BoxCollider collider = neighbor.GetComponent<BoxCollider>();
            if (collider == null)
            {
                collider = neighbor.AddComponent<BoxCollider>();
            }
            collider.enabled = true;
        }

        // Destroy the og column GameObject
        Destroy(gameObject);

        darknessFactor = initDark;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initDark = darknessFactor;
    }
}
