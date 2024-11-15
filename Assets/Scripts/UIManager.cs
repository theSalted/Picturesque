using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public GameObject reticleInteractiveUI;
    public GameObject reticleNormalUI;
    public Text labelText; // Assuming you have a Text component for the label

    private void Awake()
    {
        EnsureSingletonInstance();
    }

    private void OnEnable()
    {
        // Optionally, subscribe to events if needed
    }

    private void OnDisable()
    {
        // Optionally, unsubscribe from events if needed
    }

    private void EnsureSingletonInstance()
    {
        if (Instance == null)
        {
            Instance = this;
            // Optionally, make the singleton persistent across scenes
            // DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Debug.LogError("Multiple instances of UIManager detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        UpdateReticle();
    }

    private void UpdateReticle()
    {
        var reticleState = ReticleManager.Instance.CurrentReticleState;
        var label = ReticleManager.Instance.Label;

        switch (reticleState)
        {
            case ReticleManager.ReticleState.Normal:
                reticleInteractiveUI.SetActive(false);
                reticleNormalUI.SetActive(true);
                break;
            case ReticleManager.ReticleState.Interactive:
                reticleInteractiveUI.SetActive(true);
                reticleNormalUI.SetActive(false);
                break;
        }

        // Update the label text if applicable
        if (labelText != null)
        {
            labelText.text = label;
        }
    }
}