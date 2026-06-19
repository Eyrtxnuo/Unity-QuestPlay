using System;
using UnityEngine;

public class BindPaddle : MonoBehaviour
{
    
    public GameObject paddle;
    public float movementSpeed = 5;
    public float rotationSpeed = 180;
    private float vOffsetPaddle = 0.05f;

    
    private Rigidbody prb;
    
    void Start()
    {
        prb = paddle.GetComponent<Rigidbody>();
    }

    
    public float movementDampening = 2f;
    void FixedUpdate()
    {
        Vector3 targetPos = this.transform.position - (paddle.transform.up * vOffsetPaddle);
        
        //*
        Vector3 offsetVector = targetPos - prb.position;
        Vector3 speedVector = Vector3.ClampMagnitude((offsetVector/movementDampening) / Time.fixedDeltaTime,  movementSpeed);
        Debug.DrawLine(this.transform.position, targetPos, Color.red);
        prb.linearVelocity = speedVector;
        /*/
        prb.MovePosition(Vector3.MoveTowards(prb.position, targetPos, movementSpeed * Time.deltaTime));
        //*/
        
        //*
        Quaternion targetRotation = this.transform.rotation * Quaternion.Euler(40f, 0f, 0f);
        Quaternion deltaRotation = targetRotation * Quaternion.Inverse(prb.rotation);
        deltaRotation.ToAngleAxis(out float angle, out Vector3 axis);
        if (angle > 180f) angle -= 360f; // controlla se serve
        
        if (!float.IsNaN(axis.x) && !float.IsInfinity(axis.x))
        {
            Vector3 angularSpeed = (axis * (angle/movementDampening * Mathf.Deg2Rad)) / Time.fixedDeltaTime;
            float maxAngularSpeed = rotationSpeed * Mathf.Deg2Rad;
            prb.angularVelocity = Vector3.ClampMagnitude(angularSpeed, maxAngularSpeed);
        }
        /*/
        prb.MoveRotation(Quaternion.RotateTowards(prb.rotation, this.transform.rotation * Quaternion.Euler(40f, 0f, 0f), rotationSpeed * Time.fixedDeltaTime));
        //*/
    }
    
    public float bounceForce = 15f;
 
    
    
    // sti cosi vanno dati alla paddle, non al controller....
    // void OnCollisionEnter(Collision collision)
    // {
    //     Debug.Log("ACollision");
    //     if (collision.gameObject.CompareTag("PingPongBall"))
    //     {
    //         Debug.Log("BallCollision");
    //         Rigidbody ballRb = collision.gameObject.GetComponent<Rigidbody>();
    //     
    //         ballRb.linearVelocity = Vector3.zero;
    //
    //         Vector3 bounceDirection = (collision.transform.position - this.transform.position).normalized;
    //
    //         
    //         bounceDirection += this.transform.forward; // Modifica in base al tuo asse di gioco
    //
    //         ballRb.AddForce(bounceDirection.normalized * bounceForce, ForceMode.VelocityChange);
    //     }
    // }
    
    // private void OnCollisionEnter(Collision collision)
    // {
    //     
    //     Debug.Log("ACollision");
    //     if (collision.gameObject.CompareTag("PingPongBall"))
    //     {
    //         
    //         Debug.LogError("BallCollision");
    //         Rigidbody brb = collision.gameObject.GetComponent<Rigidbody>();
    //         Vector3 currVel = brb.linearVelocity;
    //         Vector3 bounceVel = Vector3.Reflect(currVel, transform.forward);//collision.contacts[0].normal);
    //         
    //         // change bounce speed
    //         //bounceVel = bounceVel.normalized * 3.5f; 
    //         Vector3 paddleAccelerationFactor = new Vector3(0,0,0);
    //         
    //         brb.linearVelocity = bounceVel + paddleAccelerationFactor;
    //     }
    // }
}
