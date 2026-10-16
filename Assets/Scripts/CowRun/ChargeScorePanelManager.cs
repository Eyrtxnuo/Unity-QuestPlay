using TMPro;

namespace Firing
{
    public class ChargeScorePanelManager : PanelManager
    {
        public TextMeshProUGUI partialScoreText;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI scoreText;
        
    
        public void ShowPanel(int partialScore, int timeRemaining)
        {
            partialScoreText.text = partialScore.ToString();
            timerText.SetText(timeRemaining.ToString() + "s");
            scoreText.text = (partialScore + timeRemaining).ToString();
            base.ShowPanel();
        }

        public new void HidePanel()
        {
            base.HidePanel();
            scoreText.text = "";
        }
    }
}
