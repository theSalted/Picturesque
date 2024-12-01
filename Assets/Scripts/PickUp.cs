using Unity.VisualScripting;
using UnityEngine;

public class PickUp : Movable
{

    public override void StartMoving()
    {
        base.StartMoving();
        ChangeLayer("Ignore Raycast");

    }
    // public GameObject interactText;
    // public bool playerInZone;
    // public GameObject itemUI;
    // public bool hasItem;

    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
    //     interactText.SetActive(false);
    //     playerInZone = false;
    //     itemUI.SetActive(false);
    //     hasItem = false;
    // }

    // // Update is called once per frame
    // void Update()
    // {
    //     if (playerInZone && Input.GetKeyDown(KeyCode.E))
    //     {
    //         itemUI.SetActive(true);
    //         interactText.SetActive(false);
    //         hasItem = true;
    //         transform.parent.gameObject.SetActive(false);
    //         // Destroy(transform.parent.gameObject);
    //     }
    // }


    // /// <summary>
    // /// OnTriggerEnter is called when the Collider other enters the trigger.
    // /// </summary>
    // /// <param name="other">The other Collider involved in this collision.</param>
    // void OnTriggerEnter(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         interactText.SetActive(true);
    //         playerInZone = true;
    //     }
    // }

    // /// <summary>
    // /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    // /// </summary>
    // /// <param name="other">The other Collider involved in this collision.</param>
    // void OnTriggerExit(Collider other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         interactText.SetActive(false);
    //         playerInZone = false;
    //     }
    // }
}
