using UnityEngine;

public class ReticleManager : MonoBehaviour
{
    public static ReticleManager Instance { get; private set; }

    public enum ReticleState
    {
        Normal,
        Interactive
    }

    private ReticleState _currentReticleState = ReticleState.Normal;
    public ReticleState CurrentReticleState
    {
        get { return _currentReticleState; }
        set { _currentReticleState = value; }
    }

    private string _label = "";
    public string Label
    {
        get { return _label; }
        set { _label = value; }
    }

    void Awake()
    {
        EnsureSingletonInstance();
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
            Debug.LogError("Multiple instances of ReticleManager detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }
}