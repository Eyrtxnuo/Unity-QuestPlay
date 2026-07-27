using System;
using UnityEngine;

public class FollowPoint : MonoBehaviour
{
    
    public GameObject point;
    public float movementSpeed = 5;
    public float rotationSpeed = 180;
    public float movementDampening = 2f;

    
    private Rigidbody _rb;
    private float _vOffsetPaddle = 0.05f;
    
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    
    void FixedUpdate()
    {
        Vector3 targetPos = point.transform.position - (this.transform.up * _vOffsetPaddle);
        
        Vector3 offsetVector = targetPos - _rb.position;
        Vector3 speedVector = Vector3.ClampMagnitude((offsetVector/movementDampening) / Time.fixedDeltaTime,  movementSpeed);
        Debug.DrawLine(_rb.transform.position, targetPos, Color.red);
        _rb.linearVelocity = speedVector;
        Quaternion targetRotation = point.transform.rotation * Quaternion.Euler(-40f, 0f, 0f);
        Quaternion deltaRotation = targetRotation * Quaternion.Inverse(_rb.rotation);
        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f) angle -= 360f; // controlla se serve
        
        if (!float.IsNaN(axis.x) && !float.IsInfinity(axis.x))
        {
            Vector3 angularSpeed = (axis * (angle/movementDampening * Mathf.Deg2Rad)) / Time.fixedDeltaTime;
            float maxAngularSpeed = rotationSpeed * Mathf.Deg2Rad;
            _rb.angularVelocity = Vector3.ClampMagnitude(angularSpeed, maxAngularSpeed);
        }
    }
    
    public float bounceForce = 15f;
 
}
