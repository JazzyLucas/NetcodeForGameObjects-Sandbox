using JazzyLucas.Core.Input;
using JazzyLucas.Core.Utils;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class MovementController : InputPoller
{
    private const float gravity = -18f;
    
    private Vector3 fallingVelocity = Vector3.zero;
    private float lerpValueForWalkingAndFlying = 0f;
    private bool isFlying;
    
    private readonly UnityEngine.CharacterController _characterController;
    private Transform _forwardTransform => _characterController.transform;

    // Constructor
    public MovementController(UnityEngine.CharacterController characterController) : base()
    {
        _characterController = characterController;
    }
    
    private float walkSpeed = 1;
    private float runSpeed = 2;
    private float jumpSpeed = 1;
    private float flySpeed = 2;
    private float fastFlySpeed = 4;
    private float flyUpSpeed => jumpSpeed; // TODO:
    private float GetSpeed(bool forSprint, bool forFlying)
    {
        if (forSprint)
            return forFlying ? fastFlySpeed : runSpeed;
        return forFlying ? flySpeed : walkSpeed;
    }
    
    public void ProcessMove(float deltaTime)
    {
        var input = MovementInputData.GetFromPlayerInputStruct(PollInput());
        isFlying = input.toggleFlying ? !isFlying : isFlying;
        DoLerp(input, out float forwardInputValue, out float sideInputValue, isFlying, deltaTime);
        DoMove(input, forwardInputValue, sideInputValue, deltaTime);
    }

    public void ProcessRotate(Angle yawTarget, float deltaTime)
    {
        var newRotation = Quaternion.Euler(0, (float)yawTarget, 0);
        _characterController.transform.rotation = newRotation;
    }

    private void DoMove(MovementInputData input, float forwardInputValue, float sideInputValue, float deltaTime)
    {
        if (isFlying)
        {
            var elevationChange = Vector3.zero;
            if (input.isJumping)
                elevationChange.y++;
            if (input.isCrouching)
                elevationChange.y--;
            fallingVelocity = elevationChange * flyUpSpeed * 1000 * deltaTime;
            if ((_characterController.collisionFlags & CollisionFlags.Below) != 0)
                _characterController.Move((_forwardTransform.forward * forwardInputValue + _forwardTransform.right * sideInputValue) * deltaTime);
            else
                _characterController.Move((_forwardTransform.forward * forwardInputValue + _forwardTransform.right * sideInputValue + fallingVelocity) * deltaTime);
        }
        else
        {
            if ((_characterController.collisionFlags & CollisionFlags.Below) != 0 && input.isJumping)
                fallingVelocity = Vector3.up * jumpSpeed;
            else if ((_characterController.collisionFlags & CollisionFlags.Below) != 0)
                fallingVelocity = Vector3.zero;
            else
                fallingVelocity += gravity * Vector3.up * deltaTime;
            _characterController.Move((_forwardTransform.forward * forwardInputValue + _forwardTransform.right * sideInputValue + fallingVelocity) * deltaTime);
        }
    }

    #region UTIL
    private void DoLerp(MovementInputData input, out float forwardInputValue, out float sideInputValue, bool isFlying, float deltaTime)
    {
        float moveSpeed = GetSpeed(input.isSprinting, isFlying);
        forwardInputValue = input.moveInput.y * MovementLerp(0,  moveSpeed, deltaTime);
        sideInputValue = input.moveInput.x * MovementLerp(0, moveSpeed, deltaTime);
        if (forwardInputValue == 0 && sideInputValue == 0 && (_characterController.collisionFlags & CollisionFlags.Below) != 0)
            lerpValueForWalkingAndFlying = 0f;
    }
    private float MovementLerp(float moveSpeed, float flySpeed, float deltaTime)
    {
        deltaTime *= deltaTime < 0 ? 2f : 1;
        lerpValueForWalkingAndFlying = Mathf.Clamp(0.75f * deltaTime + lerpValueForWalkingAndFlying, 0f, 1f);
        return Mathf.Lerp(moveSpeed, flySpeed, lerpValueForWalkingAndFlying);
    }
    #endregion
}