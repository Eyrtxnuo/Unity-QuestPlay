using System;
using System.Collections;
using CowRun;
using LocomotionCollision;
using PingPong;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

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
    
    
    [Header(" ")]
    public GyroController gyroController;
    
    [Header("Audio")]
    public SfxPool sfxPool;
    public AudioClip[] stepClips;

    private void Start()
    {
        StartCoroutine(StepsSfxCoroutine());
    }

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


    private IEnumerator StepsSfxCoroutine()
    { 
        const float jFactor = 1.8f;
        while (maxForwardSpeed > 0f)// fino a quando non viene fermato per l'endgame
        {
            float minSpeedThreshold = maxForwardSpeed * 0.25f;
            if (_currentForwardSpeed > minSpeedThreshold)
            {
                sfxPool.PlayClipAtPosition(
                    stepClips[Random.Range(0, stepClips.Length)],
                    transform.position,
                    0.1f,
                    Random.Range(0.9f, 1f),
                    priority:2
                    );
            
                float wait = 0.02f * math.pow(maxForwardSpeed/_currentForwardSpeed, jFactor);
                float quantizedWait = FloatQuantizer.QuantizeFloat(wait); // riduce mantissa a 3 bit (azzerando 20bit meno significativi)
                yield return WaitsCache.GetWaitForSeconds(quantizedWait);
                //quantizedWait avrà solo 30 possibili step tra 0.02 e 0.242s. Rende probabile un cache hit ed evita una crescita infinita della cache.
            }
            yield return WaitsCache.WAIT_100ms;
        }
    }
    
    
}