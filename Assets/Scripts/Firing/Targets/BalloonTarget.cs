using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Firing.Targets
{
    [RequireComponent(typeof(Rigidbody))]
    public class BalloonTarget : FiringTarget
    {
        [field: NonSerialized] public override int Punteggio { get; set; } = 1;
        

        public float floatingSpeed = 0.3f;
        
        public GameObject popParticleSystem;
        
        private Rigidbody _rb;

        void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            var startVel = _rb.linearVelocity;
            startVel.y = Mathf.MoveTowards(startVel.y, floatingSpeed, Time.fixedDeltaTime);
            _rb.linearVelocity = startVel;
        }

        protected override int OnShot(Vector3 position)
        {
            Instantiate(popParticleSystem, position, Quaternion.identity);
            return base.OnShot(position);
        }
    }
}