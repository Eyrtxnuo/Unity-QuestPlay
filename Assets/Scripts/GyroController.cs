using System;
using UnityEngine;

public class GyroController : MonoBehaviour
{
    public float maxTiltAngle = 45f;
    private float centerX = -20;
    private float centerZ = 0;
    private float deadZone = 10f;
    
    
    public Vector2 AnalogInput { get; private set; } // disabilita la modifica del valore dall'esterno della classe
    
    void Start()
    {
    }
    
    void Update()
    {   
        float pitchX = NormalizeAngle(transform.eulerAngles.x) - centerX; 
        float rollZ  = NormalizeAngle(transform.eulerAngles.z) - centerZ; 
        //Debug.Log("pitch: " + pitchX + " " + rollZ);
        
        //Se il controller è quasi verticale evito di fare minimovimenti.
        if (Math.Abs(pitchX) < deadZone && Math.Abs(pitchX) < deadZone)
        {
            pitchX = 0;
            rollZ = 0;
        }
        
        float inputY = -Mathf.Clamp(pitchX / maxTiltAngle, -1f, 1f);
        float inputX = Mathf.Clamp(-rollZ / maxTiltAngle, -1f, 1f); 
        
        AnalogInput = new Vector2(inputX, inputY);
        AnalogInput = Vector2.ClampMagnitude(AnalogInput, 1f);
        Debug.DrawRay(transform.position, Vector3.up, Color.red);
        //Debug.Log( AnalogInput.x + " " + AnalogInput.y);
        DrawDebugLine();
    }
    
    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle > 180f)
            angle -= 360f;
        return angle;
    }
    
    private float debugLineLength = 2f;
    private void DrawDebugLine()
    {
        Vector3 startPos = transform.position;
        Vector3 inputDirection3D = new Vector3(AnalogInput.x, 0f, AnalogInput.y);
        Vector3 endPos = startPos + (inputDirection3D * debugLineLength);
        Color lineColor = Color.Lerp(Color.green, Color.red, AnalogInput.magnitude);
        //Debug.Log(AnalogInput.magnitude);
        Debug.DrawLine(startPos, endPos, lineColor);
        Debug.DrawRay(startPos, Vector3.up * 0.2f, Color.red);
    }
}
