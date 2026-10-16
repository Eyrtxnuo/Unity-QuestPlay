using System;
using LocomotionCollision;
using UnityEngine;
using UnityEngine.Splines;

public class CameraForwardAlign : MonoBehaviour
{
    public SplineLocomotionCollision splineContainer;
    public GameObject cameraObject;
    public float rotationSpeed = 5f;
    
    private Vector3 _currentVelocity =  Vector3.zero;
    
    private void Start()
    {
        if(splineContainer == null ) this.enabled = false;
        if(cameraObject == null) this.enabled = false;
        if (!this.enabled)
        {
            Debug.LogWarning("[CameraForwardAlign]: CameraForwardAlign not enabled since objects are not assigned");
        }
        
    }

    private void Update()
    {
        (Vector3 closestPoint, float distance, float t, Vector3 splineTangent) = this.splineContainer.GetClosestPointAndTangent(this.transform.position);
        
        Quaternion targetRotation = Quaternion.LookRotation(splineTangent);
        this.transform.rotation = Quaternion.Slerp(
            this.transform.rotation, 
            targetRotation, 
            Time.deltaTime * rotationSpeed
        );
    }
}
