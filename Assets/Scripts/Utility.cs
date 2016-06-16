using UnityEngine;

public class Utility
{
    public static string formatTime(float _timer)
    {
        string minutes = Mathf.Floor(_timer / 60).ToString("00");
        string seconds = Mathf.Floor(_timer % 60).ToString("00");

        return minutes + ":" + seconds;
    }
}
