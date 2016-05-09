using UnityEngine;
using System.Collections;
using UnityEngine.UI; // include UI namespace so can reference UI elements
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    // static reference to game manager so can be called from other scripts directly (not just through gameobject component)
    public static GameManager gm;

    // levels to move to on victory and lose
    public string levelAfterVictory;
    public string levelAfterGameOver;

    // game performance
    public float time = 0;
    public float bestTime = 0;

    // UI elements to control
    public Text UITime;
    public Text UIBestTime;
    public Text UILevel;
    public GameObject UIGamePaused;

    private MazeGenerator _mazeGenerator;
    public GameObject maze;

    private AstarPath _astar;
    public GameObject AStarObject;

    public GameObject enemy;
    private EnemyAI _enemyAI;

    // set things up here
    void Awake() {
        // setup reference to game manager
        if (gm == null)
            gm = this.GetComponent<GameManager>();

        // setup all the variables, the UI, and provide errors if things not setup properly.
        SetupDefaults();
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

        //if(_player.GetComponent<CharacterController2D>().playerCanMove)
        //{ // game playing state, so update the timer
        //    currentTime -= Time.deltaTime;
        //    mainTimerDisplay.text = currentTime.ToString("0.00");
        //}
    }

    // setup all the variables, the UI, and provide errors if things not setup properly.
    void SetupDefaults()
    {

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

        // friendly error messages
        if (UITime == null)
            Debug.LogError("Need to set UIScore on Game Manager.");

        if (UIBestTime == null)
            Debug.LogError("Need to set UIHighScore on Game Manager.");

        if (UILevel == null)
            Debug.LogError("Need to set UILevel on Game Manager.");

        if (UIGamePaused == null)
            Debug.LogError("Need to set UIGamePaused on Game Manager.");

        if (maze == null)
            Debug.LogError("Need to set maze on Game Manager.");
        else
        {
            _mazeGenerator = maze.GetComponent<MazeGenerator>();
            _mazeGenerator.OnDrawComplete += OnMazeDrawComplete;
        }

        if (AStarObject == null)
            Debug.LogError("Need to set AStarObject on Game Manager.");
        else
            _astar = AStarObject.GetComponent<AstarPath>();

        // get stored player prefs
        RefreshPlayerState();

        // get the UI ready for the game
        RefreshGUI();
    }

    void OnMazeDrawComplete()
    {
        if(_astar)
        {
            _astar.Scan();
        }

        if(enemy)
        {
            _enemyAI = enemy.GetComponent<EnemyAI>();
            _enemyAI.OnPlayerCought += OnPlayerCought;
        }
        else
        {
            Debug.LogError("Need to set enemy on Game Manager.");
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

    // get stored Player Prefs if they exist, otherwise go with defaults set on gameObject
    void RefreshPlayerState()
    {
		time = PlayerPrefManager.GetTime();
		bestTime = PlayerPrefManager.GetBestTime();

		// save that this level has been accessed so the MainMenu can enable it
		PlayerPrefManager.UnlockLevel();
	}

	// refresh all the GUI elements
	void RefreshGUI()
    {
        if(!UITime || !UIBestTime || !UILevel)
        {
            return;
        }
        // set the text elements of the UI
        UITime.text = "Time: " + time.ToString();
        UIBestTime.text = "Best Time: " + bestTime.ToString();
		UILevel.text = SceneManager.GetActiveScene().name;
	}

    // public function to remove player life and reset game accordingly
    public void ResetGame()
    {
        // remove life and update GUI
        RefreshGUI();

        SceneManager.LoadScene(levelAfterGameOver);
    }

	// public function for level complete
	public void LevelCompete()
    {
		// save the current player prefs before moving to the next level
		PlayerPrefManager.SavePlayerState(time,bestTime);

		// use a coroutine to allow the player to get fanfare before moving to next level
		StartCoroutine(LoadNextLevel());
	}

	// load the nextLevel after delay
	IEnumerator LoadNextLevel()
    {
		yield return new WaitForSeconds(3.5f);
        SceneManager.LoadScene(levelAfterVictory);
	}
}
