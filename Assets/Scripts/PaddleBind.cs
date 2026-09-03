using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PaddleBind : MonoBehaviour
{
    
    public GameObject hand;
    public float movementSpeed = 30;
    public float rotationSpeed = 3600;
    private float vOffsetPaddle = 0.05f;

    
    private Rigidbody prb;
    
    void Start()
    {
        prb = GetComponent<Rigidbody>();
    }

    
    public float movementDampening = 2f;
    void FixedUpdate()
    {
        Vector3 targetPos = hand.transform.position - (this.transform.up * vOffsetPaddle);
        
        Vector3 offsetVector = targetPos - prb.position;
        Vector3 speedVector = Vector3.ClampMagnitude((offsetVector/movementDampening) / Time.fixedDeltaTime,  movementSpeed);
        Debug.DrawLine(hand.transform.position, targetPos, Color.red);
        prb.linearVelocity = speedVector;
        Quaternion targetRotation = hand.transform.rotation * Quaternion.Euler(40f, 0f, 0f);
        Quaternion deltaRotation = targetRotation * Quaternion.Inverse(prb.rotation);
        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f) angle -= 360f; // controlla se serve
        
        if (!float.IsNaN(axis.x) && !float.IsInfinity(axis.x))
        {
            Vector3 angularSpeed = (axis * (angle/movementDampening * Mathf.Deg2Rad)) / Time.fixedDeltaTime;
            float maxAngularSpeed = rotationSpeed * Mathf.Deg2Rad;
            prb.angularVelocity = Vector3.ClampMagnitude(angularSpeed, maxAngularSpeed);
        }
    }
    
    public float bounceForce = 15f;
 
}
