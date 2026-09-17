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


        // private Rigidbody _rb;
        //
        // void Start()
        // {
        //     TryGetComponent<Rigidbody>(out _rb);
        // }

        protected override int OnShot(Vector3 position)
        {
            this.targetFacade.SetActive(false);
            this.GetComponent<Collider>().enabled = false;
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