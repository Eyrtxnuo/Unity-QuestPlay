using TMPro;
using UnityEngine;

namespace Firing
{
    public class ChargeScorePanelManager : PanelManager
    {
        public TextMeshProUGUI partialScoreText;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI scoreText;
        
        [Header("High Score config")]
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI highScoreText;
        [Tooltip("The high score container will be disabled in case of highscore")]
        public GameObject highScoreContainer;
        
    
        public void ShowPanel(int finalScore, int partialScore, int timeRemaining, int highscore = 0)
        {
            partialScoreText.text = partialScore.ToString();
            timerText.SetText(timeRemaining.ToString() + "s");
            scoreText.text = finalScore.ToString();
            
            //highscore handling
            if (finalScore> highscore)
            {
                highScoreContainer.SetActive(false);
                titleText.text = "New high score!";
            }
            else
            {
                highScoreText.text = highscore.ToString();
            }
            
            base.ShowPanel();
        }

        public new void HidePanel()
        {
            base.HidePanel();
            scoreText.text = "";
        }
    }
}
