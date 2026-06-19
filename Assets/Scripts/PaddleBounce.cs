using UnityEngine;

public class PaddleBounce : MonoBehaviour
{
    public float interpolationFactor = 10;
    public float debounceDelay= 0.1f;

    private Vector3 averageVelocity;
    private Vector3 lastPosition;

    public float debounceLastTime = 0;
        
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 centerPaddle = this.transform.position + this.transform.up * 0.13f;
        Debug.DrawLine(centerPaddle, centerPaddle + this.transform.forward*0.3f, Color.coral);
        Debug.DrawLine(centerPaddle, centerPaddle + averageVelocity, Color.gold);
    }

    void FixedUpdate()
    {
        Vector3 lastShiftVelocity = (transform.position - lastPosition) / Time.fixedDeltaTime;
        averageVelocity = Vector3.Lerp(averageVelocity, lastShiftVelocity, interpolationFactor*Time.fixedDeltaTime );
        
        
        lastPosition = transform.position;
    }
    
    private void OnCollisionEnter(Collision collision)
    //private void OnTriggerEnter(Collision collision) (non prende il trigger)
    {
        if (Time.time - debounceDelay < debounceLastTime) return; //debounce
        if (collision.gameObject.CompareTag("PingPongBall"))
        {
            Debug.Log("COLLISION");
            Rigidbody brb = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 currVel = collision.relativeVelocity;
            Vector3 paddleNormal = transform.forward;
            if (Vector3.Dot(currVel, paddleNormal) > 0) 
            {
                paddleNormal = -paddleNormal; // faccia opposta
            }
            
            // CALCOLO DEL RIMBALZO ////////////////////////
            Vector3 bounceVel = Vector3.Reflect(currVel, paddleNormal);//contactPoint.normal);
            bounceVel += averageVelocity * 0.8f ; // sommo la velocità media della racchetta. (relativeVelocity non include la velocità della racchetta se è mossa con rb.MovePositon)
            
            // CALCOLO DELLO SPIN //////////////////////////
            Vector3 tangentialVelocity = Vector3.ProjectOnPlane(averageVelocity, paddleNormal); // proiezione della velocity sul piano di rimbalzo.
            if (tangentialVelocity.sqrMagnitude > 0.001f)
            {
                Vector3 spinAxis = Vector3.Cross(tangentialVelocity.normalized, paddleNormal);
                float spinGripMultiplier = 50f;
                Vector3 spinIncrease = spinAxis * tangentialVelocity.magnitude * spinGripMultiplier;
                brb.angularVelocity += spinIncrease;
            }
            
            
            // change bounce speed
            //bounceVel = bounceVel.normalized * 3.5f; 
            //Vector3 paddleAccelerationFactor = new Vector3(0,0,0);
            //brb.position = contactPoint.point + contactPoint.normal * 0.01f; // metto la pallina sul punto di contatto (+minimo offset verso l'esterno)
            brb.linearVelocity = bounceVel ;
            debounceLastTime = Time.time;   
            
        }
    }
}
