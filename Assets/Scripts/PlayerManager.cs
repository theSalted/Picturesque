using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    private GameObject _invetory;
    public GameObject inventory
    {
        get { return _invetory; }
        set { 
            _invetory = value; 
            if (_invetory == null)
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

    private void EnsureSingletonInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}