using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PlayerInputSystem;


public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI contentText;
    [SerializeField] private Image characterImage;

    [Header("Player References")]
    [SerializeField] private FirstPersonController firstPersonController;
    [SerializeField] private PlayerInputs playerInput;

    private DialogueSO currentDialogue;
    private int currentDialogueIndex = 0;
    private bool isDialogueActive = false;

    private void Update()
    {
        // Check if dialogue is active and player pressed interaction key
        if (isDialogueActive && playerInput.interact)
        {
            ContinueDialogue();
            // Reset interact input to prevent multiple triggers
            playerInput.interact = false;
        }
    }

    public void StartDialogue(DialogueSO dialogue)
    {
        // Disable player movement and looking
        firstPersonController.enabled = false;

        // Setup dialogue UI
        currentDialogue = dialogue;
        currentDialogueIndex = 0;
        isDialogueActive = true;

        dialoguePanel.SetActive(true);
        UpdateDialogueUI();
    }

    private void ContinueDialogue()
    {
        currentDialogueIndex++;

        // Check if dialogue has ended
        if (currentDialogueIndex >= currentDialogue.dialogueContent.Length)
        {
            EndDialogue();
            return;
        }

        UpdateDialogueUI();
    }

    private void UpdateDialogueUI()
    {
        nameText.text = currentDialogue.characterName;
        contentText.text = currentDialogue.dialogueContent[currentDialogueIndex];
        characterImage.sprite = currentDialogue.characterImage;
    }

    private void EndDialogue()
    {
        // Re-enable player movement and looking
        firstPersonController.enabled = true;

        // Hide dialogue panel
        dialoguePanel.SetActive(false);
        isDialogueActive = false;
        currentDialogue = null;
        currentDialogueIndex = 0;
    }
}