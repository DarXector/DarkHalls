using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public static class PlayerPrefManager {
    
	public static float GetTime() {
		if (PlayerPrefs.HasKey("Time")) {
			return PlayerPrefs.GetFloat("Time");
		} else {
			return 0;
		}
	}

	public static void SetTime(float time) {
		PlayerPrefs.SetFloat("Time", time);
	}

	public static float GetBestTime() {
		if (PlayerPrefs.HasKey("BestTime")) {
			return PlayerPrefs.GetFloat("BestTime");
		} else {
			return 0;
		}
	}

	public static void SetBestTime(float bestTime) {
		PlayerPrefs.SetFloat("BestTime", bestTime);
	}


	// story the current player state info into PlayerPrefs
	public static void SavePlayerState(float time, float bestTime) {
		// save currentscore and lives to PlayerPrefs for moving to next level
		PlayerPrefs.SetFloat("Time", time);
		PlayerPrefs.SetFloat("BestTime", bestTime);
	}
	
	// reset stored player state and variables back to defaults
	public static void ResetPlayerState(bool resetBestTime) {
		Debug.Log ("Player State reset.");
		PlayerPrefs.SetFloat("Time", 0);

		if (resetBestTime)
			PlayerPrefs.SetFloat("BestTime", 0);
	}

	// store a key for the name of the current level to indicate it is unlocked
	public static void UnlockLevel() {	
		PlayerPrefs.SetInt(SceneManager.GetActiveScene().name,1);
	}

	// determine if a levelname is currently unlocked (i.e., it has a key set)
	public static bool LevelIsUnlocked(string levelName) {
		return (PlayerPrefs.HasKey(levelName));
	}

	// output the defined Player Prefs to the console
	public static void ShowPlayerPrefs() {
		// store the PlayerPref keys to output to the console
		string[] values = {"Time","BestTime"};

		// loop over the values and output to the console
		foreach(string value in values) {
			if (PlayerPrefs.HasKey(value)) {
				Debug.Log (value+" = "+PlayerPrefs.GetInt(value));
			} else {
				Debug.Log (value+" is not set.");
			}
		}
	}
}
