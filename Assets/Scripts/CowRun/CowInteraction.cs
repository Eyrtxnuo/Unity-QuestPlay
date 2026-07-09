using Meta.WitAi;
using UnityEngine;

public class CowInteraction : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider triggerObject)
    {
        if (triggerObject.CompareTag("CR_Scarecrow"))
        {
            Debug.Log("Trigger scarecrow.");
            Destroy(triggerObject.gameObject);
            Scarecrow scarecrow = triggerObject.gameObject.GetComponent<Scarecrow>();
            if (scarecrow == null) return;
            var points = scarecrow.PointsValue;
            scarecrow.HitScarecrow();
            
        }
    }
}
