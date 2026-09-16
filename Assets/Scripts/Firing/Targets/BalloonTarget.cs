using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Firing.Targets
{
    [RequireComponent(typeof(Rigidbody))]
    public class BalloonTarget : FiringTarget
    {
        public override int Punteggio { get; set; } = 1;

        public float floatingSpeed = 0.3f;
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
        
    }
}