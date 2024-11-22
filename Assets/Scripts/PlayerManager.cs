using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class PlayerManager : MonoBehaviour
{

    public static event Action<GameObject> OnInventoryEvent;

    [SerializeField]
    private GameObject _inventory;
    public GameObject inventory
    {
        get { return _inventory; }
        set {

            if (_inventory == value) return;

            if (_inventory != null)
            {
                InventoryUpdateable iu;
                iu = _inventory.GetComponent<InventoryUpdateable>();
                if (iu != null)
                {
                    iu.OnExitInventory(gameObject);
                    OnInventoryEvent -= iu.OnInventoryUpdate;
                }
            }
            
            if (value != null)
            {
                InventoryUpdateable iu;
                iu = value.GetComponent<InventoryUpdateable>();
                if (iu != null)
                {
                    iu.OnEnterInventory(gameObject);
                    OnInventoryEvent += iu.OnInventoryUpdate;
                }
            }

            _inventory = value; 
            if (_inventory == null)
            {
                InteractableDetector.Instance.enabled = true;
            } else {
                InteractableDetector.Instance.enabled = false;
            }
        }
    }

    public static PlayerManager Instance { get; private set; }

    private void Awake()
    {
        EnsureSingletonInstance();
    }

    void Update()
    {
        OnInventoryEvent?.Invoke(gameObject);
    }

    private void EnsureSingletonInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}