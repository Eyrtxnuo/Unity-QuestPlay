using UnityEngine;

public class ForceRendering : MonoBehaviour
{
    public GameObject prefab;
    public Transform pos;
    
    private GameObject _go;
    void Start()
    {
        _go = Instantiate(prefab);
        
    }

    // Update is called once per frame
    void Update()
    {
        _go.transform.position = pos.position + pos.forward*1.5f;
    }
}
