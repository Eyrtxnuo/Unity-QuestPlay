using LocomotionCollision;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;


#if UNITY_EDITOR
using UnityEditor;
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
                var (_, _, _, splineTangent) = this.roadSpline.GetClosestPointAndTangent(this.transform.position);
                Quaternion stRot = Quaternion.LookRotation(splineTangent);
                
                var sc = Instantiate(prefab, this.transform.position, stRot, this.transform.parent);
                this.enabled = false;
                var sck = sc.GetComponentInChildren<ScarecrowKing>();
                if(sck)sck.roadSpline = this.roadSpline;
                Destroy(this.gameObject);
            }
        }


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Handles.DrawWireDisc(transform.position, Vector3.up, spawnDistance);
        }

        private void OnDrawGizmos()
        {
            (Vector3 closestPoint, float distance, float t, Vector3 splineTangent) = this.roadSpline.GetClosestPointAndTangent(this.transform.position);
            Quaternion stRot = Quaternion.LookRotation(splineTangent);
            
            Gizmos.color = Color.yellowNice;
            MeshFilter[] meshFilters = prefab.GetComponentsInChildren<MeshFilter>();
            var stPos = prefab.transform.position;
            foreach (var meshFilter in meshFilters)
            {
                Gizmos.DrawMesh(
                    meshFilter.sharedMesh,
                    this.transform.position + (meshFilter.transform.position - stPos),
                    stRot * meshFilter.transform.rotation,
                    meshFilter.transform.lossyScale);
            }
        }
#endif
    }

}
