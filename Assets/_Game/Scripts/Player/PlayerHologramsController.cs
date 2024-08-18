using System;
using System.Collections;
using System.Collections.Generic;
using JazzyLucas.Core;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerHologramsController : MonoBehaviour
{
    private HologramsManager hologramsManager => (HologramsManager)HologramsManager.Instance;
    private HologramsContainer hologramsContainer => hologramsManager.Container;

    [field: SerializeField] public Transform NameHologramAnchorPoint { get; private set; }
    
    [field: HideInInspector] private TextHologram nameTextHologram { get; set; }
    [field: HideInInspector] private string nameToUse { get; set; }
    private void SetNameToUse(FixedString64Bytes value) => nameToUse = value.ToString();

    private void Awake()
    {
        nameTextHologram = hologramsManager.CreateTextHologram(NameHologramAnchorPoint);
    }

    public void Init(NetworkVariable<FixedString64Bytes> networkedName)
    {
        networkedName.OnValueChanged += (oldValue, newValue) =>
        {
            SetNameToUse(newValue);
        };
        SetNameToUse(networkedName.Value);
    }
    
    private void LateUpdate()
    {
        nameTextHologram.Text.text = nameToUse;
    }
}
