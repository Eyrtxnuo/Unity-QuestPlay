using UnityEngine;
using UnityEditor;

namespace Editor
{
    [CustomEditor(typeof(PaddleBind))]
    public class PaddleBindSettingsEditor : UnityEditor.Editor
    {
        SerializedProperty lockRotX, lockRotY, lockRotZ;
        SerializedProperty offsetRotationX, offsetRotationY, offsetRotationZ;

        void OnEnable()
        {
            // Collega le proprietà
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
            DrawPropertiesExcluding(serializedObject, "lockRotX", "lockRotY", "lockRotZ",  "offsetRotationX", "offsetRotationY", "offsetRotationZ");
            
            //////////////RIGA Rotation Locks/////////////////////
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Rotation Locks", "Disable following the rotation on the selected axis"));
            float toggleWidth = 40f;
            lockRotX.boolValue = EditorGUILayout.ToggleLeft("X", lockRotX.boolValue, GUILayout.Width(toggleWidth));
            lockRotY.boolValue = EditorGUILayout.ToggleLeft("Y", lockRotY.boolValue, GUILayout.Width(toggleWidth));
            lockRotZ.boolValue = EditorGUILayout.ToggleLeft("Z", lockRotZ.boolValue, GUILayout.Width(toggleWidth));
            EditorGUILayout.EndHorizontal();
            /////////////////////////////////////////////////////
            

            /////////////RIGA offsetRotation ////////////////////
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(new GUIContent("Rotation offset", "apply a rotation offset to the target"));
            float originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 15f; 
            
            
            // EditorGUILayout.PropertyField(offsetRotationX, new GUIContent("X"));
            // EditorGUILayout.PropertyField(offsetRotationY, new GUIContent("Y"));
            // EditorGUILayout.PropertyField(offsetRotationZ, new GUIContent("Z"));
            EditorGUI.BeginDisabledGroup(lockRotX.boolValue);
            EditorGUILayout.PropertyField(offsetRotationX, new GUIContent("X"));
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginDisabledGroup(lockRotY.boolValue);
            EditorGUILayout.PropertyField(offsetRotationY, new GUIContent("Y"));
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginDisabledGroup(lockRotZ.boolValue);
            EditorGUILayout.PropertyField(offsetRotationZ, new GUIContent("Z"));
            EditorGUI.EndDisabledGroup();
            
            EditorGUIUtility.labelWidth = originalLabelWidth;

            EditorGUILayout.EndHorizontal();
            
            ///////////////////////////////////////////////////////
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
