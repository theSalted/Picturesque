using System;
using System.Collections.Generic;
using UnityEngine;

public class SnapableDetector : MonoBehaviour
{
    public static SnapableDetector Instance { get; private set; }

    private List<Movable> movableInSnap = new List<Movable>();

    void Awake()
    {
        EnsureSingletonInstance();
    }

    void OnEnable()
    {
        CameraRayController.OnRaycastEvent += OnRaycastReceived;
    }

    void OnDisable()
    {
        CameraRayController.OnRaycastEvent -= OnRaycastReceived;
    }

    private void OnRaycastReceived(Ray ray) {
        DetectPlaceable(ray);
    }

    private void DetectPlaceable(Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, CameraRayController.Instance.rayLength))
        {
            Snapable snapable = hit.collider.gameObject.GetComponent<Snapable>();
            if (snapable != null)
            {
                Movable snapableTarget = snapable.target.GetComponent<Movable>();
                if (snapableTarget == null)
                {
                    return;
                }
                if (snapableTarget.isBeingMoved && snapableTarget != null && snapableTarget.overwriteTransform == null)
                {
                    Debug.Log("Add to snap");
                    movableInSnap.Add(snapableTarget);
                    snapableTarget.overwriteTransform = snapable.gameObject.transform;
                }

                return;

            } else {
                GameObject target = hit.collider.gameObject;
                if (target.GetComponent<Snapable>() != null) {
                    return;
                }

                if (target.GetComponent<Movable>() != null) {
                    Movable movable = target.GetComponent<Movable>();
                    if (movable.isBeingMoved || movable.overwriteTransform != null) {
                        return;
                    }
                }   
            }
        }

        if (movableInSnap.Count <= 0)
        {
            return;
        }

        foreach (Movable movable in movableInSnap)
        {
            movable.overwriteTransform = null;
        }
        movableInSnap.Clear();
    }

    private void EnsureSingletonInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.LogError("Multiple instances of PlaceableDetector detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }
}
