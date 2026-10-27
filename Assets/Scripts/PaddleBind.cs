using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public class PaddleBind : MonoBehaviour
{
    
    public GameObject hand;
    
    [Header("Movement")]
    public float movementSpeed = 30;
    
    public bool lockMovX;
    public bool lockMovY;
    public bool lockMovZ;
    
    public float offsetMovementX = 0;
    [FormerlySerializedAs("verticalOffset")] public float offsetMovementY;
    public float offsetMovementZ;
    
    public float minDistanceToUpdateTarget = 0;
    public float minAngleToUpdateTarget = 0;
    
    [Header("Rotation")]
    public float rotationSpeed = 3600;
    
    public bool lockRotX;
    public bool lockRotY;
    public bool lockRotZ;
    
    public float offsetRotationX = 40f;
    public float offsetRotationY;
    public float offsetRotationZ;
    

    
    private Rigidbody _prb;
    private Vector3 _targetPos;
    private Quaternion _targetRot;
    
    
    private const float ROT_earlyStopThreshold = 5f;
    
    
    void Start()
    {
        _prb = GetComponent<Rigidbody>();
    }

    
    public float movementDampening = 2f;
    void FixedUpdate()
    {
        Vector3 localOffset = new Vector3(
            (!lockMovX) ? offsetMovementX : 0, //offset 0 if axis is locked
            (!lockMovY) ? offsetMovementY : 0, //offset 0 if axis is locked
            (!lockMovZ) ? offsetMovementZ : 0  //offset 0 if axis is locked
        );
        
        // Ruota l'offset come la mano. (riferimento fisso alla mano)
        Vector3 rotatedOffset = hand.transform.rotation * localOffset;
        
        Vector3 targetPos = new Vector3(
            lockMovX ? this.transform.position.x : hand.transform.position.x + rotatedOffset.x,
            lockMovY ? this.transform.position.y : hand.transform.position.y + rotatedOffset.y,
            lockMovZ ? this.transform.position.z : hand.transform.position.z + rotatedOffset.z
        );
        if (Vector3.Distance(targetPos, _targetPos) > minDistanceToUpdateTarget)
        {
            _targetPos = targetPos;
        }
        
        Vector3 offsetVector = _targetPos - _prb.position;
        Vector3 speedVector = Vector3.ClampMagnitude((offsetVector/movementDampening) / Time.fixedDeltaTime,  movementSpeed);
        Debug.DrawLine(hand.transform.position, _targetPos, Color.red);
        _prb.linearVelocity = speedVector;
       
        Vector3 offsetRotation = new Vector3(
            (!lockRotX) ? offsetRotationX : 0, //offset 0 if axis is locked
            (!lockRotY) ? offsetRotationY : 0, //offset 0 if axis is locked
            (!lockRotZ) ? offsetRotationZ : 0  //offset 0 if axis is locked
        );
        Quaternion targetRotationPreLock = hand.transform.rotation * Quaternion.Euler(offsetRotation);
        Vector3 eulerPreLock = targetRotationPreLock.eulerAngles;
        Vector3 currentEuler = this.transform.rotation.eulerAngles;
        
        Vector3 lockedRotation = new Vector3(
            lockRotX ? currentEuler.x : eulerPreLock.x, // assegno eulerPreLock per non ruotare gli assi bloccati.
            lockRotY ? currentEuler.y : eulerPreLock.y,
            lockRotZ ? currentEuler.z : eulerPreLock.z
        );
        
        Quaternion targetRotation = Quaternion.Euler(lockedRotation);
        
        Quaternion handLockedRotation = Quaternion.Euler(new Vector3(
                lockRotX ?  lockedRotation.x : hand.transform.rotation.eulerAngles.x ,  // assegno lockedRotation invece che 0 per mantenere Quaternion.Angle a distanza 0 per gli assi non mossi.
                lockRotY ?  lockedRotation.y : hand.transform.rotation.eulerAngles.y ,
                lockRotZ ?  lockedRotation.z : hand.transform.rotation.eulerAngles.z 
            ));
        
        
        if (Quaternion.Angle(targetRotation, this.transform.rotation) > minAngleToUpdateTarget)
        {
            _targetRot = targetRotation;
        }
        else if(Quaternion.Angle(this.transform.rotation, handLockedRotation) < ROT_earlyStopThreshold)// se la rotazione ora è estremamente vicina a quella voluta fermo la rotazione.
        {
            _targetRot = this.transform.rotation;
        }
        Quaternion deltaRotation = _targetRot * Quaternion.Inverse(_prb.rotation);
        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f) angle -= 360f; // controlla se serve
        if (!float.IsNaN(axis.x) && !float.IsInfinity(axis.x))
        {
            Vector3 angularSpeed = (axis * (angle/movementDampening * Mathf.Deg2Rad)) / Time.fixedDeltaTime;
            float maxAngularSpeed = rotationSpeed * Mathf.Deg2Rad;
            _prb.angularVelocity = Vector3.ClampMagnitude(angularSpeed, maxAngularSpeed);
        }
    }
    
 
}
