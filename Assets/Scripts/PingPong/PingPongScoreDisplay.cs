using TMPro;
using UnityEngine;

namespace PingPong
{
    [RequireComponent(typeof(TextMeshPro))]
    public class PingPongScoreDisplay : MonoBehaviour
    {
        public PingPongGameMaster gameMaster;
    
        private TextMeshPro _scoresText;
    
        void Start()
        {
            _scoresText = GetComponent<TextMeshPro>();
        }

        // Update is called once per frame
        void Update()
        {
            // Aggiorna il testo
            var score = gameMaster.GetScores();
            _scoresText.SetText("Score: {0} - {1}", score.Item1, score.Item2);
        }
    }
}
