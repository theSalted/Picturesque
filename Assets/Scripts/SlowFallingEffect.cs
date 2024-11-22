using UnityEngine;

public class SlowFallingEffect : MonoBehaviour
{
    bool hadRigidbody = false;
    float originalLD;
    public float targetLD = 1f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            hadRigidbody = true;
        } else
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        originalLD = rb.linearDamping;
        rb.linearDamping = targetLD;
    }

    private void OnDestroy()
    {

        if (!hadRigidbody)
        {
            Destroy(rb);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
