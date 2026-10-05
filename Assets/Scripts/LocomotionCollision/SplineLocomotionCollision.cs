using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LocomotionCollision
{
    
    [RequireComponent(typeof(SplineContainer))]
    public class SplineLocomotionCollision :  LocomotionCollision
    {
        private SplineContainer _roadSpline;
        
        public float roadWidth = 4;
        public float offroadWidth = 12;
        public float roadHeight = 0.1f;
        public bool roadForceEndStop = true;
        
        void Start()
        {
            _roadSpline = GetComponent<SplineContainer>();
        }

        /** This method is used to check the road collision in 3D
         * The road is considered always flat orthogonally to the spline(the y of the closest point is the current y of the road)
         */
        public override LocomotionCollisionType IsColliding(Vector3 position)
        {
            float3 positionLocal = _roadSpline.transform.InverseTransformPoint(position); // spline needs local coordinates.
            SplineUtility.GetNearestPoint(_roadSpline.Spline, positionLocal, out float3 nearestPointLocal, out float t);
            Vector3 nearestPointWorld = _roadSpline.transform.TransformPoint(nearestPointLocal); // returning to world coords.

            if (roadForceEndStop && t is <= 0 or >= 1) //if nearest t is 0 or 1, the position is outside the road spline length
            {
                return LocomotionCollisionType.NoCollision;
            }

            // deltaY checks if the point is in the road vertically 
            float deltaY = Mathf.Abs(position.y - nearestPointWorld.y);
            if (deltaY > roadHeight)
            {
                return LocomotionCollisionType.NoCollision; 
            }

            // 5. Compare 2D distances (XZ coords)
            Vector2 pos2D = new Vector2(position.x, position.z);
            Vector2 nearest2D = new Vector2(nearestPointWorld.x, nearestPointWorld.z);
            float squaredDistance = (pos2D - nearest2D).sqrMagnitude; // sqrMagnitude makes the comparison between squared numbers, removing the need for a square root.


            if (squaredDistance < (roadWidth * 0.5f) * (roadWidth * 0.5f))// * 0.5 because it's the max distance allowed in both sides.
            {
                Debug.DrawLine(position, nearestPointWorld, Color.green);
                return LocomotionCollisionType.Road;
            }
            else if (squaredDistance < (offroadWidth * 0.5f) * (offroadWidth * 0.5f))// * 0.5 because it's the max distance allowed in both sides.
            {
                Debug.DrawLine(position, nearestPointWorld, Color.orange);
                return LocomotionCollisionType.OffRoad;
            }
            else
            {
                Debug.DrawLine(position, nearestPointWorld, Color.red);
                return LocomotionCollisionType.NoCollision;
            }
        }

        /** This method is used to check the road collision only by x and z. (2D looking from the top)
         * Used to ignore height if it's not needed.
         */
        public override LocomotionCollisionType IsColliding2D(Vector2 position)
        {
            float3 positionLocal = _roadSpline.transform.InverseTransformPoint(new Vector3(position.x, 0, position.y));
            SplineUtility.GetNearestPoint(_roadSpline.Spline, positionLocal, out var nearestPointLocal, out float t);
            
            //Debug.Log("t:" + t);
            if (roadForceEndStop && t is <= 0 or >= 1) //if nearest t is 0 or 1, the position is outside the road spline length
            {
                return LocomotionCollisionType.NoCollision;
            }
            
            Vector3 nearestPointWorld = _roadSpline.transform.TransformPoint(new Vector3(nearestPointLocal.x, 0, nearestPointLocal.z));
            Vector2 nearestPoint2D = new Vector2(nearestPointWorld.x, nearestPointWorld.z);
            float squaredDistance = (position - nearestPoint2D).sqrMagnitude;

            if (squaredDistance < (roadWidth * 0.5f) * (roadWidth * 0.5f))// * 0.5 because it's the max distance allowed in both sides.
            {
                Debug.DrawLine(new Vector3(position.x, nearestPointWorld.y, position.y), nearestPointWorld, Color.green);
                return LocomotionCollisionType.Road;
            }
            else if (squaredDistance < (offroadWidth * 0.5f) * (offroadWidth * 0.5f))// * 0.5 because it's the max distance allowed in both sides.
            {
                Debug.DrawLine(new Vector3(position.x, nearestPointWorld.y, position.y), nearestPointWorld, Color.orange);
                return LocomotionCollisionType.OffRoad;
            }
            else
            {
                Debug.DrawLine(new Vector3(position.x, nearestPointWorld.y, position.y), nearestPointWorld, Color.red);
                return LocomotionCollisionType.NoCollision;
            }
        }


        public (Vector3 closestPoint, float distance, float t, Vector3 splineTangent) GetClosestPointAndTangent(Vector3 position)
        {
            Debug.Log("spline");
            Debug.Log(_roadSpline);
            Debug.Log(_roadSpline.transform);
            float3 positionLocal = _roadSpline.transform.InverseTransformPoint(position); // spline needs local coordinates.
            SplineUtility.GetNearestPoint(_roadSpline.Spline, positionLocal, out float3 nearestPointLocal, out float t);
            Vector3 nearestPointWorld = _roadSpline.transform.TransformPoint(nearestPointLocal); // returning to world coords.
            float distance = Vector3.Distance(position, nearestPointWorld);
            float3 tangentLocal = _roadSpline.Spline.EvaluateTangent(t);
            Vector3 splineTangent = _roadSpline.transform.TransformDirection(tangentLocal).normalized;
            return (nearestPointWorld, distance, t, splineTangent);
        }
        
#if UNITY_EDITOR        
        public int gizmosBorderResolution = 250;
        private void OnDrawGizmosSelected()
        {
            SplineContainer splineC = GetComponent<SplineContainer>();
            
            var drawBorderLines = new Action<float, Color>((float width, Color color) =>
            {
                Vector3[] leftPoints = new Vector3[gizmosBorderResolution + 1];
                Vector3[] rightPoints = new Vector3[gizmosBorderResolution + 1];
                for (int i = 0; i <= gizmosBorderResolution; i++)
                {
                    float t = (float)i / gizmosBorderResolution;
                    splineC.Evaluate(t, out float3 worldPos, out float3 worldTangent, out float3 worldUp);
                    Vector3 right = Vector3.Cross(((Vector3)worldUp).normalized, ((Vector3)worldTangent).normalized).normalized;
                    leftPoints[i] = (Vector3)worldPos - (right * width / 2f);
                    rightPoints[i] = (Vector3)worldPos + (right * width / 2f);
                }
                Handles.color = color;
                Handles.DrawAAPolyLine(3f, leftPoints);
                Handles.DrawAAPolyLine(3f, rightPoints);
            });

            drawBorderLines(roadWidth, Color.cyan);
            drawBorderLines(offroadWidth, Color.chocolate);
            

        }
        #endif
    }
}