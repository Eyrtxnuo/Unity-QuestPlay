using CowRun;
using Meta.WitAi;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Scarecrow : CollidingElement
{
    private Animator _animator;
    
    public override int PointsValue { get; protected set; } = 1;

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
}