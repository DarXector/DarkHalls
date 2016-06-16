using UnityEngine;
using System.Collections;
using UnityEngine.UI; // include UI namespace so can reference UI elements
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;

public class GameManager : MonoBehaviour
{

    // static reference to game manager so can be called from other scripts directly (not just through gameobject component)
    public static GameManager gm;

    // levels to move to on victory and lose
    public string levelAfterVictory;
    public string levelAfterGameOver;
    public string levelBack;

    // game performance
    [HideInInspector]
    public float time = 0;
    [HideInInspector]
    public float bestTime = 0;

    // UI elements to control
    public Text UITime;
    //public Text UIBestTime;
    //public Text UILevel;
    public GameObject UIGamePaused;
    public GameObject UIOuch;

    private MazeGenerator _mazeGenerator;
    public GameObject maze;

    private AstarPath _astar;
    public GameObject AStarObject;

    [HideInInspector]
    public  GameObject enemy;
    private EnemyAI _enemyAI;

    [HideInInspector]
    public GameObject player;

    [HideInInspector]
    public GameObject screenFader;
    private ScreenFader _screenFader;

    [HideInInspector]
    public bool timerActive;
    [HideInInspector]
    public bool playerCanMove;

    private float _timer;

    public RectTransform backButton;
    public RectTransform timeContainer;
    public RectTransform phaseButton;
    public RectTransform scanButton;

    // set things up here
    void Awake()
    {
        // setup reference to game manager
        if (gm == null)
            gm = this.GetComponent<GameManager>();

        GameModel.Instance.gameObject.GetComponent<AudioSource>().Stop();

        // setup all the variables, the UI, and provide errors if things not setup properly.
        SetupDefaults();
    }

    void Start()
    {
        LeanTween.moveX(backButton, -600f, 0f);
        LeanTween.moveX(timeContainer, 600f, 0f);
        LeanTween.moveX(phaseButton, 600f, 0f);
        LeanTween.moveX(scanButton, 600f, 0f);

        AnimateIntro();
    }

    void AnimateIntro()
    {
        LeanTween.moveX(timeContainer, 0f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.2f);
        LeanTween.moveX(phaseButton, 0f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.4f);
        LeanTween.moveX(scanButton, 0f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.6f);
        LeanTween.moveX(backButton, 0f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.8f);
    }

    // game loop
    void Update()
    {
        // if ESC pressed then pause the game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale > 0f)
            {
                UIGamePaused.SetActive(true); // this brings up the pause UI
                Time.timeScale = 0f; // this pauses the game action
            }
            else
            {
                Time.timeScale = 1f; // this unpauses the game action (ie. back to normal)
                UIGamePaused.SetActive(false); // remove the pause UI
            }
        }

        if (timerActive)
        {
            _timer += Time.deltaTime;

            string minutes = Mathf.Floor(_timer / 60).ToString("00");
            string seconds = Mathf.Floor(_timer % 60).ToString("00");

            UITime.text = minutes + ":" + seconds;
        }
    }

    internal void ShowOuch()
    {
        playerCanMove = false;
        UIOuch.SetActive(true);
        StartCoroutine(HideOuch());
    }

    // load the nextLevel after delay
    IEnumerator HideOuch()
    {
        yield return new WaitForSeconds(1f);
        playerCanMove = true;
        UIOuch.SetActive(false);
    }

    // setup all the variables, the UI, and provide errors if things not setup properly.
    void SetupDefaults()
    {
        Debug.Log("GameManager SetupDefaults");

        // if levels not specified, default to current level
        if (levelAfterVictory == "")
        {
            Debug.LogWarning("levelAfterVictory not specified, defaulted to current level");
            levelAfterVictory = SceneManager.GetActiveScene().name;
        }

        if (levelAfterGameOver == "")
        {
            Debug.LogWarning("levelAfterGameOver not specified, defaulted to current level");
            levelAfterGameOver = SceneManager.GetActiveScene().name;
        }

        if (levelBack == "")
        {
            Debug.LogWarning("levelBack not specified, defaulted to current level");
            levelBack = SceneManager.GetActiveScene().name;
        }

        // friendly error messages
        if (UITime == null)
            Debug.LogError("Need to set UIScore on Game Manager.");

        //if (UIBestTime == null)
        //    Debug.LogError("Need to set UIHighScore on Game Manager.");

        //if (UILevel == null)
        //    Debug.LogError("Need to set UILevel on Game Manager.");

        if (UIGamePaused == null)
            Debug.LogError("Need to set UIGamePaused on Game Manager.");

        if (maze == null)
            Debug.LogError("Need to set maze on Game Manager.");
        else
        {
            Debug.Log("GameManager SetupDefaults maze " + maze);
            _mazeGenerator = maze.GetComponent<MazeGenerator>();
            _mazeGenerator.OnDrawComplete += OnMazeDrawComplete;
        }

        if (AStarObject == null)
            Debug.LogError("Need to set AStarObject on Game Manager.");
        else
            _astar = AStarObject.GetComponent<AstarPath>();

        if (screenFader == null)
            Debug.LogError("Need to set screenFader on Game Manager.");
        else
        {
            _screenFader = screenFader.GetComponent<ScreenFader>();
            _screenFader.FadeFrom(new Color(0.0f, 0.0f, 0.0f, 1.0f));
        }

        // get the UI ready for the game
        RefreshGUI();

        playerCanMove = true;

    }

    void OnMazeDrawComplete()
    {
        Debug.Log("OnMazeDrawComplete");

        enemy = _mazeGenerator.enemy;
        if(enemy)
        {
            enemy.GetComponent<EnemyAI>().OnPlayerCought += OnPlayerCought;
        }

        player = _mazeGenerator.player;

        if (_astar)
        {
            _astar.Scan();
        }
    }

    void OnPlayerCought()
    {
        ResetGame();
    }

    public Transform GetWaypoint()
    {
        return _mazeGenerator.GetRandomNode();
    }

    public void StartScan()
    {
        GetComponent<ScanPowerUp>().StartScan();
        var target = EventSystem.current.currentSelectedGameObject;
        LeanTween.moveX(target, target.transform.position.x + 200f, 0.4f).setEase(LeanTweenType.easeInOutQuad);
    }

    public void StartPhase()
    {
        player.GetComponent<PhasePowerUp>().StartPhase();
        var target = EventSystem.current.currentSelectedGameObject;
        LeanTween.moveX(target, target.transform.position.x + 200f, 0.4f).setEase(LeanTweenType.easeInOutQuad);
    }

    // refresh all the GUI elements
    void RefreshGUI()
    {
        timerActive = false;

        if (!UITime)
        {
            return;
        }
        // set the text elements of the UI
        UITime.text = "00:00";
    }

    void AnimateOut()
    {
        LeanTween.moveX(timeContainer, 600f, 0.4f).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.moveX(phaseButton, 600f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.2f);
        LeanTween.moveX(scanButton, 600f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.4f);
        LeanTween.moveX(backButton, -600f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.6f);
    }

    // public function to remove player life and reset game accordingly
    public void ResetGame()
    {
        playerCanMove = false;
        timerActive = false;

        AnimateOut();

        _screenFader.FadeTo(new Color(0.0f, 0.0f, 0.0f, 1.0f));
        StartCoroutine(LoadSameLevel());
    }

    public void Back()
    {
        playerCanMove = false;
        timerActive = false;

        AnimateOut();

        _screenFader.FadeTo(new Color(0.0f, 0.0f, 0.0f, 1.0f));
        StartCoroutine(LoadLevelBack());

        GameModel.Instance.gameObject.GetComponent<AudioSource>().Play();
    }

    public void LevelCompete()
    {
        playerCanMove = false;
        timerActive = false;

        AnimateOut();

        var model = GameModel.Instance;

        if (_timer < model.currentLevel.bestTime || model.currentLevel.bestTime == 0f)
        {
            GameModel.Instance.SaveTime(_timer);
        }

        _screenFader.FadeTo(new Color(1.0f, 1.0f, 1.0f, 1.0f));
        StartCoroutine(LoadNextScene());

        GameModel.Instance.gameObject.GetComponent<AudioSource>().Play();
    }

    // load the nextLevel after delay
    IEnumerator LoadNextScene()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(levelAfterVictory);
    }

    IEnumerator LoadSameLevel()
    {
        yield return new WaitForSeconds(2);
        RefreshGUI();
        SceneManager.LoadScene(levelAfterGameOver);
    }

    IEnumerator LoadLevelBack()
    {
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(levelBack);
    }

    void OnDestroy()
    {
        if(_mazeGenerator)
        {
            _mazeGenerator.OnDrawComplete -= OnMazeDrawComplete;
        }

        if(enemy)
        {
            enemy.GetComponent<EnemyAI>().OnPlayerCought -= OnPlayerCought;
        }
    }
}
