using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    [field: SerializeField] public CharacterController characterController { get; private set; }
    [field: SerializeField] public Transform viewTransform { get; private set; }
    
    private MovementController _characterController;
    private ViewController _viewController;
    
    private void Awake()
    {
        _characterController ??= new(characterController);
        _viewController ??= new(viewTransform);
        
        _characterController.Enable();
        _viewController.Enable();
    }

    private void Update()
    {
        _characterController.ProcessMove(Time.deltaTime);
        _characterController.ProcessRotate(_viewController.GetPitch(), Time.deltaTime);
        _viewController.Process();
    }
}
