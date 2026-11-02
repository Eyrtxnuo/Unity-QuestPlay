using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class testRecenterOnLoad : MonoBehaviour
{

    void Awake()
    {
        RecenterTrackingOrigin();
        
    }
    
    
    
    public void RecenterTrackingOrigin()
    {
        var inputSubsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetSubsystems(inputSubsystems);

        foreach (var subsystem in inputSubsystems)
        {
            if (subsystem.running)
            {
                subsystem.TryRecenter();
            }
        }
    }
}
