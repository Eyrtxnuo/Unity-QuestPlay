using UnityEngine;


public class TooltipFollow : MonoBehaviour
{
    public GameObject followObject;
    public Vector3 offset;
    
    private GameObject _camera;

    void Start()
    {
        _camera = GameObject.FindGameObjectWithTag("MainCamera");
        
    }

    void LateUpdate()
    {
        if (!followObject)return;
        
        Vector3 dir = followObject.transform.position - _camera.transform.position;
        Quaternion rot = Quaternion.LookRotation(dir);
        this.transform.position = followObject.transform.position + (rot * offset);
        this.transform.rotation = rot;
        
    }
}
