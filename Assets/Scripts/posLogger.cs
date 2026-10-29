using UnityEngine;

public class posLogger : MonoBehaviour
{
    public string name = "posLogger";
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("[pos:"+name+"] pos:" + transform.position + " , rot:" + transform.rotation.eulerAngles.y);
    }
}
