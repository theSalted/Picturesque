using UnityEngine;

public class ChestOpen : MonoBehaviour
{

    public PickUp pickUp;
    public Animator anim;
    public GameObject obj;
    public GameObject warningText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
        obj.SetActive(false);
        warningText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun
    /// touching another rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (pickUp.hasItem)
            {
                anim.SetBool("isOpen", true);
                pickUp.itemUI.SetActive(false);
                obj.SetActive(true); 
                Destroy(this.GetComponent<BoxCollider>());
            }
            else
            {
                warningText.SetActive(true);
            }
        }
    }

    /// <summary>
    /// OnTriggerExit is called when the Collider other has stopped touching the trigger.
    /// </summary>
    /// <param name="other">The other Collider involved in this collision.</param>
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            warningText.SetActive(false);
        }
    }
}
