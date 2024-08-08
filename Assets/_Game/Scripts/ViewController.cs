using System.Collections;
using System.Collections.Generic;
using JazzyLucas.Core.Input;
using JazzyLucas.Core.Utils;
using UnityEngine;

public class ViewController : InputPoller
{
    private Angle yaw;
    public Angle GetYaw() => _viewTransform.rotation.eulerAngles.x;
    private Angle pitch;
    public Angle GetPitch() => _viewTransform.rotation.eulerAngles.y;

    private readonly Transform _viewTransform;

    public ViewController(Transform viewTransform) : base()
    {
        _viewTransform = viewTransform;
    }
        
    public void Process()
    {
        var input = PollInput();
        DoRotateCamera(input.MouseDelta);
    }
        
    private void DoRotateCamera(Vector2 delta)
    {
        yaw += delta.x;
        pitch += delta.y;
            
        pitch = AngleUtil.CustomClampAngle(pitch);
            
        var rotation = Quaternion.Euler((float)pitch, (float)yaw, 0);
        _viewTransform.rotation = rotation;
    }

}
