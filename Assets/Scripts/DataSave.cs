using UnityEngine;

public static class DataSave 
{
    public static void SetVal(DataKeys key,int val)
    {
        PlayerPrefs.SetInt(KeyDecoder(key), val);
        PlayerPrefs.Save();
    }
    
    public static int GetVal(DataKeys key)
    {
        return PlayerPrefs.GetInt(KeyDecoder(key));
    }

    public enum DataKeys
    {
        HIGHSCORE_RANGE,
        HIGHSCORE_CHARGE,
        HIGHSCORE_PINGPONG_SURVIVAL
    }

    public static string KeyDecoder(DataKeys key) => key switch
    {
        DataKeys.HIGHSCORE_RANGE => "HighScore_Range",
        DataKeys.HIGHSCORE_CHARGE => "HighScore_Charge",
        DataKeys.HIGHSCORE_PINGPONG_SURVIVAL => "HighScore_PingPong-Survival",
        _ => ""
    };

    public static string GetScoreboardKey(DataKeys key) => key switch
    {
        DataKeys.HIGHSCORE_RANGE => "LEADERBOARD_RANGE",
        DataKeys.HIGHSCORE_CHARGE => "LEADERBOARD_CHARGE",
        DataKeys.HIGHSCORE_PINGPONG_SURVIVAL => "LEADERBOARD_PINGPONG_SURVIVAL",
        _ => ""
    };
}
