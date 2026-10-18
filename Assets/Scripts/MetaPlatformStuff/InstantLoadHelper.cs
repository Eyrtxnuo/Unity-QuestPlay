using System;
using UnityEngine;

namespace MetaPlatformStuff
{
    [RequireComponent(typeof(LeaderboardHandle))]
    public class InstantLoadHelper : MonoBehaviour
    {
        private LeaderboardHandle _lh;
        
        private static bool loadedFirstTime = false;

        public void LoadCallback()
        {
            loadedFirstTime = true;
            _lh = GetComponent<LeaderboardHandle>();
            _lh.LoadScores(DataSave.DataKeys.HIGHSCORE_CHARGE, "Charge!");
        }


        public void Start()
        {
            if(!loadedFirstTime)return;
            LoadCallback();
        }
    }
}
