using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class StartUIControl : MonoBehaviour {

    public RectTransform logoGroup;
    public RectTransform playButton;
    public RectTransform intructionsButton;
    public RectTransform quitButton;
    public RectTransform creditsButton;
    public RectTransform authenticateButton;

    public AudioClip tapSFX;
    private AudioSource _audio;

    public string levelSelectScene;
    public string instructionsScene;
    public string creditsScene;

    private string _nextScene;

    // Use this for initialization
    void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    void Start ()
    {
        LeanTween.moveX(playButton, -600f, 0f);
        LeanTween.moveX(intructionsButton, -600f, 0f);
        LeanTween.moveX(quitButton, -600f, 0f);
        LeanTween.moveX(authenticateButton, -600f, 0f);
        LeanTween.moveX(creditsButton, -600f, 0f);

        if (GameModel.Instance.navigated)
        {
            LeanTween.moveY(logoGroup, 0, 0);
            AnimateIntroButtons();
        }
        else
        {
            LeanTween.moveY(logoGroup, -460, 0f);
            Invoke("AnimateIntro", 4f);
        }
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
    }

    void AnimateIntro()
    {
        LeanTween.moveY(logoGroup, 0, 0.4f).setEase(LeanTweenType.easeInOutQuad).onComplete += AnimateIntroButtons;
    }

    void AnimateIntroButtons()
    {
        LeanTween.moveX(playButton, 0f, 0.4f).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.moveX(intructionsButton, 0f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.2f);
        LeanTween.moveX(creditsButton, 0f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.4f);
        LeanTween.moveX(quitButton, 0f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.6f);

        if (!GameModel.Instance.gameData.authenticated)
        {
            LeanTween.moveX(authenticateButton, 0f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.8f);
        }
        
    }

    void AnimateOutroButtons()
    {
        LeanTween.moveX(playButton, -600f, 0.4f).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.moveX(intructionsButton, -600f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.2f);
        LeanTween.moveX(creditsButton, -600f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.4f);
        LeanTween.moveX(quitButton, -600f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.6f);
        LeanTween.moveX(authenticateButton, -600f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.8f).onComplete += ChangeScene;
    }

    public void Authenticate()
    {
        _audio.PlayOneShot(tapSFX);
        LeanTween.moveX(authenticateButton, -600f, 0.4f).setEase(LeanTweenType.easeInOutQuad);
        GameModel.Instance.Authenticate();
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(_nextScene);
    }

    public void LoadLevelSelect()
    {
        _audio.PlayOneShot(tapSFX);
        _nextScene = levelSelectScene;
        AnimateOutroButtons();
    }

    public void LoadInstructions()
    {
        _audio.PlayOneShot(tapSFX);
        _nextScene = instructionsScene;
        AnimateOutroButtons();
    }

    public void LoadCredits()
    {
        _audio.PlayOneShot(tapSFX);
        _nextScene = creditsScene;
        AnimateOutroButtons();
    }

    public void Quit()
    {
        _audio.PlayOneShot(tapSFX);
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
