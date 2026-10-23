using System;
using CowRun;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(ScarecrowRow))]
    [CanEditMultipleObjects]
    public class ScarecrowRowEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            GUILayout.Space(10);

            // Create the button and check if it was clicked
            if (GUILayout.Button("Generate scarecrow row"))
            {
                foreach (ScarecrowRow script in targets)
                {
                    script.GenerateRow();
                }
            }
            
            
        }

        public void OnValidate()
        {
            this.GetComponent<ScarecrowRow>().GenerateRow();
        }
    }
}
