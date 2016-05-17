using UnityEngine;
using UnityEngine.SceneManagement;

public class UIControl : MonoBehaviour {

    public RectTransform logoGroup;
    public RectTransform backgroundCanvas;
    public RectTransform playButton;
    public RectTransform intructionsButton;
    public RectTransform quitButton;

    public string levelSelectScene;
    public string instructionsScene;

    private string _nextScene;

    // Use this for initialization
    void Awake()
    {
        instructionsScene = "";
        LeanTween.moveX(playButton, -500f, 0f);
        LeanTween.moveX(intructionsButton, -500f, 0f);
        LeanTween.moveX(quitButton, -500f, 0f);
        LeanTween.moveY(logoGroup, -360f, 0f);
    }

    void Start ()
    {
        Invoke("AnimateIntro", 5f);
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
        
    }

    public void LoadInstructions()
    {
        _nextScene = instructionsScene;
        AnimateOutroButtons();
    }

    public void Quit()
    {
        Debug.Log("QUIT!");
        Application.Quit();
    }
}
