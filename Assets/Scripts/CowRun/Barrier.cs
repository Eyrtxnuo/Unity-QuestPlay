using CowRun;
using LocomotionCollision;
using Meta.WitAi;
using UnityEngine;
using UnityEngine.Splines;

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

    public void AlignToSpline()
    {
        GameObject spline = GameObject.FindGameObjectWithTag("Locomotion");
        if (spline is null)
        {
            Debug.LogWarning("Spline not found");
            return;
        }
        
        SplineLocomotionCollision slc = spline.GetComponent<SplineLocomotionCollision>();
        if (slc is null)
        {
            Debug.LogWarning("Spline Collision not found in 'Locomotion' tagged GameObject");
            return;
        }

        var outs =slc.GetClosestPointAndTangent(this.transform.position);
        this.transform.rotation = Quaternion.LookRotation(outs.splineTangent);
        
    }
}