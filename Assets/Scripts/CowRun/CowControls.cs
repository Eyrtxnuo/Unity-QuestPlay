using UnityEngine;

public class CowControls : MonoBehaviour
{
    public float speed = 0.5f;
    
    public GyroController gyroController;
    void FixedUpdate()
    {
        var movement = gyroController.AnalogInput;
        this.transform.Translate(movement * (speed * Time.fixedDeltaTime));
        
    }

}