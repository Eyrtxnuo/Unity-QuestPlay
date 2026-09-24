using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Firing.Targets
{
    public class TargetTarget : FiringTarget
    {
        public delegate void TargetCallback();

        [field: NonSerialized] public override int Punteggio { get; set; } = 1;
        public GameObject targetFacade;
        public TargetExploder targetExploder;
        
        [Header("Audio")]
        public AudioClip[] breakSounds;
        public SfxPool sfxPool;

        private TargetCallback _onDestroyCallback;

        protected override int OnShot(Vector3 position)
        {
            this.targetFacade.SetActive(false);
            this.GetComponent<Collider>().enabled =
                false; // getComponent viene eseguito solo una volta (poi il collider viene disattivato), non serve cache
            this.tag = "Untagged";
            targetExploder.Explode(position, OnCallback);
            PlayBreakSound();
            return Punteggio;
        }

        private void OnCallback()
        {
            base.OnShot(transform.position);
        }

        public void SetupDestroyCallback(TargetCallback onDestroy)
        {
            this._onDestroyCallback = onDestroy;
        }

        private void OnDestroy()
        {
            _onDestroyCallback?.Invoke();
        }
        
        private void PlayBreakSound()
        {
            if (breakSounds != null && breakSounds.Length > 0 && sfxPool is not null)
            {
                sfxPool.PlayClipAtPosition(breakSounds[Random.Range(0, breakSounds.Length)], transform.position);
            }
        }
        
    }
}