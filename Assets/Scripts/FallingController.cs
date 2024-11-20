using System;
using UnityEditor;
using UnityEngine;

public class FallingController : MonoBehaviour
{
    Rigidbody rb;
    float mass = 20;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    bool hadRb;

    private float gracePeriod = 1f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        hadRb = true;
        if (rb == null) {
            hadRb = false;
            rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = mass;
            rb.useGravity = true;
            rb.isKinematic = false;
            // give rb a random force 
            rb.AddForce(new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)) * 1, ForceMode.Impulse);
        }
    }

    void OnDestroy()
    {
        if (!hadRb) {
            Destroy(rb);
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gracePeriod -= Time.deltaTime;
        //  Monitor the object's velocity, when it stops moving, destroy it
        if (rb == null) {
            Destroy(this);
            return;
        }
        if (rb.linearVelocity.magnitude < 0.1f)
        {
            if (gracePeriod <= 0) {
                Destroy(this);
            }
        } else {
            gracePeriod = 1f;
        }
        
    }
}
