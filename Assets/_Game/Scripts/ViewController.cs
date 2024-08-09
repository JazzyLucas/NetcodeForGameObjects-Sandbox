using System;
using System.Collections;
using System.Collections.Generic;
using JazzyLucas.Core.Input;
using JazzyLucas.Core.Utils;
using UnityEngine;

public class ViewController : MonoBehaviour
{
    [field: SerializeField] public Transform ViewTransform { get; private set; }

    private InputPoller inputPoller;
    
    private Angle yaw;
    public Angle GetYaw() => ViewTransform.rotation.eulerAngles.x;
    private Angle pitch;
    public Angle GetPitch() => ViewTransform.rotation.eulerAngles.y;

    private void Awake()
    {
        inputPoller = new();
    }

    private void Update()
    {
        Process();
    }

    public void Process()
    {
        var input = inputPoller.PollInput();
        DoRotateCamera(input.MouseDelta);
    }
        
    private void DoRotateCamera(Vector2 delta)
    {
        yaw += delta.x;
        pitch += delta.y;
            
        pitch = AngleUtil.CustomClampAngle(pitch);
            
        var rotation = Quaternion.Euler((float)pitch, (float)yaw, 0);
        ViewTransform.rotation = rotation;
    }
}
