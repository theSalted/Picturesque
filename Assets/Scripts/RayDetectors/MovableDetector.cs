using UnityEngine;



public class MovableDetector : MonoBehaviour
{

    public static MovableDetector Instance { get; private set; }

    private bool _isPlaceable = false;

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

        int layerMask = ~LayerMask.GetMask("Overlay", "Placeable");

        RaycastHit hitData;

        if (Physics.Raycast(ray, out hitData, CameraRayController.Instance.rayLength, layerMask))
        // if (Physics.Raycast(ray, out hitData, CameraRayController.Instance.rayLength))
        {

            isPlaceable = true;

            placePosition = hitData.point;

            normal = hitData.normal;

            Color debugColor = Color.blue;

            interactableLabel = "Place";

            if (hitData.collider.gameObject.CompareTag("Unplaceable"))
            {

                isPlaceable = false;

                placePosition = ray.origin + ray.direction * CameraRayController.Instance.rayLength / 2;

                normal = Vector3.zero;

                interactableLabel = "Can't Place Here";

                debugColor = Color.black;

            }

            // Draw debug visuals...



            // Update ReticleManager

            // ReticleManager.Instance.CurrentReticleState = ReticleManager.ReticleState.Interactive;
            if (ReticleManager.Instance != null)
                ReticleManager.Instance.Label = interactableLabel;

        }
        else
        {

            isPlaceable = true;

            placePosition = ray.origin + ray.direction * CameraRayController.Instance.rayLength / 2;

            normal = Vector3.zero;



            interactableLabel = "Must Be Placed On Ground";



            // Update ReticleManager

            // ReticleManager.Instance.CurrentReticleState = ReticleManager.ReticleState.Normal;

            ReticleManager.Instance.Label = "";

        }
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