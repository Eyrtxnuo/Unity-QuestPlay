using System;
using UnityEngine;
using Random = UnityEngine.Random;

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
        private float upBounceForce = 4;
        private float maxHorizontalForce = 1;
        public float terminalSpeed = 4f;

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
                _rb.linearVelocity = new Vector3(Random.Range(-maxHorizontalForce, maxHorizontalForce), upBounceForce, Random.Range(-maxHorizontalForce, maxHorizontalForce));
                
            }

            if (_rb.linearVelocity.magnitude > terminalSpeed)
            {
                _rb.linearVelocity = _rb.linearVelocity.normalized * terminalSpeed;
            }
            
            return Punteggio;
        }
    }
}