using UnityEngine;

namespace MetaPlatformStuff
{
    [RequireComponent(typeof(LeaderboardHandle))]
    public class InstantLoadHelper : MonoBehaviour
    {
        private LeaderboardHandle _lh;

        public void LoadCallback()
        {
            _lh = GetComponent<LeaderboardHandle>();
            _lh.LoadScores(DataSave.DataKeys.HIGHSCORE_CHARGE, "Charge!");
        }
    }
}
