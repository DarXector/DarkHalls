using UnityEngine;
using System.Collections;

public class GlowWave : MonoBehaviour {

    MazeGenerator _maze;

    float _currentScanRadius = 0f;

    public float timeToScan = 10f;
    public float scanSpeed = 0.5f;
    public float waveWidth = 0.5f;

    float _remaningTimeToScan;

	// Use this for initialization
	void Start () {
        _maze = GetComponent<MazeGenerator>();

        _remaningTimeToScan = timeToScan;
    }
	
	// Update is called once per frame
	void Update () {
	
        if(!_maze)
        {
            return;
        }

        if(Input.GetButtonDown("Jump"))
        {
            StartScan();
        }

        if (_remaningTimeToScan > 0f)
        {
            _remaningTimeToScan -= Time.deltaTime;
            _currentScanRadius += scanSpeed;

            foreach (var tile in _maze.glowTiles)
            {
                var distance = Vector3.Distance(_maze.player.transform.position, tile.transform.position);
                if(distance < _currentScanRadius + waveWidth / 2 && distance > _currentScanRadius - waveWidth / 2)
                {
                    tile.GetComponent<Renderer>().enabled = true;
                }
                else
                {
                    tile.GetComponent<Renderer>().enabled = false;
                }
            }
        }

	}

    public void StartScan()
    {
        _remaningTimeToScan = timeToScan;
        _currentScanRadius = 0;
    }
}
