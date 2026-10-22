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
        HIGHSCORE_CHARGE
    }

    public static string KeyDecoder(DataKeys key) => key switch
    {
        DataKeys.HIGHSCORE_RANGE => "HighScore_Range",
        DataKeys.HIGHSCORE_CHARGE => "HighScore_Charge",
        _ => ""
    };

    public static string GetScoreboardKey(DataKeys key) => key switch
    {
        DataKeys.HIGHSCORE_RANGE => "LEADERBOARD_RANGE",
        DataKeys.HIGHSCORE_CHARGE => "LEADERBOARD_CHARGE",
        _ => ""
    };
}
