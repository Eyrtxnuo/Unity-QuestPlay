using UnityEngine;
using UnityEngine.Serialization;

public class CustomGravity : MonoBehaviour
{
    [FormerlySerializedAs("gravityScale")] public float gravityAcceleration = -9.81f; 
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; 
    }

    void FixedUpdate()
    {
        Vector3 gravity = new Vector3(0, gravityAcceleration, 0);
        rb.AddForce(gravity, ForceMode.Acceleration);
    }
}
