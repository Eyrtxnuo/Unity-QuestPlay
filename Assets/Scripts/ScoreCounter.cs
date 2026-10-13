using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class ScoreCounter : MonoBehaviour
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

    private TextMeshPro _scoreText;

    void Start()
    {
        _scoreText =  GetComponent<TextMeshPro>();
        ScoreUpdate();
    }

    private void ScoreUpdate()
    {
        _scoreText.SetText("Score: " + Score.ToString());
    }
        
        
}