using System;
using System.Collections;
using System.Collections.Generic;
using JazzyLucas.Core.Utils;
using UnityEngine;

public abstract class Hologram : MonoBehaviour
{
    [field: HideInInspector] public RectTransform Transform { get; private set; }
    [field: HideInInspector] public Transform AnchorTo { get; set; }
    [field: HideInInspector] public Camera MainCamera { get; set; }

    private void Awake()
    {
        if (!Transform)
        {
            Transform = (RectTransform)transform;
        }
    }

    private void LateUpdate()
    {
        if (!AnchorTo)
        {
            // TODO: destroy after a set amount of time? in case it's switching anchors?
            Destroy(this.gameObject);
        }
        UIUtil.AnchorToWorldSpacePoint(AnchorTo, Transform, MainCamera);
    }
}
