using System;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class PaddleBind : MonoBehaviour
{
    
    public GameObject hand;
    public float movementSpeed = 30;
    public float rotationSpeed = 3600;
    
    public bool lockRotX;
    public bool lockRotY;
    public bool lockRotZ;
    
    public float offsetRotationX = 40f;
    public float offsetRotationY;
    public float offsetRotationZ;
    
    public float verticalOffset = 0.05f;

    
    private Rigidbody prb;
    
    void Start()
    {
        prb = GetComponent<Rigidbody>();
    }

    
    public float movementDampening = 2f;
    void FixedUpdate()
    {
        Vector3 targetPos = hand.transform.position - (this.transform.up * verticalOffset);
        Vector3 offsetVector = targetPos - prb.position;
        Vector3 speedVector = Vector3.ClampMagnitude((offsetVector/movementDampening) / Time.fixedDeltaTime,  movementSpeed);
        Debug.DrawLine(hand.transform.position, targetPos, Color.red);
        prb.linearVelocity = speedVector;
       
        Vector3 offsetRotation = new Vector3(
            (!lockRotX) ? offsetRotationX : 0, //offset 0 if axis is locked
            (!lockRotY) ? offsetRotationY : 0, //offset 0 if axis is locked
            (!lockRotZ) ? offsetRotationZ : 0  //offset 0 if axis is locked
        );
        Quaternion targetRotationPreLock = hand.transform.rotation * Quaternion.Euler(offsetRotation);
        Vector3 eulerPreLock = targetRotationPreLock.eulerAngles;
        Vector3 currentEuler = this.transform.rotation.eulerAngles;
        
        Vector3 lockedRotation = new Vector3(
            lockRotX ? currentEuler.x : eulerPreLock.x,
            lockRotY ? currentEuler.y : eulerPreLock.y,
            lockRotZ ? currentEuler.z : eulerPreLock.z
        );
        
        Quaternion targetRotation = Quaternion.Euler(lockedRotation);
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
