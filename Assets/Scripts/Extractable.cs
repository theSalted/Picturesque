using UnityEngine;

public class Extractable : MonoBehaviour, Interactable
{
    [SerializeField] private bool _isInteractable = true;
    private Collider collider;
    private LayerMask _layerMask;
    private bool _isTrigger;
    
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
        _isTrigger = collider.isTrigger;
        collider.isTrigger = false;
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

    }

    public void OnStare() {
        gameObject.layer = LayerMask.NameToLayer("Default");
        collider.isTrigger = _isTrigger;
    }
    
    public void OnStareExit() {
        gameObject.layer = _layerMask;
        collider.isTrigger = false;
    } 
}
