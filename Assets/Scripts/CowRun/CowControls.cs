using CowRun;
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
        Vector3 projectedPosition = transform.position + (transform.forward * (_currentForwardSpeed * Time.fixedDeltaTime));
        
        float currentMaxSpeed = maxForwardSpeed;
        if (!CheckGround(projectedPosition))
        {
            currentMaxSpeed = maxOffroadSpeed;
        }

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
        transform.position += transform.forward * (_currentForwardSpeed * Time.fixedDeltaTime);
    }

    bool CheckGround(Vector3 targetPosition)
    {
        Debug.DrawRay(targetPosition, Vector3.down, Color.red);
        return Physics.Raycast(targetPosition + Vector3.up * 0.1f, Vector3.down, 1f, floorLayer);
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