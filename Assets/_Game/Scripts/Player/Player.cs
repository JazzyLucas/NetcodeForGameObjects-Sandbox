using System;
using System.Collections;
using System.Collections.Generic;
using JazzyLucas.Core;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using L = JazzyLucas.Core.Utils.Logger;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(ViewController))]
[RequireComponent(typeof(PlayerHologramsController))]
public class Player : NetworkBehaviour
{
    [field: HideInInspector] public MovementController movementController { get; private set; }
    [field: HideInInspector] public ViewController viewController { get; private set; }
    [field: HideInInspector] public PlayerHologramsController hologramsController { get; private set; }
    
    [field: HideInInspector] public NetworkVariable<FixedString64Bytes> Name = new (
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Owner
    );
    
    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        viewController = GetComponent<ViewController>();
        hologramsController = GetComponent<PlayerHologramsController>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            // TODO: disable hologramController
            movementController.enabled = false;
            viewController.enabled = false;
        }
        if (IsOwner)
        {
            var newName = NameGenerator.GenerateFirstName();
            Name.Value = newName;
            L.Log($"Client-side name: {newName}");
        }
        hologramsController.Init(Name);
    }

    private void LateUpdate()
    {
        
    }

    #region Back and Forth RPC Example
    [Rpc(SendTo.ClientsAndHost)]
    private void TestClientRpc(int value, ulong sourceNetworkObjectId)
    {
        L.Log($"Client Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
        if (IsOwner)
        {
            TestServerRpc(value + 1, sourceNetworkObjectId);
        }
    }
    [Rpc(SendTo.Server)]
    private void TestServerRpc(int value, ulong sourceNetworkObjectId)
    {
        L.Log($"Server Received the RPC #{value} on NetworkObject #{sourceNetworkObjectId}");
        TestClientRpc(value, sourceNetworkObjectId);
    }
    #endregion
}
