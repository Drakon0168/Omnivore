using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MovementSystem;
using CombatSystem;

public class EnemyBase : MonoBehaviour
{
    protected enum AIState
    {
        Unaware,
        Searching,
        Combat
    }

    protected MSEntity movementEntity;
    protected CSEntity combatEntity;
    protected NavMeshAgent navigation;
    [SerializeField]
    protected CSWeapon weapon;
    [SerializeField]
    protected Animator animator;

    [SerializeField]
    protected Transform lookTarget;
    protected Vector3 targetPosition;

    protected Transform player;
    [SerializeField]
    protected AIState aiState = AIState.Unaware;

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

    protected virtual void Awake()
    {
        movementEntity = GetComponent<MSEntity>();
        combatEntity = GetComponent<CSEntity>();
        navigation = GetComponent<NavMeshAgent>();

        player = GameObject.Find("Player").transform;
    }

    protected virtual void Update()
    {
        transform.LookAt(new Vector3(lookTarget.position.x, transform.position.y, lookTarget.position.z));

        switch (aiState)
        {
            case AIState.Unaware:
                Idle();
                break;
            case AIState.Searching:
                Search();
                break;
            case AIState.Combat:
                Fight();
                break;
        }

        //Update Animator Parameters
        Vector3 localVelocity = transform.InverseTransformDirection(navigation.velocity);
        float speed = new Vector2(localVelocity.x, localVelocity.z).magnitude;

        Speed = speed;
        MoveDirection = new Vector2(localVelocity.x, localVelocity.z) / speed;
        Walking = speed > 0.1f;
    }

    protected virtual void FixedUpdate()
    {
        movementEntity.Move((navigation.nextPosition - transform.position).normalized);
        navigation.nextPosition = transform.position;
    }

    protected virtual void Idle()
    {
        lookTarget.position = targetPosition;
    }

    protected virtual void Search()
    {
        lookTarget.position = targetPosition;
    }

    protected virtual void Fight()
    {
        lookTarget.position = player.position + Vector3.up * 1.0f;
        navigation.SetDestination(player.position);
    }
}
