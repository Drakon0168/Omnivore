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

    protected enum Behavior
    {
        Attack,
        Circle
    }

    [SerializeField]
    protected float alertRadius;
    [SerializeField]
    protected float combatRadius;
    [SerializeField]
    protected float circleRadius;
    [SerializeField]
    protected float circleSpeed;

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
    [SerializeField]
    protected Behavior behavior = Behavior.Circle;
    protected Coroutine behaviorSwap;
    protected float circleDirection = 1.0f;

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
        combatEntity.OnDeath += OnDeath;
        combatEntity.OnEntityEnter += OnWeaponCollide;

        weapon.Combo.AttackEnd += OnComboEnd;
        weapon.Combo.WindupStart += () => { animator.SetTrigger("Attack"); };

        navigation = GetComponent<NavMeshAgent>();
        navigation.speed = movementEntity.Stats.SprintSpeed;

        player = GameObject.Find("Player").transform;
    }

    protected virtual void Update()
    {
        if (!combatEntity.Dead)
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
    }

    protected virtual void OnWeaponCollide(CSEntity other)
    {
        if(other.tag == "Player")
        {
            other.TakeDamage(weapon.Stats.BaseDamage * weapon.Combo.ActiveAttack.DamageMult);
        }
    }

    protected virtual void OnDeath()
    {
        animator.SetTrigger("Death");
    }

    protected virtual void OnComboEnd()
    {
        animator.SetTrigger("ComboEnd");
        behaviorSwap = StartCoroutine(SwitchBehavior());
    }

    protected virtual void FixedUpdate()
    {
        navigation.nextPosition = transform.position;
    }

    protected virtual void Idle()
    { 
        lookTarget.position = targetPosition;

        if((player.position - transform.position).sqrMagnitude < alertRadius * alertRadius)
        {
            aiState = AIState.Searching;
            navigation.speed = movementEntity.Stats.SprintSpeed;
            animator.SetBool("Running", true);
        }
    }

    protected virtual void Search()
    {
        lookTarget.position = transform.position + Vector3.up + transform.forward;
        navigation.SetDestination(player.position);

        if ((player.position - transform.position).sqrMagnitude < combatRadius * combatRadius)
        {
            navigation.speed = movementEntity.Stats.MoveSpeed;
            aiState = AIState.Combat;
            animator.SetBool("Running", false);
            behavior = Behavior.Attack;
            behaviorSwap = StartCoroutine(SwitchBehavior());
        }
    }

    protected virtual void Fight()
    {
        lookTarget.position = player.position + Vector3.up * 1.0f;

        if (weapon.Attacking)
        {
            navigation.speed = 0;
        }

        switch (behavior)
        {
            case Behavior.Circle:
                navigation.speed = circleSpeed;
                CirclePlayer();
                break;
            case Behavior.Attack:
                navigation.speed = movementEntity.Stats.MoveSpeed;
                navigation.destination = player.position;

                if ((player.position - transform.position).sqrMagnitude < 2.0f && !weapon.Attacking)
                {
                    weapon.Attack(0);
                }
                break;
        }

        if (weapon.Attacking)
        {
            navigation.speed = 0;
        }

        if ((player.position - transform.position).sqrMagnitude >= combatRadius * combatRadius)
        {
            navigation.speed = movementEntity.Stats.SprintSpeed;
            aiState = AIState.Searching;
            animator.SetBool("Running", true);

            if (behaviorSwap != null)
            {
                StopCoroutine(behaviorSwap);
            }
        }
    }

    protected void CirclePlayer()
    {
        float angle = (Mathf.PI * circleDirection) / 5;

        Quaternion rotation = new Quaternion(0, Mathf.Sin(angle / 2), 0, Mathf.Cos(angle / 2));
        Vector3 offset = rotation * ((transform.position - player.position).normalized * circleRadius);
        navigation.SetDestination(player.position + offset);

        Debug.DrawRay(player.position + Vector3.up, offset, Color.yellow);
    }

    protected IEnumerator SwitchBehavior()
    {
        float random = Random.value;

        if (random < 0.5)
        {
            behavior = Behavior.Circle;
        }
        else if(random < 0.75)
        {
            behavior = Behavior.Circle;
            circleDirection *= -1;
        }
        else
        {
            behavior = Behavior.Attack;
        }

        if (behavior != Behavior.Attack)
        {
            yield return new WaitForSeconds(2.5f);

            behaviorSwap = StartCoroutine(SwitchBehavior());
        }
    }
}
