using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatSystem;

namespace CombatSystem
{

    public class CSEntity : MonoBehaviour
    {
        public delegate void CSEntityEvent();
        public delegate void CSCollisionEvent(CSEntity other);

        [SerializeField]
        private CSEntityStats stats;
        private float health;
        private float shields;

        private bool healthRegening;
        private bool shieldRegening;
        private bool invulnerable;

        private Coroutine healthCoroutine;
        private Coroutine shieldCoroutine;

        #region Accessors

        /// <summary>
        /// The base stats of the entity
        /// </summary>
        public CSEntityStats Stats
        {
            get { return stats; }
        }

        /// <summary>
        /// The current health of the combat entity
        /// </summary>
        public float Health
        {
            get { return health; }
            set
            {
                health = value;

                if (health <= 0)
                {
                    health = 0;
                    OnDeath?.Invoke();
                }
                else if (health >= Stats.MaxHealth)
                {
                    health = Stats.MaxHealth;
                    OnFullHealth?.Invoke();
                }
            }
        }

        /// <summary>
        /// The current shields of the combat entity
        /// </summary>
        public float Shields
        {
            get { return shields; }
            set
            {
                shields = value;

                if(shields <= 0)
                {
                    shields = 0;
                    OnEmptyShield?.Invoke();
                }
                else if (shields >= Stats.MaxShields)
                {
                    shields = Stats.MaxShields;
                    OnFullShield?.Invoke();
                }
            }
        }

        /// <summary>
        /// Whether or not the entity's health is currently able to regenerate
        /// </summary>
        public bool HealthRegening
        {
            get { return healthRegening; }
        }

        /// <summary>
        /// Whether or not the entity's shields are currently able to regenerate
        /// </summary>
        public bool ShieldRegening
        {
            get { return shieldRegening; }
        }

        /// <summary>
        /// Whether or not the entity is currently able to take damage
        /// </summary>
        public bool Invulnerable
        {
            get { return invulnerable; }
            set { invulnerable = value; }
        }

        #endregion

        #region Events

        public event CSEntityEvent OnDeath;

        public event CSEntityEvent OnFullHealth;

        public event CSEntityEvent OnFullShield;

        public event CSEntityEvent OnEmptyShield;

        public event CSEntityEvent OnHealthRegenStart;

        public event CSEntityEvent OnShieldRegenStart;

        public event CSEntityEvent OnDamageTaken;

        public event CSCollisionEvent OnEntityEnter;

        public event CSCollisionEvent OnEntityHit;

        public event CSCollisionEvent OnEntityExit;

        #endregion

        private void Awake()
        {
            Health = Stats.MaxHealth;
            Shields = Stats.MaxShields;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (OnEntityEnter != null)
            {
                CSEntity other = collision.gameObject.GetComponent<CSEntity>();

                if (other == null)
                {
                    other = collision.gameObject.GetComponentInParent<CSEntity>();
                }

                if (other != null)
                {
                    OnEntityEnter(other);
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            if (OnEntityExit != null)
            {
                CSEntity other = collision.gameObject.GetComponent<CSEntity>();

                if (other == null)
                {
                    other = collision.gameObject.GetComponentInParent<CSEntity>();
                }

                if (other != null)
                {
                    OnEntityExit(other);
                }
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            if (OnEntityHit != null)
            {
                CSEntity other = collision.gameObject.GetComponent<CSEntity>();

                if (other == null)
                {
                    other = collision.gameObject.GetComponentInParent<CSEntity>();
                }

                if (other != null)
                {
                    OnEntityHit(other);
                }
            }
        }

        /// <summary>
        /// Applies damage using the entity's health and shield values as well as it's invulnerability state
        /// </summary>
        /// <param name="damage">The amount of damage to take</param>
        public void TakeDamage(float damage)
        {
            if(damage == 0)
            {
                return;
            }

            if (!Invulnerable)
            {
                if (Stats.ShieldsEnabled)
                {
                    if (Shields > 0)
                    {
                        if (Shields >= damage)
                        {
                            Shields -= damage;
                        }
                        else
                        {
                            damage -= Shields;
                            Shields = 0;
                        }

                        if (shieldCoroutine != null)
                        {
                            StopCoroutine(shieldCoroutine);
                        }

                        shieldCoroutine = StartCoroutine(RegenShield());
                    }

                    if(damage > 0)
                    {
                        if (healthCoroutine != null)
                        {
                            StopCoroutine(shieldCoroutine);
                        }

                        healthCoroutine = StartCoroutine(RegenHealth());
                    }

                    Health -= damage;
                }
                else
                {
                    if (healthCoroutine != null)
                    {
                        StopCoroutine(shieldCoroutine);
                    }

                    healthCoroutine = StartCoroutine(RegenHealth());

                    Health -= damage;
                }
            }

            OnDamageTaken?.Invoke();
        }

        /// <summary>
        /// Starts the time delay until the unit's health is able to regenerate
        /// </summary>
        public IEnumerator RegenHealth()
        {
            yield return new WaitForSeconds(Stats.HealthRegenTime);

            healthRegening = true;
            OnHealthRegenStart?.Invoke();
        }

        /// <summary>
        /// Starts the time delay until the unit's shields are able to regenerate
        /// </summary>
        public IEnumerator RegenShield()
        {
            yield return new WaitForSeconds(Stats.ShieldRegenTime);

            shieldRegening = true;
            OnShieldRegenStart?.Invoke();
        }
    }
}