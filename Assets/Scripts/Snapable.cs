using System.Linq;
using System.Runtime.Serialization.Json;
using UnityEngine;

public class Snapable : MonoBehaviour
{
    public GameObject target;

    void Reset()
    {
        Collider[] colliders = GetComponents<Collider>();
        Collider[] childrenColliders = GetComponentsInChildren<Collider>();
        colliders = colliders.Concat(childrenColliders).ToArray();

        foreach (Collider collider in colliders)
        {
            if (!collider.isTrigger) {
                Debug.LogWarning("SnapController: Collider can't be trigger, disabling trigger");
                collider.isTrigger = true;
            }
        }

        MeshRenderer[] meshRenderers = GetComponents<MeshRenderer>();
        MeshRenderer[] childrenMeshRenderers = GetComponentsInChildren<MeshRenderer>();
        meshRenderers = meshRenderers.Concat(childrenMeshRenderers).ToArray();

        foreach (MeshRenderer meshRenderer in meshRenderers)
        {
            meshRenderer.enabled = false;
        }
    }
}
