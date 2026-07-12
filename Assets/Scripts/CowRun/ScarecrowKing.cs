using UnityEngine;

namespace CowRun
{
    [RequireComponent(typeof(Animator))]
    public class ScarecrowKing : CollidingElement
    {
        private Animator _animator;
    
        public override int PointsValue { get; protected set; } = 10;

        protected override void Start()
        {
            base.Start();
            this._animator = GetComponent<Animator>();
        }
        
        void FixedUpdate()
        {
            // implementa movimento TODO
        }

        public override CollisionEffect Hit()
        {
            base.Hit();
            _animator.Play("fallAnimation");
            return new SpeedMultiplierEffect(0.96f);
        }
    }
}