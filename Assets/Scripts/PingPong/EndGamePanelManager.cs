using PingPong;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndGamePanelManager : MonoBehaviour
{
    public GameObject OVRController;
    public TextMeshProUGUI resultTitleText;
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI opponentScoreText;
    public Image background;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeGameButtonHandler()
    {
        StartCoroutine(MainMenu.LoadSceneAsyncRoutine("MainMenu"));
    }
    
    public void PlayAgainButtonHandler()
    {
        StartCoroutine(MainMenu.LoadSceneAsyncRoutine("PingPong"));
    }

    public void ShowPanel(Scores scoreContainer)
    {
        //Debug.Log("Showing winner: " + scoreContainer.GetWinner());
        if(scoreContainer?.GetWinner() == null)return;

        Debug.Log("log1: " + (scoreContainer.GetWinner() == Team.Player));
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
        
        
        OVRController.SetActive(true);
        gameObject.SetActive(true);
    }



    public void HidePanel()
    {
        OVRController.SetActive(false);
        gameObject.SetActive(false);
        resultTitleText.text = "";
        playerScoreText.text = "";
        opponentScoreText.text = "";
        background.color = new Color32(111, 111, 111, 255);
    }
}
