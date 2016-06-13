using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

public class GameModel : Singleton<GameModel>
{
    protected GameModel() { } // guarantee this will be always a singleton only - can't use the constructor!

    public Levels levelsData = new Levels();
    internal LevelData currentLevel;

    public delegate void OnLevelLoadedEvent(LevelData level);
    public event OnLevelLoadedEvent OnLevelLoaded;

    public bool navigated = false;
    private int _currentLevelIndex;

    public void Start()
    {
        levelsData = new Levels();

        Load(0);
        currentLevel = levelsData.levels[0];
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

        Debug.Log("Save levels.levels.Count " + levelsData.levels.Count);

        if (levelsData.levels.Count >= index + 1)
        {
            level = levelsData.levels[index];
            Debug.Log("Save level exists " + level);
        }
        else
        {
            level = new LevelData();
            levelsData.levels.Add(level);
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
            bf.Serialize(file, levelsData);
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
            return;
        }

        file.Close();

    }

    public void SaveTime(float time)
    {
        Debug.Log("SaveTime " + time);

        currentLevel.bestTime = time;

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/levels.bytes", FileMode.Create);

        try
        {
            bf.Serialize(file, levelsData);
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
            return;
        }
    }

    public void Load(int index)
    {
        Debug.Log("Load levels.levels.Count " + levelsData.levels.Count);

        if (levelsData.levels.Count <= 0)
        {
            BinaryFormatter bf = new BinaryFormatter();
            if (Application.isEditor)
            {
                FileStream file = File.Open(Application.dataPath + "/Resources/levels.bytes", FileMode.Open);
                levelsData = (Levels)bf.Deserialize(file);
                Debug.Log("Load levels " + levelsData);
                file.Close();
            }
            else if (!File.Exists(Application.persistentDataPath + "/levels.bytes"))
            {
                TextAsset assets = Resources.Load<TextAsset>("levels");
                Debug.Log("Load TextAsset " + assets);
                Stream s = new MemoryStream(assets.bytes);
                levelsData = bf.Deserialize(s) as Levels;
            }
            else
            {
                FileStream file = File.Open(Application.persistentDataPath + "/levels.bytes", FileMode.Open);
                levelsData = (Levels)bf.Deserialize(file);
                file.Close();

                TextAsset assets = Resources.Load<TextAsset>("levels");
                Stream s = new MemoryStream(assets.bytes);
                Levels levelsFromAssets = bf.Deserialize(s) as Levels;

                if (levelsData.version != levelsFromAssets.version)
                {
                    foreach (LevelData levelFromAssets in levelsFromAssets.levels)
                    {
                        levelFromAssets.bestTime = levelsData.levels[levelFromAssets.index].bestTime;
                    }

                    levelsData = levelsFromAssets;
                }
            }
        }

        if (levelsData.levels.Count > index)
        {
            Debug.Log("Loaded level " + levelsData.levels[index].index);
            if(OnLevelLoaded != null)
            {
                _currentLevelIndex = index;
                OnLevelLoaded(levelsData.levels[index]);
            }
        }
    }

    internal void NextLevel()
    {
        var nextLevelIndex = _currentLevelIndex += 1;
        if (levelsData.levels.Count > nextLevelIndex)
        {
            currentLevel = levelsData.levels[nextLevelIndex];
        }
            
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
public class Levels
{
    [SerializeField]
    public List<LevelData> levels = new List<LevelData>();
    public string version = "0.0.2";
}

