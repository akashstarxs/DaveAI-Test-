using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NavigationManager : MonoBehaviour
{
    private PlayerActionController inputActions;

    [SerializeField]
    private CharacterController characterController;
    [SerializeField]
    private Animator Playeranimator;
    [SerializeField]
    private string VelocityParameter;
    [SerializeField]
    private Transform CharacterModel;


    private Vector2 currentMovementInput;
    private Vector3 currentMovement;
    private bool isMovementPressed;
    private Vector3 positionToLookAt;
    private float rotationDelta=1f;
    private void OnEnable()
    {
        inputActions.Player.Enable();
    }
    private void OnDisable()
    {
        inputActions.Player.Disable();
    }
    private void Awake()
    {
        inputActions = new PlayerActionController();
        if (characterController == null)
            characterController = FindObjectOfType<CharacterController>();
        if (Playeranimator == null)
            Debug.LogException(new Exception("Animator not assigned"));

        //OnKeyPressed
        inputActions.Player.Move.started += context => {
            OnUserInput(context);            
        };

        //OnKeyReleased
        inputActions.Player.Move.canceled += context => {
            OnUserInput(context);
        };
    }
    

    private void OnUserInput(InputAction.CallbackContext cxt)
    {
        currentMovementInput = cxt.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;

        isMovementPressed = currentMovement.x != 0 || currentMovement.z != 0;
    }

    private void LateUpdate()
    {
        HandleMovement();
        UpdateAnimation();
        HandleRotation();
    }

    private void HandleMovement()
    {
        characterController.Move(currentMovement * Time.deltaTime);

    }
    private void HandleRotation()
    {
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;
        Quaternion currentRotation = transform.rotation;
        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            Quaternion.Slerp(currentRotation, targetRotation, rotationDelta);
            CharacterModel.rotation = targetRotation;
        }
    }

    private void UpdateAnimation()
    {
        if (isMovementPressed)
            SetAnimationVelocity(1);
        else if (!isMovementPressed)
            SetAnimationVelocity(0);
    }
    
    private void SetAnimationVelocity(float velocity)
    {
        Playeranimator.SetFloat(VelocityParameter, velocity);
    }
}
