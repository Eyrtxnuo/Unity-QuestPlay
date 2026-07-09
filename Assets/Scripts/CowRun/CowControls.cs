using UnityEngine;

public class CowControls : MonoBehaviour
{
    [Header("Movement Settings")]
    public float forwardAcceleration = 0.5f;
    public float maxForwardSpeed = 4f;
    public float maxOffroadSpeed = 0.5f;
    
    [Header("Rotation Settings")]
    public float turnAcceleration = 1f;
    public float maxTurnSpeed = 15;
    [Header("Physics Settings")]
    [Min(1f)]
    public float brakeIntensity = 1.05f;
    public LayerMask floorLayer = 64; // Walkable
      
    private float _currentForwardSpeed = 0f;
    private float _currentTurnSpeed = 0f;
    
    public GyroController gyroController;

    void FixedUpdate()
    {
        var movement = gyroController.AnalogInput;

        if (Mathf.Abs(movement.x) < 0.1f)
        {
            _currentTurnSpeed /= brakeIntensity;
            if (Mathf.Abs(_currentTurnSpeed) < 0.1f)
            {
                _currentTurnSpeed = 0f;
            }
        }
        else
        {
            _currentTurnSpeed = Mathf.Clamp(_currentTurnSpeed + movement.x * turnAcceleration, -maxTurnSpeed, maxTurnSpeed);
        }

        if (Mathf.Abs(movement.y) < 0.1f)
        {
            _currentForwardSpeed /= brakeIntensity;
            if (Mathf.Abs(_currentForwardSpeed) < 0.1f)
            {
                _currentForwardSpeed = 0f;
            }
        }
        else
        {
            _currentForwardSpeed = Mathf.Clamp(_currentForwardSpeed + movement.y * forwardAcceleration, -maxForwardSpeed, maxForwardSpeed);
        }

        transform.Rotate(0, _currentTurnSpeed * Time.fixedDeltaTime, 0, Space.World);

        Vector3 velocityVector = transform.forward * _currentForwardSpeed;
        Vector3 targetPosition = transform.position + (velocityVector * Time.fixedDeltaTime);
        
        Debug.Log(CheckGround(targetPosition));
        
        if (!CheckGround(targetPosition))
        {
            _currentForwardSpeed = Mathf.Clamp(_currentForwardSpeed, -maxOffroadSpeed, maxOffroadSpeed);;
            velocityVector = transform.forward * _currentForwardSpeed;
        }

        transform.position += velocityVector * Time.fixedDeltaTime;
    }

    bool CheckGround(Vector3 targetPosition)
    {
        Debug.DrawRay(targetPosition, Vector3.down, Color.red);
        return Physics.Raycast(targetPosition + Vector3.up * 0.1f, Vector3.down, 1f, floorLayer);
    }
}