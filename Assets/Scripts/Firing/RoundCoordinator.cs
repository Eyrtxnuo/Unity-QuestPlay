using UnityEngine;

namespace Firing
{
    public class RoundCoordinator : MonoBehaviour
    {
        [Header("Gameplay configuration")] public float waitPreGame = 2f;
        public float durationPhase1 = 30f;
        public float durationPhase2 = 40f;
        public float durationPhase3 = 30f;
        public BoundArea playingArea;

        private int _phase = 0;
        private float _timer = 0;

        [Header("Phase 1")] public GameObject[] balloonPrefabs;
        public GameObject goldBalloonPrefab;
        public GameObject balloonContainer;
        public float spawnYOffset = -2f;
        public float spawnDelayRangeMin = 1f, spawnDelayRangeMax = 1.7f;
        public int maxSpawnCount = 3;
        [Range(0f, 1f)] public float goldProbability = 0.1f;

        private float _randWait = 0;

        private void FixedUpdate()
        {
            switch (_phase)
            {
                case 0: // pre-game
                    _timer += Time.fixedDeltaTime;
                    if (_timer >= waitPreGame)
                    {
                        PassToPhase(1);
                        break;
                    }
                    
                    break;
                case 1: //Balloons
                    _timer += Time.fixedDeltaTime;
                    if (_timer >= durationPhase1)
                    {
                        PassToPhase(2);
                    }

                    if (_timer >= _randWait)
                    {
                        int spawnCount = Random.Range(0, maxSpawnCount);
                        for (int i = 0; i < spawnCount; i++)
                        {
                            SpawnBalloon();
                        }
                        _randWait = _timer + Random.Range(spawnDelayRangeMin, spawnDelayRangeMax);
                    }
                    break;
                case 2:
                    _timer += Time.fixedDeltaTime;
                    if (_timer >= durationPhase2)
                    {
                        PassToPhase(3);
                    }

                    break;
                case 3:
                    _timer += Time.fixedDeltaTime;
                    if (_timer >= durationPhase3)
                    {
                        EndGame();
                    }

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
                playingArea.bounds.min.y + spawnYOffset,
                Random.Range(playingArea.bounds.min.z, playingArea.bounds.max.z));


            GameObject prefab = Random.value < goldProbability
                ? goldBalloonPrefab
                : balloonPrefabs[Random.Range(0, balloonPrefabs.Length)];
            GameObject balloon = Instantiate(prefab, balloonPos, Quaternion.identity);
            balloon.transform.SetParent(balloonContainer.transform, true);

        }


        private void PassToPhase(int phase)
        {
            _timer = 0;
            _phase = phase;
            switch (phase)
            {
                case 0:
                    break;
                case 1:
                    //spawn ballons under map

                    break;
                case 2:
                    // clear balloons
                    // start can launcher
                    break;
                case 3:
                    // clear cans (or await all are dead)
                    // start target summoner
                    break;
            }
        }

        private void EndGame()
        {
        }
    }
}