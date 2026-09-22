using TMPro;

namespace Firing
{
    public class ShootingScorePanelManager : PanelManager
    {
        public TextMeshProUGUI scoreText;
    
        public new void ShowPanel(int score)
        {
            scoreText.text = score.ToString();
            base.ShowPanel();
        }

        public new void HidePanel()
        {
            base.HidePanel();
            scoreText.text = "";
        }
    }
}
