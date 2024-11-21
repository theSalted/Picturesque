using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueSO dialogueToTrigger;
    [SerializeField] private DialogueManager dialogueManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Start the dialogue
            dialogueManager.StartDialogue(dialogueToTrigger);
        }
    }
}