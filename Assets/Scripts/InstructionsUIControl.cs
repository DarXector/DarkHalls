using UnityEngine;
using System;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class InstructionsUIControl : MonoBehaviour {

    public String backScene;

    public AudioClip tapSFX;
    private AudioSource _audio;

    void Awake()
    {
        _audio = GetComponent<AudioSource>();
    }

    void Start()
    {
        GameModel.Instance.navigated = true;
    }

    public void GoBack()
    {
        _audio.PlayOneShot(tapSFX);
        SceneManager.LoadScene(backScene);
    }
}
