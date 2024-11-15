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
    public InputSystemActions playerInputs;
    private InputAction interact;
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

    public bool _isBeingMoved = false;
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
        playerInputs = new InputSystemActions();

        // Save original states
        originalOutlineColor = outline.OutlineColor;
        originalColliderTrigger = collider.isTrigger;
        originalLayer = gameObject.layer;
        originalColliderEnabled = collider.enabled;
        originalPosition = transform.position;
    }

    void OnEnable()
    {
        InteractEnable();   
    }

    void OnDisable()
    {
        InteractDisable();
    }

    void Update()
    {
        if (isBeingMoved)
        {
            Vector3 targetPosition = CameraRayController.Instance.placePosition;

            // Always move the object to the target position
            Vector3 adjustedPosition;

            if (CameraRayController.Instance.normal == Vector3.zero)
            {
                // No valid hit, skip collision avoidance
                CameraRayController.Instance.isPlaceable = false;
                adjustedPosition = targetPosition;
            }
            else
            {
                // Valid hit, perform collision avoidance
                adjustedPosition = GetAdjustedPosition(targetPosition);
            }

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
        if (isBeingMoved && this.enabled)
        {
            // Try to place the object
            if (CameraRayController.Instance.isPlaceable)
            {
                // Place the object and stop moving
                isBeingMoved = false;
            }
        }
    }

    public void OnInteract()
    {
        if (!isBeingMoved && this.enabled)
        {
            // Start moving the object
            isBeingMoved = true;
        }
    }

    public void OnStare() {
        // Since This is can be called via interface, thus bypassing rendering loop, we need to check if the component is enabled
        if (this.enabled && !isBeingMoved) {
            outline.enabled = true;
        }
        // collider.isTrigger = _isTrigger;
    }
    
    public void OnStareExit() {
        // Since This is can be called via interface, thus bypassing rendering loop, we need to check if the component is enabled
        if (this.enabled && !isBeingMoved)  {
            outline.enabled = false;
        }
    } 

    private void StartMoving()
    {
        if (!isMovingInitialized)
        {
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            gameObject.layer = LayerMask.NameToLayer("Overlay");
            outline.enabled = true;
            collider.isTrigger = true; // Set collider to trigger during movement
            // Do not disable the collider to keep bounds valid
            // collider.enabled = false; // This line is removed
            isMovingInitialized = true;
            Destroy(gameObject.GetComponent<FallingController>());
            InteractEnable();
        }
    }

    private void StopMoving()
    {
        meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        // Restore original states
        gameObject.layer = LayerMask.NameToLayer("Default");
        outline.OutlineColor = originalOutlineColor;
        outline.enabled = false;
        collider.isTrigger = false;
        collider.enabled = true;
        isMovingInitialized = false;
        gameObject.AddComponent<FallingController>();
        InteractDisable();
    }

    private Vector3 GetAdjustedPosition(Vector3 targetPosition)
    {
        Collider objectCollider = GetComponent<Collider>();
        if (objectCollider != null)
        {
            // Get the normal of the hit surface
            Vector3 surfaceNormal = CameraRayController.Instance.normal;

            if (surfaceNormal == Vector3.zero)
            {
                // No valid surface normal, skip collision avoidance
                return targetPosition;
            }

            // Ensure the normal is normalized
            surfaceNormal = surfaceNormal.normalized;

            // Get the collider's bounds extents
            Vector3 boundsExtents = objectCollider.bounds.extents;

            // Compute the extent along the normal
            float extentAlongNormal =
                Mathf.Abs(boundsExtents.x * surfaceNormal.x) +
                Mathf.Abs(boundsExtents.y * surfaceNormal.y) +
                Mathf.Abs(boundsExtents.z * surfaceNormal.z);

            // Adjust the target position along the normal to prevent sinking into the surface
            targetPosition += surfaceNormal * extentAlongNormal;

            Vector3 objectSize = objectCollider.bounds.size;
            Vector3 halfSize = objectSize / 2f;
            int maxIterations = 10; // Maximum number of adjustments
            int iterations = 0;
            float checkDistance = 0.1f; // Distance to move along normal each iteration

            while (IsColliding(targetPosition, halfSize) && iterations < maxIterations)
            {
                // Move the target position along the normal by checkDistance
                targetPosition += surfaceNormal * checkDistance;
                iterations++;
            }

            if (iterations >= maxIterations)
            {
                // Collision could not be avoided
                CameraRayController.Instance.isPlaceable = false;
            }
            else
            {
                // Position is placeable
                CameraRayController.Instance.isPlaceable = true;
            }
        }

        return targetPosition;
    }

    private bool IsColliding(Vector3 position, Vector3 halfSize)
    {
        // Check for any colliders using OverlapBox
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

    void InteractEnable()
    {
        interact = playerInputs.Player.Interact;

        interact.Enable();

        interact.performed += OnInteractCallback;
    }

    void InteractDisable()
    {
        interact.performed -= OnInteractCallback; // Unsubscribe from the event
        interact.Disable();
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