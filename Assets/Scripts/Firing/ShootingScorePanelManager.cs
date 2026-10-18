using TMPro;
using UnityEngine;

namespace Firing
{
    public class ShootingScorePanelManager : PanelManager
    {
        public TextMeshProUGUI scoreText;
        
        [Header("High Score config")]
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI highScoreText;
        [Tooltip("The high score container will be disabled in case of highscore")]
        public GameObject highScoreContainer;

    
        public void ShowPanel(int score, int highscore)
        {
            scoreText.text = score.ToString();
            
            
            //highscore handling
            if (score> highscore)
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
