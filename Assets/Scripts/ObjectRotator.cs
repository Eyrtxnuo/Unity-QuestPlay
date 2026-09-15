using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ObjectRotator : MonoBehaviour
{
    public float speed = 180.0f;
    
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    
    void FixedUpdate()
    {
        //transform.Rotate(Vector3.up, speed * Time.fixedDeltaTime);
        float step = speed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, step, 0));
    }
}
