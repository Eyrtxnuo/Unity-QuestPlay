using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Firing.Targets
{
    
        [RequireComponent(typeof(AudioSource))]
        [RequireComponent(typeof(Rigidbody))]
    public class CanTarget : TargetTarget
    {
        [field: NonSerialized] public override int Punteggio { get; set; } = 1;
        public BoundArea returnArea;
        public int maxBounces = 5;
        public float despawnUnderY = -0.2f;

        private int _bouncesCount = 0;
        private Rigidbody _rb;
        private float _upBounceForce = 3;
        private float _maxHorizontalForce = 1;
        private float _terminalFallSpeed = 2.5f;
        private float _maxAngularVelocity = 30;
        

        void Awake()
        {
            _rb = this.GetComponent<Rigidbody>();
        }

        protected override int OnShot(Vector3 position)
        {
            _bouncesCount++;
            Punteggio = _bouncesCount+1;

            if (_bouncesCount >= maxBounces)
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
                    var randForce = Random.Range(0, _maxHorizontalForce);

                    _rb.linearVelocity = new Vector3(
                        randForce * approachAreaDirection.x,
                        _upBounceForce,
                        randForce * approachAreaDirection.z
                    );
                }
                else
                {
                    _rb.linearVelocity = new Vector3(Random.Range(-_maxHorizontalForce, _maxHorizontalForce), _upBounceForce, Random.Range(-_maxHorizontalForce, _maxHorizontalForce));
                }
                _rb.angularVelocity = Random.onUnitSphere * Random.Range(0f, _maxAngularVelocity);
            }

            
            return Punteggio;
        }

        void FixedUpdate()
        {
            if(this.transform.position.y < despawnUnderY) Destroy(this.gameObject);// death plane virtuale a Y < (-0.2f default)
            
            // clamp velocità (applicato solo in caduta)
            if (_rb.linearVelocity.y < 0 && _rb.linearVelocity.sqrMagnitude > _terminalFallSpeed * _terminalFallSpeed)
            {
                _rb.linearVelocity = Vector3.ClampMagnitude(_rb.linearVelocity, _terminalFallSpeed);
            }
        }
    }
}