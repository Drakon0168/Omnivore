using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MovementSystem
{
    [CustomEditor(typeof(MSEntityStats))]
    public class MSEntityStatsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            //Move Settings
            GUIContent label = new GUIContent();
            label.text = "Move Speed";
            label.tooltip = "The speed that the entity moves at by default.";
            EditorGUILayout.PropertyField(serializedObject.FindProperty("moveSpeed"), label);

            label.text = "Sprint Speed";
            label.tooltip = "The speed that the entity moves at while sprinting.";
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sprintSpeed"), label);

            label.text = "Acceleration Time";
            label.tooltip = "The target time for it to take the entity to move at its desired velocity.";
            EditorGUILayout.PropertyField(serializedObject.FindProperty("accelerationTime"), label);

            label.text = "Move Force";
            label.tooltip = "The maximum force to use for this entity's movement";
            EditorGUILayout.PropertyField(serializedObject.FindProperty("moveForce"), label);

            //Variable Move Speed Settings
            label.text = "Variable Move Speed";
            label.tooltip = "Whether or not movespeed is affected by the entity's forward direction.";
            EditorGUILayout.PropertyField(serializedObject.FindProperty("variableSpeed"), label);
            if (serializedObject.FindProperty("variableSpeed").boolValue)
            {
                label.text = "Strafe Speed Multiplier";
                label.tooltip = "Sideways movement multiplier.";
                EditorGUILayout.PropertyField(serializedObject.FindProperty("strafeSpeedMult"), label);

                label.text = "Backward Speed Multiplier";
                label.tooltip = "Backwards speed multiplier.";
                EditorGUILayout.PropertyField(serializedObject.FindProperty("backSpeedMult"), label);

                label.text = "Max Forward Angle";
                label.tooltip = "The maximum angle from the transform's forward to count as forward movement in degrees.";
                EditorGUILayout.PropertyField(serializedObject.FindProperty("forwardAngle"), label);

                label.text = "Max Backward Angle";
                label.tooltip = "The maximum angle from the transform's back to count as forward movement in degrees.";
                EditorGUILayout.PropertyField(serializedObject.FindProperty("backwardsAngle"), label);
            }

            label.text = "Can Fly";
            label.tooltip = "Whether or not the entity can fly, if true the entity can apply move forces in the y axis.";
            EditorGUILayout.PropertyField(serializedObject.FindProperty("canFly"), label);

            label.text = "Jump Velocity";
            label.tooltip = "The upwards velocity to apply when jumping.";
            EditorGUILayout.PropertyField(serializedObject.FindProperty("jumpVelocity"), label);

            label.text = "Dash Speed";
            label.tooltip = "The default speed of the dash.";
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dashSpeed"), label);

            label.text = "Dash Time";
            label.tooltip = "The amount of time taken to dash.";
            EditorGUILayout.PropertyField(serializedObject.FindProperty("dashTime"), label);

            label.text = "Variable Dash Speed";
            label.tooltip = "Whether or not the dash speed changes over the course of a dash. Toggle on to enable dash speed curve.";
            EditorGUILayout.PropertyField(serializedObject.FindProperty("variableDashSpeed"), label);

            if (serializedObject.FindProperty("variableDashSpeed").boolValue)
            {
                label.text = "Dash Speed Over Time";
                label.tooltip = "Curve indicating the speed of the dash over its length. X of 0 is the start of the dash, X of 1 is the end, Y of 1 is dash speed, Y of 0 means there is no movement.";
                EditorGUILayout.PropertyField(serializedObject.FindProperty("dashSpeedOverTime"), label);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}