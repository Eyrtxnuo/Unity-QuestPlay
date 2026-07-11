using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace LocomotionCollision
{
    
    [RequireComponent(typeof(SplineContainer))]
    public class SplineLocomotionCollision :  LocomotionCollision
    {
        private SplineContainer _roadSpline;
        
        public float roadWidth = 4;
        public float offroadWidth = 10;
        public float roadHeight = 0.1f;
        
        void Start()
        {
            _roadSpline = GetComponent<SplineContainer>();
        }


        public override LocomotionCollisionType IsColliding(Vector3 position)
        {
            float deltaY = Mathf.Abs(position.y - transform.position.y);
            if (deltaY > roadHeight)
            {
                return LocomotionCollisionType.NoCollision;
            }
            else
            {
                return IsColliding2D(new Vector2(position.x, position.z));
            }
        }

        /** This method is used to check the road collision only by x and z. (2D looking from the top)
         * Used to ignore height if it's not needed.
         */
        public override LocomotionCollisionType IsColliding2D(Vector2 position)
        {
            float3 positionLocal = _roadSpline.transform.InverseTransformPoint(new Vector3(position.x, 0, position.y));
            float3 nearestPointLocal;
            float t;
            SplineUtility.GetNearestPoint(_roadSpline.Spline, positionLocal, out nearestPointLocal, out t);
            Vector3 nearestPointWorld = _roadSpline.transform.TransformPoint(new Vector3(nearestPointLocal.x, 0, nearestPointLocal.z));
            Vector2 nearestPoint2D = new Vector2(nearestPointWorld.x, nearestPointWorld.z);
            float squaredDistance = (position - nearestPoint2D).sqrMagnitude;

            if (squaredDistance < (roadWidth*roadWidth))
            {
                Debug.DrawLine(new Vector3(position.x, nearestPointWorld.y, position.y), nearestPointWorld, Color.green);
                return LocomotionCollisionType.Road;
            }else if (squaredDistance < (offroadWidth*offroadWidth))
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
    }
}