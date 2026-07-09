using UnityEngine;

public class CowControls : MonoBehaviour
{
    public float acceleration = 0.1f;
    public float maxSpeed = 4f;
    [Min(1f)]
    public float brakeIntensity = 1.05f;
    public LayerMask floorLayer = 64; // Walkable
      
    private Vector3 _velocity = Vector3.zero;
    
    public GyroController gyroController;
    void FixedUpdate()
    {
        var movement = gyroController.AnalogInput;
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
            this._velocity.z = Mathf.Clamp(this._velocity.z + movement.y * acceleration, -maxSpeed, maxSpeed);
        }
        Vector3 targetPosition = transform.position + (_velocity * Time.fixedDeltaTime);
        Debug.Log(CheckGround(targetPosition));
        if (!CheckGround(targetPosition))
        {
            _velocity.x = 0;
        }
        this.transform.Translate(_velocity * Time.fixedDeltaTime, Space.World);
        
    }

    bool CheckGround(Vector3 targetPosition)
    {
        Debug.DrawRay(targetPosition, Vector3.down, Color.red);
        return Physics.Raycast(targetPosition, Vector3.down, 1, floorLayer);
    }
}