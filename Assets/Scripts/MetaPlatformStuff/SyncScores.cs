using MetaPlatformStuff;
using UnityEngine;

public class SyncScores : MonoBehaviour
{
    // syncs highscores. se le highscore sono state fatte offline le carica all'apertura del gioco.
    public void OnMetaReady()
    {
        if(MetaLeaderboardManager.Instance.IsOnlineReady)
        {
            Debug.LogError("Function called before the meta platform is ready!");
            return;
        }

        var chargeHs = DataSave.GetVal(DataSave.DataKeys.HIGHSCORE_CHARGE);
        if (chargeHs > 0)
        {
            MetaLeaderboardManager.Instance.SubmitScoreToMeta(DataSave.DataKeys.HIGHSCORE_CHARGE, chargeHs);
        }

        var rangeHs = DataSave.GetVal(DataSave.DataKeys.HIGHSCORE_RANGE);
        if (rangeHs > 0)
        {
            MetaLeaderboardManager.Instance.SubmitScoreToMeta(DataSave.DataKeys.HIGHSCORE_RANGE, rangeHs);
        }

    }
}
