using System;
using System.Collections;
using System.Collections.Generic;
using JazzyLucas.Core;
using UnityEngine;

public class HologramsManager : Manager
{
    [field: SerializeField] public HologramsContainer Container { get; private set; }
    protected override Container BaseContainer => Container;

    [field: SerializeField] public Canvas Canvas { get; private set; }
    [field: SerializeField] public Camera MainCamera { get; private set; }
    
    protected override void Init()
    {
        
        
        // TODO: create a pool of Holograms and use those
    }

    public TextHologram CreateTextHologram(Transform anchorTo)
    {
        var prefab = Container.TextHologramPrefab;
        var newHologramGO = Instantiate(prefab, Canvas.transform);
        var newHologram = newHologramGO.GetComponent<TextHologram>();
        newHologram.MainCamera = MainCamera;
        newHologram.AnchorTo = anchorTo;
        return newHologram;
    }
}