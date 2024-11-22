using UnityEngine;

[ExecuteAlways]
public class BackdropController : MonoBehaviour
{
    private Bounds _bounds;

    private void OnDrawGizmos()
    {
        if (CalculateBounds())
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_bounds.center, _bounds.size);
        }
    }

    private bool CalculateBounds()
    {
        bool hasBounds = false;
        _bounds = new Bounds(Vector3.zero, Vector3.zero);

        foreach (Transform child in transform)
        {
            Collider collider = child.GetComponent<Collider>();

            if (collider != null)
            {
                if (!hasBounds)
                {
                    _bounds = collider.bounds;
                    hasBounds = true;
                }
                else
                {
                    _bounds.Encapsulate(collider.bounds);
                }
            }
        }

        return hasBounds;
    }
}