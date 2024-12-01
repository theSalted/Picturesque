using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class KeyBoxController : MonoBehaviour
{
    public bool boxOpen = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<FallingController>() != null && !boxOpen)
        {
            boxOpen = true;
        }

        if (boxOpen)
        {
            Movable movable = gameObject.GetComponent<Movable>();
            Destroy(movable);

            Collider colider = gameObject.GetComponent<Collider>();
            Destroy(colider);

            ExtractableRespawnable extractableRespawnable = gameObject.GetComponent<ExtractableRespawnable>();
            Destroy(extractableRespawnable);

            Extractable extractable = gameObject.GetComponent<Extractable>();
            Destroy(extractable);

            // Find all children of the game object
            foreach (Transform child in transform)
            {
                GameObject childGameObject = child.gameObject; 
                childGameObject.AddComponent<FallingController>();
                childGameObject.AddComponent<BoxCollider>();
                childGameObject.AddComponent<Movable>();
                childGameObject.AddComponent<Respawnable>();
                // Change layer to default layer mask
                ChangeLayerRecursively(child.gameObject, "Default");
                // Give a random force to the child object

                Rigidbody rb = childGameObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(new Vector3(Random.Range(-5f, 5), Random.Range(5f, 3f), Random.Range(-5f, 5f)) * 10f, ForceMode.Impulse);
                }
                
                Outline outline = childGameObject.GetComponent<Outline>();
                if (outline != null)
                {
                    outline.enabled = false;
                }
            }

            // Destroy the parent object
            Destroy(this);
        }
    }

    // Recursively change the layer of the game object and all its children

    private void ChangeLayerRecursively(GameObject obj, string layerName)
    {
        obj.layer = LayerMask.NameToLayer(layerName);
        foreach (Transform child in obj.transform)
        {
            ChangeLayerRecursively(child.gameObject, layerName);
        }
    }

}
