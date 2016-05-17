using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

public class GameModel : Singleton<GameModel>
{
    protected GameModel() { } // guarantee this will be always a singleton only - can't use the constructor!

    public Levels levels = new Levels();
    public FileStream file;
    internal LevelData currentLevel;

    public delegate void OnLevelLoadedEvent(LevelData level);
    public event OnLevelLoadedEvent OnLevelLoaded;

    public void Awake()
    {
        Load(0);
        currentLevel = levels.levels[0];
    }

    public void Save(int mazeID, string mazeCode, Vector2 mazeSize)
    {
        Debug.Log("Save mazeID " + mazeID);
        Debug.Log("Save mazeCode " + mazeCode);

        if (mazeCode == "")
        {
            return;
        }

        if (mazeID <= 0)
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

        Debug.Log("Save levels.levels.Count " + levels.levels.Count);

        if (levels.levels.Count > mazeID)
        {
            level = levels.levels[mazeID - 1];
            Debug.Log("Save level exists " + level);
        }
        else
        {
            level = new LevelData();
            levels.levels.Add(level);
            Debug.Log("Save level does not exist " + level);
        }

        level.data = mazeCode;
        level.level = mazeID.ToString();
        level.width = mazeSize.x.ToString();
        level.height = mazeSize.y.ToString();

        //Debug.Log("Save level " + levels.levels[mazeID - 1].data);

        Debug.Log("Save file " + file);

        try
        {
            bf.Serialize(file, levels);
        }
        catch (Exception e)
        {
            Debug.LogException(e, this);
            return;
        }

        file.Close();
    }

    public void Load(int levelNumber)
    {
        if(levels.levels.Count <= 0)
        {
            BinaryFormatter bf = new BinaryFormatter();
            if (Application.isEditor)
            {
                FileStream file = File.Open(Application.dataPath + "/Resources/levels.bytes", FileMode.Open);
                levels = (Levels)bf.Deserialize(file);
                file.Close();
            }
            else if (!File.Exists(Application.persistentDataPath + "/levels.bytes"))
            {
                TextAsset assets = Resources.Load<TextAsset>("levels");
                Debug.Log("TextAsset " + assets);
                Stream s = new MemoryStream(assets.bytes);
                levels = bf.Deserialize(s) as Levels;
            }
            else
            {
                FileStream file = File.Open(Application.persistentDataPath + "/levels.bytes", FileMode.Open);
                levels = (Levels)bf.Deserialize(file);
                file.Close();
            }
        }

        if (levelNumber > 0 && levels.levels.Count > levelNumber)
        {
            Debug.Log("Loaded level " + levels.levels[0].level);
            if(OnLevelLoaded != null)
            {
                OnLevelLoaded(levels.levels[levelNumber - 1]);
            }
        }
    }
}

[Serializable]
public class LevelData
{
    public string data;
    public string level;
    public string width;
    public string height;
    public int bestTime = 0;
}

[Serializable]
public class Levels
{
    [SerializeField]
    public List<LevelData> levels = new List<LevelData>();
}

