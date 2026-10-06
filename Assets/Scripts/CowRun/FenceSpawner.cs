using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace CowRun
{

    [RequireComponent(typeof(SplineContainer))]
    public class FenceSpawner : MonoBehaviour
    {

        private SplineContainer _roadSpline;

        public float fenceWidth = 14;
        public Mesh fenceMesh;
        public Material fenceMaterial;
        public Vector3 fenceScale = Vector3.one;
        public Vector3 fenceRotation = Vector3.zero;

        private List<Matrix4x4[]> matrixInstances = new List<Matrix4x4[]>();
       
        void Start()
    {
        _roadSpline = GetComponent<SplineContainer>();
        Vector3 bounds = Vector3.Scale(fenceMesh.bounds.size, fenceScale);
        float fenceLength = bounds.x;

        List<Matrix4x4> allFences = new List<Matrix4x4>();

        CalculateFencesForSide(-fenceWidth / 2f, fenceLength, allFences);
        CalculateFencesForSide(fenceWidth / 2f, fenceLength, allFences);

        List<Matrix4x4> instanceChunk = new List<Matrix4x4>();
        foreach (var matrix in allFences)
        {
            instanceChunk.Add(matrix);
            if (instanceChunk.Count == 1023)
            {
                matrixInstances.Add(instanceChunk.ToArray());
                instanceChunk.Clear();
            }
        }
        if (instanceChunk.Count > 0)
        {
            matrixInstances.Add(instanceChunk.ToArray());
        }

        Debug.Log($"[FenceSpawner] Generati {allFences.Count} recinti distribuiti in {matrixInstances.Count} draw calls.");
    }

    private void CalculateFencesForSide(float offset, float spacing, List<Matrix4x4> outputList)
    {
        float splineLength = _roadSpline.Spline.GetLength();
        
        int resolution = Mathf.Max(2, Mathf.CeilToInt(splineLength * 2f));
        
        List<Vector3> points = new List<Vector3>();
        List<Vector3> forwards = new List<Vector3>();
        List<Vector3> ups = new List<Vector3>();

        for (int i = 0; i <= resolution - 1; i++)
        {
            float t = (float)i / resolution + (0.5f/resolution);
            _roadSpline.Evaluate(t, out float3 pos, out float3 tangent, out float3 up);
            Vector3 fwd = ((Vector3)tangent).normalized;
            Vector3 upVec = ((Vector3)up).normalized;
            Vector3 right = Vector3.Cross(upVec, fwd).normalized;
            
            points.Add((Vector3)pos + (right * offset));
            forwards.Add(fwd);
            ups.Add(upVec);
        }

        Quaternion offsetRot = Quaternion.Euler(fenceRotation);
        outputList.Add(Matrix4x4.TRS(points[0], Quaternion.LookRotation(forwards[0], ups[0]) * offsetRot, fenceScale));
        float distanceAccumulator = 0f;
        Vector3 currentPos = points[0];

        for (int i = 1; i < points.Count; i++)
        {
            Vector3 nextPos = points[i];
            float segLength = Vector3.Distance(currentPos, nextPos);

            while (distanceAccumulator + segLength >= spacing)
            {
                float t = (spacing - distanceAccumulator) / segLength;
                Vector3 exactPos = Vector3.Lerp(currentPos, nextPos, t);
                Vector3 exactFwd = Vector3.Lerp(forwards[i-1], forwards[i], t).normalized;
                Vector3 exactUp = Vector3.Lerp(ups[i-1], ups[i], t).normalized;

                Quaternion exactRot = Quaternion.LookRotation(exactFwd, exactUp) * offsetRot;
                outputList.Add(Matrix4x4.TRS(exactPos, exactRot, fenceScale));

                currentPos = exactPos;
                segLength = Vector3.Distance(currentPos, nextPos);
                distanceAccumulator = 0f;
            }
            
            distanceAccumulator += segLength;
            currentPos = nextPos;
        }
    }

        void Update()
    {
        
        foreach (var lotto in matrixInstances)
        {
            Graphics.DrawMeshInstanced(fenceMesh, 0, fenceMaterial, lotto);
        }
    }


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            SplineContainer splineContainer = GetComponent<SplineContainer>();
            if (splineContainer == null || splineContainer.Spline == null) return;
            _roadSpline = splineContainer;

            Vector3 bounds = Vector3.Scale(fenceMesh.bounds.size, fenceScale);
            float fenceLength = bounds.x;

            List<Matrix4x4> gizmoFences = new List<Matrix4x4>();

            CalculateFencesForSide( -fenceWidth / 2f, fenceLength, gizmoFences);
            CalculateFencesForSide( fenceWidth / 2f, fenceLength, gizmoFences);

            Gizmos.color = Color.burlywood;

            foreach (var matrix in gizmoFences)
            {
                Gizmos.matrix = matrix;
                Gizmos.DrawWireMesh(fenceMesh);
            }
            Gizmos.matrix = Matrix4x4.identity;


        }
#endif
    }
}
