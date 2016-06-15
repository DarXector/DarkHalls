﻿using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class EndUIControl : MonoBehaviour
{
    public RectTransform yourTimeLabel;
    public RectTransform yourTimeContainer;

    public RectTransform bestTimeLabel;
    public RectTransform bestTimeContainer;

    public RectTransform leaderBoardButton;
    public RectTransform replayButton;
    public RectTransform nextButton;
    public RectTransform backButton;

    public AudioClip tapSFX;
    private AudioSource _audio;

    public string gameScene;
    public string backScene;

    private string _nextScene;

    // Use this for initialization
    void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    void Start ()
    {
        LeanTween.moveX(yourTimeLabel, -600f, 0f);
        LeanTween.moveX(yourTimeContainer, 600f, 0f);
        LeanTween.moveX(bestTimeLabel, -600f, 0f);
        LeanTween.moveX(bestTimeContainer, 600f, 0f);
        LeanTween.moveX(leaderBoardButton, 600f, 0f);
        LeanTween.moveX(replayButton, -600f, 0f);
        LeanTween.moveX(nextButton, 600f, 0f);
        LeanTween.moveX(backButton, -600f, 0f);

        AnimateIntroButtons();
    }

    void AnimateIntroButtons()
    {
        LeanTween.moveX(yourTimeLabel, 0f, 0.4f).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.moveX(yourTimeContainer, 185f, 0.4f).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.moveX(bestTimeLabel, 0f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.2f);
        LeanTween.moveX(bestTimeContainer, 185f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.2f);
        LeanTween.moveX(leaderBoardButton, 30f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.4f);
        LeanTween.moveX(replayButton, -60f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.6f);
        LeanTween.moveX(nextButton, 30f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.6f);
        LeanTween.moveX(backButton, 0f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.8f);
    }

    void AnimateOutroButtons()
    {
        LeanTween.moveX(yourTimeLabel, -600f, 0.4f).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.moveX(yourTimeContainer, 600f, 0.4f).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.moveX(bestTimeLabel, -600f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.2f);
        LeanTween.moveX(bestTimeContainer, 600f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.2f);
        LeanTween.moveX(leaderBoardButton, 600f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.4f);
        LeanTween.moveX(replayButton, -600f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.6f);
        LeanTween.moveX(nextButton, 600f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.6f);
        LeanTween.moveX(backButton, -600f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.8f).onComplete += ChangeScene;
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(_nextScene);
    }

    public void Next()
    {
        GameModel.Instance.NextLevel();
        _nextScene = gameScene;
        AnimateOutroButtons();
        _audio.PlayOneShot(tapSFX);

    }

    public void Replay()
    {
        _nextScene = gameScene;
        AnimateOutroButtons();
        _audio.PlayOneShot(tapSFX);
    }

    public void Back()
    {
        _nextScene = backScene;
        AnimateOutroButtons();
        _audio.PlayOneShot(tapSFX);
    }
}
