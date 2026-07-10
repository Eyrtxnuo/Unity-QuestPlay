using CowRun;
using Meta.WitAi;
using UnityEngine;

[RequireComponent(typeof(Collider))] 
public abstract class CollidingElement : MonoBehaviour
{
    public abstract int PointsValue { get; protected set; } 
    protected Collider _collider;
    
    
    protected virtual void Start()
    {
        _collider = GetComponent<Collider>();
    }

    
    // Default (da estendere)
    public virtual CollisionEffect Hit()
    {
        Debug.Log("Collider Hit.");
        _collider.enabled = false;
        //Destroy(this.gameObject);
        return null;
    }
}