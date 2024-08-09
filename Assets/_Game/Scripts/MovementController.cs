using JazzyLucas.Core.Input;
using JazzyLucas.Core.Utils;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class MovementController : MonoBehaviour
{
    [field: SerializeField] public CharacterController CharacterController { get; private set; }
    [field: SerializeField] public Transform DirectionTransform { get; private set; }
    public Angle GetDirection() => DirectionTransform.rotation.eulerAngles.y;

    private InputPoller inputPoller;
    
    private const float gravity = -18f;
    
    private Vector3 fallingVelocity = Vector3.zero;
    private float lerpValueForWalkingAndFlying = 0f;
    private bool isFlying;
    
    private Transform _forwardTransform => CharacterController.transform;
    
    private float walkSpeed = 1;
    private float runSpeed = 2;
    private float jumpSpeed = 5;
    private float flySpeed = 2;
    private float fastFlySpeed = 4;
    private float flyUpSpeed => jumpSpeed; // TODO:
    private float GetSpeed(bool forSprint, bool forFlying)
    {
        if (forSprint)
            return forFlying ? fastFlySpeed : runSpeed;
        return forFlying ? flySpeed : walkSpeed;
    }

    protected virtual void Awake()
    {
        inputPoller = new();
    }

    protected virtual void Update()
    {
        ProcessMove(Time.deltaTime, inputPoller.PollInput());
        ProcessRotate(GetDirection());
    }
    
    public void ProcessMove(float deltaTime, InputData input)
    {
        var movementInput = MovementInputData.GetFromPlayerInputStruct(input);
        isFlying = movementInput.toggleFlying ? !isFlying : isFlying;
        DoLerp(movementInput, out float forwardInputValue, out float sideInputValue, isFlying, deltaTime);
        DoMove(movementInput, forwardInputValue, sideInputValue, deltaTime);
    }

    public void ProcessRotate(Angle yawTarget)
    {
        var newRotation = Quaternion.Euler(0, (float)yawTarget, 0);
        CharacterController.transform.rotation = newRotation;
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
            if ((CharacterController.collisionFlags & CollisionFlags.Below) != 0)
                CharacterController.Move((_forwardTransform.forward * forwardInputValue + _forwardTransform.right * sideInputValue) * deltaTime);
            else
                CharacterController.Move((_forwardTransform.forward * forwardInputValue + _forwardTransform.right * sideInputValue + fallingVelocity) * deltaTime);
        }
        else
        {
            if ((CharacterController.collisionFlags & CollisionFlags.Below) != 0 && input.isJumping)
                fallingVelocity = Vector3.up * jumpSpeed;
            else if ((CharacterController.collisionFlags & CollisionFlags.Below) != 0)
                fallingVelocity = Vector3.zero;
            else
                fallingVelocity += gravity * Vector3.up * deltaTime;
            CharacterController.Move((_forwardTransform.forward * forwardInputValue + _forwardTransform.right * sideInputValue + fallingVelocity) * deltaTime);
        }
    }

    #region UTIL
    private void DoLerp(MovementInputData input, out float forwardInputValue, out float sideInputValue, bool isFlying, float deltaTime)
    {
        float moveSpeed = GetSpeed(input.isSprinting, isFlying);
        forwardInputValue = input.moveInput.y * MovementLerp(0,  moveSpeed, deltaTime);
        sideInputValue = input.moveInput.x * MovementLerp(0, moveSpeed, deltaTime);
        if (forwardInputValue == 0 && sideInputValue == 0 && (CharacterController.collisionFlags & CollisionFlags.Below) != 0)
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