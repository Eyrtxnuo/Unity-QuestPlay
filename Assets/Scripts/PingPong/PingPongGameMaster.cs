using System;
using System.Collections;
using UnityEngine;

namespace PingPong
{
    public class PingPongGameMaster : MonoBehaviour
    {
        public GameObject netObject;
        public Ball3D ball;
        public float paddleDebounceTime = 0.1f;
        public float tableDebounceTime = 0.3f;
        public GameObject ghostBallPrefab;
        public GameObject haloPrefab;
        public int haloRipples = 3;
        public float rippleDelay = 0.4f;
        public FXManager fxManager;
        public EndGamePanelManager endgamePanel;
        public TooltipHandler tooltipH;

        private Scores _scores = new Scores();
        private Team _serveTurn = Team.Player;
        private short _bouncesLeft = 0;
        private Team _lastTouch = Team.Player;
        private bool _playerServing = true;
        private bool _ballTossed = false;
        private float _lastPaddleTouchTime = 0;
        private float _lastTableTouchTime = 0;
        private bool _playEnded; //TODO: far capire meglio che lo scambio è concluso.
        private bool _matchEnded;
        private GameObject _playerPaddle;
        private GameObject _opponentPaddle;
        private PaddleBotV2 _opponentBot;
        private PaddleSafeRespawn _safeRespawnComponent;

        private float _waitingInputFrom;

        private const float TOOLTIP_WAIT_TIME = 1.5f;

        void Start()
        {
            ball.enabled = true;
            ball.ServeBind = true;
            _playerPaddle = GameObject.FindWithTag("PingPongPlayerPaddle");
            _opponentPaddle = GameObject.FindWithTag("PingPongOpponentPaddle");
            _opponentBot = _opponentPaddle.GetComponent<PaddleBotV2>();
            _safeRespawnComponent = _playerPaddle.GetComponent<PaddleSafeRespawn>();
        }

        private void OnEnable()
        {
            ball.OnAnyCollision += HandleBounce;
            PauseManager.OnPause += HandlePause;
            PauseManager.OnResume += HandleResume;
        }

        private void OnDisable()
        {
            ball.OnAnyCollision -= HandleBounce;
            PauseManager.OnPause -= HandlePause;
            PauseManager.OnResume -= HandleResume;
        }

        // Update is called once per frame
        void Update()
        {
            Debug.DrawRay(netObject.transform.position, netObject.transform.right, Color.red);
            Debug.DrawRay(netObject.transform.position, -netObject.transform.right, Color.darkRed);

            if (tooltipH && Time.time > _waitingInputFrom + TOOLTIP_WAIT_TIME)
            {
                if (_playerServing && !_ballTossed)
                {
                    tooltipH.ShowToss(true);
                    _waitingInputFrom = float.MaxValue;
                }
                else if (!_matchEnded && _playEnded)
                {
                    tooltipH.ShowContinue(true);
                    _waitingInputFrom = float.MaxValue;
                }
            }
        }

        void HandleBounce(Collision collision)
        {
            if (_playEnded) return;
            if (_matchEnded) return;

            if (_lastTouch == Team.Opponent && _opponentBot.GetTurn())
            {
                _opponentBot.SetTurn(false);
            }

            if (_bouncesLeft <= 2 && !_opponentBot.GetTurn() &&
                (_lastTouch == Team.Player || collision.gameObject.CompareTag("PingPongPlayerPaddle")))
            {
                _opponentBot.SetTurn(true);
            }
            else
            {
                Debug.Log(_bouncesLeft + "  -  " + !_opponentBot.GetTurn() + "  -  " + (_lastTouch == Team.Player));
            }
            
            //Table Debounce (collisioni sono buggate e ogni tanto vengono emessi due eventi)
            if (collision.gameObject.CompareTag("PingPongField"))
            {
                if (Time.time - _lastTableTouchTime < tableDebounceTime) return;
                _lastTableTouchTime = Time.time;
            }

            //Paddle Debounce
            if (collision.gameObject.CompareTag("PingPongPlayerPaddle") ||
                collision.gameObject.CompareTag("PingPongOpponentPaddle"))
            {
                _opponentBot.bouncedInBotField = false;
                
                if (Time.time - _lastPaddleTouchTime < paddleDebounceTime)
                {
                    return;
                }

                _lastPaddleTouchTime = Time.time;
            }

            Debug.Log(collision.gameObject.name);


            if (_playerServing)
            {
                if (collision.gameObject.CompareTag("PingPongPlayerPaddle"))
                {
                    _playerServing = false;
                    _ballTossed = true;
                    ball.ServeBind = false;
                    _bouncesLeft = 2;
                    _lastTouch = Team.Player;
                    tooltipH.ShowToss(false);
                    return;
                }
                else
                {
                    // fallo, ripeti battuta. (ora da il punto)
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
                    if (GetTeamSide(collision.contacts[0].point) != _lastTouch)
                    {
                        // primo rimbalzo battuta non nel campo del player che ha battuto
                        // punto a chi non è _lastTouch
                        Debug.Log(
                            $"SCORED: punto a {NotTeam(_serveTurn)} — primo rimbalzo di battuta fuori dal campo di chi ha servito");
                        ScoredRoutine(NotTeam(_serveTurn), collision);
                    }

                   

                    return;
                }
                else if (_bouncesLeft == 0)
                {
                    var side = GetTeamSide(collision.contacts[0].point);
                    Debug.Log(side);
                    if (side == _lastTouch)
                    {
                        // palla rimbalza nel campo di chi l'ha toccata.
                        // punto a chi non è _lastTouch
                        Debug.Log(
                            $"SCORED: punto a {NotTeam(_lastTouch)} — la palla è rimbalzata di nuovo nel campo di chi l'ha toccata");
                        ScoredRoutine(NotTeam(_lastTouch), collision);

                    }
                    
                    if (GetTeamSide(collision.contacts[0].point) == Team.Opponent)
                    {
                        _opponentBot.bouncedInBotField = true;
                        Debug.Log("BounceInBotField");
                    }
                    
                    return;
                }
                else
                {
                    Debug.Log($"SCORED: punto a {_lastTouch} — la palla è rimbalzata troppe volte");
                    ScoredRoutine(_lastTouch, collision);
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
                return;
            }

            if (_lastTouch == Team.Opponent && collision.gameObject.CompareTag("PingPongOpponentPaddle"))
            {
                // Fallo, punto Player (toccata pallina quando doveva toccarla l'altro)
                Debug.Log("SCORED: punto a Player — l'opponent ha fatto un doppio tocco");
                ScoredRoutine(Team.Player, collision);
                return;
            }

            if (_lastTouch == Team.Opponent && collision.gameObject.CompareTag("PingPongPlayerPaddle") &&
                _bouncesLeft > 0)
            {
                // Fallo, punto Opponent (toccata pallina prima del rimbalzo)
                Debug.Log("SCORED: punto a Opponent — il player ha toccato la pallina prima del rimbalzo");
                ScoredRoutine(Team.Opponent, collision);
                return;
            }

            if (_lastTouch == Team.Player && collision.gameObject.CompareTag("PingPongOpponentPaddle") &&
                _bouncesLeft > 0)
            {
                // Fallo, punto Player (toccata pallina prima del rimbalzo) 
                Debug.Log("SCORED: punto a Player — l'Opponent ha toccato la pallina prima del rimbalzo");
                ScoredRoutine(Team.Player, collision);
                return;
            }

            if (collision.gameObject.CompareTag("PingPongPlayerPaddle") ||
                collision.gameObject.CompareTag("PingPongOpponentPaddle"))
            {
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
                return;
            }
            else
            {
                // Punto == _lastTouch, palla a terra quando va presa
                Debug.Log($"SCORED: punto a {_lastTouch} — palla fuori dal campo dopo il rimbalzo, non presa.");
                ScoredRoutine(_lastTouch, collision);


                return;
            }
        }

        Team GetTeamSide(Vector3 position)
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
                var ballSpawnPosition = ball.GetSpawnPosition();
                _opponentPaddle.transform.position = ballSpawnPosition + new Vector3(0, 0, -0.1f);
                _opponentPaddle.transform.rotation = Quaternion.identity;
                _opponentBot.ClearTargetState(ballSpawnPosition);
                ball.ServeBind = false;
                _playerServing = false;
                ball.Relaunch();
                fxManager.OpponentServe(ballSpawnPosition);
            }
            else
            {
                _safeRespawnComponent.EnableSafeMode(true);
                _playerServing = true;
                _ballTossed = false;
                ball.ServeBind = true;
                ball.StopMovement();
                _waitingInputFrom = Time.time;
            }
        }


        private const bool CAN_RECATCH_TOSS = true;

        public void BallTossInput()
        {
            if (_playerServing && !_ballTossed)
            {
                tooltipH.ShowToss(false);
                ball.ServeBind = false;
                //_playerServing = false;
                _ballTossed = true;
                ball.Toss();
                fxManager.PlayerServe(ball.transform.position);
                if (CAN_RECATCH_TOSS) StartCoroutine(RecatchTossCoroutine());
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
            _waitingInputFrom = Time.time;
            _ballTossed = false;
            ball.ServeBind = false;
            _playerServing = false;
            _opponentBot.SetTurn(false);
            if (_matchEnded) return;

            _scores.Scored(team);

            Team turn = _scores.GetTurn();
            _serveTurn = turn;
            _playEnded = true;

            //add ghost ball and halo
            if (collision == null) return;
            Vector3 touchPos = BallPosFromCollision(collision);
            if (ghostBallPrefab)
            {
                Instantiate(ghostBallPrefab, touchPos, ball.transform.rotation);// IMPORTANTE: il prefab deve avere tag "PingPongRemoveOnServe", altrimenti non despawnerà
            }

            if (haloPrefab)
            {
                StartCoroutine(SpawnRipplesCoroutine(collision.GetContact(0).point, collision.GetContact(0).normal,
                    team));
            }

            if (_scores.GetWinner() is { } win) // se non null la partita è finita
            {
                Debug.Log("Match ended, winner: " + win);
                _matchEnded = true;

                _playerPaddle.SetActive(false);
                _opponentPaddle.SetActive(false);


                if (win == Team.Player)
                {
                    fxManager.WinMatchFx(Team.Player);
                }
                else
                {
                    fxManager.WinMatchFx(Team.Opponent);
                }

                endgamePanel.ShowPanel(_scores);
                return;
            }
            else
            {
                fxManager.ScoredFx(team); // scoredFX can interfere with WinMatchFX  
            }
        }

        public void Ready_Button()
        {
            if (_matchEnded) return;
            if (_playEnded)
            {
                tooltipH.ShowContinue(false);
                _playEnded = false;
                Serve();
                var listOfDespawn = GameObject.FindGameObjectsWithTag("PingPongRemoveOnServe");
                foreach (var item in listOfDespawn)
                {
                    Destroy(item);
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
                yield return WaitsCache.GetWaitForSeconds(rippleDelay);
            }
        }


        IEnumerator RecatchTossCoroutine(float endDelay = 2f)
        {
            var startTime = Time.time;
            yield return WaitsCache.WAIT_100ms;

            while (Time.time - startTime < endDelay)
            {
                if (!_ballTossed) yield break;
                if (!_playerServing) yield break;

                if (ball.CanCancelToss())
                {
                    _ballTossed = false;
                    ball.ServeBind = true;
                    _waitingInputFrom = Time.time;
                }

                yield return null;
            }
        }


        private void HandlePause()
        {
            fxManager.PauseAll();
        }

        private void HandleResume()
        {
            fxManager.ResumeAll();
        }
    }

    public class Scores
    {
        private int _player;
        private int _opponent;

        public Scores(int player = 0, int opponent = 0)
        {
            this._player = player;
            this._opponent = opponent;
        }

        public void Scored(Team team)
        {
            if (team == Team.Player)
            {
                _player++;
            }
            else if (team == Team.Opponent)
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
            int tot = _player + _opponent;
            // Vantaggi, si inverte ogni punto
            if (_player >= 10 && _opponent >= 10)
            {
                return tot % 2 == 0 ? Team.Player : Team.Opponent;
            }

            // Normale, si inverte ogni 2 punti
            return (tot / 2) % 2 == 0 ? Team.Player : Team.Opponent;
        }


        private const int WIN_POINTS = 11;

        public Team? GetWinner()
        {
            if (_player >= (WIN_POINTS - 1) && _opponent >= (WIN_POINTS - 1) && Math.Abs(_player - _opponent) >= 2)
            {
                return _player - _opponent >= 0 ? Team.Player : Team.Opponent;
            }

            if (_player >= WIN_POINTS && _opponent <= (WIN_POINTS - 2))
            {
                return Team.Player;
            }

            if (_opponent >= WIN_POINTS && _player <= (WIN_POINTS - 2))
            {
                return Team.Opponent;
            }

            return null;
        }
    }

    public enum Team
    {
        Player,
        Opponent
    }
}