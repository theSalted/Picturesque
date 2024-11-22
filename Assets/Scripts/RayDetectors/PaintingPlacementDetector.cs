using UnityEngine;

public class PaintingPlacementDetector : MonoBehaviour
{
    public static PaintingPlacementDetector Instance { get; private set; }
    private bool _isPlaceable = true;

    public bool isPlaceable
    {
        get { return _isPlaceable; }
        set
        {
            if (value != _isPlaceable)
            {
                _isPlaceable = value;
                UpdateReticleManager();
            }
        }
    }

    public Vector3 placePosition = Vector3.zero;
    public Vector3 normal = Vector3.zero;
    private string _interactableLabel = "Place";

    public string interactableLabel
    {
        get { return _interactableLabel; }
        set
        {
            if (value != _interactableLabel)
            {
                _interactableLabel = value;
                UpdateReticleManager();
            }
        }
    }

    void Awake()
    {
        EnsureSingletonInstance();
    }

    void OnEnable()
    {
        CameraRayController.OnRaycastEvent += OnRaycastReceived;
    }

    void OnDisable()
    {
        CameraRayController.OnRaycastEvent -= OnRaycastReceived;
    }

    private void EnsureSingletonInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.LogError("Multiple instances of PlaceableDetector detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    private void OnRaycastReceived(Ray ray)
    {
        DetectPlaceable(ray);
    }

    private void DetectPlaceable(Ray ray)
    {
        // int layerMask = ~LayerMask.GetMask("Overlay");
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, CameraRayController.Instance.rayLength))
        {
        //    isPlaceable = false;
        }
        else
        {
            // isPlaceable = true;
        }

        placePosition = ray.origin + ray.direction * CameraRayController.Instance.rayLength;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateReticleManager()
    {
        if (isPlaceable)
        {
            // ReticleManager.Instance.CurrentReticleState = ReticleManager.ReticleState.Interactive;
            ReticleManager.Instance.Label = interactableLabel;
        }
        else
        {
            // ReticleManager.Instance.CurrentReticleState = ReticleManager.ReticleState.Normal;
            ReticleManager.Instance.Label = "";
        }
    }
}
