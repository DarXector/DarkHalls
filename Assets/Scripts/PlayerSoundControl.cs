using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class PlayerSoundControl : MonoBehaviour {

    public AudioClip[] ouchSounds;

    private AudioSource _audioSource;
    // Use this for initialization
    void Awake () {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayOuch()
    {
        var random = Random.Range(0, ouchSounds.Length);

        _audioSource.PlayOneShot(ouchSounds[random]);
    }
}
