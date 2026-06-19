using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Ball3D : MonoBehaviour
{
    public float launchForce = 4f;
    public float magnusConstant = 0.05f;
    
    private Rigidbody rb;
    private Vector3 originalPosition;
    
    void Start()
    {
        
        originalPosition = transform.position;
        rb = GetComponent<Rigidbody>();
        //rb.maxLinearVelocity = 9;
        Launch();
    }

    public void Relaunch()
    {
        RigidbodyInterpolation pre_interpolation = rb.interpolation;
        rb.interpolation = RigidbodyInterpolation.None; // disattiva interpolazione per evitare di far impazzire unity
        rb.position = originalPosition;
        rb.rotation = Quaternion.identity;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.interpolation = pre_interpolation; // riattiva l'interpolazione al valore precedente
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
        rb.AddForce(launchDirection * launchForce, ForceMode.VelocityChange);
    }
    
    void FixedUpdate()
    {
        Vector3 magnusForce = Vector3.Cross(rb.angularVelocity, rb.linearVelocity) * magnusConstant;
        rb.AddForce(magnusForce, ForceMode.Force);
    }
}