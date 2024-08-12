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
    private void Awake()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer && IsOwner)
        {
            TestServerRpc(0, NetworkObjectId);
        }
    }

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
}
