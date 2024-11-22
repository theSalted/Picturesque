using UnityEngine;

public class Respawnable : MonoBehaviour
{
    public float deathFloor = -10.0f;
    private Vector3 ogPosition;
    private Quaternion ogRotation;
    
    void Awake() {
        ogPosition = transform.position;
        ogRotation = transform.rotation;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < deathFloor) {
            Debug.Log("Respawning");
            Respawn();
        }
    }

    public virtual void Respawn() {
        transform.position = ogPosition;
        transform.rotation = ogRotation;
    }
}
