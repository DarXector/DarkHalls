using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour {

    private Image _image;
    private Color _color;

    public float duration = 1f;

    // Use this for initialization
    void Awake ()
    {
        _image = GetComponent<Image>();
    }
	
	public void FadeTo(Color color)
    {
        _color = color;
        _color.a = 0f;
        gameObject.SetActive(true);
        LeanTween.value(gameObject, SetAlpha, 0f, 1f, duration);
    }

    public void FadeFrom(Color color)
    {
        _color = color;
        _color.a = 1f;
        gameObject.SetActive(true);
        LeanTween.value(gameObject, SetAlpha, 1f, 0f, duration).onComplete += OnAnimationComplete;
    }

    void SetAlpha(float val)
    {
        _color.a = val;
        _image.color = _color;
    }

    void OnAnimationComplete()
    {
        gameObject.SetActive(false);
    }
}
