using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace CombatSystem
{
    [CustomEditor(typeof(CSWeapon))]
    public class CSWeaponEditor : Editor
    {
        SerializedProperty stats;
        SerializedProperty combo;
        SerializedProperty attackTypes;

        private void OnEnable()
        {
            stats = serializedObject.FindProperty("stats");
            combo = serializedObject.FindProperty("combo");
            attackTypes = serializedObject.FindProperty("attackTypes");
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(stats, new GUIContent("Stats", "The base stats of this weapon."));
            EditorGUILayout.PropertyField(combo, new GUIContent("Combo", "The combo tree used by this weapon."));

            int attackCount = attackTypes.arraySize;
            EditorGUILayout.PropertyField(attackTypes, new GUIContent("Attack Types", "List of the different types of attacks available to this weapon."));

            if (GUILayout.Button("Edit Combo"))
            {
                ((CSCombo)combo.objectReferenceValue).Weapon = (CSWeapon)serializedObject.targetObject;
                EditorWindow.GetWindow<CSComboEditor>().OpenWindow((CSCombo)combo.objectReferenceValue);
            }

            if (serializedObject.ApplyModifiedProperties())
            {
                if(attackCount != attackTypes.arraySize)
                {
                    ((CSCombo)combo.objectReferenceValue).Chains = ((CSWeapon)serializedObject.targetObject).AttackTypes;
                }
            }
        }
    }
}