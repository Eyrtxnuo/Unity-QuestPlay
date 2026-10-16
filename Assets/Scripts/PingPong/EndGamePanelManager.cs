using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PingPong
{
    public class EndGamePanelManager : PanelManager
    {
        public TextMeshProUGUI resultTitleText;
        public TextMeshProUGUI playerScoreText;
        public TextMeshProUGUI opponentScoreText;
        public Image background;
    
    

        public void ShowPanel(Scores scoreContainer)
        {
            //Debug.Log("Showing winner: " + scoreContainer.GetWinner());
            if(scoreContainer?.GetWinner() == null)return;

            if (scoreContainer.GetWinner() == Team.Player)
            {
            
                resultTitleText.text = "You Win!";
                background.color = new Color32(100, 100, 130, 255);
            }
            else
            {
                resultTitleText.text = "You Lose!";
                background.color = new Color32(130, 100, 100, 255);
            }

            var score = scoreContainer.GetScores();
            playerScoreText.text = score.Item1.ToString();
            opponentScoreText.text = score.Item2.ToString();
        
            base.ShowPanel();
        }



        public new void HidePanel()
        {
            base.HidePanel();
            resultTitleText.text = "";
            playerScoreText.text = "";
            opponentScoreText.text = "";
            background.color = new Color32(111, 111, 111, 255);
        }
    }
}
