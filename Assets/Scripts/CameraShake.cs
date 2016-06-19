using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SmoothFollow))]
public class CameraShake : MonoBehaviour {

    public float shakeAmt = 0.1f;
    public float shakeDuration = 0.4f;

    public Camera mainCamera;
    private SmoothFollow smoothFollow;

    void Awake()
    {
        mainCamera = gameObject.GetComponent<Camera>();
        smoothFollow = gameObject.GetComponent<SmoothFollow>();
    }

    public void StartShaking()
    {
        Debug.Log("StartShaking");

        smoothFollow.ReleaseTarget();

        InvokeRepeating("Shake", 0, .01f);
        Invoke("StopShaking", shakeDuration);

        Time.timeScale = 0.5f;
    }

    void Shake()
    {
        if (shakeAmt > 0)
        {
            float quakeAmtZ = Random.value * shakeAmt * 2 - shakeAmt;
            float quakeAmtX = Random.value * shakeAmt * 2 - shakeAmt;
            Vector3 pp = mainCamera.transform.position;
            pp.z += quakeAmtZ; // can also add to x and/or z
            pp.x += quakeAmtX; // can also add to x and/or z
            mainCamera.transform.position = pp;
        }
    }

    void StopShaking()
    {
        smoothFollow.GetTarget();
        Time.timeScale = 1f;
        CancelInvoke("Shake");
    }
}
