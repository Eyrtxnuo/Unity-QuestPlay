using UnityEngine;

namespace Firing
{
    public class GunGrabPoint : MonoBehaviour
    {
        public Transform rotator;

        void Update()
        {
            var rot = Quaternion.LookRotation(transform.localPosition);
            rotator.localRotation = rot;
            
        }
    }
}
