using System.Collections;
using UnityEngine;

public class ClimbController : MonoBehaviour
{

    public float speed;
    public Rigidbody rb;
    private bool inZone;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        inZone = false;
        Debug.Log("HEER");
    }

    // Update is called once per frame
    void Update()
    {
        if (inZone && Input.GetButton("Up"))
        {
            rb.AddForce(transform.up * speed, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// OnCollisionEnter is called when this collider/rigidbody has begun
    /// touching another rigidbody/collider.
    /// </summary>
    /// <param name="other">The Collision data associated with this collision.</param>
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Climable"))
        {
            Debug.Log("Detect");
            inZone = true;
        }
    }
}
