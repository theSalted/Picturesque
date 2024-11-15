using UnityEngine;
using Cinemachine;

public class DragAndDrop : MonoBehaviour
{
    private GameObject selectedObject;
    private Vector3 offset;

    [SerializeField]
    private LayerMask interactableLayer;

    private CinemachineBrain cinemachineBrain;

    void Start()
    {
        // Get the CinemachineBrain component from the main camera
        cinemachineBrain = Camera.main.GetComponent<CinemachineBrain>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button down
        {
            TryPickUpObject();
        }
        else if (Input.GetMouseButtonUp(0)) // Left mouse button up
        {
            DropObject();
        }

        if (selectedObject != null)
        {
            DragObject();
        }
    }

    void TryPickUpObject()
    {
        // Get the active virtual camera
        var activeVirtualCamera = cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
        if (activeVirtualCamera == null)
        {
            Debug.LogWarning("No active Cinemachine Virtual Camera found.");
            return;
        }

        // Use the virtual camera's position and forward direction for raycasting
        Ray ray = new Ray(activeVirtualCamera.transform.position, activeVirtualCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, interactableLayer))
        {
            if (hitInfo.collider.CompareTag("Interactable"))
            {
                selectedObject = hitInfo.collider.gameObject;
                offset = selectedObject.transform.position - hitInfo.point;
                Debug.Log("Picked up: " + selectedObject.name);
            }
            else
            {
                Debug.Log("Hit object without Interactable tag.");
            }
        }
        else
        {
            Debug.Log("Raycast did not hit any objects.");
        }
    }

    void DragObject()
    {
        var activeVirtualCamera = cinemachineBrain.ActiveVirtualCamera as CinemachineVirtualCamera;
        if (activeVirtualCamera == null) return;

        Ray ray = new Ray(activeVirtualCamera.transform.position, activeVirtualCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
        {
            selectedObject.transform.position = hitInfo.point + offset;
        }
    }

    void DropObject()
    {
        if (selectedObject != null)
        {
            Debug.Log("Dropped: " + selectedObject.name);
            selectedObject = null;
        }
    }
}
