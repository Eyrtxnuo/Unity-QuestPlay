using UnityEngine;

public class Paddle3DBot : MonoBehaviour
{
    public float speed = 7f;
    public float rotationSpeed = 600;
    

    private Rigidbody rb;
    public GameObject follow;
    public GameObject bounceDirTarget;

    private Rigidbody followRigidBody;
    private Vector3 lastBallPos;
    private float bounceFactor = 1;

    void Start()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.01f * Time.timeScale;
        rb = GetComponent<Rigidbody>();
        followRigidBody = follow.GetComponent<Rigidbody>();
        
        
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders) //Ricerca dell'elemento a mano visto che unity < css
        {
            if (col.material == null) continue;
            if (!col.gameObject.CompareTag("MainSubcomponent")) continue;
            bounceFactor = col.material.bounciness;
            break;
        }

    }

    void FixedUpdate()
    {
        // Read position
        float x = follow.transform.position.x;
        float y = follow.transform.position.y-0.13f;
        float z = this.transform.position.z;

        
        rb.MovePosition(Vector3.MoveTowards(rb.position, new Vector3(x, y, z), speed * Time.deltaTime));
        if (
                Vector3.Distance(follow.transform.position, this.transform.position) 
                < 
                Vector3.Distance(this.lastBallPos, this.transform.position) 
            &&
                Vector3.Distance(follow.transform.position, this.transform.position) > 0.05
            )
        {   
            rb.MoveRotation(Quaternion.RotateTowards(this.transform.rotation, CalculatePaddleRotation(), rotationSpeed *  Time.deltaTime));
        }
        else
        {
            CalculatePaddleRotation();// serve per disegnare i debug, non applico l'angolo.
        }
        //this.transform.forward = CalculatePaddleRotation();
        
        
        this.lastBallPos = follow.transform.position;
    }


    Quaternion CalculatePaddleRotation()
    {
        float horizontalTravel = Mathf.Abs(bounceDirTarget.transform.position.z - this.transform.position.z);
        float totalTravel = Vector3.Distance(bounceDirTarget.transform.position, this.transform.position);
        float horizontalRatio = totalTravel > 0.01f ? (horizontalTravel / totalTravel) : 1f;
        
        float currentSpeed = Mathf.Abs(followRigidBody.linearVelocity.magnitude);
        float afterBounceSpeed = currentSpeed * bounceFactor;
        if (afterBounceSpeed < 0.1f) afterBounceSpeed = 0.1f;
        
        float equivalentHorizontalSpeed = afterBounceSpeed * horizontalRatio;
        float drag = followRigidBody.linearDamping;

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
                Debug.LogWarning("Not reaching point!");
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
        
        Vector3 inverseTravelDirection = -followRigidBody.linearVelocity.normalized;
        
        Vector3 targetBouncePos = new Vector3(follow.transform.position.x, follow.transform.position.y-0.13f, this.transform.position.z);
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
}