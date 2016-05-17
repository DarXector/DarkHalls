using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;

public class MazeGenerator : MonoBehaviour
{

    public Vector2 mazeSize = new Vector2(4, 6);
    public Vector2 mazeStart = new Vector2(0, 1);
    public float tileSize = 0.5f;
    public GameObject wallPrefab;
    public GameObject playerPrefab;
    public GameObject endPrefab;
    public GameObject glowFloorPrefab;
    public GameObject enemyPrefab;
    public GameObject navigationNodes;
    public GameObject navigationNode;

    [HideInInspector]
    public List<GameObject> glowTiles;
    [HideInInspector]
    public GameObject player;
    [HideInInspector]
    public GameObject enemy;

    public string mazeCode;
    public int mazeID = 1;

    protected const char NORTH = 'N';
    protected const char SOUTH = 'S';
    protected const char EAST = 'E';
    protected const char WEST = 'W';

    protected int[,] _maze;
    protected List<int> _moves;
    protected int _width;
    protected int _height;

    // Event Handler
    public delegate void OnDrawCompleteEvent();
    public event OnDrawCompleteEvent OnDrawComplete;

    // Use this for initialization
    void Start()
    {
        GameModel.Instance.OnLevelLoaded += Parse;
        GameModel.Instance.Load(mazeID);
    }

    protected void ResetMaze()
    {
        mazeCode = "";

        while (transform.childCount != 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        while (navigationNodes.transform.childCount != 0)
        {
            DestroyImmediate(navigationNodes.transform.GetChild(0).gameObject);
        }

        glowTiles = new List<GameObject>();
    }

    public void Generate()
    {
        ResetMaze();

        _width = (int)mazeSize.x * 2 + 1;
        _height = (int)mazeSize.y * 2 + 1;

        InitData();
        CreateData();
        DrawMaze();
    }

    void InitData()
    {
        _maze = new int[_width, _height];
        glowTiles = new List<GameObject>();

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                _maze[x, y] = 1;
            }
        }

        _maze[(int)mazeStart.x, (int)mazeStart.y] = 2;
        _maze[UnityEngine.Random.Range(1, _width - 1), _height - 1] = 3;
    }

    void CreateData()
    {
        int back;
        int move;
        String possibleDirections;
        Vector2 pos = mazeStart;

        _moves = new List<int>();
        _moves.Add((int)(pos.x + (pos.y * _width)));

        while (_moves.Count > 0)
        {
            possibleDirections = "";

            if ((pos.y + 2 < _height) && (_maze[(int)pos.x, (int)pos.y + 2] == 1) && (pos.y + 2 != 0) && (pos.y + 2 != _height - 1))
            {
                possibleDirections += NORTH;
            }

            if ((pos.y - 2 >= 0) && (_maze[(int)pos.x, (int)pos.y - 2] == 1) && (pos.y - 2 != 0) && (pos.y - 2 != _height - 1))
            {
                possibleDirections += SOUTH;
            }

            if ((pos.x - 2 >= 0) && (_maze[(int)pos.x - 2, (int)pos.y] == 1) && (pos.x - 2 != 0) && (pos.x - 2 != _width - 1))
            {
                possibleDirections += WEST;
            }

            if ((pos.x + 2 < _width) && (_maze[(int)pos.x + 2, (int)pos.y] == 1) && (pos.x + 2 != 0) && (pos.x + 2 != _width - 1))
            {
                possibleDirections += EAST;
            }

            //Debug.Log("possibleDirections: " + possibleDirections);

            if (possibleDirections.Length > 0)
            {
                move = UnityEngine.Random.Range(0, possibleDirections.Length);

                //Debug.Log("move: " + move);
                //Debug.Log("move: " + possibleDirections[move]);

                switch (possibleDirections[move])
                {
                    case SOUTH:
                        _maze[(int)pos.x, (int)pos.y - 2] = 0;
                        _maze[(int)pos.x, (int)pos.y - 1] = 0;
                        pos.y -= 2;
                        break;

                    case NORTH:
                        _maze[(int)pos.x, (int)pos.y + 2] = 0;
                        _maze[(int)pos.x, (int)pos.y + 1] = 0;
                        pos.y += 2;
                        break;

                    case WEST:
                        _maze[(int)pos.x - 2, (int)pos.y] = 0;
                        _maze[(int)pos.x - 1, (int)pos.y] = 0;
                        pos.x -= 2;
                        break;

                    case EAST:
                        _maze[(int)pos.x + 2, (int)pos.y] = 0;
                        _maze[(int)pos.x + 1, (int)pos.y] = 0;
                        pos.x += 2;
                        break;
                }

                _moves.Add((int)(pos.x + (pos.y * _width)));
            }
            else
            {
                back = _moves[_moves.Count - 1];
                pos.x = back % _width;
                pos.y = Mathf.Round(back / _width);

                _moves.Remove(back);
            }
        }
    }

    protected void DrawMaze()
    {
        GameObject w = null;
        float tileWidth = 0;

        for (int y = 0; y < _height; y++)
        {
            w = null;
            tileWidth = 0;

            for (int x = 0; x < _width; x++)
            {
                mazeCode += _maze[x, y];

                switch(_maze[x,y])
                {
                    case 1:
                        tileWidth = SpawnWall(x, y, w, tileWidth);
                        break;
                    case 2:
                        SpawnPlayer(x, y);
                        break;
                    case 3:
                        SpawnEnd(x, y);
                        break;
                }

                if(_maze[x, y] != 1)
                {
                    w = null;
                    SpawnCorridor(x, y);
                }
            }
        }

        SpawnEnemy();

        OnDrawComplete();
    }

    private void SpawnCorridor(int x, int y)
    {
        var t = (GameObject)Instantiate(glowFloorPrefab, new Vector3(x * tileSize, -tileSize / 2f, y * tileSize), Quaternion.identity);
        t.transform.parent = gameObject.transform;
        t.GetComponent<Renderer>().enabled = false;
        glowTiles.Add(t);

        var n = (GameObject)Instantiate(navigationNode, new Vector3(x * tileSize, 0, y * tileSize), Quaternion.identity);
        n.transform.parent = navigationNodes.transform;
    }

    private void SpawnEnd(int x, int y)
    {
        var t = (GameObject)Instantiate(endPrefab, new Vector3(x * tileSize, 0, y * tileSize), Quaternion.identity);
        t.transform.parent = gameObject.transform;
    }

    private void SpawnPlayer(int x, int y)
    {
        player = (GameObject)Instantiate(playerPrefab, new Vector3(x * tileSize, 0, y * tileSize), Quaternion.identity);
        player.transform.parent = gameObject.transform;
    }

    float SpawnWall(int x, int y, GameObject w, float tileWidth)
    {
        if (!w)
        {
            w = (GameObject)Instantiate(wallPrefab, new Vector3(x * tileSize, 0, y * tileSize), Quaternion.identity);
            w.transform.parent = gameObject.transform;
            tileWidth = tileSize;
        }
        else if (w)
        {
            tileWidth += tileSize;
            w.transform.position = new Vector3(x * tileSize - (tileWidth - tileSize) / 2f, 0, y * tileSize);
            w.transform.localScale = new Vector3(tileWidth, tileSize, tileSize);
        }

        return tileWidth;
    }

    void SpawnEnemy()
    {
        var position = GetRandomNode().position;
        enemy = (GameObject)Instantiate(enemyPrefab, new Vector3(position.x, position.y, position.z), Quaternion.identity);
        enemy.transform.parent = gameObject.transform;
        GameManager.gm.enemy = enemy;
    }

    public Transform GetRandomNode()
    {
        return navigationNodes.transform.GetChild(UnityEngine.Random.Range(0, navigationNodes.transform.childCount)).transform;
    }

    public void Parse(LevelData level)
    {
        Debug.Log("Parse " + level);

        ResetMaze();

        mazeSize = new Vector2(int.Parse(level.width), int.Parse(level.height));

        _width = (int)mazeSize.x * 2 + 1;
        _height = (int)mazeSize.y * 2 + 1;

        _maze = new int[_width, _height];

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                _maze[x, y] = int.Parse(level.data[y * _width + x].ToString());


                if (_maze[x, y] == 2)
                {
                    mazeStart = new Vector2(x, y);
                }
            }
        }

        DrawMaze();
    }
}