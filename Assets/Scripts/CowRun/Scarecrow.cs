using Meta.WitAi;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Collider))] 
public class Scarecrow : MonoBehaviour
{
    public int PointsValue { get; private set; } = 1; 
    private Animator _animator;
    private Collider _collider;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider triggerObject)
    {
    }

    public void HitScarecrow()
    {
        Debug.Log("Hit scarecrow.");
        // Default (fatto per overridare)
        _collider.enabled = false;
        _animator.Play("fallAnimation");
        //Destroy(this.gameObject);
    }
}