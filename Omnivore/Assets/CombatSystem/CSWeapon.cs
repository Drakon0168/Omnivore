using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem {
    public class CSWeapon : MonoBehaviour
    {
        [SerializeField]
        private CSCombo combo;

        [SerializeField]
        private CSWeaponStats stats;

        [SerializeField]
        private string[] attackTypes;

        /// <summary>
        /// The attack stats associated with this weapon
        /// </summary>
        public CSWeaponStats Stats
        {
            get { return stats; }
        }

        /// <summary>
        /// List of attack types available to this weapon
        /// </summary>
        public string[] AttackTypes
        {
            get { return attackTypes; }
        }

        private void Awake()
        {
            combo.QueueAttack += StartCurrentAttack;
            combo.Weapon = this;
            combo.Reset();
        }

        /// <summary>
        /// Sends an attack to the combo tree to figure out which hitboxes to enable
        /// </summary>
        /// <param name="attack">The integer ID of the attack type to use</param>
        public void Attack(int attack)
        {
            combo.Chain(attack);
        }

        private void StartCurrentAttack()
        {
            if (!combo.ActiveAttack.IsEntry)
            {
                StartCoroutine(combo.ActiveAttack.Attack());
            }
        }
    }
}