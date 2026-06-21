using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserPointer : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [Header("Laser Settings")]
    public float maxLaserDistance = 50f;
    public LayerMask collisionLayers; // Select which layers the laser can hit

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        
        // Ensure the Line Renderer has 2 points (Start and End)
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        ShootLaser();
    }

    void ShootLaser()
    {
        // Start position is the origin of this GameObject
        Vector3 startPos = transform.position;
        lineRenderer.SetPosition(0, startPos);

        // Raycast forward from the GameObject
        Ray ray = new Ray(startPos, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxLaserDistance, collisionLayers))
        {
            // If we hit something, stop the laser at the hit point
            lineRenderer.SetPosition(1, hit.point);
            
            // Optional: Do something with the object we hit
            // Debug.Log("Laser hit: " + hit.collider.name);
        }
        else
        {
            // If we hit nothing, extend the laser to its max distance
            Vector3 endPos = startPos + (transform.forward * maxLaserDistance);
            lineRenderer.SetPosition(1, endPos);
        }
    }
}