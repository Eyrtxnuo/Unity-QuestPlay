using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Firing.Targets
{
    public class ExplodingTarget : FiringTarget
    {
        public GameObject targetFacade;
        public TargetExploder targetExploder;
        

        protected override int OnShot(Vector3 position)
        {
            this.targetFacade.SetActive(false);
            this.GetComponent<Collider>().enabled =
                false; // getComponent viene eseguito solo una volta (poi il collider viene disattivato), non serve cache
            this.tag = "Untagged";
            targetExploder.Explode(position, OnExplosionCallback);
            return Punteggio;
        }

        private void OnExplosionCallback()
        {
            base.OnShot(transform.position);//destroy
        }
        
    }
}