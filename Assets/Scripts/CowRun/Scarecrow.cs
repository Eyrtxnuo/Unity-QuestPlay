using Meta.WitAi;
using UnityEngine;

public class Scarecrow : MonoBehaviour
{
    public int PointsValue { get; private set; } = 1; 
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider triggerObject)
    {
    }

    public void HitScarecrow()
    {
        // Default (fatto per overridare)
        Destroy(this.gameObject);
    }
}