using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public float rotationDuration; // Duration of rotation in seconds
    public float targetRotationY; // Target Y rotation angle
    public bool isRotating = false;      // To check if the door is already rotating
    public GameObject text;
    public bool playerInZone = false;
    public Transform door;
    public bool isOpened = false; 
    public GameObject warningText;
    public PickUp pickUp;
    // public bool key = false;
    

    private Quaternion initialRotation;   // The original rotation of the door
    private Quaternion finalRotation;     // The final rotation of the door

    // public GameObject keyUI;

    void Start()
    {
        // Store the initial rotation of the door
        initialRotation = door.rotation;
        // Calculate the final rotation of the door (rotate 90 degrees on the Y-axis)
        finalRotation = Quaternion.Euler(0, targetRotationY, 0) * initialRotation;
        text.SetActive(false);
        warningText.SetActive(false);
    }

    void Update()
    {
        // Check if the player is in the trigger zone and the "O" key is pressed
        if (playerInZone && !isRotating && !isOpened && Input.GetKeyDown(KeyCode.E))
        {
            if (pickUp.hasItem)
            {
                text.SetActive(false);
                StartCoroutine(RotateDoor());
            }
            else
            {
                text.SetActive(false);
                warningText.SetActive(true);
            }
            // keyUI.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        // Check if the player collided with the door
        if (col.CompareTag("Player") && !isOpened)
        {
            text.SetActive(true);
            playerInZone = true;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            text.SetActive(false);
            playerInZone = false;
            warningText.SetActive(false);
        }
    }


    private IEnumerator RotateDoor()
    {
        isRotating = true;
        float timeElapsed = 0f;

        // Gradually rotate the door over the specified duration
        while (timeElapsed < rotationDuration)
        {
            door.rotation = Quaternion.Slerp(initialRotation, finalRotation, timeElapsed / rotationDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final rotation is exactly at target
        door.rotation = finalRotation;
        isRotating = false;

        isOpened = true;
        text.SetActive(false);
    }
}