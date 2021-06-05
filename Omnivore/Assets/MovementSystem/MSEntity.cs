using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MovementSystem;

namespace MovementSystem
{
    public delegate void OnDash();

    public class MSEntity : MSPhysicsObject
    {
        [SerializeField]
        private MSEntityStats stats;

        private bool sprinting;
        private bool dashing;
        private float dashTime;

        #region Accessors

        /// <summary>
        /// The current move speed of the entity (returns sprint speed if sprinting)
        /// </summary>
        public float CurrentSpeed
        {
            get
            {
                if (dashing)
                {
                    if (stats.VariableDashSpeed)
                        return stats.DashSpeedOverTime.Evaluate(dashTime / stats.DashTime) * stats.DashSpeed;
                    else
                        return stats.DashSpeed;
                }
                else if (sprinting)
                    return stats.SprintSpeed;
                else
                    return stats.MoveSpeed;
            }
        }

        /// <summary>
        /// The movement stats of the entity
        /// </summary>
        public MSEntityStats Stats
        {
            get { return stats; }
            set { stats = value; }
        }

        /// <summary>
        /// Whether or not the unit is currently sprinting
        /// </summary>
        public bool Sprinting
        {
            get { return sprinting; }
            set { sprinting = value; }
        }

        /// <summary>
        /// Whether or not the entity is currently dashing
        /// </summary>
        public bool Dashing
        {
            get { return dashing; }
        }

        /// <summary>
        /// Called when the entity begins a dash
        /// </summary>
        public event OnDash DashStart;

        /// <summary>
        /// Called when the entity ends a dash
        /// </summary>
        public event OnDash DashEnd;

        #endregion

        #region Movement

        /// <summary>
        /// Applies a movement force in the specified direction. The movement force is specified in the attached MSEntityStats.
        /// </summary>
        /// <param name="moveDirection">The direction to move in.</param>
        /// <param name="absoluteDirection">Whether or not the direction is given in absolute or relative coordinates</param>
        public void Move(Vector3 moveDirection, bool absoluteDirection = true)
        {
            if (!dashing)
            {
                //Rotate the move direction if it was given in relative coordinates
                if (!absoluteDirection)
                {
                    moveDirection = transform.rotation * moveDirection;
                }

                float speed = CurrentSpeed;

                //Calculate the speed if variable move speed is enabled
                if (stats.VariableMoveSpeed)
                {
                    //HACK: Rounding issues with the dot product were causing NaN errors temporary fix with the Mathf.Clamp()
                    float angle = Mathf.Acos(Mathf.Clamp(Vector3.Dot(transform.forward, moveDirection), -1.0f, 1.0f));

                    if (angle > (180 - stats.BackAngle) * Mathf.Deg2Rad)
                    {
                        speed *= stats.BackSpeedMult;
                    }
                    else if (angle > stats.ForwardAngle * Mathf.Deg2Rad)
                    {
                        speed *= stats.StrafeSpeedMult;
                    }
                }

                //Calculate the movement force to apply
                Vector3 targetVelocity = moveDirection * speed;
                Vector3 forceDirection = targetVelocity - Velocity;

                if (!stats.CanFly)
                    forceDirection.y = 0;

                float forceLength = forceDirection.magnitude;

                if (forceLength != 0)
                    forceDirection /= forceLength;

                ApplyForce(forceDirection * Mathf.Clamp(forceLength / stats.AccelerationTime, 0.0f, stats.MoveForce));
            }
        }

        /// <summary>
        /// Initiates a dash in the specified direction
        /// </summary>
        /// <param name="dashDirection">The direction to dash in</param>
        /// <param name="absoluteDirection">Whether or not the dash direction is in absolute coordinates</param>
        public void Dash(Vector3 dashDirection, bool absoluteDirection = true)
        {
            if (!absoluteDirection)
            {
                dashDirection = transform.rotation * dashDirection;
            }

            StartCoroutine(StartDash(dashDirection));
        }

        /// <summary>
        /// Moves the player in the specified direction while dashing
        /// </summary>
        /// <param name="dashDirection"></param>
        private IEnumerator StartDash(Vector3 dashDirection)
        {
            DashStart?.Invoke();
            dashing = true;
            dashTime = 0.0f;
            dashDirection.y = 0;
            dashDirection = dashDirection.normalized;

            while(dashTime < stats.DashTime)
            {
                dashTime += Time.deltaTime;

                Velocity = new Vector3(0, Velocity.y, 0) + dashDirection * CurrentSpeed;

                yield return new WaitForEndOfFrame();
            }

            dashing = false;
            DashEnd?.Invoke();
        }

        /// <summary>
        /// Adds an upwards velocity to the unit to allow it to jump
        /// </summary>
        /// <param name="velocityShift">Whether the velocity should be shifted by the jump velocity or set to the jump velocity, set to true to shift velocity</param>
        public void Jump(bool velocityShift = false)
        {
            if (velocityShift)
                Velocity += Vector3.up * stats.JumpVelocity;
            else
                Velocity = new Vector3(Velocity.x, stats.JumpVelocity, Velocity.z);
        }

        #endregion
    }
}