using UnityEngine;

[CreateAssetMenu(fileName = "DialogueSO", menuName = "Scriptable Objects/DialogueSO")]
public class DialogueSO : ScriptableObject
{
    [Header("Dialogue Information")]
    [SerializeField]
    public string characterName;
    [SerializeField]
    public Sprite characterImage;
    [SerializeField]
    [TextArea(3, 10)]
    public string[] dialogueContent;
}
