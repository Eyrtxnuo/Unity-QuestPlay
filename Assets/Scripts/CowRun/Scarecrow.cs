using System.Collections;
using UnityEngine;

namespace CowRun
{
    [RequireComponent(typeof(Animator))]
    public class Scarecrow : CollidingElement
    {
        protected Animator _animator;
    
        public override int PointsValue { get; protected set; } = 1;
        public GameObject container;

        protected override void Start()
        {
            base.Start();
            this._animator = GetComponent<Animator>();
        }

        public override CollisionEffect Hit()
        {
            base.Hit();
            _animator.Play("fallAnimation");
            return new SpeedMultiplierEffect(0.96f);
        }

        public void DecommissionPrefab()
        {
            Destroy(container);
        }
    }
}