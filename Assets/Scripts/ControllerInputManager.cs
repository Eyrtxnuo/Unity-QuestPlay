using System;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInputManager : MonoBehaviour
{

    public GameObject ball;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            Debug.Log("Pulsante B premuto, reset palla.");
            ball.GetComponent<Ball3D>().Relaunch();
        }
    }
}
