using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem
{
    [CreateAssetMenu(fileName = "New Combo", menuName = "CombatSystem/Combo")]
    public class CSCombo : ScriptableObject
    {
        [HideInInspector]
        [SerializeReference]
        private List<CSAttack> attacks;
        private CSAttack activeAttack = null;
        private CSAttack nextAttack = null;

        [SerializeField]
        private string[] chains;
        [SerializeReference]
        private CSWeapon weapon;

        #region Callbacks

        /// <summary>
        /// Called when the current attack is first started.
        /// </summary>
        public event AttackTimer WindupStart;

        /// <summary>
        /// Called when the current attack becomes active, this is when hitboxes will be activated
        /// </summary>
        public event AttackTimer AttackStart;

        /// <summary>
        /// Called at the end of the active time of the current attack this is when hitboxes should be disabled
        /// </summary>
        public event AttackTimer AttackEnd;

        /// <summary>
        /// Called at the end of the cooldown period when the current attack ends this is when new actions can be taken again
        /// </summary>
        public event AttackTimer CooldownEnd;

        /// <summary>
        /// Called when the combo timer for the current attack is finished this is the point to stop listening for new combo input
        /// </summary>
        public event AttackTimer ComboEnd;

        /// <summary>
        /// Used to tell the weapon when to start attacks
        /// </summary>
        public event AttackTimer QueueAttack;

        #endregion

        #region Accessors

        /// <summary>
        /// Whether or not the combo is in the middle of attacking
        /// </summary>
        public bool Attacking
        {
            get { return activeAttack.Attacking; }
        }

        /// <summary>
        /// The full list of attacks in this combo tree
        /// </summary>
        private List<CSAttack> Attacks
        {
            get
            {
                if(attacks == null || attacks.Count == 0)
                {
                    attacks = new List<CSAttack>();
                    attacks.Add(new CSAttack("Entry", true));
                    attacks[0].ChainCount = Chains.Length;
                }

                return attacks;
            }
        }

        /// <summary>
        /// The idle point of the combat tree
        /// </summary>
        public CSAttack Entry
        {
            get
            {
                return Attacks[0];
            }
            set
            {
                Attacks[0] = value;
                activeAttack = value;
            }
        }

        /// <summary>
        /// The available actions to use when chaining attacks
        /// </summary>
        public string[] Chains
        {
            get { return chains; }
            set
            {
                chains = value;

                foreach (CSAttack attack in attacks)
                {
                    attack.ChainCount = value.Length;
                }
            }
        }

        public CSAttack ActiveAttack
        {
            get { return activeAttack; }
        }

        public CSAttack NextAttack
        {
            get { return nextAttack;}
        }

        public CSWeapon Weapon
        {
            get { return weapon; }
            set
            {
                weapon = value;

                foreach(CSAttack attack in attacks)
                {
                    attack.Weapon = value;
                }
            }
        }

        #endregion

        #region Attack Management

        private void OnValidate()
        {
            if (Entry.Chains.Length != Chains.Length)
            {
                Entry.ChainCount = Chains.Length;

                foreach(CSAttack attack in attacks)
                {
                    attack.ChainCount = Chains.Length;
                }
            }
        }

        /// <summary>
        /// Adds an attack to the attack list 
        /// </summary>
        /// <param name="attack">The attack to add</param>
        /// <param name="parent">The attack to chain from</param>
        /// <param name="chain">The chain index to attach the new attack to</param>
        public void AddAttack(CSAttack attack, CSAttack parent, int chain)
        {
            attack.ChainCount = Chains.Length;
            attack.Weapon = Weapon;
            attacks.Add(attack);

            if (parent != null)
            {
                if (parent.Chains[chain] != null)
                {
                    RemoveAttack(parent.Chains[chain]);
                }

                parent.Chains[chain] = attack;
                attack.Parent = parent;
            }
        }

        /// <summary>
        /// Removes the specified attack and any chains from the combo tree
        /// </summary>
        /// <param name="attack">The attack to remove</param>
        public void RemoveAttack(CSAttack attack)
        {
            for (int i = 0; i < attack.ChainCount; i++)
            {
                if (attack.Chains[i] != null)
                {
                    RemoveAttack(attack.Chains[i]);
                }
            }

            if (attack.Parent != null)
            {
                for (int i = 0; i < attack.Parent.ChainCount; i++)
                {
                    if (attack.Parent.Chains[i] == attack)
                    {
                        attack.Parent.Chains[i] = null;
                        break;
                    }
                }

                attack.Parent = null;
            }

            attacks.Remove(attack);
        }

        /// <summary>
        /// Resets the combo to its idle state
        /// </summary>
        public void Reset()
        {
            SwitchAttack(Entry);
        }

        /// <summary>
        /// Sets the next attack to move to when the current attack finishes
        /// </summary>
        /// <param name="input">The index of the input to use in the Chains list</param>
        public void Chain(int input)
        {
            if (activeAttack.Chains[input] != null)
            {
                if (activeAttack.Chains[input] != null)
                {
                    nextAttack = activeAttack.Chains[input];
                }

                if(nextAttack != null && (activeAttack.IsEntry || activeAttack.CanCombo))
                {
                    SwitchAttack(nextAttack);
                }
            }
        }

        /// <summary>
        /// Switches the active attack to the given attack
        /// </summary>
        /// <param name="attack">The attack to switch to</param>
        private void SwitchAttack(CSAttack attack)
        {
            activeAttack.WindupStart -= OnWindup;
            activeAttack.AttackStart -= OnAttackStart;
            activeAttack.AttackEnd -= OnAttackEnd;
            activeAttack.CooldownEnd -= OnCooldownEnd;
            activeAttack.ComboEnd -= OnComboEnd;

            activeAttack = attack;
            nextAttack = null;

            activeAttack.WindupStart += OnWindup;
            activeAttack.AttackStart += OnAttackStart;
            activeAttack.AttackEnd += OnAttackEnd;
            activeAttack.CooldownEnd += OnCooldownEnd;
            activeAttack.ComboEnd += OnComboEnd;

            QueueAttack?.Invoke();
        }

        #endregion

        #region Callbacks

        private void OnWindup()
        {
            WindupStart?.Invoke();
        }

        private void OnAttackStart()
        {
            AttackStart?.Invoke();
        }

        private void OnAttackEnd()
        {
            AttackEnd?.Invoke();
        }

        private void OnCooldownEnd()
        {
            CooldownEnd?.Invoke();

            if (nextAttack != null)
            {
                SwitchAttack(nextAttack);
            }
        }

        private void OnComboEnd()
        {
            ComboEnd?.Invoke();
            Reset();
        }

        #endregion
    }
}