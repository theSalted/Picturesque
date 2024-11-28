using UnityEngine;
using UnityEngine.AI;

public class Extractable : MonoBehaviour, Interactable
{
    
    private new Collider collider;
    [HideInInspector]
    public LayerMask _layerMask;


    [HideInInspector]
    public bool _isTrigger;
    
    private Outline _outline;
    [HideInInspector]
    public Outline outline {
        get {
            return _outline;
        }
        set {
            _outline = value;
        }
    }

    [SerializeField] private bool _isInteractable = true;
    public bool isInteractable {
        get {
            return _isInteractable;
        }
        set {
            _isInteractable = value;
        }
    }

    void Awake() {
        _layerMask = gameObject.layer;
        collider = GetComponent<Collider>();
        if (collider != null) {
            _isTrigger = collider.isTrigger;
            collider.isTrigger = false;
        } else {
            _isTrigger = false;
        }
        outline = gameObject.GetComponent<Outline>();
        if (outline == null) {
            outline = gameObject.AddComponent<Outline>();
        }
        gameObject.AddComponent<ExtractableRespawnable>();
        outline.OutlineColor = new Color(0.4196f, 0.8706f, 0.4392f);
        outline.enabled = false;    
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnInteract() {
        if (this.enabled) {
            // Create a new Movable component
            Movable movable = gameObject.AddComponent<Movable>();
            movable.isBeingMoved = true;
            movable._isTrigger = _isTrigger;
            movable._layerMask = _layerMask;
            ChangeLayer("Overlay");
            // Disable the Extractable component
            // Remove the Extractable component
            Destroy(this);
        }
    }

    public void OnStareEnter() {
        
    }

    public void OnStare() {
        // Since This is can be called via interface, thus bypassing rendering loop, we need to check if the component is enabled
        if (this.enabled) {
            ChangeLayer("Overlay");
            outline.enabled = true;
        }
        // collider.isTrigger = _isTrigger;
    }
    
    public void OnStareExit() {
        // Since This is can be called via interface, thus bypassing rendering loop, we need to check if the component is enabled
        if (this.enabled)  {
            ChangeLayer("Stencil");;
            collider.isTrigger = true;
            outline.enabled = false;
        }
    } 

    void ChangeLayer(string name)
    {
        gameObject.layer = LayerMask.NameToLayer(name);
        foreach (Transform child in transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer(name);
        }
    }
}
