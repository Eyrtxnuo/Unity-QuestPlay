using System;
using PingPong;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(FollowPoint))]
public class Ball3D : MonoBehaviour
{
    public float launchForce = 4f;
    public float magnusConstant = 0.05f;
    public float maxSpeed = 20f;
    public event Action<Collision> OnAnyCollision;
    public Transform playerUntrackedServeTransform;
    public Transform tableTop = null;

    public bool ServeBind
    {
        get => _isServeBind;
        set
        {
            _bindToServeHand.enabled = value;
            _isServeBind = value;
        }
    }

    private Rigidbody _rb;
    private Vector3 _originalPosition;
    private bool _isServeBind;
    private FollowPoint _bindToServeHand;

    private void Awake()
    {
        _originalPosition = transform.position;
        _rb = GetComponent<Rigidbody>();
        _bindToServeHand = GetComponent<FollowPoint>();
        //rb.maxLinearVelocity = 9;
        //Launch();
        this.enabled = false;
    }

    public void Relaunch()
    {
        this.enabled = true;
        RigidbodyInterpolation pre_interpolation = _rb.interpolation;
        _rb.position = _originalPosition;
        _rb.interpolation = RigidbodyInterpolation.None; // disattiva interpolazione per evitare di far impazzire unity
        _rb.rotation = Quaternion.identity;
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.interpolation = pre_interpolation; // riattiva l'interpolazione al valore precedente
        Launch();
    }

    public void Launch()
    {
        float directionZ = 1; //Random.value < 0.5f ? -1f : 1f;

        // Add a slight randomization to the X (left/right) and Y (upward bounce)
        Vector3 launchDirection = new Vector3(0, -0.6f, directionZ).normalized;
        Random.Range(-0.3f, 0.3f);
        // Apply an impulse force to shoot the ball
        _rb.AddForce(launchDirection * launchForce, ForceMode.VelocityChange);
    }


    void FixedUpdate()
    {
        if(ServeBind)
        {
            var lTracked = OVRInput.GetControllerPositionTracked(OVRInput.Controller.LTouch);
            _bindToServeHand.active = lTracked;
            if (!lTracked)
            {
                _rb.transform.position = playerUntrackedServeTransform.position;
                _rb.transform.rotation = playerUntrackedServeTransform.rotation;
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
            return;
        };
        
        
        Vector3 magnusForce = Vector3.Cross(_rb.angularVelocity, _rb.linearVelocity) * magnusConstant;
        _rb.AddForce(magnusForce, ForceMode.Force);

        //clamp della velocità per evitare di creare satelliti
        if (_rb.linearVelocity.magnitude > maxSpeed)
        {
            _rb.linearVelocity = Vector3.ClampMagnitude(_rb.linearVelocity, maxSpeed);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnAnyCollision?.Invoke(collision); // invokes the event if listeners are present
    }

    public void Toss()
    {
        _rb.AddForce(new Vector3(0, launchForce, 0), ForceMode.VelocityChange);
    }


    public Vector3 PredictFallPosition()
    {
        float tableY = 0;
        if (tableTop) tableY = tableTop.position.y;
        return GetFallPositionAtY(_rb.position, _rb.linearVelocity, _rb.angularVelocity, _rb.mass, magnusConstant, _rb.angularDamping, tableY);
    }
    
    
    public static Vector3 GetFallPositionAtY(
        Vector3 startPos, 
        Vector3 startVel, 
        Vector3 startAngularVel, 
        float mass, 
        float magnusConst, 
        float angularDamping, 
        float targetY = 0f,
        int maxIterations = 250)
    {
        Vector3 currentPos = startPos;
        Vector3 currentVel = startVel;
        Vector3 currentAngularVel = startAngularVel; 
    
        float dt = Time.fixedDeltaTime; 
        int iteration = 0;
        while (currentPos.y > targetY && iteration < maxIterations)
        {
            Vector3 magnusForce = Vector3.Cross(currentAngularVel, currentVel) * magnusConst;
            Vector3 totalAcceleration = Physics.gravity + (magnusForce / mass);
            currentVel += totalAcceleration * dt;
            currentPos += currentVel * dt;
            currentAngularVel *= Mathf.Clamp01(1f - angularDamping * dt);

            iteration++;
        }

        return currentPos.y > targetY ? 
            Vector3.negativeInfinity : currentPos;// if the currentPos is still over Y, the while exited for the iteration count. Returning a extreme value to stop render. 
    }
}