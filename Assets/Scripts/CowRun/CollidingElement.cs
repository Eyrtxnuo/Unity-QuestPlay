using CowRun;
using Meta.WitAi;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(Collider))] 
public abstract class CollidingElement : MonoBehaviour
{
    public abstract int PointsValue { get; protected set; } 
    protected Collider _collider;
    public bool IsHit { get; protected set; } = false;
    public AudioClip hitSound;
    
    
    protected virtual void Start()
    {
        _collider = GetComponent<Collider>();
    }

    
    // Default (da estendere)
    public virtual CollisionEffect Hit()
    {
        IsHit = true;
        //Debug.Log("Collider Hit.");
        _collider.enabled = false;
        //Destroy(this.gameObject);
        return null;
    }

    public AudioClip GetHitSound()
    {
        return hitSound;
    }
}