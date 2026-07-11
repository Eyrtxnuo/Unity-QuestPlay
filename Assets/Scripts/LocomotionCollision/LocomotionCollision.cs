using UnityEngine;

namespace LocomotionCollision
{
    public abstract class LocomotionCollision : MonoBehaviour
    {
        public abstract LocomotionCollisionType IsColliding(Vector3 position);
        public abstract LocomotionCollisionType IsColliding2D(Vector2 position);
    }


    public enum LocomotionCollisionType
    {
        Road,
        OffRoad,
        NoCollision
    }
}
