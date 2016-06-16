using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

using GooglePlayGames;
using UnityEngine.SocialPlatforms;
using GooglePlayGames.BasicApi;
using System.Reflection;

public class GameModel : Singleton<GameModel>
{
    protected GameModel() { } // guarantee this will be always a singleton only - can't use the constructor!

    public GameData gameData = new GameData();
    internal LevelData currentLevel;

    public delegate void OnLevelLoadedEvent(LevelData level);
    public event OnLevelLoadedEvent OnLevelLoaded;

    public delegate void OnLeaderBoardLoaded(LeaderboardScoreData leaderBoard);

    [HideInInspector]
    public bool authenticated = false;

    [HideInInspector]
    public bool navigated = false;
    private int _currentLevelIndex;
    internal float lastTime;

    public void Start()
    {
        // recommended for debugging:
        PlayGamesPlatform.DebugLogEnabled = true;
        // Activate the Google Play Games platform
        PlayGamesPlatform.Activate();

        gameData = new GameData();

        Load(0);
        currentLevel = gameData.levels[0];
    }

    public void Save(int index, string mazeCode, Vector2 mazeSize, bool hasEnemy)
    {
        Debug.Log("Save mazeID " + index);
        Debug.Log("Save mazeCode " + mazeCode);

        if (mazeCode == "")
        {
            return;
        }

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        Debug.Log("Application.dataPath " + Application.dataPath);
        if (Application.isEditor)
        {
            file = File.Open(Application.dataPath + "/Resources/levels.bytes", FileMode.Create);
        }
        else
        {
             
            file = File.Open(Application.persistentDataPath + "/levels.bytes", FileMode.Create);
        }

        LevelData level;

        Debug.Log("Save levels.levels.Count " + gameData.levels.Count);

        if (gameData.levels.Count >= index + 1)
        {
            level = gameData.levels[index];
            Debug.Log("Save level exists " + level);
        }
        else
        {
            level = new LevelData();
            gameData.levels.Add(level);
            Debug.Log("Save level does not exist " + level);
        }

        Debug.Log("Save maze index  " + index);

        level.data = mazeCode;
        level.index = index;
        level.width = mazeSize.x;
        level.height = mazeSize.y;
        level.hasEnemy = hasEnemy;

        //Debug.Log("Save level " + levels.levels[mazeID - 1].data);

        Debug.Log("Save file " + file);

        try
        {
            bf.Serialize(file, gameData);
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
            return;
        }

        file.Close();

    }

    string GetBoardID(int index)
    {
        switch (currentLevel.index)
        {
            case 0:
                return GPGSlds.leaderboard_level1;
        }

        return "";
    }

    public void SaveTime(float time)
    {
        Debug.Log("SaveTime " + time);

        currentLevel.bestTime = time;

        string boardID = GetBoardID(currentLevel.index);

        Debug.Log("boardID " + boardID);

        PostScore((int)(time * 1000), boardID);

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/levels.bytes", FileMode.Create);

        try
        {
            bf.Serialize(file, gameData);
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
            return;
        }
    }

    public void Load(int index)
    {
        Debug.Log("Load levels.levels.Count " + gameData.levels.Count);

        if (gameData.levels.Count <= 0)
        {
            BinaryFormatter bf = new BinaryFormatter();
            if (Application.isEditor)
            {
                FileStream file = File.Open(Application.dataPath + "/Resources/levels.bytes", FileMode.Open);
                gameData = (GameData)bf.Deserialize(file);
                Debug.Log("Load levels " + gameData);
                file.Close();
            }
            else if (!File.Exists(Application.persistentDataPath + "/levels.bytes"))
            {
                TextAsset assets = Resources.Load<TextAsset>("levels");
                Debug.Log("Load TextAsset " + assets);
                Stream s = new MemoryStream(assets.bytes);
                gameData = bf.Deserialize(s) as GameData;
            }
            else
            {
                FileStream file = File.Open(Application.persistentDataPath + "/levels.bytes", FileMode.Open);
                gameData = (GameData)bf.Deserialize(file);
                file.Close();

                TextAsset assets = Resources.Load<TextAsset>("levels");
                Stream s = new MemoryStream(assets.bytes);
                GameData dataFromAssets = bf.Deserialize(s) as GameData;

                if (gameData.version != dataFromAssets.version)
                {
                    dataFromAssets.authenticated = gameData.authenticated;

                    foreach (LevelData levelFromAssets in dataFromAssets.levels)
                    {
                        levelFromAssets.bestTime = gameData.levels[levelFromAssets.index].bestTime;
                    }

                    gameData = dataFromAssets;
                }
            }
        }

        if (gameData.levels.Count > index)
        {
            Debug.Log("Loaded level " + gameData.levels[index].index);
            if(OnLevelLoaded != null)
            {
                _currentLevelIndex = index;
                OnLevelLoaded(gameData.levels[index]);
            }
        }

        Debug.Log("gameData authenticated " + gameData.authenticated);

        if (gameData.authenticated)
        {
            Authenticate();
        }
    }

    internal void NextLevel()
    {
        var nextLevelIndex = _currentLevelIndex += 1;
        if (gameData.levels.Count > nextLevelIndex)
        {
            currentLevel = gameData.levels[nextLevelIndex];
        }
            
    }

    public void Authenticate()
    {
        Debug.Log("Authenticate");
        Social.localUser.Authenticate((bool success) => {

            Debug.Log("Authenticate succes: " + success);

            if (success)
            {
                authenticated = true;
                gameData.authenticated = true;

                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.persistentDataPath + "/levels.bytes", FileMode.Create);

                try
                {
                    bf.Serialize(file, gameData);
                }
                catch (Exception e)
                {
                    Debug.LogException(e, this);
                    return;
                }
            }
            else
            {
                authenticated = false;
            }
        });
    }

    public void PostScore(int time, string boardID)
    {
        if (!authenticated)
        {
            return;
        }

        Debug.Log("PostScore: " + time + " boardID " + boardID);

        Social.ReportScore(time, boardID, (bool success) => {
            Debug.Log("ReportScore succes: "+ success);
        });
    }

    public void GetLeaderBoard(string leaderBoardID, OnLeaderBoardLoaded LeaderBoardLoaded )
    {
        PlayGamesPlatform.Instance.LoadScores(
            leaderBoardID,
            LeaderboardStart.TopScores,
            5,
            LeaderboardCollection.Public,
            LeaderboardTimeSpan.AllTime,
            (data) =>
            {
                Debug.Log("Leaderboard data valid: " + data.Valid);
                Debug.Log("approx:" + data.ApproximateCount + " have " + data.Scores.Length);
                LeaderBoardLoaded(data);
            });
    }
}

[Serializable]
public class LevelData
{
    public string data;
    public int index;
    public float width;
    public float height;
    public float bestTime = 0f;
    public bool hasEnemy;
}

[Serializable]
public class GameData
{
    [SerializeField]
    public List<LevelData> levels = new List<LevelData>();
    public string version = "0.0.3";
    public bool authenticated = false;
}

[Serializable]
public class Levels : GameData
{}
