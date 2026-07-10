using CowRun;
using Meta.WitAi;
using UnityEngine;

public class CowInteraction : MonoBehaviour
{
    public CowControls cowControls;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider triggerObject)
    {
        if (triggerObject.CompareTag("CR_Collider"))
        {
            Debug.Log("Trigger collider .");
            var collided = triggerObject.gameObject.GetComponent<CollidingElement>();
            if (collided == null) return;
            var points = collided.PointsValue;
            CollisionEffect effect = collided.Hit();
            if (cowControls)
            {
                cowControls.ApplyEffect(effect);
            }
            
        }
    }
}
