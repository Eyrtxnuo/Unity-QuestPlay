using UnityEngine;

namespace MetaPlatformStuff
{
    public class ShowLeaderboard : MonoBehaviour
    {
        private static GameObject _openLeaderboard;
        public LeaderboardHandle leaderboardPrefab;


        public void Show(DataSave.DataKeys scoreboardKey, string leaderboardName)
        {
            if(_openLeaderboard != null) Destroy(_openLeaderboard);
            
            var handle = Instantiate(leaderboardPrefab);
            handle.LoadScores(scoreboardKey, leaderboardName);
            _openLeaderboard = handle.gameObject;
        }

        public void ShowCharge()
        {
            this.Show(DataSave.DataKeys.HIGHSCORE_CHARGE, "Charge!");
        }
        
        public void ShowRange()
        {
            this.Show(DataSave.DataKeys.HIGHSCORE_RANGE, "Shooting Range");
        }
    }
}