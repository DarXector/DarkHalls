using UnityEngine;
using System.Collections;

public class BreadingLight : MonoBehaviour {

    Light _light;
    Light _halo;

    public float speed = 2f;

    private float _baseHaloRange;
    private float _baseIntensity;
    private float _baseLightRange;

    public float lightIntensityModifier = 0.5f;
    public float haloRangeModifier = 0.03f;

    private GameObject _enemy;

    // Use this for initialization
    void Start ()
    {
        _light = GetComponent<Light>();
        _halo = transform.GetChild(0).GetComponent<Light>();
        _enemy = GameManager.gm.enemy;
        _baseLightRange = _light.range;
        _baseIntensity = _light.intensity;
        _baseHaloRange = _halo.range;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!_light)
        {
            return;
        }

        if(!_halo)
        {
            return;
        }

        if (_enemy)
        {
            float distance = Mathf.Abs(Vector3.Distance(_enemy.transform.position, transform.position));

            if(distance < 1f)
            {
                _light.range = _baseLightRange * distance / 1f;
            }
            
        }

        _light.intensity = _baseIntensity + lightIntensityModifier * Mathf.Sin(Time.time * speed);
        _halo.range = _baseHaloRange + haloRangeModifier * Mathf.Sin(Time.time * speed);
    }
}
