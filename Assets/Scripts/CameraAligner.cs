using System;
using UnityEngine;

public class CameraAligner : MonoBehaviour
{
    public GameObject baseTarget;
    public Vector3 offsetPosition = Vector3.zero;
    public GameObject cameraObject;
    public float smoothTime = 5f;
    

    private Vector3 _currentVelocity = Vector3.zero;
    
    private void Start()
    {
        if(baseTarget == null ) this.enabled = false;
        if(cameraObject == null) this.enabled = false;
        if (!this.enabled)
        {
            Debug.LogWarning("[CameraAligner]: CameraAligner not enabled since objects are not assigned");
        }
        
    }
    
    private void Update()
    {
        Vector3 targetCameraPosition = baseTarget.transform.position + offsetPosition;
        Vector3 cameraDamp = Vector3.SmoothDamp(cameraObject.transform.position, targetCameraPosition,ref _currentVelocity, smoothTime);
        Vector3 move = cameraDamp - cameraObject.transform.position;
        this.transform.position += move;
        
    }

    private void OnDrawGizmosSelected()
    {
        if(baseTarget == null ) return;
        if(cameraObject == null) return;
        
        Vector3 targetPosition = baseTarget.transform.position + offsetPosition;
        Vector3 deltaPosition = targetPosition - cameraObject.transform.position;
        
        Vector3 move = deltaPosition;
        
        Gizmos.color = Color.orchid;
        Gizmos.DrawSphere(targetPosition, 0.2f);
        Gizmos.color = Color.darkMagenta;
        Gizmos.DrawLine(cameraObject.transform.position, cameraObject.transform.position + move);
        
    }

    private void OnDrawGizmos()
    {
        if(baseTarget == null ) return;
        Vector3 targetPosition = baseTarget.transform.position + offsetPosition;
        Gizmos.DrawIcon(targetPosition, "targetgizmo.png", true);
    }
}
