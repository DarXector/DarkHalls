using UnityEngine;
using System.Collections;
using MaterialCache = System.Collections.Generic.Dictionary<UnityEngine.Material, UnityEngine.Material>;

public class FadeInOut : MonoBehaviour
{

    // publically editable speed
    public float fadeDelay = 0.0f;
    public float fadeTime = 0.5f;
    public float fadeoutDelay = 0.2f;
    public bool fadeInOnStart = false;
    public bool fadeOutOnStart = false;
    private bool logInitialFadeSequence = false;

    MaterialCache _cache;

    // store colours
    private Color[] colors;

    // allow automatic fading on the start of the scene

    void Awake()
    {
        _cache = new MaterialCache();
    }

    IEnumerator Start()
    {
        //yield return null; 
        yield return new WaitForSeconds(fadeDelay);

        if (fadeInOnStart)
        {
            logInitialFadeSequence = true;
            FadeIn();
        }

        if (fadeOutOnStart)
        {
            FadeOut(fadeTime);
        }
    }

    // check the alpha value of most opaque object
    float MaxStrength()
    {
        float maxStrength = 0.0f;
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            if (renderer.sharedMaterial.HasProperty("_MKGlowTexStrength"))
            {
                Material adjusted;
                if (_cache.TryGetValue(renderer.sharedMaterial, out adjusted))
                {
                    maxStrength = Mathf.Max(maxStrength, adjusted.GetFloat("_MKGlowTexStrength"));
                }
                else
                {
                    // Not cached yet, instantiate a new material from this renderer and cache it
                    // for later reuse.
                    adjusted = renderer.material;
                    maxStrength = Mathf.Max(maxStrength, adjusted.GetFloat("_MKGlowTexStrength"));
                    _cache.Add(renderer.sharedMaterial, adjusted);
                }
            }
        }
        return maxStrength;
    }

    // fade sequence
    IEnumerator FadeSequence(float fadingOutTime)
    {
        // log fading direction, then precalculate fading speed as a multiplier
        bool fadingOut = (fadingOutTime < 0.0f);
        float fadingOutSpeed = 1.0f / fadingOutTime;

        Renderer[] renderers = gameObject.GetComponents<Renderer>();

        // make all objects visible
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].enabled = true;
        }


        float strength = MaxStrength();


        // This is a special case for objects that are set to fade in on start. 
        // it will treat them as alpha 0, despite them not being so. 
        if (logInitialFadeSequence && !fadingOut)
        {
            strength = 0.0f;
            logInitialFadeSequence = false;
        }

        while ((strength >= 0.0f && fadingOut) || (strength <= 10.0f && !fadingOut))
        {
            strength += Time.deltaTime * fadingOutSpeed;

            Mathf.Clamp(strength, 0f, 10f);

            foreach (Renderer renderer in renderers)
            {
                if (renderer.sharedMaterial.HasProperty("_MKGlowTexStrength"))
                {
                    Material adjusted;
                    if (_cache.TryGetValue(renderer.sharedMaterial, out adjusted))
                    {
                        // The adjusted material is already available, write this instance.
                        adjusted.SetFloat("_MKGlowTexStrength", strength);
                        adjusted.SetFloat("_MKGlowPower", strength / 5f);
                        renderer.material = adjusted;
                    }
                    else
                    {
                        // Not cached yet, instantiate a new material from this renderer and cache it
                        // for later reuse.
                        adjusted = renderer.material;
                        adjusted.SetFloat("_MKGlowTexStrength", strength);
                        adjusted.SetFloat("_MKGlowPower", strength / 5f);
                        _cache.Add(renderer.sharedMaterial, adjusted);
                    }
                }
            }

            yield return null;
        }

        // turn objects off after fading out
        if (fadingOut)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].enabled = false;
            }
        }

        //Debug.Log("fade sequence end : " + fadingOut);
    }


    public void FadeIn()
    {
        FadeIn(fadeTime);
        Invoke("FadeOut", fadeoutDelay);
    }

    public void FadeOut()
    {
        FadeOut(fadeTime);
    }

    void FadeIn(float newFadeTime)
    {
        StopAllCoroutines();
        StartCoroutine("FadeSequence", newFadeTime);
    }

    void FadeOut(float newFadeTime)
    {
        StopAllCoroutines();
        StartCoroutine("FadeSequence", -newFadeTime);
    }
}