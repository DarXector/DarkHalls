using UnityEngine;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class InstructionsUIControl : MonoBehaviour {

    public String backScene;
    public String gameScene;

    private String _nextScene;

    public AudioClip tapSFX;
    private AudioSource _audio;

    public RectTransform instructions;
    public RectTransform backButton;
    public GameObject playButton;

    void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    void Start()
    {
        GameModel.Instance.navigated = true;

        LeanTween.moveX(instructions, -800f, 0f);
        LeanTween.moveX(backButton, -600f, 0f);

        LeanTween.moveX(instructions, 0f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.2f);
        LeanTween.moveX(backButton, 0f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.6f);

        if(GameModel.Instance.instructionsBeforePlay)
        {
            playButton.SetActive(true);
        }

        if(!GameModel.Instance.gameData.seenInstructions)
        {
            GameModel.Instance.SetSeenInstructions();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoBack();
        }
    }

    void AnimateOutro()
    {
        LeanTween.moveX(instructions, -800f, 0.4f).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.moveX(backButton, -600f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.2f).onComplete += ChangeScene;
    }

    public void Play()
    {
        _audio.PlayOneShot(tapSFX);
        _nextScene = gameScene;
        AnimateOutro();
    }

    public void GoBack()
    {
        _audio.PlayOneShot(tapSFX);
        _nextScene = backScene;
        AnimateOutro();
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(_nextScene);
    }
}
