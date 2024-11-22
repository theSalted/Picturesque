using UnityEngine;

public class CutsceneTrigger : MonoBehaviour
{
    
    public string cutsceneID;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// OnTriggerEnter is called when the Collider other enters the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerCutscene();
        }
    }

    void TriggerCutscene()
    {
        if (!string.IsNullOrEmpty(cutsceneID))
        {
            CutsceneManager.Instance.PlayAnimatorByIdentifier(cutsceneID);
        }
        else
        {
            Debug.LogWarning("Cutscene ID is not assigned!");
        }
    }
}
