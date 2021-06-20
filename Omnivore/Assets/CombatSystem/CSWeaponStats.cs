using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem
{
    [CreateAssetMenu(fileName = "NewWeaponStats", menuName = "CombatSystem/CSWeaponStats")]
    public class CSWeaponStats : ScriptableObject
    {
        [SerializeField]
        [Tooltip("The base amount of damage that this weapon deals.")]
        private float baseDamage;
    }
}