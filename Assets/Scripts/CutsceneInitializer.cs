using UnityEngine;
using UnityEngine.Timeline;

public class CutsceneInitializer : MonoBehaviour
{
    public Animator[] anims;
    public string[] cutsceneIDs;

    void Start()
    {
        // Ensure arrays are the same length
        if (anims.Length != cutsceneIDs.Length)
        {
            Debug.LogError("Animator and Cutscene ID arrays must have the same length!");
            return;
        }

        // Register each Animator with its cutscene ID
        for (int i = 0; i < anims.Length; i++)
        {
            if (CutsceneManager.Instance != null)
            {
                Debug.Log("WTF");
            }
            else
            {
                Debug.LogError("CutsceneManager instance not found!");
            }
        }
    }
}
