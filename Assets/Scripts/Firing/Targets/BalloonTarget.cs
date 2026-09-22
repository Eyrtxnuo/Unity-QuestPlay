using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Firing.Targets
{
    [RequireComponent(typeof(Rigidbody))]
    public class BalloonTarget : FiringTarget
    {
        [field: NonSerialized] public override int Punteggio { get; set; } = 1;
        

        public float floatingSpeed = 0.3f;
        public float speedDeviation = 0.1f;
        public float despawnY = 10f;
        
        public GameObject popParticleSystem;
        
        private float _despawnLength = 3f;
        private Vector3 _startScale;
        private float _despawnTime = 0f;
        private Rigidbody _rb;

        void Start()
        {
            floatingSpeed += Random.Range(-speedDeviation, speedDeviation);
            _rb = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            var startVel = _rb.linearVelocity;
            startVel.y = Mathf.MoveTowards(startVel.y, floatingSpeed, Time.fixedDeltaTime);
            _rb.linearVelocity = startVel;
        }

        void Update()
        {
            if (_despawnTime == 0 && transform.position.y > despawnY)
            {
                _despawnTime = Time.time;
                _startScale = transform.localScale;
            }
            if(_despawnTime != 0)
            {
                transform.localScale = Vector3.Lerp(_startScale, Vector3.zero, (Time.time - _despawnTime) / _despawnLength);
                
                if (_despawnTime + _despawnLength < Time.time)
                {
                    Destroy(gameObject);
                }
            }

        }

        protected override int OnShot(Vector3 position)
        {
            Instantiate(popParticleSystem, position, Quaternion.identity);
            return base.OnShot(position);
        }
    }
}