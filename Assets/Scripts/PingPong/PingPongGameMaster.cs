using System;
using System.Collections;
using UnityEngine;
namespace PingPong
{
    public class PingPongGameMaster : MonoBehaviour
    {
        public GameObject netObject;
        public Ball3D ball;
        public Transform serveHandPosition;
        public float paddleDebounceTime = 0.1f;
        public GameObject ghostBallPrefab;
        public GameObject haloPrefab;
        public int haloRipples = 3;
        public float rippleDelay = 0.4f;
        
        private Scores _scores =  new Scores();
        private Team _serveTurn = Team.Player;
        private short _bouncesLeft = 0;
        private Team _lastTouch = Team.Player;
        private bool _playerServing = false;
        private bool _ballTossed = false;
        private float _lastPaddleTouchTime = 0;
        private bool _playEnded;//TODO: far capire meglio che lo scambio è concluso.
        private GameObject _ghost;


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
            if(_playEnded) return;
            //Paddle Debounce
            if (collision.gameObject.CompareTag("PingPongPlayerPaddle") ||
                collision.gameObject.CompareTag("PingPongOpponentPaddle"))
            {
                if (Time.time - _lastPaddleTouchTime < paddleDebounceTime)
                {
                    return;
                }
                _lastPaddleTouchTime = Time.time;
            }
                
            
            Debug.Log(collision.gameObject.name);
            if(_playerServing)
            {
                if (collision.gameObject.CompareTag("PingPongPlayerPaddle"))
                {
                    
                    _playerServing = false;
                    _ballTossed = true;
                    ball.ServeBind = false;
                    _bouncesLeft = 2;
                    _lastTouch = Team.Player;
                    return;
                }
                else
                {
                    //TODO: fallo, ripeti battuta. (ora da il punto)
                    Debug.Log($"SCORED: punto a {NotTeam(_serveTurn)} — lancio battuta caduto a terra");
                    ScoredRoutine(NotTeam(_serveTurn), collision);
                    return;
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
                        Debug.Log($"SCORED: punto a {NotTeam(_serveTurn)} — primo rimbalzo di battuta fuori dal campo di chi ha servito");
                        ScoredRoutine(NotTeam(_serveTurn), collision);
                        //TODO: reset stato
                    }
                    return;
                }
                else  if (_bouncesLeft == 0)
                {
                    Debug.Log(IsPlayerSide(collision.contacts[0].point));
                    if (IsPlayerSide(collision.contacts[0].point) == _lastTouch)
                    {
                        // palla rimbalza nel campo di chi l'ha toccata.
                        // punto a chi non è _lastTouch
                        Debug.Log($"SCORED: punto a {NotTeam(_lastTouch)} — la palla è rimbalzata di nuovo nel campo di chi l'ha toccata");
                        ScoredRoutine(NotTeam(_lastTouch), collision);
                        
                        //TODO: reset stato
                    }
                    return;
                }
                else
                {
                    Debug.Log($"SCORED: punto a {NotTeam(_lastTouch)} — la palla è rimbalzata troppe volte");
                    ScoredRoutine(NotTeam(_lastTouch), collision);
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
                Debug.Log("SCORED: punto a Opponent — il player ha fatto un doppio tocco");
                ScoredRoutine(Team.Opponent, collision);
                //TODO: reset stato
                return;
            }
            if (_lastTouch == Team.Opponent && collision.gameObject.CompareTag("PingPongOpponentPaddle"))
            {
                // Fallo, punto Player (toccata pallina quando doveva toccarla l'altro)
                Debug.Log("SCORED: punto a Player — l'opponent ha fatto un doppio tocco");
                ScoredRoutine(Team.Player, collision);
                //TODO: reset stato
                return;
            }
            if (_lastTouch == Team.Opponent && collision.gameObject.CompareTag("PingPongPlayerPaddle") && _bouncesLeft > 0)
            {
                // Fallo, punto Opponent (toccata pallina prima del rimbalzo)
                Debug.Log("SCORED: punto a Opponent — il player ha toccato la pallina prima del rimbalzo");
                ScoredRoutine(Team.Opponent, collision);
                //TODO: reset stato
                return;
            }
            if (_lastTouch == Team.Player && collision.gameObject.CompareTag("PingPongOpponentPaddle") && _bouncesLeft > 0)
            {
                // Fallo, punto Player (toccata pallina prima del rimbalzo) 
                Debug.Log("SCORED: punto a Player — l'Opponent ha toccato la pallina prima del rimbalzo");
                ScoredRoutine(Team.Player, collision);
                //TODO: reset stato
                return;
            }
            
            if (collision.gameObject.CompareTag("PingPongPlayerPaddle") || collision.gameObject.CompareTag("PingPongOpponentPaddle"))
            {
                //TODO: ritorna palla
                _lastTouch = collision.gameObject.CompareTag("PingPongPlayerPaddle") ? Team.Player : Team.Opponent;
                _bouncesLeft = 1;
                return;
            }
            
            
            // tocco qualunque altro oggetto

            if (_bouncesLeft > 0)
            {
                // Punto != _lastTouch, campo mancato
                
                Debug.Log($"SCORED: punto a {NotTeam(_lastTouch)} — palla fuori dal campo prima del rimbalzo");
                ScoredRoutine(NotTeam(_lastTouch), collision);
                //TODO: reset stato
                return;
            }
            else
            {
                // Punto == _lastTouch, palla a terra quando va presa
                Debug.Log($"SCORED: punto a {_lastTouch} — palla fuori dal campo dopo il rimbalzo, non presa.");
                ScoredRoutine(_lastTouch, collision);

                
                //TODO: reset stato
                return;
            }
        }

        Team IsPlayerSide(Vector3 position)
        {
            Vector3 localPosition = netObject.transform.InverseTransformPoint(position);
            if (localPosition.z > 0) 
            {
                // Campo player
                return Team.Player;
            } 
            else if (localPosition.z < 0) 
            {
                // Campo avversario
                return Team.Opponent;
            }
            else return Team.Opponent;
        }

        public void Serve(Team? team = null)
        {
            team ??= _serveTurn;
            _serveTurn = team.Value;
            _lastTouch = _serveTurn;
            _bouncesLeft = 2;
            if (team == Team.Opponent)
            {
                ball.ServeBind = false;
                _playerServing = false;
                ball.Relaunch();
            }
            else
            {
                _playerServing = true;
                _ballTossed = false;
                ball.ServeBind = true;
            }
            
            
        }

        public void BallTossInput()
        {
            if (_playerServing && !_ballTossed)
            {
                ball.ServeBind = false;
                //_playerServing = false;
                _ballTossed = true;
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

        void ScoredRoutine(Team team, Collision collision = null)
        {
            _scores.Scored(team);
            Team turn = _scores.GetTurn();
            //Serve(turn);return;// serve ora  avviene quando il player schiaccia il tasto A. 
            _serveTurn = turn;
            _playEnded = true;
            
            //add ghost ball and halo
            if(collision == null) return;
            Vector3 touchPos = BallPosFromCollision(collision);
            if (ghostBallPrefab)
            {
                _ghost = Instantiate(ghostBallPrefab, touchPos, ball.transform.rotation);
            }

            if (haloPrefab)
            {
                StartCoroutine(SpawnRipplesCoroutine(collision.GetContact(0).point, collision.GetContact(0).normal, team));
            }

        }

        public void Ready_Button()
        {
            if (_playEnded)
            {
                _playEnded = false;
                Serve();
                //TODO: resetta pallina, fantasma pallina e tutto il resto
                if (_ghost)
                {
                    Destroy(_ghost);
                    _ghost = null;
                }
            }
        }

        public Vector3 BallPosFromCollision(Collision collision)
        {
            ContactPoint contactPoint = collision.GetContact(0);
            return contactPoint.point + contactPoint.normal * 0.025f; // 0.025 è il raggio della pallina
            
        }
        
        void SpawnHalo(Vector3 position, Vector3 normal, Team scoringTeam)
        {
            Debug.Log("Spawning halo");
            GameObject haloObj = Instantiate(haloPrefab);
            HaloHandler halo = haloObj.GetComponent<HaloHandler>();
            halo.SpawnHalo(position, normal, scoringTeam);
        }
        
        IEnumerator SpawnRipplesCoroutine(Vector3 position, Vector3 normal, Team scoringTeam)
        {
            for (int i = 0; i < haloRipples; i++)
            {
                SpawnHalo(position, normal, scoringTeam);
                yield return new WaitForSeconds(rippleDelay);
            }
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

        public Team GetTurn()
        {
            int tot = _player +  _opponent;
            // Vantaggi, si inverte ogni punto
            if (_player >= 10 && _opponent >= 10)
            {
                return tot % 2 == 0 ? Team.Player : Team.Opponent;
            }
            // Normale, si inverte ogni 2 punti
            return (tot/2) % 2 == 0 ? Team.Player : Team.Opponent;
        }
    }

    public enum Team
    {
        Player,
        Opponent
    }
}
