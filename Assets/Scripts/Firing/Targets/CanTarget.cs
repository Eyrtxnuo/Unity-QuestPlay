using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Firing.Targets
{
    
        [RequireComponent(typeof(AudioSource))]
        [RequireComponent(typeof(Rigidbody))]
    public class CanTarget : ExplodingTarget
    {
        [field: NonSerialized] public override int Punteggio { get; set; } = 1;
        public BoundArea returnArea;
        public int maxBounces = 5;
        public float despawnUnderY = -0.2f;
        public  Rigidbody rb;
        
        [Header("Audio")]
        public AudioClip[] canHitSounds;
        public AudioClip[] deathPlaneSounds;
        public SfxPool sfxPool;// usata per il suono death plane
        private AudioSource _audioSource; // usata per i suoni hit (segue il movimento della lattina)

        private int _bouncesCount = 0;
        private float _upBounceForce = 3;
        private float _maxHorizontalForce = 1;
        private float _terminalFallSpeed = 2.5f;
        private float _maxAngularVelocity = 30;

        private void Start()
        {
            TryGetComponent(out _audioSource);
        }

        protected override int OnShot(Vector3 position)
        {
            _bouncesCount++;
            Punteggio = _bouncesCount;

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

                    rb.linearVelocity = new Vector3(
                        randForce * approachAreaDirection.x,
                        _upBounceForce,
                        randForce * approachAreaDirection.z
                    );
                }
                else
                {
                    rb.linearVelocity = new Vector3(Random.Range(-_maxHorizontalForce, _maxHorizontalForce), _upBounceForce, Random.Range(-_maxHorizontalForce, _maxHorizontalForce));
                }
                rb.angularVelocity = Random.onUnitSphere * Random.Range(0f, _maxAngularVelocity);
            }
            PlayHitSound();
            
            return Punteggio;
        }

        void FixedUpdate()
        {
            if(this.transform.position.y < despawnUnderY)// death plane virtuale a Y < (-0.2f default)
            {
                PlayDeathPlaneSound();// audio di movimento di erba
                Destroy(this.gameObject); 
            }
            
            // clamp velocità (applicato solo in caduta)
            if (rb.linearVelocity.y < 0 && rb.linearVelocity.sqrMagnitude > _terminalFallSpeed * _terminalFallSpeed)
            {
                rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, _terminalFallSpeed);
            }
        }


        private void PlayHitSound() // try first audiosource, if null  use sfxpool
        {
            if(canHitSounds==null || canHitSounds.Length == 0) return;
            var clip = canHitSounds[Random.Range(0, canHitSounds.Length)];
            if (_audioSource is not null)
            {
                _audioSource.PlayOneShot(clip);
            }else if (sfxPool)
            {
                sfxPool.PlayClipAtPosition(clip, transform.position);
            }
        }

        private void PlayDeathPlaneSound()
        {
            if(deathPlaneSounds==null || deathPlaneSounds.Length == 0) return;
            if (!sfxPool) return;
            sfxPool.PlayClipAtPosition(deathPlaneSounds[Random.Range(0, deathPlaneSounds.Length)], transform.position);
        }
    }
}