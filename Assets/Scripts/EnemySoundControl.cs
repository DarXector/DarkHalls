using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class EnemySoundControl : MonoBehaviour {

    public AudioClip continuousGrowl;
    public AudioClip suddenRoar;

    private AudioSource _audioSource;

    public delegate void AudioCallback();

    public float roarTimeout = 5f;
    private float _timeAfterRoar = 0f;

    // Use this for initialization
    void Awake () {

        _audioSource = GetComponent<AudioSource>();

        _audioSource.clip = continuousGrowl;
        _audioSource.loop = true;
        _audioSource.playOnAwake = true;

    }

    void Update()
    {
        _timeAfterRoar += Time.deltaTime;
    }

    void ReturnToLoop()
    {
        _audioSource.clip = continuousGrowl;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    public void PlayRoar()
    {
        if(_timeAfterRoar <= roarTimeout)
        {
            return;
        }

        _timeAfterRoar = 0f;

        _audioSource.PlayOneShot(suddenRoar);
        StartCoroutine(DelayedCallback(suddenRoar.length, ReturnToLoop));
    }
    private IEnumerator DelayedCallback(float time, AudioCallback callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }
}
