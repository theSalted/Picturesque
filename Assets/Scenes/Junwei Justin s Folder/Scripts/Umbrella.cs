using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Umbrella : MonoBehaviour, Interactable, InventoryUpdateable
{
    private Outline _outline;
    [HideInInspector]
    public Outline outline
    {
        get
        {
            return _outline;
        }
        set
        {
            _outline = value;
        }
    }

    [SerializeField] private bool _isInteractable = true;
    public bool isInteractable
    {
        get
        {
            return _isInteractable;
        }
        set
        {
            _isInteractable = value;
        }
    }

    private Rigidbody playerRigidbody;
    private float originalDrag;

    private void Awake()
    {
        outline = gameObject.GetComponent<Outline>();
        if (outline == null)
        {
            outline = gameObject.AddComponent<Outline>();
        }

        outline.OutlineColor = new Color(0.4196f, 0.8706f, 0.4392f);
        outline.enabled = false;
    }
    public void OnInteract()
    {
        if (this.enabled)
        {
            outline.enabled = false;
            PlayerManager.Instance.inventory = gameObject;
        }
    }

    public void resetDrag()
    {
        if (playerRigidbody != null)
        {

        }
    }

    public void OnStareEnter()
    {

    }

    public void OnStare()
    {
        if (this.enabled)
        {
            outline.enabled = true;
        }
    }

    public void OnStareExit()
    {
        if (this.enabled)
        {
            outline.enabled = false;
        }
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void InventoryUpdateable.OnEnterInventory(GameObject owner)
    {
        owner.AddComponent<SlowFallingEffect>();
    }

    void InventoryUpdateable.OnExitInventory(GameObject owner)
    {
        Destroy(owner.GetComponent<SlowFallingEffect>());
    }

    void InventoryUpdateable.OnInventoryUpdate(GameObject owner)
    {
    }
}
