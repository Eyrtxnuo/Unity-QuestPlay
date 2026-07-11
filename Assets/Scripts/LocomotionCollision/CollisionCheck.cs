using UnityEngine;

namespace LocomotionCollision
{
    public class CollisionCheck : MonoBehaviour
    {
        public LocomotionCollision collision;
    
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            Debug.Log(collision.IsColliding2D(new  Vector2(transform.position.x, transform.position.z)));
        }
    }
}
