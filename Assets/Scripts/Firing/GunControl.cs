using Firing;
using UnityEngine;

public class GunControl : MonoBehaviour
{
    public float maxHitDistance = 50f;
    public LayerMask collisionLayers;
    public FiringScoreCounter scoreCounter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
    }

    // Update is called once per frame
    protected virtual void Update()
    {
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

    public float recoilForce = 5f;

    protected void ExecuteShot()
    {
        Vector3 startPos = transform.position;
        Ray ray = new Ray(startPos, transform.forward);
        Debug.DrawLine(transform.position, transform.forward * 6, Color.green, 2.0f);
        // Esegue il raycast solo in questo istante
        if (Physics.SphereCast(ray, 0.05f, out RaycastHit hit, maxHitDistance, collisionLayers))
        {
            // Controlla se l'oggetto ha il tag corretto
            if (hit.collider.CompareTag("AimTarget"))
            {
                // Debug.Log("Eliminato: " + hit.collider.name);
                // Destroy(hit.collider.gameObject);
                Debug.Log("Colpito: " + hit.collider.name);
                FiringTarget target = hit.collider.GetComponent<FiringTarget>();
                var points = target.Hit(hit.point);
                if (scoreCounter) scoreCounter.Score += points;
            }
        }


        GameObject gun = GameObject.FindWithTag("Gun");

        if (gun)
        {
            Rigidbody grb = gun.GetComponent<Rigidbody>();
            grb.AddForce(-grb.transform.forward * 4, ForceMode.Impulse);
            grb.AddRelativeTorque(Vector3.left * (recoilForce / 4), ForceMode.Impulse);
        }
    }
}