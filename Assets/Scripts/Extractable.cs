using UnityEngine;

public class Extractable : MonoBehaviour, Interactable
{
    [SerializeField] private bool _isInteractable = true;
    private LayerMask _layerMask;
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
        _layerMask = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
    
    public void OnStareExit() {
        gameObject.layer = LayerMask.NameToLayer("Stencil");
    } 
}
