using UnityEngine;
using System.Collections;

public class PhasePowerUp : MonoBehaviour
{

    public GameObject phaseEffect;

    private GameObject _phaseEffectInstance;
    private AudioSource _phaseSFX;

    void OnTriggerExit(Collider other)
    {
        GetComponent<Collider>().isTrigger = false;

        if(_phaseEffectInstance)
        {
            var from = _phaseSFX.volume;
            var to = 0;
            var time = 0.5f;

            LeanTween.value(gameObject, _updateSoundVolume, from, to, time).onComplete = _OnAudioFadeComplete;

            _phaseEffectInstance.GetComponent<ShowHideEffect>().Hide();
        }
    }

    private void _updateSoundVolume(float value)
    {
        if (_phaseSFX)
        {
            _phaseSFX.volume = value;
        }
    }

    private void _OnAudioFadeComplete()
    {
        if (_phaseEffectInstance)
        {
            Destroy(_phaseEffectInstance);
            _phaseEffectInstance = null;
        }
    }

    public void StartPhase()
    {
        GetComponent<Collider>().isTrigger = true;
        _phaseEffectInstance = (GameObject)Instantiate(phaseEffect, transform.position, Quaternion.identity);
        _phaseEffectInstance.transform.parent = transform;
        _phaseSFX = _phaseEffectInstance.GetComponent<AudioSource>();
    }
}
