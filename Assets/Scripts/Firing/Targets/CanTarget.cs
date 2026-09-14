using System;
using UnityEngine;

namespace Firing.Targets
{
    
        [RequireComponent(typeof(AudioSource))]
        [RequireComponent(typeof(Rigidbody))]
    public class CanTarget : FiringTarget
    {
        public override int Punteggio { get; set; } = 1;

        public int maxBounces = 5;

        private int bouncesCount = 0;
        private Rigidbody _rb;
        private Vector3 bounceForce = new Vector3(0f, 4, 0);

        void Awake()
        {
            _rb = this.GetComponent<Rigidbody>();
        }

        protected override int OnShot(Vector3 position)
        {
            bouncesCount++;

            if (bouncesCount >= maxBounces)
            {
                base.OnShot(position);//Destroy
            }
            else
            {
                _rb.linearVelocity = bounceForce;
            }
            
            return Punteggio;
        }
    }
}