using LocomotionCollision;
using UnityEditor;


#if UNITY_EDITOR
using UnityEngine;
#endif

namespace CowRun
{
    public class ScarecrowSpawner : MonoBehaviour
    {
        public GameObject target;
    
        public GameObject prefab;
        public float spawnDistance = 10;

        public SplineLocomotionCollision roadSpline;

        void Start()
        {
            if (target == null || prefab == null)
            {
                Debug.LogWarning("[ScarecrowSpawner] Prefab o target non settati!");
                this.enabled = false;
            }
        }
        void FixedUpdate()
        {
            if (Vector3.Distance(this.transform.position, target.transform.position) <= spawnDistance)
            {
                var sc = Instantiate(prefab, this.transform.position, Quaternion.identity,  this.transform);
                this.enabled = false;
                sc.TryGetComponent(out ScarecrowKing sck);
                if(sck)sck.roadSpline = this.roadSpline;
            }
        }


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Handles.DrawWireDisc(transform.position, Vector3.up, spawnDistance);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            MeshFilter[] meshFilters = prefab.GetComponentsInChildren<MeshFilter>();
            foreach (var meshFilter in meshFilters)
            {
                Mesh mesh = meshFilter.sharedMesh;
                Gizmos.DrawWireMesh(mesh, this.transform.position, Quaternion.FromToRotation(Vector3.forward, Vector3.up));
            }
        }
#endif
    }

}
