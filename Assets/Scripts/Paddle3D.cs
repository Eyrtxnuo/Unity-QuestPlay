using UnityEngine;

public class Paddle3D : MonoBehaviour
{
    public float speed = 15f;
    public string horizontalAxis = "Horizontal"; // A/D or Left/Right arrows
    public string verticalAxis = "Vertical";     // W/S or Up/Down arrows

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() 
    {
        // Read input
        float moveX = -Input.GetAxisRaw(horizontalAxis);
        float moveY = Input.GetAxisRaw(verticalAxis); 
        
        // Move the paddle in 3D space using physics
        Vector3 movement = new Vector3(moveX, moveY, 0) * (speed * Time.fixedDeltaTime);
        rb.MovePosition(transform.position + movement);
    }
}