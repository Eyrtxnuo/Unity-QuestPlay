using UnityEngine;

public class TeleportTo : MonoBehaviour
{
    public GameObject teleport;
    public bool rotateAlso = false;
    
    void Start()
    {
        this.transform.position = teleport.transform.position;
        if(rotateAlso)
        {
            this.transform.rotation = Quaternion.Euler(new Vector3(
                    0,
                    this.transform.rotation.eulerAngles.y,
                    0
                ));
        }
        this.enabled = false;
    }

}
