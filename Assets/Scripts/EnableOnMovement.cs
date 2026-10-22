using UnityEngine;

public class EnableOnMovement : MonoBehaviour
{
    public MonoBehaviour[] components;
    public GameObject waitMovement;
    
    private Vector3 _oldPosition;

    void Start()
    {
        _oldPosition = waitMovement.transform.position;
    }
    
    
    void Update()
    {
        if (_oldPosition != waitMovement.transform.position)
        {
            foreach (MonoBehaviour component in components)
            {
                component.enabled = true;
            }
            this.enabled = false;
        }
    }
}
