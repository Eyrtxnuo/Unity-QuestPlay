using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Firing.Targets
{
    public class TargetTarget : ExplodingTarget
    {
        public delegate void TargetCallback();

        [field: NonSerialized] public override int Punteggio { get; set; } = 1;
        
        [Header("Audio")]
        public AudioClip[] breakSounds;
        public SfxPool sfxPool;

        private TargetCallback _onDestroyCallback;

        protected override int OnShot(Vector3 position)
        {
            base.OnShot(position);
            PlayBreakSound();
            return Punteggio;
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