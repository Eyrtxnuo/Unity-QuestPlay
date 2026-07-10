using UnityEngine;

public class CowControls : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardAcceleration = 1f;
    public float maxForwardSpeed = 4f;
    public float maxOffroadSpeed = 1f;
    
    [Header("Rotation Settings")]
    public float turnAcceleration = 10f;
    public float maxTurnSpeed = 20;
    
    [Header("Physics Settings")]
    [Min(1f)]
    public float brakeIntensity = 1.05f;
    public LayerMask floorLayer = 64; // Walkable
      
    private float _currentForwardSpeed = 0f;
    private float _currentTurnSpeed = 0f;
    
    public GyroController gyroController;

    void FixedUpdate()
    {
        var input = gyroController.AnalogInput;
        
        // Rotation handlind
        float targetTurnSpeed = input.x * maxTurnSpeed; 
        _currentTurnSpeed = Mathf.MoveTowards(_currentTurnSpeed, targetTurnSpeed, turnAcceleration * Time.fixedDeltaTime);

        // movement handling
        float currentMaxSpeed = maxForwardSpeed;
        Vector3 projectedPosition = transform.position + (transform.forward * (_currentForwardSpeed * Time.fixedDeltaTime));
        
        if (!CheckGround(projectedPosition))
        {
            currentMaxSpeed = maxOffroadSpeed;
        }

        float targetForwardSpeed = input.y * currentMaxSpeed;
        _currentForwardSpeed = Mathf.MoveTowards(_currentForwardSpeed, targetForwardSpeed, forwardAcceleration * Time.fixedDeltaTime);

        // Applying 
        transform.Rotate(0, _currentTurnSpeed * Time.fixedDeltaTime, 0, Space.World);
        transform.position += transform.forward * (_currentForwardSpeed * Time.fixedDeltaTime);
    }

    bool CheckGround(Vector3 targetPosition)
    {
        Debug.DrawRay(targetPosition, Vector3.down, Color.red);
        return Physics.Raycast(targetPosition + Vector3.up * 0.1f, Vector3.down, 1f, floorLayer);
    }
}