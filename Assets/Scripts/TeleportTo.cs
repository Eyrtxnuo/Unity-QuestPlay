using UnityEngine;

public class TeleportTo : MonoBehaviour
{
    public GameObject teleport;
    
    void Start()
    {
        this.transform.position = teleport.transform.position;
        this.enabled = false;
    }

}
