using UnityEngine;
using UnityEngine.Splines;
#if UNITY_EDITOR
using Unity.Mathematics;
using UnityEditor;
#endif


namespace CowRun
{

    [RequireComponent(typeof(SplineContainer))]
    public class FenceSpawner : MonoBehaviour
    {

        private SplineContainer _roadSpline;

        public float fenceWidth = 14;
        public GameObject fencePrefab;
       
        void Start()
        {
            _roadSpline = GetComponent<SplineContainer>();
            Spline spline = _roadSpline.Spline;
            Vector3 bounds = fencePrefab.GetComponentInChildren<Renderer>().bounds.size;
            float fenceLength = bounds.z * 0.9f;
            float splineLength = spline.GetLength();
            int resolution = Mathf.Max(2, Mathf.CeilToInt(splineLength / fenceLength));
            
    
            Debug.Log("[FenceSpawner] splineLength " + splineLength + "; fenceLength " + fenceLength + "; resolution " + resolution);
            
            for (int i = 0; i <= resolution-1; i++)
            {
                // Sample based on percentage of distance
                float t = (float)i / resolution + (0.5f/resolution);
                _roadSpline.Evaluate(t, out float3 worldPos, out float3 worldTangent, out float3 worldUp);
                Vector3 forward = ((Vector3)worldTangent).normalized;
                Vector3 up = ((Vector3)worldUp).normalized;
                Vector3 right = Vector3.Cross(up, forward).normalized;

                Quaternion splineRotation = Quaternion.LookRotation(forward, up);
                
                GameObject leftFence = Instantiate(fencePrefab, (Vector3)worldPos - (right * fenceWidth /2), splineRotation, this.transform);
                GameObject rightFence = Instantiate(fencePrefab, (Vector3)worldPos + (right * fenceWidth /2), splineRotation, this.transform);
            }
        }


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            SplineContainer splineContainer = GetComponent<SplineContainer>();
            if (splineContainer == null || splineContainer.Spline == null) return;


            // Reset of the Gizmos matrix to absolute world space
            Gizmos.matrix = Matrix4x4.identity;

            Spline mainSpline = splineContainer.Spline;

            // 1. Adaptive sampling based on length
            float splineLength = mainSpline.GetLength();
            // At least 2 samples, otherwise one every 'targetDistance' meters
            int resolution = Mathf.Max(2, Mathf.CeilToInt(splineLength / 1));

            // Create arrays for points

            // 2. Uniform sample over length (not 't')
            for (int i = 0; i <= resolution; i++)
            {
                // Sample based on percentage of distance
                float t = (float)i / resolution;
                splineContainer.Evaluate(t, out float3 worldPos, out float3 worldTangent, out float3 worldUp);
                Vector3 forward = ((Vector3)worldTangent).normalized;
                Vector3 up = ((Vector3)worldUp).normalized;
                Vector3 right = Vector3.Cross(up, forward).normalized;

                Quaternion splineRotation = Quaternion.LookRotation(forward, up);
                
                // GameObject leftFence = Instantiate(fencePrefab, (Vector3)worldPos - (right * fenceWidth), splineRotation);
                // GameObject rightFence = Instantiate(fencePrefab, (Vector3)worldPos + (right * fenceWidth), splineRotation);
                Gizmos.DrawSphere((Vector3)worldPos - (right * fenceWidth / 2), 0.1f);
                Gizmos.DrawSphere((Vector3)worldPos + (right * fenceWidth / 2), 0.1f);
            }


        }
#endif
    }
}
