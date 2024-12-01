using System;
using System.Collections.Generic;
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

    public List<Movable> neighborMovable = new List<Movable>();

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

    private Transform playerTransform;
    private Quaternion initialRotationOffset;

    // Add the new public Transform
    public Transform overwriteTransform;

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

        // Get the player's transform (assuming the player has the tag "Player")
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
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
            
            
            Vector3 targetPosition = MovableDetector.Instance.placePosition;

            // Always move the object to the target position
            Vector3 adjustedPosition;
            Quaternion adjustedRotation = playerTransform.rotation * initialRotationOffset;

            if (MovableDetector.Instance.normal == Vector3.zero)
            {
                // No valid hit, skip collision avoidance
                adjustedPosition = targetPosition;
            }
            else
            {
                // Valid hit, perform collision avoidance
                adjustedPosition = GetAdjustedPosition(targetPosition);
            }

            if (overwriteTransform != null)
            {
                // Snap to overwriteTransform's position and rotation
                adjustedPosition = overwriteTransform.position;
                adjustedRotation = overwriteTransform.rotation;
                Debug.Log("SnapController: Snapping to target");
            }

            // Lerp towards the adjusted target position
            transform.position = Vector3.Lerp(transform.position, adjustedPosition, lerpSpeed * Time.deltaTime);

            // Update the object's rotation to follow the player
            transform.rotation = Quaternion.Lerp(transform.rotation, adjustedRotation, lerpSpeed * Time.deltaTime);

            // Update the outline color based on the isPlaceable state
            if (MovableDetector.Instance.isPlaceable)
            {
                outline.OutlineColor = new Color(0.4196f, 0.8706f, 0.4392f); // Green color
            }
            else
            {
                outline.OutlineColor = Color.red;
            }
        }
    }

    public virtual void OnInteractCallback(InputAction.CallbackContext context)
    {
        if (isBeingMoved && this.enabled)
        {
            // Try to place the object
            if (MovableDetector.Instance.isPlaceable)
            {
                // Place the object and stop moving
                isBeingMoved = false;
            }
        }
    }

    public void OnStareEnter()
    {
        // Optional implementation
    }

    public void OnInteract()
    {
        if (!isBeingMoved && this.enabled)
        {
            // Start moving the object
            isBeingMoved = true;
        }
    }

    public void OnStare()
    {
        // Since this can be called via interface, thus bypassing rendering loop, we need to check if the component is enabled
        if (this.enabled && !isBeingMoved)
        {
            outline.enabled = true;
        }
    }

    public void OnStareExit()
    {
        // Since this can be called via interface, thus bypassing rendering loop, we need to check if the component is enabled
        if (this.enabled && !isBeingMoved)
        {
            outline.enabled = false;
        }
    }

    public virtual void StartMoving()
    {
        if (!isMovingInitialized)
        {
            if (meshRenderer != null)
            {
                meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            PlayerManager.Instance.inventory = gameObject;
            ChangeLayer("Overlay");
            outline.enabled = true;
            collider.isTrigger = true; // Set collider to trigger during movement
            isMovingInitialized = true;
            Destroy(gameObject.GetComponent<FallingController>());
            InteractEnable();

            if (neighborMovable.Count > 0)
            {
                foreach (Movable movable in neighborMovable)
                {
                    movable.gameObject.AddComponent<FallingController>();
                }
            }
            overwriteTransform = null; // Reset overwriteTransform when movement starts

            // Compute the initial rotation offset
            initialRotationOffset = Quaternion.Inverse(playerTransform.rotation) * transform.rotation;
        }
    }

    public virtual void StopMoving()
    {
        if (meshRenderer != null)
        {
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        }
        // Restore original states
        PlayerManager.Instance.inventory = null;
        ChangeLayer("Default");
        outline.OutlineColor = originalOutlineColor;
        outline.enabled = false;
        collider.isTrigger = false;
        collider.enabled = true;
        isMovingInitialized = false;
        OnStopMoving();
        InteractDisable();
        overwriteTransform = null; // Reset overwriteTransform when movement stops
    }

    public virtual void OnStopMoving()
    {
        gameObject.AddComponent<FallingController>();
    }

    private Vector3 GetAdjustedPosition(Vector3 targetPosition)
    {
        Collider objectCollider = GetComponent<Collider>();
        if (objectCollider != null)
        {
            // Get the normal of the hit surface
            Vector3 surfaceNormal = MovableDetector.Instance.normal;

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
            int maxIterations = 5; // Maximum number of adjustments
            int iterations = 0;
            float checkDistance = 0.1f; // Distance to move along normal each iteration

            while (IsColliding(targetPosition, halfSize) && iterations < maxIterations)
            {
                // Move the target position along the normal by checkDistance
                targetPosition += surfaceNormal * checkDistance;
                iterations++;
            }

            // If the object is still colliding, set isPlaceable to false

            MovableDetector.Instance.isPlaceable = true;
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

    public virtual void OnDrawGizmos()
    {
        if (isBeingMoved)
        {
            // Optional: Draw a gizmo to show where the overlap box is checking
            Gizmos.color = Color.red;
            Vector3 objectSize = collider != null ? collider.bounds.size : Vector3.one;
            Gizmos.DrawWireCube(MovableDetector.Instance.placePosition, objectSize);
        }
    }

    public void ChangeLayer(string name)
    {
        gameObject.layer = LayerMask.NameToLayer(name);
        foreach (Transform child in transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer(name);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Movable movable = collision.gameObject.GetComponent<Movable>();
        if (movable != null)
        {
            neighborMovable.Add(movable);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        Movable movable = collision.gameObject.GetComponent<Movable>();
        if (movable != null)
        {
            neighborMovable.Remove(movable);
        }
    }
}