using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MetaPlatformStuff
{
    public class LeaderboardHandle : MonoBehaviour
    {
        public TextMeshProUGUI leaderboardTitle;
        public GameObject loadingSpinner;
        public GameObject scoresList;
        public ScoreEntryWriter scoreEntryPrefab;
        public ScoreEntryWriter userScoreEntry;
        public Color selfHighlightColor;
        
        public Sprite errorSprite;
        
        public void LoadScores(DataSave.DataKeys leaderboardApiKey, string leaderboardName)
        {
            leaderboardTitle.text = leaderboardName;
            
            MetaLeaderboardManager.Instance.FetchTopScoresAndPlayer(leaderboardApiKey, 5, (list, player) =>
            {
                loadingSpinner.SetActive(false);
                scoresList.gameObject.SetActive(true);
                foreach (var entry in list)
                {
                    ScoreEntryWriter entryObject = Instantiate(scoreEntryPrefab, scoresList.transform);
                    entryObject.Configure(entry);
                    if (entry.User.ID == MetaLeaderboardManager.Instance.CurrentUserId)
                    {
                        entryObject.SetBackgroundColor(selfHighlightColor);
                    }
                }

                if (player is not null)
                {
                    userScoreEntry.Configure(player);
                    userScoreEntry.gameObject.SetActive(true);
                }
            }, error =>
            {
                Animator get;
                Image img;
                loadingSpinner.TryGetComponent<Animator>(out get);
                loadingSpinner.TryGetComponent<Image>(out img);
                get.enabled = false;
                img.sprite = errorSprite;
                Debug.LogError(error);
            });
        }

        public void DestroyLeaderboard()
        {
            Destroy(this.gameObject);
        }
    }
}
