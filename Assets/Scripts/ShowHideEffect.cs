using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShowHideEffect : MonoBehaviour {

    public List<GameObject> lights;
    public List<GameObject> particles;

	public void Show()
    {
        foreach(var light in lights)
        {
            light.SetActive(true);
        }

        foreach (var particle in particles)
        {
            particle.SetActive(true);
            particle.GetComponent<ParticleSystem>().Play();
        }
    }

    public void Hide()
    {
        foreach (var light in lights)
        {
            light.SetActive(false);
        }

        foreach (var particle in particles)
        {
            particle.GetComponent<ParticleSystem>().Stop();
        }
    }
}
