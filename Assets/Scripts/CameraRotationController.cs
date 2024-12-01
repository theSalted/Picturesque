using UnityEngine;

public class CameraRotationController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject inventory = PlayerManager.Instance.inventory;
        if (inventory == null)
        {
            return;
        }

        Movable movable = inventory.GetComponent<Movable>();

        if (movable == null)
        {
            return;
        }

        // If camera is pictching up
        if (gameObject.transform.rotation.x < 0)
        {
            // Rotate camera down   
            // movable.avoidanceFlags = new Vector3(0, -1, 0);
        } else {
            // Rotate camera up
            // movable.avoidanceFlags = new Vector3(0, 1, 0);
        }
        
    }
}
