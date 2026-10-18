using System.Collections;
using Oculus.Platform.Models;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace MetaPlatformStuff
{
    public class ScoreEntryWriter : MonoBehaviour
    {
        public TextMeshProUGUI labelPosition;
        public TextMeshProUGUI labelUsername;
        public TextMeshProUGUI labelScore;
        public RawImage userImage;
    
        public Image background;

        public void Configure(LeaderboardEntry entry)
        {
            labelPosition.text = entry.Rank.ToString() + "°";
            labelUsername.text = entry.User.DisplayName;
            labelScore.text = entry.Score.ToString();
            if (userImage is not null && entry.User.SmallImageUrl is not null)
            {
                StartCoroutine(LoadImageFromUrl(entry.User.SmallImageUrl, userImage));
            }
        }


        public IEnumerator LoadImageFromUrl(string url, RawImage targetRawImage)
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
            {
                yield return uwr.SendWebRequest();
                if (uwr.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                    targetRawImage.texture = texture;
                }
            }
        }

        public void SetBackgroundColor(Color color)
        {
            background.color = color;
        }
    }
}
