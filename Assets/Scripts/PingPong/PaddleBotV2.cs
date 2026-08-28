using System;
using UnityEngine;

public class PaddleBotV2 : MonoBehaviour
{
    public float speed = 10f;
    public float rotationSpeed = 600;
    

    private Rigidbody _rb;
    public Ball3D ball;
    public GameObject bounceDirTarget;
    public Collider tableTopCollider;
    public GameObject testTargetPrefab;

    private Vector3 _lastPredicedBouncePos;
    private float _bounceFactor = 1.4f;

    void Start()
    {
        // Time.timeScale = 1f;
        // Time.fixedDeltaTime = 0.005f * Time.timeScale;
        _rb = GetComponent<Rigidbody>();
        
        
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders) //Ricerca dell'elemento a mano visto che unity < css
        {
            if (col.material == null) continue;
            if (!col.gameObject.CompareTag("MainSubcomponent")) continue;
            _bounceFactor = col.material.bounciness;
            break;
        }

    }

    private void Update()
    {
        Debug.DrawLine(this.transform.position + new Vector3(0, 0.13f, 0), this.transform.position + this.transform.forward*0.3f + new Vector3(0, 0.13f, 0), Color.coral);
    }

    void FixedUpdate()
    {
        // Go to predicted bounce position
        var bouncePositionsNullable = ball.GetBouncePositions();
        if(bouncePositionsNullable == null)return;
        
        var fallPosition = bouncePositionsNullable.Value.fallPosition;
        if(fallPosition.x > tableTopCollider.bounds.max.x || fallPosition.x < -tableTopCollider.bounds.max.x ||
           fallPosition.z > tableTopCollider.bounds.max.z || fallPosition.z < -tableTopCollider.bounds.max.z) return;
        
        var afterBouncePosition =  bouncePositionsNullable.Value.afterBouncePosition;
        
        testTargetPrefab.transform.position = afterBouncePosition;
        
        float x = afterBouncePosition.x;
        float y = afterBouncePosition.y + 0.13f;
        float z = afterBouncePosition.z - 0.1f;

        
        _rb.MovePosition(Vector3.MoveTowards(_rb.position, new Vector3(x, y, z), speed * Time.deltaTime));
        if (
                Vector3.Distance(afterBouncePosition, this.transform.position) 
                < 
                Vector3.Distance(this._lastPredicedBouncePos, this.transform.position) 
            &&
                Vector3.Distance(afterBouncePosition, this.transform.position) > 0.05
            )
        {   
            _rb.MoveRotation(Quaternion.RotateTowards(this.transform.rotation, CalculatePaddleRotation(), rotationSpeed *  Time.deltaTime));
        }
        else
        {
            CalculatePaddleRotation();// serve per disegnare i debug, non applico l'angolo.
        }
        //this.transform.forward = CalculatePaddleRotation();
        
        
        this._lastPredicedBouncePos = afterBouncePosition;
    }


    Quaternion CalculatePaddleRotation()
    {
        float horizontalTravel = Mathf.Abs(bounceDirTarget.transform.position.z - this.transform.position.z);
        float totalTravel = Vector3.Distance(bounceDirTarget.transform.position, this.transform.position);
        float horizontalRatio = totalTravel > 0.01f ? (horizontalTravel / totalTravel) : 1f;
        
        float currentSpeed = Mathf.Abs(ball.GetRigidBody().linearVelocity.magnitude);
        float afterBounceSpeed = currentSpeed * _bounceFactor;
        if (afterBounceSpeed < 0.1f) afterBounceSpeed = 0.1f;
        
        float equivalentHorizontalSpeed = afterBounceSpeed * horizontalRatio;
        float drag = ball.GetRigidBody().linearDamping;

        float fligthETA;
        if (drag <= 0.001f)
        {   //no drag approximation
            fligthETA = horizontalTravel /  equivalentHorizontalSpeed;
        }
        else
        {
            //decayng factor
            float decayingFactor = (horizontalTravel * drag) / equivalentHorizontalSpeed;
        
            if (decayingFactor >= 1f)
            {
                //ball won't reach middle, handle by adding speed?
                fligthETA = horizontalTravel /  equivalentHorizontalSpeed * 2f; 
                Debug.LogWarning("Cant Reach other side!");
            }
            else
            {
                // Ballistic formula:
                fligthETA = -(1f / drag) * Mathf.Log(1f - decayingFactor);
            }
        }
        
        
        float gravity = Mathf.Abs(Physics.gravity.y);
        float gravityHeigthLoss = 0.5f * gravity * (Mathf.Pow(fligthETA, 2));
        Vector3 compensatedTarget = bounceDirTarget.transform.position + new Vector3(0, gravityHeigthLoss, 0);
        
        Vector3 inverseTravelDirection = -ball.GetRigidBody().linearVelocity.normalized;
        
        Vector3 targetBouncePos = new Vector3(ball.GetRigidBody().transform.position.x, ball.GetRigidBody().transform.position.y-0.13f, this.transform.position.z);
        Vector3 targetBounceDir = (compensatedTarget - targetBouncePos).normalized;
        
        Vector3 targetBounceSurfaceNormal = (targetBounceDir + inverseTravelDirection).normalized;
        
        
        Debug.DrawLine(targetBouncePos, targetBouncePos + targetBounceSurfaceNormal * 2f, Color.cyan);
        
        
        Debug.DrawLine(bounceDirTarget.transform.position, bounceDirTarget.transform.position + new Vector3(0,0,0.2f), Color.blue);
        Debug.DrawLine(bounceDirTarget.transform.position, bounceDirTarget.transform.position + new Vector3(0,0.2f,0), Color.green);
        Debug.DrawLine(bounceDirTarget.transform.position, bounceDirTarget.transform.position + new Vector3(0.2f,0,0), Color.red);
        Debug.DrawLine(compensatedTarget, compensatedTarget + new Vector3(0,0,0.2f), Color.Lerp(Color.blue, Color.black, .5f));
        Debug.DrawLine(compensatedTarget, compensatedTarget + new Vector3(0,0.2f,0), Color.Lerp(Color.green, Color.black, .5f));
        Debug.DrawLine(compensatedTarget, compensatedTarget + new Vector3(0.2f,0,0), Color.Lerp(Color.red, Color.black, .5f));
    
        //return targetBounceSurfaceNormal;
        
        // Quaternion rotation = Quaternion.LookRotation(-transform.forward, targetBounceSurfaceNormal);
        // return rotation;
        
        // float angoloInGradi = Mathf.Atan2(targetBounceSurfaceNormal.z, targetBounceSurfaceNormal.x) * Mathf.Rad2Deg;
        // angoloInGradi -= 90f;
        // Quaternion rotazioneZ = Quaternion.Euler(0, 0, angoloInGradi);
        // return rotazioneZ;
        
        Quaternion nuovaRotazione = Quaternion.FromToRotation(Vector3.forward, targetBounceSurfaceNormal);
        return nuovaRotazione;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PingPongBall"))
        {
            Rigidbody brb = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 currVel = brb.linearVelocity;
            Vector3 bounceVel = Vector3.Reflect(currVel, transform.forward);//collision.contacts[0].normal);
            
            // change bounce speed
           //bounceVel = bounceVel.normalized * 3.5f; 
            
            brb.linearVelocity = bounceVel * _bounceFactor;
        }
    }
    
}