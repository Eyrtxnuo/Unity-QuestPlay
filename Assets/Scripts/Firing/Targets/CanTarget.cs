using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Firing.Targets
{
    
        [RequireComponent(typeof(AudioSource))]
        [RequireComponent(typeof(Rigidbody))]
    public class CanTarget : FiringTarget
    {
        [field: NonSerialized] public override int Punteggio { get; set; } = 1;
        public BoundArea returnArea;

        public int maxBounces = 5;

        private int bouncesCount = 0;
        private Rigidbody _rb;
        private float upBounceForce = 3;
        private float maxHorizontalForce = 1;
        private float terminalSpeed = 3;
        private float maxAngularVelocity = 30;
        

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
                if (returnArea && !returnArea.bounds.Contains(transform.position))
                {
                    Vector3 direction = (returnArea.bounds.center - transform.position).normalized;
                    var randomDirection = Random.insideUnitCircle.normalized;
                    var approachAreaDirection = (direction + new Vector3(randomDirection.x, 0, randomDirection.y)).normalized;
                    var randForce = Random.Range(0, maxHorizontalForce);

                    _rb.linearVelocity = new Vector3(
                        randForce * approachAreaDirection.x,
                        upBounceForce,
                        randForce * approachAreaDirection.z
                    );
                }
                else
                {
                    _rb.linearVelocity = new Vector3(Random.Range(-maxHorizontalForce, maxHorizontalForce), upBounceForce, Random.Range(-maxHorizontalForce, maxHorizontalForce));
                }
                _rb.angularVelocity = Random.onUnitSphere * Random.Range(0f, maxAngularVelocity);
            }

            if (_rb.linearVelocity.magnitude > terminalSpeed)
            {
                _rb.linearVelocity = _rb.linearVelocity.normalized * terminalSpeed;
            }
            
            return Punteggio;
        }
    }
}