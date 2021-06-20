using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace CombatSystem {
    public class CSAttackElement : VisualElement
    {
        private CSAttack data;

        private ObjectField colliderField;
        private FloatField damageField;
        private FloatField windupTimeField;
        private FloatField attackTimeField;
        private FloatField cooldownTimeField;
        private FloatField comboTimeField;

        public CSAttackElement(CSAttack data)
        {
            this.data = data;
            
            Add(Resources.Load<VisualTreeAsset>("CSAttackLayout").CloneTree());

            colliderField = this.Q<ObjectField>("ColliderField");
            colliderField.objectType = typeof(Collider);
            colliderField.SetValueWithoutNotify(data.Collider);
            colliderField.RegisterValueChangedCallback(e =>
            {
                this.data.Collider = (Collider)e.newValue;
            });

            damageField = this.Q<FloatField>("DamageField");
            damageField.SetValueWithoutNotify(data.DamageMult);
            damageField.RegisterValueChangedCallback(e =>
            {
                this.data.DamageMult = e.newValue;
            });

            windupTimeField = this.Q<FloatField>("WindupField");
            windupTimeField.SetValueWithoutNotify(data.WindupTime);
            windupTimeField.RegisterValueChangedCallback(e =>
            {
                this.data.WindupTime = e.newValue;
            });

            attackTimeField = this.Q<FloatField>("AttackTimeField");
            attackTimeField.SetValueWithoutNotify(data.AttackTime);
            attackTimeField.RegisterValueChangedCallback(e =>
            {
                this.data.AttackTime = e.newValue;
            });

            cooldownTimeField = this.Q<FloatField>("CooldownField");
            cooldownTimeField.SetValueWithoutNotify(data.CooldownTime);
            cooldownTimeField.RegisterValueChangedCallback(e =>
            {
                this.data.CooldownTime = e.newValue;
            });

            comboTimeField = this.Q<FloatField>("ComboTimeField");
            comboTimeField.SetValueWithoutNotify(data.ComboTime);
            comboTimeField.RegisterValueChangedCallback(e =>
            {
                this.data.ComboTime = e.newValue;
            });
        }
    }
}