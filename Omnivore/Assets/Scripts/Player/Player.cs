using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MovementSystem;
using CombatSystem;

public class Player : MonoBehaviour
{
    private MSEntity movementEntity;
    private CSEntity combatEntity;
    [SerializeField]
    private CSWeapon weapon;
    private InputActionAsset inputActions;
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Transform lookTarget;
    [SerializeField]
    private float lookSensetivity;

    private Vector2 moveInput;
    private Vector2 lookInput;

    #region Animator Settings

    /// <summary>
    /// The walking parameter on the animator
    /// </summary>
    public bool Walking
    {
        get { return animator.GetBool("Walking"); }
        set { animator.SetBool("Walking", value); }
    }

    /// <summary>
    /// The runnning parameter on the animator
    /// </summary>
    public bool Running
    {
        get { return animator.GetBool("Running"); }
        set { animator.SetBool("Running", value); }
    }

    /// <summary>
    /// Vector2 representing the move direction parameters on the animator
    /// </summary>
    public Vector2 MoveDirection
    {
        set 
        {
            animator.SetFloat("DirectionX", value.x);
            animator.SetFloat("DirectionY", value.y);
        }
    }

    /// <summary>
    /// The speed parameter on the animator
    /// </summary>
    public float Speed
    {
        set { animator.SetFloat("Speed", value); }
    }

    #endregion

    private void Awake()
    {
        movementEntity = GetComponent<MSEntity>();
        combatEntity = GetComponent<CSEntity>();
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

        //Update Animator Parameters
        Vector3 localVelocity = transform.InverseTransformDirection(movementEntity.Velocity);
        float speed =  new Vector2(localVelocity.x, localVelocity.z).magnitude;

        Speed = speed;
        MoveDirection = new Vector2(localVelocity.x, localVelocity.z) / speed;
        Walking = speed > 0.1f;
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
        Running = movementEntity.Sprinting;
    }

    private void OnDodge(InputAction.CallbackContext context)
    {
        movementEntity.Dash(new Vector3(moveInput.x, 0, moveInput.y), false);
        animator.SetTrigger("Dodge");
    }

    #endregion
}
