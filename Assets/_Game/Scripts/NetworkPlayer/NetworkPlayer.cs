using System;
using System.Collections;
using System.Collections.Generic;
using JazzyLucas.Core;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(ViewController))]
public class NetworkPlayer : NetworkBehaviour
{
    public MovementController movementController { get; private set; }
    public ViewController viewController { get; private set; }
    
    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        viewController = GetComponent<ViewController>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            movementController.enabled = false;
            viewController.enabled = false;
        }
    }

    
     #region Back and Forth RPC Example
    [Rpc(SendTo.ClientsAndHost)]
    private void TestClientRpc(int value, ulong sourceNetworkObjectId)
    {
        Debug.Log($"Client Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
        if (IsOwner)
        {
            TestServerRpc(value + 1, sourceNetworkObjectId);
        }
    }
    [Rpc(SendTo.Server)]
    private void TestServerRpc(int value, ulong sourceNetworkObjectId)
    {
        Debug.Log($"Server Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
        TestClientRpc(value, sourceNetworkObjectId);
    }
    #endregion
}
