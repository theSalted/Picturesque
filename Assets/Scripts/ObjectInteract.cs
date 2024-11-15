using UnityEngine;

public class ObjectInteract : MonoBehaviour
{

    public GameObject interactText;
    public bool playerInZone;
    public string cutsceneID;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        interactText.SetActive(false);
        playerInZone = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInZone && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Do something");
            TriggerCutscene();
        }
    }

    void TriggerCutscene()
    {
        if (!string.IsNullOrEmpty(cutsceneID))
        {
            Debug.Log("HEREEE");
            CutsceneManager.Instance.PlayAnimatorByIdentifier(cutsceneID);
        }
        else
        {
            Debug.LogWarning("Cutscene ID is not assigned!");
        }
    }

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactText.SetActive(true);
            playerInZone = true;
        }
    }

    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactText.SetActive(false);
            playerInZone = false;
        }
    }
}
