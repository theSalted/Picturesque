using UnityEngine;
using UnityEngine.InputSystem;

public class Movable : MonoBehaviour, Interactable
{
    [SerializeField] private bool _isInteractable = true;

    private new Collider collider;
    private Outline _outline;
    private MeshRenderer meshRenderer;

    [HideInInspector]
    public bool _isTrigger;
    [HideInInspector]
    public LayerMask _layerMask;

    private Color originalOutlineColor;
    private bool originalColliderTrigger;
    private int originalLayer;
    private bool originalColliderEnabled;
    private Vector3 originalPosition;

    private bool isMovingInitialized = false;
    public float lerpSpeed = 10f; // Speed of interpolation for smooth movement

    public Outline outline
    {
        get { return _outline; }
        set { _outline = value; }
    }

    public bool isInteractable
    {
        get { return _isInteractable; }
        set { _isInteractable = value; }
    }

    private bool _isBeingMoved = false;
    public bool isBeingMoved
    {
        get { return _isBeingMoved; }
        set
        {
            if (_isBeingMoved != value)
            {
                _isBeingMoved = value;
                if (_isBeingMoved)
                {
                    StartMoving();
                }
                else
                {
                    StopMoving();
                }
            }
        }
    }

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        collider = GetComponent<Collider>();
        _isTrigger = collider.isTrigger;
        outline = gameObject.GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
        }

        // Save original states
        originalOutlineColor = outline.OutlineColor;
        originalColliderTrigger = collider.isTrigger;
        originalLayer = gameObject.layer;
        originalColliderEnabled = collider.enabled;
        originalPosition = transform.position;
    }

    void Update()
    {
        if (isBeingMoved)
        {
            Vector3 targetPosition = CameraRayController.Instance.placePosition;

            // Adjust targetPosition to account for collisions and bounding box
            Vector3 adjustedPosition = GetAdjustedPosition(targetPosition);

            // Lerp towards the adjusted target position
            transform.position = Vector3.Lerp(transform.position, adjustedPosition, lerpSpeed * Time.deltaTime);

            // Update the outline color based on the isPlaceable state
            if (CameraRayController.Instance.isPlaceable)
            {
                outline.OutlineColor = new Color(0.4196f, 0.8706f, 0.4392f); // green color
            }
            else
            {
                outline.OutlineColor = Color.red;
            }
        }
    }

    public void OnInteractCallback(InputAction.CallbackContext context)
    {
        
    }

    public void OnInteract()
    {
        Debug.Log("On Interact");
        if (isBeingMoved)
        {
            // Try to place the object
            if (CameraRayController.Instance.isPlaceable)
            {
                Debug.Log("Placed");
                // Place the object and stop moving
                isBeingMoved = false;
            }
        }
        else
        {
            Debug.Log("Start Moving");
            // Start moving the object
            isBeingMoved = true;
        }
    }

    private void StartMoving()
    {
        if (!isMovingInitialized)
        {
            gameObject.layer = LayerMask.NameToLayer("Overlay");
            outline.enabled = true;
            collider.isTrigger = false;
            collider.enabled = false;
            isMovingInitialized = true;
        }
    }

    private void StopMoving()
    {
        // Restore original states
        gameObject.layer = originalLayer;
        outline.OutlineColor = originalOutlineColor;
        outline.enabled = false;
        collider.isTrigger = originalColliderTrigger;
        collider.enabled = originalColliderEnabled;
        isMovingInitialized = false;
    }

    private Vector3 GetAdjustedPosition(Vector3 targetPosition)
    {
        Collider objectCollider = GetComponent<Collider>();
        if (objectCollider != null)
        {
            Vector3 objectSize = objectCollider.bounds.size;
            Vector3 halfSize = objectSize / 2f;
            int maxIterations = 5; // Maximum number of upward adjustments
            int iterations = 0;
            float checkDistance = 0.1f; // Distance to move up each iteration

            while (IsColliding(targetPosition, halfSize) && iterations < maxIterations)
            {
                // Move the target position up by checkDistance
                targetPosition += Vector3.up * checkDistance;
                iterations++;
            }

            if (iterations >= maxIterations)
            {
                // Debug.LogWarning("Max iterations reached, possible collision could not be avoided.");
                CameraRayController.Instance.isPlaceable = false;
            }
        }

        return targetPosition;
    }

    private bool IsColliding(Vector3 position, Vector3 halfSize)
    {
        // Check for any colliders in the default layer using OverlapBox
        Collider[] colliders = Physics.OverlapBox(position, halfSize, Quaternion.identity);

        // Exclude this object's own collider
        foreach (Collider c in colliders)
        {
            if (c != collider)
            {
                return true;
            }
        }

        return false;
    }

    void OnDrawGizmos()
    {
        if (isBeingMoved)
        {
            // Optional: Draw a gizmo to show where the overlap box is checking
            Gizmos.color = Color.red;
            Vector3 objectSize = collider != null ? collider.bounds.size : Vector3.one;
            Gizmos.DrawWireCube(CameraRayController.Instance.placePosition, objectSize);
        }
    }
}