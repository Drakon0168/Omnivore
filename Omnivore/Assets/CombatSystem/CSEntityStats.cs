using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem
{
    [CreateAssetMenu(fileName = "NewCombatEntityStats", menuName = "CombatSystem/CSEntityStats")]
    public class CSEntityStats : ScriptableObject
    {
        [Header("Health")]
        [Tooltip("The maximum amount of health that this entity can have.")]
        [SerializeField]
        private float maxHealth = 100.0f;
        [Tooltip("Whether or not health should be able to regenerate. True to enable health regen.")]
        [SerializeField]
        private bool healthRegenEnabled = false;
        [Tooltip("The amount of health to regenerate per second when health regen starts.")]
        [SerializeField]
        private float healthRegen = 5.0f;
        [Tooltip("The amount of time to wait before starting health regeneration after taking damage.")]
        [SerializeField]
        private float healthRegenTime = 5.0f;

        [Header("Shields")]
        [Tooltip("Whether or not to enable shields.")]
        [SerializeField]
        private bool shieldsEnabled = true;
        [Tooltip("The maximum shields that this entity can have.")]
        [SerializeField]
        private float maxShield = 50.0f;
        [Tooltip("The rate at which shields regenerate per second when shield regen starts.")]
        [SerializeField]
        private float shieldRegen = 20.0f;
        [Tooltip("The amount of time to wait before starting shield regen after taking damage.")]
        [SerializeField]
        private float shieldRegenTime = 2.5f;

        /// <summary>
        /// The maximum amount of health that this entity can have.
        /// </summary>
        public float MaxHealth
        {
            get { return maxHealth; }
        }

        /// <summary>
        /// Whether or not health should be able to regenerate. True to enable health regen.
        /// </summary>
        public bool HealthRegenEnabled
        {
            get { return healthRegenEnabled; }
        }

        /// <summary>
        /// The amount of health to regenerate per second when health regen starts.
        /// </summary>
        public float HealthRegen
        {
            get { return healthRegen; }
        }

        /// <summary>
        /// The amount of time to wait before starting health regeneration after taking damage.
        /// </summary>
        public float HealthRegenTime
        {
            get { return healthRegenTime; }
        }

        /// <summary>
        /// Whether or not to enable shields.
        /// </summary>
        public bool ShieldsEnabled
        {
            get { return shieldsEnabled; }
        }

        /// <summary>
        /// The maximum shields that this entity can have.
        /// </summary>
        public float MaxShields
        {
            get { return maxShield; }
        }

        /// <summary>
        /// The rate at which shields regenerate per second when shield regen starts.
        /// </summary>
        public float ShieldRegen
        {
            get { return shieldRegen; }
        }

        /// <summary>
        /// The amount of time to wait before starting shield regen after taking damage.
        /// </summary>
        public float ShieldRegenTime
        {
            get { return shieldRegenTime; }
        }
    }
}