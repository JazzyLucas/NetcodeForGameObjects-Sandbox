using System;
using System.Collections;
using System.Collections.Generic;
using JazzyLucas.Core;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using L = JazzyLucas.Core.Utils.Logger;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(ViewController))]
public class Player : NetworkBehaviour
{
    private HologramsContainer hologramsContainer => CoreManager.Instance.GetContainer(typeof(HologramsContainer)) as HologramsContainer;
    private HologramsManager hologramsManager => this.hologramsContainer.Manager;
    
    [field: SerializeField] public Transform NameHologramAnchorPoint { get; private set; }

    [field: HideInInspector] public MovementController movementController { get; private set; }
    [field: HideInInspector] public ViewController viewController { get; private set; }
    
    public NetworkVariable<FixedString64Bytes> playerName = new (
        readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Owner
    );
    
    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        viewController = GetComponent<ViewController>();
    }

    public override void OnNetworkSpawn()
    {
        var playerNameHologram = hologramsManager.CreateTextHologram(NameHologramAnchorPoint);
        playerNameHologram.Text.text = playerName.Value.ToString();
        playerName.OnValueChanged += (oldValue, newValue) =>
        {
            playerNameHologram.Text.text = newValue.ToString();
        };
        
        if (!IsOwner)
        {
            movementController.enabled = false;
            viewController.enabled = false;
        }
        if (IsOwner)
        {
            var newName = NameGenerator.GenerateFirstName();
            playerName.Value = newName;
            L.Log($"Client-side name: {newName}");
        }
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
