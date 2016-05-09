using UnityEngine;
using System.Collections;

public class End : MonoBehaviour {

	void OnTriggerEnter(Collider other)
    {
        GameManager.gm.LevelCompete();
    }
}
