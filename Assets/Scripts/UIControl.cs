using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class UIControl : MonoBehaviour {

    public RectTransform logoGroup;
    public RectTransform backgroundCanvas;
    public RectTransform playButton;
    public RectTransform intructionsButton;
    public RectTransform quitButton;

    public AudioClip tapSFX;
    private AudioSource _audio;

    public string levelSelectScene;
    public string instructionsScene;

    private string _nextScene;

    // Use this for initialization
    void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    void Start ()
    {
        LeanTween.moveX(playButton, -500f, 0f);
        LeanTween.moveX(intructionsButton, -500f, 0f);
        LeanTween.moveX(quitButton, -500f, 0f);

        if (GameModel.Instance.navigated)
        {
            LeanTween.moveY(logoGroup, 0, 0);
            AnimateIntroButtons();
        }
        else
        {
            LeanTween.moveY(logoGroup, -Screen.height / 2, 0f);
            Invoke("AnimateIntro", 4f);
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
        LeanTween.moveX(quitButton, 0f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.4f);
    }

    void AnimateOutroButtons()
    {
        LeanTween.moveX(playButton, -500f, 0.4f).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.moveX(intructionsButton, -500f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.2f);
        LeanTween.moveX(quitButton, -500f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.4f).onComplete += ChangeScene;
    }

    void ChangeScene()
    {
        
        SceneManager.LoadScene(_nextScene);
    }

    public void LoadLevelSelect()
    {
        _nextScene = levelSelectScene;
        AnimateOutroButtons();
        _audio.PlayOneShot(tapSFX);

    }

    public void LoadInstructions()
    {
        Debug.Log("Load " + instructionsScene);
        _nextScene = instructionsScene;
        AnimateOutroButtons();
        _audio.PlayOneShot(tapSFX);
    }

    public void Quit()
    {
        _audio.PlayOneShot(tapSFX);
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
