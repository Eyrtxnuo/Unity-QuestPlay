using UnityEngine;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(PaddleBind))]
    public class PaddleBindSettingsEditor : UnityEditor.Editor
    {
        
        SerializedProperty lockMovX, lockMovY, lockMovZ;
        SerializedProperty offsetMovementX, offsetMovementY, offsetMovementZ;
        
        SerializedProperty lockRotX, lockRotY, lockRotZ;
        SerializedProperty offsetRotationX, offsetRotationY, offsetRotationZ;

        void OnEnable()
        {
            //Movement
            lockMovX = serializedObject.FindProperty("lockMovX");
            lockMovY = serializedObject.FindProperty("lockMovY");
            lockMovZ = serializedObject.FindProperty("lockMovZ");
            
            offsetMovementX = serializedObject.FindProperty("offsetMovementX");
            offsetMovementY = serializedObject.FindProperty("offsetMovementY");
            offsetMovementZ = serializedObject.FindProperty("offsetMovementZ");
            
            
            //Rotation
            lockRotX = serializedObject.FindProperty("lockRotX");
            lockRotY = serializedObject.FindProperty("lockRotY");
            lockRotZ = serializedObject.FindProperty("lockRotZ");
            
            offsetRotationX = serializedObject.FindProperty("offsetRotationX");
            offsetRotationY = serializedObject.FindProperty("offsetRotationY");
            offsetRotationZ = serializedObject.FindProperty("offsetRotationZ");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, 
                "lockMovX", "lockMovY", "lockMovZ",  "offsetMovementX", "offsetMovementY", "offsetMovementZ",
                                    "lockRotX", "lockRotY", "lockRotZ",  "offsetRotationX", "offsetRotationY", "offsetRotationZ");
            
            
            //////////////Movement //////////////////////
            // Riga movementLocks
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Movement Locks", "Disable following the movement on the selected axis"));
            float toggleWidth_m = 40f;
            lockMovX.boolValue = EditorGUILayout.ToggleLeft("X", lockMovX.boolValue, GUILayout.Width(toggleWidth_m));
            lockMovY.boolValue = EditorGUILayout.ToggleLeft("Y", lockMovY.boolValue, GUILayout.Width(toggleWidth_m));
            lockMovZ.boolValue = EditorGUILayout.ToggleLeft("Z", lockMovZ.boolValue, GUILayout.Width(toggleWidth_m));
            EditorGUILayout.EndHorizontal();
            

            // RIGA offsetRotation
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Movement offset", "apply a position offset to the target"));
            float originalLabelWidth_m = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 15f; 
            
            
            EditorGUI.BeginDisabledGroup(lockMovX.boolValue);
            EditorGUILayout.PropertyField(offsetMovementX, new GUIContent("X"));
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginDisabledGroup(lockMovY.boolValue);
            EditorGUILayout.PropertyField(offsetMovementY, new GUIContent("Y"));
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginDisabledGroup(lockMovZ.boolValue);
            EditorGUILayout.PropertyField(offsetMovementZ, new GUIContent("Z"));
            EditorGUI.EndDisabledGroup();
            
            EditorGUIUtility.labelWidth = originalLabelWidth_m;

            EditorGUILayout.EndHorizontal();
            
            ///////////////////////////////////////////////////////
            
            
            
            
            //////////////Rotation //////////////////////
            // Riga rotationLocks
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Rotation Locks", "Disable following the rotation on the selected axis"));
            float toggleWidth_r = 40f;
            lockRotX.boolValue = EditorGUILayout.ToggleLeft("X", lockRotX.boolValue, GUILayout.Width(toggleWidth_r));
            lockRotY.boolValue = EditorGUILayout.ToggleLeft("Y", lockRotY.boolValue, GUILayout.Width(toggleWidth_r));
            lockRotZ.boolValue = EditorGUILayout.ToggleLeft("Z", lockRotZ.boolValue, GUILayout.Width(toggleWidth_r));
            EditorGUILayout.EndHorizontal();
            

            // RIGA offsetRotation
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Rotation offset", "apply a rotation offset to the target"));
            float originalLabelWidth_r = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 15f; 
            
            
            EditorGUI.BeginDisabledGroup(lockRotX.boolValue);
            EditorGUILayout.PropertyField(offsetRotationX, new GUIContent("X"));
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginDisabledGroup(lockRotY.boolValue);
            EditorGUILayout.PropertyField(offsetRotationY, new GUIContent("Y"));
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginDisabledGroup(lockRotZ.boolValue);
            EditorGUILayout.PropertyField(offsetRotationZ, new GUIContent("Z"));
            EditorGUI.EndDisabledGroup();
            
            EditorGUIUtility.labelWidth = originalLabelWidth_r;

            EditorGUILayout.EndHorizontal();
            
            ///////////////////////////////////////////////////////
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
