using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Firing.Targets
{
    public class TargetTarget : FiringTarget
    {
        [field: NonSerialized] public override int Punteggio { get; set; } = 1;
        public TargetExploder targetExploder;

        protected override int OnShot(Vector3 position)
        {
            targetExploder.Explode(position);
            this.gameObject.SetActive(false);
            return Punteggio;
        }
    }
}