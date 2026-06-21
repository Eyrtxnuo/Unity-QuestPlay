using UnityEngine;

public class GunControl : MonoBehaviour
{
    
    public float maxHitDistance = 50f;
    public LayerMask collisionLayers;    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update(){
    
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            //Debug.Log("Pulsante B premuto, reset palla.");
        }
        if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger, OVRInput.Controller.RTouch))
        {
            Debug.Log("Trigger premuto.");
            ExecuteShot();
        }
    }
    
    void ExecuteShot()
    {
        Vector3 startPos = transform.position;
        Ray ray = new Ray(startPos, transform.forward);
        RaycastHit hit;
        Debug.DrawLine(transform.position, transform.forward*6, Color.green, 2.0f);
        // Esegue il raycast solo in questo istante
        if (Physics.Raycast(ray, out hit, maxHitDistance, collisionLayers))
        {
            // Controlla se l'oggetto ha il tag corretto
            if (hit.collider.CompareTag("AimTarget"))
            {
                Debug.Log("Eliminato: " + hit.collider.name);
                Destroy(hit.collider.gameObject);
            }
        }
        
    }
}
