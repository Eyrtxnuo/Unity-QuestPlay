using Oculus.Interaction;
using UnityEngine;

public class RotationRecogniser : MonoBehaviour, IActiveState
{

    public GameObject obj;
    public Vector3 direction;
    public float slack = 20;
    
    public bool Active
    {
        get
        {
            Debug.DrawRay(obj.transform.position, direction, Color.red);
            float angleDiff = Vector3.Angle(obj.transform.forward, direction);
            return angleDiff <= slack;
        }
    }
}
