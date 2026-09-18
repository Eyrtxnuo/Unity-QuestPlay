using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Firing
{
    [RequireComponent(typeof(TextMeshPro))]
    public class FiringScoreCounter : MonoBehaviour
    {
        public int Score
        {
            get => _score;
            set
            {
                _score = value;
                ScoreUpdate();
            }
        }
        private int _score;

        public TextMeshPro scoreText;

        void Start()
        {
            scoreText =  GetComponent<TextMeshPro>();
            ScoreUpdate();
        }

        private void ScoreUpdate()
        {
            scoreText.SetText("Score: " + Score.ToString());
        }
        
    }
}
