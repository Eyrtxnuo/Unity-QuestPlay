using UnityEngine;

public class HertzSet : MonoBehaviour
{
    public int screenHertz = 120;
    void Start()
    {
        OVRPlugin.systemDisplayFrequency = screenHertz;
    }
}