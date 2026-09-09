using UnityEngine;

namespace Firing
{
    public class TriggerPull : MonoBehaviour
    {
    
        public float maxRotationAngle = 25f;
        public float minRotationAngle = 0f;
        public Axis rotAxis;


        void Update()
        {
            float triggerValue = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
            float rotationAngle = Mathf.Lerp(minRotationAngle, maxRotationAngle, triggerValue);
            this.transform.localRotation = rotAxis switch
            {
                Axis.X => Quaternion.Euler(rotationAngle, 0f, 0f),
                Axis.Y => Quaternion.Euler(0f, rotationAngle, 0f),
                Axis.Z => Quaternion.Euler(0f, 0f, rotationAngle),
                _ => transform.localRotation
            };

        }

        public enum Axis
        {
            X, Y, Z
        }
    }
}
