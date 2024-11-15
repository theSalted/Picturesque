using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraRayController : MonoBehaviour
{
    public static CameraRayController Instance { get; private set; }
    
    [Header("Ray Settings")]
    public InputSystemActions playerInputs;
    public float rayLength = 5f;

    [Header("Reticle Assets")]
    public GameObject reticleInteractiveUI;
    public GameObject reticleNormalUI;
    
    private GameObject player;
    private InputAction interact;
    
    private HashSet<GameObject> staredObjects = new HashSet<GameObject>();

    /// <summary>
    /// This delegate is triggered when the player start and stopped aiming at an interactable object. 
    /// </summary>
    /// <param name="start"> True if the player started aiming at an interactable object, false if the player stopped aiming at an interactable object. </param>
    public delegate void OnInteractable(bool start, string label);
    public static event OnInteractable OnInteractableEvent;

    private string _interactableLabel = "Interact";

    public string interactableLabel {
        get {
            return _interactableLabel;
        }

        set {
            // Update only if the value is different.
            if (value != _interactableLabel) {
                OnInteractableEvent?.Invoke(isInteractable, value);
                _interactableLabel = value;
            }
        }
    }

    public delegate void OnInteractedWithItem(GameObject item);
    public static event OnInteractedWithItem OnInteractedWithItemEvent;

    private bool _isInteractable = false;

    public bool isInteractable {
        get {
            return _isInteractable;
        }

        set {
            // Update only if the value is different.
            if (value != _isInteractable) {
                OnInteractableEvent?.Invoke(value, interactableLabel);
                _isInteractable = value;
            }
        }
    }

    [HideInInspector]
    public bool isPlaceable = false;
    public Vector3 placePosition = new Vector3(0, 0, 0);
    public Vector3 normal = new Vector3(0, 0, 0);

    void Awake()
    {
        EnsureSingletonInstance();
        player = GameObject.FindGameObjectWithTag("Player");
        playerInputs = new InputSystemActions();
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        placePosition = ray.origin + ray.direction * rayLength / 2;
    }

    void OnEnable()
    {
        interact = playerInputs.Player.Interact;

        interact.Enable();

        interact.performed += OnInteractCallback;
    }

    void OnDisable()
    {
        interact.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetReticle();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        ResetReticle();
        DetectIneractable(ray);
        DetectPlaceable(ray);
        Debug.DrawRay(transform.position, transform.forward * rayLength, Color.red);
    }

    public void OnInteractCallback(InputAction.CallbackContext context)
    {
        CheckInteract(null);
    }

    private void DetectPlaceable(Ray ray) {
        int allLayersMask = ~0;
        RaycastHit hitData;
        
        if (Physics.Raycast(ray, out hitData, rayLength, allLayersMask))
        {
            isPlaceable = true;
            placePosition = hitData.point;
            normal = hitData.normal;
            Color debugColor = Color.blue;

            interactableLabel = "Place";
            
            if (hitData.collider.gameObject.tag == "Unplaceable") {
                isPlaceable = false;
                placePosition = ray.origin + ray.direction * rayLength / 2;
                normal = new Vector3(0, 0, 0);
                
                interactableLabel = "Can't Place Here";
                debugColor = Color.black;
            }
            
            // Draw a small sphere at the hit point
            Debug.DrawRay(hitData.point, hitData.normal * 0.5f, debugColor, 0.1f);
            
            // Optionally, draw the impact point
            Debug.DrawLine(hitData.point - Vector3.up * 0.05f, hitData.point + Vector3.up * 0.05f, debugColor, 0.1f);
            Debug.DrawLine(hitData.point - Vector3.left * 0.05f, hitData.point + Vector3.left * 0.05f, debugColor, 0.1f);
            Debug.DrawLine(hitData.point - Vector3.forward * 0.05f, hitData.point + Vector3.forward * 0.05f, debugColor, 0.1f);
        } else {
            isPlaceable = false;
            placePosition = ray.origin + ray.direction * rayLength / 2;
            normal = new Vector3(0, 0, 0);

            interactableLabel = "Must Be Place On Ground";
        }
    } 

    private void DetectIneractable(Ray ray) {
        RaycastHit hitData;
        if (Physics.Raycast(ray, out hitData, rayLength)) 
        {
            GameObject hitObject = hitData.transform.gameObject;
            Interactable interactable = hitObject.GetComponent<Interactable>();

            interactableLabel = "Interact";

            if (hitObject.tag == "UI") {
                Debug.Log("UI");
            }
            // If the object is not interactable, return.
            if (!interactable?.isInteractable ?? true) { 
                isInteractable = false;
                OnStareExist();
                return; 
            }

            // if (hitObject.tag == "Collectable") {
            //     interactableLabel = "Collect";
            // }

            isInteractable = true;

            ReticleInteractive();
            staredObjects.Add(hitObject);
            interactable?.OnStare();
            if (!staredObjects.Contains(hitObject)) {
                interactable?.OnStareEnter();
            }
        } else {
            OnStareExist();
            isInteractable = false;
        }
    }

    private void OnStareExist() {
        if (staredObjects.Count != 0) { 
            staredObjects.ToList().ForEach(obj => obj.GetComponent<Interactable>()?.OnStareExit());
            staredObjects.Clear();
        }
    }

    private void CheckInteract(InputAction inputAction) {
        Ray ray =  Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hitData;

        if (Physics.Raycast(ray, out hitData, rayLength)) 
        {
            GameObject hitObject = hitData.transform.gameObject;
            Interactable interactable = hitObject.GetComponent<Interactable>();

            // If the object is not interactable, return.
            if (interactable == null || !interactable.isInteractable) { return; }

            // if (hitObject.tag == "Collectable") {
            //     print("collected a new object");
            // }

            interactable?.OnInteract();
            OnInteractedWithItemEvent?.Invoke(hitObject);
        } 
    }

    private void ReticleInteractive() {
        reticleInteractiveUI.SetActive(true);
        reticleNormalUI.SetActive(false);
    }

    private void ResetReticle() {
        reticleInteractiveUI.SetActive(false);
        reticleNormalUI.SetActive(true);
    }

    private void EnsureSingletonInstance() {
        if (Instance == null) {
            Instance = this;
        } else if (Instance != this) {
            Debug.LogError("There are multiple instances of the CameraRayController class.");
        }
    }
}