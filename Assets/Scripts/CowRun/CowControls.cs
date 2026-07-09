using UnityEngine;

public class CowControls : MonoBehaviour
{
    public float acceleration = 0.1f;
    public float maxSpeed = 4f;
    [Range(1f, 2f)]
    public float brakeIntensity = 1.05f;
    
    private Vector3 _velocity = Vector3.zero;
    
    public GyroController gyroController;
    void FixedUpdate()
    {
        var movement = gyroController.AnalogInput;
        Debug.Log(movement.magnitude);
        if (movement.magnitude < 0.1f)
        {
            this._velocity /= brakeIntensity;
            if (this._velocity.magnitude < 0.1f)
            {
                this._velocity = Vector3.zero;
            }
        }
        else
        {
            this._velocity.x = Mathf.Clamp(this._velocity.x + movement.x * acceleration, -maxSpeed, maxSpeed);
            this._velocity.y = Mathf.Clamp(this._velocity.y + movement.y * acceleration, -maxSpeed, maxSpeed);
        }
        
        this.transform.Translate(_velocity * Time.fixedDeltaTime);
    }

}