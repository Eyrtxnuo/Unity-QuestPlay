using UnityEngine;

public class ControllerInputExamples : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    void Update()
    {
        // ==========================================
        // 1. PULSANTI (A, B, X, Y)
        // ==========================================
        
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            Debug.Log("Pulsante A premuto!");
        }

        if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.LTouch))
        {
            Debug.Log("Pulsante X in pressione...");
        }

        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            Debug.Log("Pulsante B premuto!");
        }
        
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.LTouch))
        {
            Debug.Log("Pulsante Y premuto!");
        }

        // ==========================================
        // 2. GRILLETTI (Index / Grip) - Valori 1D (0.0 a 1.0)
        // ==========================================
        
        float rightIndexTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
        if (rightIndexTrigger > 0.1f) 
        {
            Debug.Log("Pressione grilletto destro: " + rightIndexTrigger);
        }

        float leftGripTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch);


        // ==========================================
        // 3. LEVETTE ANALOGICHE (Thumbsticks) - Valori 2D (-1.0 a 1.0)
        // ==========================================
        
        Vector2 leftThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
        
        if (leftThumbstick.magnitude > 0.1f) 
        {
            Debug.Log("Movimento levetta sinistra - X: " + leftThumbstick.x + ", Y: " + leftThumbstick.y);
        }
        
        if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch))
        {
            Debug.Log("Click sulla levetta destra!");
        }
        
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
        {
            Debug.Log("Click Grip!");
        }

        if (OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.LTouch))
        {
            Debug.Log("Pulsante \"Options\"");
        }
        
        if (OVRInput.GetDown(OVRInput.Button.Start, OVRInput.Controller.RTouch))
        {
            Debug.Log("Pulsante \"Oculus\"");
        }
    }
}
