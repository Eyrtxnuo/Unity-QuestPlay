using UnityEngine;

[RequireComponent(typeof(Collider))]
public class IgnoreCollisionWith : MonoBehaviour
{

    public GameObject otherObject;

    void Start()
    { 
        //Physics.IgnoreCollision(otherObject.GetComponent<Collider>(), GetComponent<Collider>());
    }

}
