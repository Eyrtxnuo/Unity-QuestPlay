using System;
using UnityEngine;
namespace PingPong
{
    public class PingPongGameMaster : MonoBehaviour
    {
        public GameObject netObject;
        public Ball3D ball;
        
        private Tuple<int, int> _scores =  new Tuple<int, int>(0, 0);
        private Team ServeTurn = Team.Player;
        private short bouncesLeft = 0;
        
    
        void Start()
        {
            ball.enabled = false;
        }
        
        private void OnEnable()
        {
            ball.OnAnyCollision += HandleBounce;
        }

        private void OnDisable()
        {
            ball.OnAnyCollision -= HandleBounce;
        }

        // Update is called once per frame
        void Update()
        {
            Debug.DrawRay(netObject.transform.position, netObject.transform.right,  Color.red);
            Debug.DrawRay(netObject.transform.position, -netObject.transform.right,  Color.darkRed);
        }

        void HandleBounce(Collision collision)
        {
            
            Debug.DrawRay(netObject.transform.position, netObject.transform.forward * 3f, Color.red, 2f);

            Debug.DrawLine(netObject.transform.position, collision.transform.position, Color.blue, 2f);

            if (collision.gameObject.CompareTag("PingPongField"))
            {
                IsPlayerSide(collision.contacts[0].point);
            }
        }

        Team IsPlayerSide(Vector3 position)
        {
            Vector3 localPosition = netObject.transform.InverseTransformPoint(position);
            if (localPosition.z > 0) 
            {
                // Campo avversario
                return Team.Opponent;
            } 
            else if (localPosition.z < 0) 
            {
                // Campo player
                return Team.Player;
            }
            else return Team.Opponent;
        }

        void Serve(Team? team)
        {
            team ??= ServeTurn;
            
            
            
        }
        
    }


    public enum Team
    {
        Player,
        Opponent
    }
}
