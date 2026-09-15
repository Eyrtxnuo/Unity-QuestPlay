using UnityEngine;

public class TriggerPull : MonoBehaviour
{
    
    public float maxRotationAngle = 25f;

    // Update is called once per frame
    void Update()
    {
        float triggerValue = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
        float rotationAngle = triggerValue * maxRotationAngle;
        this.transform.localRotation = Quaternion.Euler(0f, 0f, rotationAngle);
    }
}
