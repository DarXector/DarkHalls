using UnityEngine;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class InstructionsUIControl : MonoBehaviour {

    public String backScene;

    public AudioClip tapSFX;
    private AudioSource _audio;

    public RectTransform instructions;
    public RectTransform backButton;

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
    }

    public void GoBack()
    {
        _audio.PlayOneShot(tapSFX);

        LeanTween.moveX(instructions, -800f, 0.4f).setEase(LeanTweenType.easeInOutQuad);
        LeanTween.moveX(backButton, -600f, 0.4f).setEase(LeanTweenType.easeInOutQuad).setDelay(0.2f).onComplete += ChangeScene;

        
    }

    void ChangeScene()
    {
        SceneManager.LoadScene(backScene);
    }
}
