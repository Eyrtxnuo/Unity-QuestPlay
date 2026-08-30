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

    public Rigidbody GetRigidBody()
    {
        return _rb;
    }

    public Vector3? PredictFallPosition()
    {
        float tableY = 0;
        if (tableTop) tableY = tableTop.position.y;
        return GetFallPositionAtY(_rb.position, _rb.linearVelocity, _rb.angularVelocity, _rb.mass, magnusConstant,_rb.linearDamping, _rb.angularDamping, tableY);
    }


    public static (Vector3 position, Vector3 velocity, Vector3 angularVelocity)? GetImpactState(
        Vector3 startPos,
        Vector3 startVel,
        Vector3 startAngularVel,
        float mass,
        float magnusConst,
        float linearDrag,
        float angularDamping,
        float targetY = 0f,
        int maxIterations = 600)
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
            currentVel *= Mathf.Clamp01(1f - linearDrag * dt); 

            iteration++;
        }

        return iteration >= maxIterations ? null : (currentPos, currentVel, currentAngularVel);
    }

    public Vector3? GetFallPositionAtY(
        Vector3 startPos,
        Vector3 startVel,
        Vector3 startAngularVel,
        float mass,
        float magnusConst,
        float linearDrag,
        float angularDamping,
        float targetY = 0f,
        int maxIterations = 250)
    {
        return GetImpactState(startPos, startVel, startAngularVel, mass, magnusConst, linearDrag, angularDamping, targetY, maxIterations)?.position;
    }

    public static (Vector3 position, Vector3 velocity, Vector3 angularVelocity) GetStateAfterBounce(
        Vector3 impactPos,
        Vector3 impactVel,
        Vector3 impactAngVel,
        float mass,
        float magnusConst,
        float angularDamping,
        float linearDrag,
        float bounciness = 0.9f,
        float dynamicFriction = 0.2f,
        float ballRadius = 0.045f,
        float timeAfterBounce = 0.5f)
    {
        Vector3 currentPos = impactPos;
        Vector3 currentVel = impactVel;
        Vector3 currentAngularVel = impactAngVel; 
        float dt = Time.fixedDeltaTime;

        // --- 1. RISOLUZIONE IMPATTO (Attrito di Coulomb e trasferimento Spin) ---
        float normalImpulse = (1f + bounciness) * Mathf.Abs(currentVel.y) * mass;
        currentVel.y = -currentVel.y * bounciness; // Inversione asse Y

        Vector3 contactPointOffset = new Vector3(0, -ballRadius, 0);
        Vector3 contactSurfaceVel = currentVel + Vector3.Cross(currentAngularVel, contactPointOffset);
        Vector3 tangentVel = new Vector3(contactSurfaceVel.x, 0, contactSurfaceVel.z);

        if (tangentVel.sqrMagnitude > 0.0001f)
        {
            Vector3 tangentDir = tangentVel.normalized;
            float inertia = (2f / 3f) * mass * ballRadius * ballRadius; 
            float maxFrictionImpulse = dynamicFriction * normalImpulse;
            
            float slipFactor = 1f / ( (1f / mass) + (ballRadius * ballRadius / inertia) );
            float desiredTangentImpulse = tangentVel.magnitude * slipFactor;
            
            float appliedImpulse = Mathf.Min(desiredTangentImpulse, maxFrictionImpulse);
            Vector3 frictionImpulseVector = -tangentDir * appliedImpulse;

            currentVel += frictionImpulseVector / mass;
            currentAngularVel += Vector3.Cross(contactPointOffset, frictionImpulseVector) / inertia;
        }

        // --- 2. SIMULAZIONE VOLO POST-RIMBALZO ---
        int iterations = Mathf.RoundToInt(timeAfterBounce / dt);
        for (int i = 0; i < iterations; i++)
        {
            Vector3 magnusForce = Vector3.Cross(currentAngularVel, currentVel) * magnusConst;
            Vector3 totalAcceleration = Physics.gravity + (magnusForce / mass);
            currentVel += totalAcceleration * dt;
            currentPos += currentVel * dt;
            currentAngularVel *= Mathf.Clamp01(1f - angularDamping * dt);
            currentVel *= Mathf.Clamp01(1f - linearDrag * dt);
        }

        return (currentPos, currentVel, currentAngularVel);
    }
    
    public (Vector3 fallPosition, (Vector3 position, Vector3 velocity, Vector3 angularVelocity) afterBounceState)? GetBounceStates(float timeAfterBounce = 0.5f)
    {
        float tableY = tableTop ? tableTop.position.y : 0f;
        
        var impactNullable = GetImpactState(
            _rb.position, _rb.linearVelocity, _rb.angularVelocity, _rb.mass,
            magnusConstant, _rb.linearDamping, _rb.angularDamping, tableY);
        
        if (!impactNullable.HasValue) return null;
        var impact = impactNullable.Value;

        var afterBounceState = GetStateAfterBounce(
            impact.position, impact.velocity, impact.angularVelocity, 
            _rb.mass, magnusConstant, _rb.angularDamping, _rb.linearDamping, 
            0.95f, 0.05f, 0.05f, timeAfterBounce);
        
        return (impact.position, afterBounceState);
    }

    public Vector3 GetSpawnPosition()
    {
        return _originalPosition;
    }
}