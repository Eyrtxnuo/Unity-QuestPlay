using System;
using Firing;
using MetaPlatformStuff;
using UnityEngine;

namespace PingPong
{
    public class SurvivalPingPongMaster : MonoBehaviour
    {
        public Ball3D ballPrefab;
        public GameObject ghostBallPrefab;
        public GameObject haloPrefab;
        public int haloRipples = 3;
        public float rippleDelay = 0.4f;
    
    
        public FXManager fxManager;
        public BouncesSfxManager bouncesSfxManager;
        public ScoreCounter scoreCounter;
        public ShootingScorePanelManager endgamePanel;
        public TooltipHandler tooltipH;
        public GameObject netObject;
    
        public float paddleDebounceTime = 0.1f;
        public float tableDebounceTime = 0.3f;
        
        private Ball3D _currBall;
        private bool _started = false;
        private bool _ended = false;
        private GameObject _playerPaddle;
        private float _lastPaddleTouchTime = 0;
        private float _lastTableTouchTime = 0;
        private Team _lastTouch = Team.Opponent;
        private int score = 0;
    
        void Start()
        {
            _playerPaddle = GameObject.FindWithTag("PingPongPlayerPaddle");
            tooltipH.ShowContinue();
        }
        
        private void OnEnable()
        {
            if (_currBall != null) _currBall.OnAnyCollision += HandleBounce;
            PauseManager.OnPause += HandlePause;
            PauseManager.OnResume += HandleResume;
        }

        private void OnDisable()
        {
            if (_currBall != null) _currBall.OnAnyCollision -= HandleBounce;
            PauseManager.OnPause -= HandlePause;
            PauseManager.OnResume -= HandleResume;
        }

        public void Ready_Button()
        {
            if (_started) return;
            _started = true;

            tooltipH.ShowContinue(false);
            score = 0;
            SpawnBall();
            var listOfDespawn = GameObject.FindGameObjectsWithTag("PingPongRemoveOnServe");
            foreach (var item in listOfDespawn)
            {
                Destroy(item);
            }
        }

        private void SpawnBall()
        {
            if (_currBall != null)
            {
                _currBall.OnAnyCollision -= HandleBounce;
                _currBall.DespawnAfter();
            }
            
            _currBall = Instantiate(ballPrefab); // scambio pallina seguita
            _currBall.OnAnyCollision += HandleBounce;
            _lastTouch = Team.Opponent;
            _currBall.Relaunch(0.8f);
            bouncesSfxManager.ChangeBall(_currBall);
        }

        private void HandleBounce(Collision collision)
        {
            if(!_started || _ended)return;
            
            // tocco rete, ok
            if (collision.gameObject == netObject)
            {
                return;
            }
            
            //Table Debounce (collisioni sono buggate e ogni tanto vengono emessi due eventi)
            if (collision.gameObject.CompareTag("PingPongField"))
            {
                if (Time.time - _lastTableTouchTime < tableDebounceTime) return;
                _lastTableTouchTime = Time.time;
            }
            
            //Paddle Debounce
            if (collision.gameObject.CompareTag("PingPongPlayerPaddle"))
            {
                if (Time.time - _lastPaddleTouchTime < paddleDebounceTime)
                {
                    return;
                }
                _lastPaddleTouchTime = Time.time;
            }


            if (_lastTouch == Team.Opponent)
            {
                if (collision.gameObject.CompareTag("PingPongField"))
                {
                    return;
                }
                if (collision.gameObject.CompareTag("PingPongPlayerPaddle"))
                {
                    _lastTouch = Team.Player;
                    return;
                }
                EndGame();
                return;
            }
            
            
            
            if (_lastTouch == Team.Player 
                && collision.gameObject.CompareTag("PingPongField") 
                && GetTeamSide(collision.contacts[0].point) == Team.Opponent)
            {
                score++;
                SpawnBall();
                scoreCounter.Score = score;
                return;
            }
            
            /* se arriva qui, significa:
              - ultimo tocco player (pallina rimandata)
              - non ha toccato rete
              - non ha toccato il campo avversario
              
              Significa FALLO: partita finita.
             */
            EndGame();
            
            //throw new Exception("TODO: finire la partita");
        }

        private void EndGame()
        {
            _ended = true;
            
            int highScore = DataSave.GetVal(DataSave.DataKeys.HIGHSCORE_PINGPONG_SURVIVAL);
            if (score > highScore)
            {
                try{
                    DataSave.SetVal(DataSave.DataKeys.HIGHSCORE_PINGPONG_SURVIVAL, score);
                    MetaLeaderboardManager.Instance?.SubmitScoreToMeta(DataSave.DataKeys.HIGHSCORE_PINGPONG_SURVIVAL, score);
                }
                catch(Exception e)
                {
                    Debug.LogError("Failed to submit score: " + e, this);
                }
            }
            endgamePanel.ShowPanel(score, highScore);
            scoreCounter.gameObject.SetActive(false);
            _playerPaddle.gameObject.SetActive(false);
        }


        private void HandlePause()
        {
            fxManager.PauseAll();
        }

        private void HandleResume()
        {
            fxManager.ResumeAll();
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
    }
}
