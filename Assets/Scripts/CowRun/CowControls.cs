using CowRun;
using LocomotionCollision;
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
    public float brakeIntensity = 2f;
    public LocomotionCollision.LocomotionCollision collision;
      
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
        float currentMaxSpeed = GetCurrentMaxForwardSpeed();

        float targetForwardSpeed = input.y * currentMaxSpeed;
        if (_currentForwardSpeed < targetForwardSpeed)
        {
            _currentForwardSpeed = Mathf.MoveTowards(_currentForwardSpeed, targetForwardSpeed, forwardAcceleration * Time.fixedDeltaTime);
        }
        else
        {
            _currentForwardSpeed = _currentForwardSpeed = Mathf.Lerp(_currentForwardSpeed, targetForwardSpeed, brakeIntensity * Time.fixedDeltaTime);
        }

        // Applying 
        transform.Rotate(0, _currentTurnSpeed * Time.fixedDeltaTime, 0, Space.World);
        var newPosition = transform.position + transform.forward * (_currentForwardSpeed * Time.fixedDeltaTime);
        if (collision.IsColliding2D(new Vector2(newPosition.x, newPosition.z)) !=
            LocomotionCollisionType.NoCollision)   // avoid exiting the road
        {
            transform.position = newPosition;
        }
    }

    /* Gets the max speed based on the road collision
     */
    private float GetCurrentMaxForwardSpeed()
    {
        var collType = collision.IsColliding2D(new Vector2(transform.position.x, transform.position.z));
        switch (collType)
        {
            case LocomotionCollisionType.Road:
                return maxForwardSpeed;
            case LocomotionCollisionType.OffRoad:
                return maxOffroadSpeed;
            case LocomotionCollisionType.NoCollision:
            default:
                return 0;
        }
    }

    public void ApplyEffect(CollisionEffect effect)
    {
        switch (effect.type)
        {
            case CollisionType.SpeedSet:
                SpeedSetEffect spe = (SpeedSetEffect)effect;
                this._currentForwardSpeed = spe.speed;
                break;
            case CollisionType.SpeedMultiplier:
                SpeedMultiplierEffect sme = (SpeedMultiplierEffect)effect;
                this._currentForwardSpeed *= sme.speedMultiplier;
                break;
        }
        
    }
}