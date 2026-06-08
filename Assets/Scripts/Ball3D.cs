using UnityEngine;

public class Ball3D : MonoBehaviour
{
    public float launchForce = 0.15f;
    
    private Rigidbody rb;
    private Vector3 originalPosition;
    
    void Start()
    {
        
        originalPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        rb.maxLinearVelocity = 9;
        Launch();
    }

    public void Relaunch()
    {
        this.transform.position = originalPosition;
        rb.linearVelocity = Vector3.zero;
        Launch();
    }

    public void Launch()
    {
        // Randomly pick player 1 or player 2's side of the table (Z axis)
        float directionZ = 1;//Random.value < 0.5f ? -1f : 1f;
        
        // Add a slight randomization to the X (left/right) and Y (upward bounce)
        Vector3 launchDirection = new Vector3(0, -0.6f, directionZ).normalized;
        Random.Range(-0.3f, 0.3f);
        // Apply an impulse force to shoot the ball
        rb.AddForce(launchDirection * launchForce, ForceMode.Impulse);
    }
}