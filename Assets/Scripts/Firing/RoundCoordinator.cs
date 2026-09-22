using System;
using Firing.Targets;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Firing
{
    public class RoundCoordinator : MonoBehaviour
    {
        [Header("Gameplay configuration")] public float phaseChangeTime = 5f;
        public float durationPhase1 = 30f;
        
        public float durationPhase3 = 30f;

        public BoundArea playingArea;

        private int _phase = 3; //TODO: riporta a 0
        private float _timer = 0;

        [Header("Phase 1")] public GameObject[] balloonPrefabs;
        public GameObject goldBalloonPrefab;
        public float balloonSpawnYOffset = -2f;
        public float balloonSpawnDelayRangeMin = 0.5f;
        public float balloonSpawnDelayRangeMax = 1f;
        public int balloonMaxSpawnCount = 3;
        [Range(0f, 1f)] public float goldBalloonProbability = 0.1f;

        [Header("Phase 2")] public CanLauncher canLauncher;
        public CanPhase[] phases;
        private int _phaseIndex = -1;
        private int _spawnCounter = 0;
        private float _forcedEndTime = 0;

        [Header("Phase 3")] public TargetTarget standardTarget;
        public TargetTarget negativeTarget;
        [Range(0f, 1f)] public float negativeTargetProbability = 0.25f;
        public TargetTarget goldTarget;
        [Range(0f, 1f)] public float goldTargetProbability = 0.1f;
        public Vector2Int gridSize;
        public float targetSpawnDelay;
        public int targetMaxSpawnCount;
        private Quaternion _targetSpawnRotation = Quaternion.FromToRotation(Vector3.up, Vector3.forward);
        private bool[,] gridOccupationMatrix;
        Vector3 planeCenter;
        private Vector2 planeSize;
        private Vector3 gridOrigin;
        private Vector3 slotsSize;
        
        
        [Header("End Game")] public ShootingScorePanelManager endGamePanelManager;
        public FiringScoreCounter scoreCounter;
        public GameObject gun;
        
        void Start()
        {
            // calcoli per la Target grid 
            gridOccupationMatrix = new bool[gridSize.x, gridSize.y];
            planeCenter = playingArea.bounds.center;
            planeSize = new Vector2(
                playingArea.bounds.extents.x * 2,
                playingArea.bounds.extents.y * 2);
            gridOrigin = new Vector3(
                planeCenter.x - planeSize.x / 2,
                planeCenter.y - planeSize.y / 2,
                planeCenter.z);
            slotsSize = new Vector2(
                planeSize.x / gridSize.x,
                planeSize.y / gridSize.y);
        }


        private float _randWait = 0;
        private int _nextPhase = 0;

        private void FixedUpdate()
        {
            switch (_phase)
            {
                case -1: // phase change
                    _timer += Time.fixedDeltaTime;
                    if (_timer >= phaseChangeTime)
                    {
                        PassToPhase(_nextPhase);
                    }

                    break;
                case 0: // pre-game
                    _timer += Time.fixedDeltaTime;
                    if (_timer >= phaseChangeTime)
                    {
                        PassToPhase(1);
                        break;
                    }

                    break;
                case 1: //Balloons
                    _timer += Time.fixedDeltaTime;
                    if (_timer >= durationPhase1)
                    {
                        // per passare alla fase 2, aspettiamo che il timer finisca, e che tutti i palloncini siano despawnati, + fallback se i palloncini non sono despawnati in 10s
                        if (this.transform.childCount == 0 || _timer >= durationPhase1 + 10)
                        {
                            for (int i = this.transform.childCount - 1; i >= 0; i--) //cancella tutti i children rimasti (caso fallback 10s)
                            {
                                Destroy(this.transform.GetChild(i).gameObject);
                            }

                            PhaseTransition(2);
                        }

                        break;
                    }

                    if (_timer >= _randWait) // spawna 1-3 palloncini ogni 0,5 - 1 secondi
                    {
                        int spawnCount = Random.Range(1, balloonMaxSpawnCount);
                        for (int i = 0; i < spawnCount; i++)
                        {
                            SpawnBalloon();
                        }

                        _randWait = _timer + Random.Range(balloonSpawnDelayRangeMin, balloonSpawnDelayRangeMax);
                    }

                    break;
                case 2: // Cans
                    _timer += Time.fixedDeltaTime;

                    if (_spawnCounter <= 0 && (this.transform.childCount == 0 || _timer >= _forcedEndTime))
                    {
                        _phaseIndex++;
                        if (_phaseIndex >= phases.Length)
                        {
                            if (this.transform.childCount == 0)
                            {
                                Debug.Log("CanLauncher phase exiting");
                                PhaseTransition(3);
                            }
                        }
                        else
                        {
                            _forcedEndTime = _timer + phases[_phaseIndex].maxDuration;
                            _spawnCounter = phases[_phaseIndex].canNumber;
                            Debug.Log("New can launcher phase: " + _phaseIndex);
                        }

                        break;
                    }

                    if (_timer >= _randWait && _spawnCounter > 0)
                    {
                        //spawn can
                        Vector3 randomPos = new Vector3(
                            Random.Range(playingArea.bounds.min.x, playingArea.bounds.max.x),
                            Random.Range(playingArea.bounds.min.y, playingArea.bounds.max.y),
                            Random.Range(playingArea.bounds.min.z, playingArea.bounds.max.z));
                        var can = canLauncher.LaunchCan(randomPos);
                        can.transform.SetParent(this.transform, true);
                        //update variabili 
                        _spawnCounter--;
                        _randWait = _timer + phases[_phaseIndex].canDelay;
                    }


                    break;
                case 3: // Targets
                    _timer += Time.fixedDeltaTime;
                    if (_timer >= durationPhase3)
                    {
                        if (this.transform.childCount == 0 ||  _timer >= durationPhase3 + 10) //aspetta despawn di tutti i target + fallback dopo 10s
                        {
                            for (int i = this.transform.childCount - 1; i >= 0; i--) //cancella tutti i children rimasti (caso fallback 10s)
                            {
                                Destroy(this.transform.GetChild(i).gameObject);
                            }
                            
                            EndGame();
                        }
                        break;
                    }

                    if (_timer >= _randWait)
                    {
                        int spawnCount = Random.Range(1, targetMaxSpawnCount);
                        for (int i = 0; i < spawnCount; i++)
                        {
                            int rx = -1;
                            int ry = -1;
                            for (int k = 0; k < 10; k++)
                            {
                                int kx = Random.Range(0, gridSize.x);
                                int ky = Random.Range(0, gridSize.y);
                                if (gridOccupationMatrix[kx, ky]) continue;
                                rx = kx;
                                ry = ky;
                                break;
                            }
                            SpawnTarget(rx, ry);
                            _randWait = _timer + targetSpawnDelay;
                        }
                    }

                    break;
                case -2:// end game / stato vuoto
                    break;
                default:
                    Debug.LogWarning("Unknown phase " + _phase + ", defaulting to 0");
                    PassToPhase(0);
                    break;
            }
        }

        private void SpawnBalloon()
        {
            Vector3 balloonPos = new Vector3(
                Random.Range(playingArea.bounds.min.x, playingArea.bounds.max.x),
                playingArea.bounds.min.y + balloonSpawnYOffset,
                Random.Range(playingArea.bounds.min.z, playingArea.bounds.max.z));


            GameObject prefab = Random.value < goldBalloonProbability
                ? goldBalloonPrefab
                : balloonPrefabs[Random.Range(0, balloonPrefabs.Length)];
            GameObject balloon = Instantiate(prefab, balloonPos, Quaternion.identity);
            balloon.transform.SetParent(this.transform, true);
        }

        /* Spawna un target alle coordinate specificate della griglia.
         * Se le coordinate non sono valide o il posto è occupato return;
         * Il tipo di target è scelto random con le percentuali specificate.
         * La probabilità di target standard è 1 - gold - negative.
         */
        private void SpawnTarget(int x = -1, int y = -1)
        { 
            if (x < 0 || x >= gridSize.x || y < 0 || y >= gridSize.y) return;
            if (gridOccupationMatrix[x, y]) return;
            gridOccupationMatrix[x, y] = true;


            Vector3 center = gridOrigin + new Vector3(
                slotsSize.x * x + slotsSize.x / 2,
                slotsSize.y * y + slotsSize.y / 2,
                0);

            var randomTypeVal = Random.value;
            TargetTarget prefab;
            if (randomTypeVal < goldTargetProbability)
            {
                prefab = goldTarget;
            }
            else if (randomTypeVal < goldTargetProbability + negativeTargetProbability)
            {
                prefab = negativeTarget;
            }
            else
            {
                prefab = standardTarget;
            }

            TargetTarget target = Instantiate(prefab, center, _targetSpawnRotation);
            target.transform.SetParent(this.transform, true);
            target.SetupDestroyCallback(() => { gridOccupationMatrix[x, y] = false; });
        }


        private void
            PhaseTransition(int phase) // passa alla fase scelta dopo un periodo di pausa lungo "phaseChangeTime"
        {
            _nextPhase = phase;
            PassToPhase(-1); // transition phase
        }

        private void PassToPhase(int phase)
        {
            _timer = 0;
            _randWait = 0;
            _phase = phase;
        }

        private void EndGame()
        {
            PassToPhase(-2);
            endGamePanelManager.ShowPanel(scoreCounter.Score);
            scoreCounter.gameObject.SetActive(false);
            gun?.gameObject.SetActive(false);
            // open end game panel
        }

#if (UNITY_EDITOR)
        private void OnDrawGizmosSelected()
        {
            Vector3 planeCenter = playingArea.bounds.center;
            Vector2 planeSize = new Vector2(
                playingArea.bounds.extents.x * 2,
                playingArea.bounds.extents.y * 2);
            Vector3 gridOrigin = new Vector3(
                planeCenter.x - planeSize.x / 2,
                planeCenter.y - planeSize.y / 2,
                planeCenter.z);

            Gizmos.DrawWireCube(planeCenter, planeSize);
            Vector2 slotsSize = new Vector2(
                planeSize.x / gridSize.x,
                planeSize.y / gridSize.y);
            for (int i = 0; i < gridSize.x; i++)
            {
                for (int j = 0; j < gridSize.y; j++)
                {
                    Vector3 center = gridOrigin + new Vector3(
                        slotsSize.x * i + slotsSize.x / 2,
                        slotsSize.y * j + slotsSize.y / 2,
                        0);
                    Gizmos.DrawWireCube(center, slotsSize);
                    Gizmos.DrawSphere(center, 0.01f);
                }
            }
        }
#endif
    }


    [Serializable]
    public struct CanPhase
    {
        public int canNumber;
        public float maxDuration;
        public float canDelay;
    }
}