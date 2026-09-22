using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Firing.Targets
{
    public class TargetTarget : FiringTarget
    {
        public delegate void TargetCallback();

        [field: NonSerialized] public override int Punteggio { get; set; } = 1;
        public GameObject targetFacade;
        public TargetExploder targetExploder;

        private TargetCallback _onDestroyCallback;

        protected override int OnShot(Vector3 position)
        {
            this.targetFacade.SetActive(false);
            this.GetComponent<Collider>().enabled =
                false; // getComponent viene eseguito solo una volta (poi il collider viene disattivato), non serve cache
            this.tag = "Untagged";
            targetExploder.Explode(position, OnCallback);
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
    }
}