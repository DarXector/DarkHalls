using UnityEngine;
using System.Collections;
using UnityStandardAssets.Utility;

public class AttachToCamera : MonoBehaviour {

	// Use this for initialization
	void Awake () {
	    foreach(var cam in Camera.allCameras)
        {
            Debug.Log("cam " + cam.name);
            cam.GetComponent<SmoothFollow>().target = gameObject.transform;
        }
	}
}
