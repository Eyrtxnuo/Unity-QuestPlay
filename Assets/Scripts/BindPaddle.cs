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

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 targetPos = this.transform.position - (paddle.transform.up * vOffsetPaddle);
        Vector3 offsetVector = targetPos - prb.position;
        Vector3 speedVector = Vector3.ClampMagnitude(offsetVector / Time.fixedDeltaTime,  movementSpeed);
        Debug.DrawLine(this.transform.position, targetPos, Color.red);
        prb.linearVelocity = speedVector;
        //prb.MovePosition(Vector3.MoveTowards(prb.position, targetPos, movementSpeed * Time.deltaTime));
        
        prb.MoveRotation(Quaternion.RotateTowards(prb.rotation, this.transform.rotation * Quaternion.Euler(40f, 0f, 0f), rotationSpeed * Time.fixedDeltaTime));

    }
}
