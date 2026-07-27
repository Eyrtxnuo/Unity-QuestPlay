using System;
using UnityEngine;
namespace PingPong
{
    public class PingPongGameMaster : MonoBehaviour
    {
        public GameObject netObject;
        public Ball3D ball;
        public Transform serveHandPosition;
        
        private Tuple<int, int> _scores =  new Tuple<int, int>(0, 0);
        private Team _serveTurn = Team.Player;
        private short _bouncesLeft = 0;
        private bool _playerServing = false;
        
    
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
            Debug.Log(collision.gameObject.name);
            if(_playerServing)
            {
                _playerServing = false;
                ball.ServeBind = false;
            };
            
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

        public void Serve(Team? team)
        {
            team ??= _serveTurn;
            if (team == Team.Opponent)
            {
                ball.ServeBind = false;
                _playerServing = false;
                ball.Relaunch();
            }
            else
            {
                _playerServing = true;
                ball.ServeBind = true;
            }
            
            
        }

        public void BallTossInput()
        {
            if (_playerServing)
            {
                ball.ServeBind = false;
                _playerServing = false;
                ball.Toss();
            }
        }
    }


    public enum Team
    {
        Player,
        Opponent
    }
}
