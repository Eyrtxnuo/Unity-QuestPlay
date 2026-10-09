using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TeleportIfDistant : MonoBehaviour
{
    public GameObject target;
    public float maxDistance;


    private void FixedUpdate()
    {
        if (Vector3.Distance(this.transform.position, target.transform.position) >= maxDistance)
        {
            this.transform.position = target.transform.position;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Handles.DrawWireDisc(transform.position, Vector3.up, maxDistance);
    }
#endif
}
