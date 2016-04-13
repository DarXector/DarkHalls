using UnityEngine;
using System.Collections;

public class BreadingLight : MonoBehaviour {

    Light _light;
    Light _halo;

    public float speed = 2f;

    // Use this for initialization
    void Start () {
        _light = GetComponent<Light>();
        _halo = transform.GetChild(0).GetComponent<Light>(); ;
    }
	
	// Update is called once per frame
	void Update () {

        if (!_light)
        {
            return;
        }

        if(!_halo)
        {
            return;
        }

        _light.intensity = 7f + 2f * Mathf.Sin(Time.time * speed);
        _halo.range = 0.29f + 0.03f * Mathf.Sin(Time.time * speed);
    }
}
