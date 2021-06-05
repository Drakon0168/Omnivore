using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MovementSystem;

public class Player : MonoBehaviour
{
    private MSEntity movementEntity;
    private InputActionAsset inputActions;

    [SerializeField]
    private Transform lookTarget;
    [SerializeField]
    private float lookSensetivity;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private void Awake()
    {
        movementEntity = GetComponent<MSEntity>();
        inputActions = GetComponent<PlayerInput>().actions;

        //Setup input events
        InputActionMap gameplayMap = inputActions.FindActionMap("Gameplay");
        gameplayMap.FindAction("Move").performed += OnMove;
        gameplayMap.FindAction("Move").canceled += OnMove;

        gameplayMap.FindAction("Look").performed += OnLook;
        gameplayMap.FindAction("Look").canceled += OnLook;

        gameplayMap.FindAction("Sprint").performed += OnSprint;

        gameplayMap.FindAction("Dodge").performed += OnDodge;
    }

    private void Update()
    {
        transform.LookAt(new Vector3(lookTarget.position.x, transform.position.y, lookTarget.position.z));

        transform.Rotate(0, lookInput.x * lookSensetivity * Time.deltaTime, 0);
        movementEntity.Move(new Vector3(moveInput.x, 0, moveInput.y), false);
    }

    #region Input Management

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void OnSprint(InputAction.CallbackContext context)
    {
        movementEntity.Sprinting = context.ReadValue<float>() > 0.0f;
    }

    private void OnDodge(InputAction.CallbackContext context)
    {
        movementEntity.Dash(new Vector3(moveInput.x, 0, moveInput.y), false);
    }

    #endregion
}
