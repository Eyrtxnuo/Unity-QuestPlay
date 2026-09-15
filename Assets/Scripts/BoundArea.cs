using UnityEngine;

public class BoundArea : MonoBehaviour
{
    [SerializeField] private Vector3 size = new Vector3(5f, 5f, 5f);

    public Bounds bounds => new Bounds(transform.position, size);
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, size);
    }
}
