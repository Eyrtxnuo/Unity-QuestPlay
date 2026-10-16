using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Barrier))]
    [CanEditMultipleObjects]
    public class BarrierColliderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();


            GUILayout.Space(10);

            // Create the button and check if it was clicked
            if (GUILayout.Button("Align To SplineRoad"))
            {
                foreach (Barrier script in targets)
                {
                    script.AlignToSpline();
                }
            }
        }
    }
}
