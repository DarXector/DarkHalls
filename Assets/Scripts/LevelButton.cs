using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    public LevelData level;

    void Start()
    {
        Button b = gameObject.GetComponent<Button>();

        if(level.unlocked || Application.isEditor)
        {
            b.onClick.AddListener(delegate ()
            {
                LevelControl.instance.StartGame(level);
            });

            GetComponentsInChildren<Text>()[0].text = (level.index + 1).ToString();
        }
        else
        {
            GetComponentsInChildren<Text>()[0].text = "-";
        }

        string minutes = Mathf.Floor(level.bestTime / 60).ToString("00");
        string seconds = Mathf.Floor(level.bestTime % 60).ToString("00");

        GetComponentsInChildren<Text>()[1].text = minutes + ":" + seconds;

    }

}
