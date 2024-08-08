using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    PlayerInput playerInput;
    InputAction moveAction;
    InputAction shootAction;

    private NetworkVariable<Vector2> moveInput = new();

    [SerializeField] private float playerSpeed = 10;
    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Move");
        shootAction = playerInput.actions.FindAction("Shoot");
    }

    void Update()
    {
        if (IsLocalPlayer)
        {
            ReadInput();
        }
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            transform.position += (Vector3)moveInput.Value;
        }
    }

    private void ReadInput()
    {
        
    }

    [Rpc(SendTo.Server)]
    private void MoveRPC(Vector2 data)
    {
        moveInput.Value = data;
    }
    
}
