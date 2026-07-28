using System;
using UnityEngine;
namespace PingPong
{
    public class PingPongGameMaster : MonoBehaviour
    {
        public GameObject netObject;
        public Ball3D ball;
        public Transform serveHandPosition;
        
        private Scores _scores =  new Scores();
        private Team _serveTurn = Team.Player;
        private short _bouncesLeft = 0;
        private Team _lastTouch = Team.Player;
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
                if (collision.gameObject.CompareTag("PingPongPlayerPaddle"))
                {
                    _playerServing = false;
                    ball.ServeBind = false;
                    _bouncesLeft = 2;
                    _lastTouch = Team.Player;
                }
                else
                {
                    //TODO: fallo, ripeti battuta.
                }
            }
            
            
            Debug.DrawRay(netObject.transform.position, netObject.transform.forward * 3f, Color.red, 2f);

            Debug.DrawLine(netObject.transform.position, collision.transform.position, Color.blue, 2f);
            
            
            if (collision.gameObject.CompareTag("PingPongField"))
            {
                _bouncesLeft--;
                if (_bouncesLeft > 0)
                {
                    
                    if (IsPlayerSide(collision.contacts[0].point) != _lastTouch)
                    {
                        // primo rimbalzo battuta non nel campo del player che ha battuto
                        // punto a chi non è _lastTouch
                        _scores.Scored(NotTeam(_serveTurn));
                        //TODO: reset stato
                    }
                    return;
                }
                else if (_bouncesLeft == 0)
                {
                    if (IsPlayerSide(collision.contacts[0].point) == _lastTouch)
                    {
                        // palla rimbalza nel campo di chi l'ha toccata.
                        // punto a chi non è _lastTouch
                        _scores.Scored(NotTeam(_serveTurn));
                        
                        //TODO: reset stato
                    }
                    return;
                }
            }
            // tocco rete, ok, ma non conta come rimbalzo
            if (collision.gameObject == netObject)
            {
                return;
            }
            
            // tocco racchette:

            if (_lastTouch == Team.Player && collision.gameObject.CompareTag("PingPongPlayerPaddle"))
            {
                // Fallo, punto Opponent (toccata pallina quando doveva toccarla l'altro)
                _scores.Scored(Team.Opponent);
                //TODO: reset stato
            }
            if (_lastTouch == Team.Opponent && collision.gameObject.CompareTag("PingPongOpponentPaddle"))
            {
                // Fallo, punto Player (toccata pallina quando doveva toccarla l'altro)
                _scores.Scored(Team.Player);
                //TODO: reset stato
            }
            if (_lastTouch == Team.Opponent && collision.gameObject.CompareTag("PingPongPlayerPaddle") && _bouncesLeft > 0)
            {
                // Fallo, punto Opponent (toccata pallina prima del rimbalzo)
                _scores.Scored(Team.Opponent);
                //TODO: reset stato
            }
            if (_lastTouch == Team.Player && collision.gameObject.CompareTag("PingPongOpponentPaddle") && _bouncesLeft > 0)
            {
                // Fallo, punto Player (toccata pallina prima del rimbalzo) 
                _scores.Scored(Team.Player);
                //TODO: reset stato
            }
            
            
            // tocco qualunque altro oggetto

            if (_bouncesLeft > 0)
            {
                // Punto != _lastTouch, campo mancato
                _scores.Scored(NotTeam(_serveTurn));
                //TODO: reset stato
            }
            else
            {
                // Punto == _lastTouch, palla a terra quando va presa
                _scores.Scored(_serveTurn);
                //TODO: reset stato
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

        private Team NotTeam(Team team)
        {
            return team == Team.Opponent ? Team.Player : Team.Opponent;
        }

        public Tuple<int, int> GetScores()
        {
            return _scores.GetScores();
        }

        void ScoredRoutine(Team team)
        {
            _scores.Scored(team);
            
        }
        
    }

    public class Scores
    {
        private int _player;
        private int _opponent;
        
        public Scores(int player = 0, int opponent= 0)
        {
            this._player = player;
            this._opponent = opponent;
        }

        public void Scored(Team team)
        {
            if (team == Team.Player)
            {
                _player++;
            }else if (team == Team.Opponent)
            {
                _opponent++;
            }
        }

        public Tuple<int, int> GetScores()
        {
            return new Tuple<int, int>(_player, _opponent);
        }
    }

    public enum Team
    {
        Player,
        Opponent
    }
}
