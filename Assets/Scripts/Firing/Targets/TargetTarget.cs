using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Firing.Targets
{
    public class TargetTarget : FiringTarget
    {
        [field: NonSerialized] public override int Punteggio { get; set; } = 1;
        public GameObject targetFacade;
        public TargetExploder targetExploder;



        protected override int OnShot(Vector3 position)
        {
            this.targetFacade.SetActive(false);
            this.GetComponent<Collider>().enabled = false;// getComponent viene eseguito solo una volta (poi il collider viene disattivato), non serve cache
            this.tag = "Untagged";
            targetExploder.Explode(position,  OnCallback);
            return Punteggio;
        }

        private void OnCallback()
        {
            base.OnShot(transform.position);
        }
    }
}