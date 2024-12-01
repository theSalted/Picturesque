using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour, Interactable
{
    public float rotationDuration; // Duration of rotation in seconds
    public float targetRotationY; // Target Y rotation angle
    // public bool isRotating = false;      // To check if the door is already rotating
    // public GameObject text;
    public bool playerInZone = false;
    public Transform door;
    public bool isOpened = false; 
    // public GameObject warningText;
    public GameObject unlockItem = null;
    public Transform snapPoint = null;
    // public bool key = false;
    private Quaternion initialRotation;   // The original rotation of the door
    private Quaternion finalRotation;     // The final rotation of the door

    // public GameObject keyUI;

    // Implement the isInteractable property
    /// <summary>
    ///  Private store of the isInteractable property.
    /// </summary>
    private bool disableInteractable = false;

    /// <summary>
    /// Public getter and setter for the isInteractable property.
    /// </summary>
    public bool isInteractable
    {
        get { 
            // if (disableInteractable) {
            //     return false;
            // }

            if (PlayerManager.Instance.inventory == unlockItem) {
                return true;
            }
            
            return true; 
        }
        set { 
            disableInteractable = value; 
        }
    }

    /// <summary>
    /// Private store of the outline property.
    /// </summary>
    private Outline _outline;

    /// <summary>
    /// Public getter and setter for the outline property.
    /// </summary>
    public Outline outline
    {
        get { return _outline; }
        set { _outline = value; }
    }

    void Awake() {
        outline = gameObject.GetComponent<Outline>();
        if (outline == null) {
            outline = gameObject.AddComponent<Outline>();
        }
        outline.OutlineColor = new Color(0.4196f, 0.8706f, 0.4392f);
        outline.enabled = false;
    }
    
    void Start()
    {
        // Store the initial rotation of the door
        initialRotation = door.rotation;
        // Calculate the final rotation of the door (rotate 90 degrees on the Y-axis)
        finalRotation = Quaternion.Euler(0, targetRotationY, 0) * initialRotation;
        // text.SetActive(false);
        // warningText.SetActive(false);
    }

    public void OnInteract()
    {
        if (isInteractable && unlockItem == null && !isOpened)
        {
            // text.SetActive(false);
            StartCoroutine(RotateDoor());
        }
        else
        {
            // text.SetActive(false);
            // warningText.SetActive(true);
        }
    }

    public void OnStare()
    {
        // Since this can be called via interface, thus bypassing rendering loop, we need to check if the component is enabled
        if (this.enabled)
        {
            outline.enabled = true;
        }
    }

    public void OnStareExit()
    {
        // Since this can be called via interface, thus bypassing rendering loop, we need to check if the component is enabled
        if (this.enabled)
        {
            outline.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject == unlockItem && !isOpened)
        {
            // text.SetActive(true);
            if (snapPoint != null) {
                unlockItem.transform.position = snapPoint.position;
                unlockItem.transform.rotation = snapPoint.rotation;
                unlockItem.transform.SetParent(snapPoint);
                
                Movable movable = unlockItem.GetComponent<Movable>();
                if (movable != null) {
                    movable.isBeingMoved = false;
                    Destroy(movable);
                }
                Outline outline = unlockItem.GetComponent<Outline>();
                if (outline != null) {
                    outline.enabled = false;
                    Destroy(outline);
                }
                FallingController fallingController = unlockItem.GetComponent<FallingController>();
                if (fallingController != null) {
                    Destroy(fallingController);
                }
            }
            StartCoroutine(RotateDoor());
        }
    }

    

//    void Update()
//     {
//         // Check if the player is in the trigger zone and the "O" key is pressed
//         if (playerInZone && !isRotating && !isOpened && Input.GetKeyDown(KeyCode.E))
//         {
//             if (pickUp.hasItem)
//             {
//                 text.SetActive(false);
//                 StartCoroutine(RotateDoor());
//             }
//             else
//             {
//                 text.SetActive(false);
//                 warningText.SetActive(true);
//             }
//             // keyUI.SetActive(false);
//         }
//     }

    // private void OnTriggerEnter(Collider col)
    // {
    //     // Check if the player collided with the door
    //     if (col.CompareTag("Player") && !isOpened)
    //     {
    //         text.SetActive(true);
    //         playerInZone = true;
    //     }
    // }

    // void OnTriggerExit(Collider col)
    // {
    //     if (col.CompareTag("Player"))
    //     {
    //         text.SetActive(false);
    //         playerInZone = false;
    //         warningText.SetActive(false);
    //     }
    // }


    private IEnumerator RotateDoor()
    {
        isOpened = true;
        // isRotating = true;
        float timeElapsed = 0f;

        // Gradually rotate the door over the specified duration
        while (timeElapsed < rotationDuration)
        {
            door. rotation = Quaternion.Slerp(initialRotation, finalRotation, timeElapsed / rotationDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final rotation is exactly at target
        door.rotation = finalRotation;
        // isRotating = false;

        isOpened = true;
        // text.SetActive(false);
    }
}