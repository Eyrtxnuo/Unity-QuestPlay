using CowRun;
using Meta.WitAi;
using UnityEngine;

public class Barrier : CollidingElement
{
    public override int PointsValue { get; protected set; } = -5;

    protected override void Start() 
    {
        base.Start();// importante
    }    
    
    public override CollisionEffect Hit()
    {
        base.Hit();
        Debug.Log("Barrier Hit");
        Destroy(this.gameObject);
        return new SpeedSetEffect(0);
    }
}