using UnityEngine;
using System.Collections;

/**
 * Handles game events and modes.
 * @author Christian Gunderman
 */
public class GameManager : MonoBehaviour {

	public AudioClip deathSound;

	// singleton, save the instance
	private static GameManager instance;
	private static GameMode mode;

	// Use this for initialization
	void Start () {
		instance = this;

		// start the game at the beginning menu
		mode = GameMode.StartMenu;
	}
	
	// Update is called once per frame
	void Update () {
		// guard case, exit function if this isn't a pause/unpause situation
		if (!Input.GetKeyDown ("escape")) {
			return;
		}

		// toggle game paused state
		switch(GameManager.GetGameMode()) {
		case GameMode.Paused:
			UnPause();
			break;
		case GameMode.UnPaused:
			Pause();
			break;
		}
	}

	public static bool IsPaused() {
		return mode != GameMode.UnPaused;
	}

	public static GameMode GetGameMode() {
		return mode;
	}

	public static void Pause() {
		Debug.Log ("Paused.");
		mode = GameMode.Paused;
	}

	public static void UnPause() {
		Debug.Log ("Unpaused.");
		mode = GameMode.UnPaused;
	}

	public static void StartGame() {
		// register handler for game over event
		//OnScreenDisplay.RegisterDeathCallback (new OnDeathHandler ());
		UnPause ();
	}

	public static void RestartGame() {
		
		// return player to above the grid
		GameObject fpc = GameObject.Find("First Person Controller");
		fpc.transform.position = new Vector3 (0, 3, 0);
		
		// reset first person health
		fpc.GetComponent<CharacterScript> ().ResetHealth ();
		OnScreenDisplay.SetHealthPoints (100, false);

		// reset flashlight value
		FlashLight.UpdateBattery (100, false);

		// Switch camera to player view
		GameObject.Find ("CutsceneCam").GetComponent<Cutscene> ().EndScene ();

		// set game mode
		mode = GameMode.StartMenu;
	}

	public static void PlayerDied() {
		instance.audio.Stop ();
		instance.audio.PlayOneShot (instance.deathSound);
		RestartGame ();
	}

	public static void EndGame() {
		if (GameObject.Find ("CutsceneCam").GetComponent<Cutscene> ().time_to_complete == 0f) {
			GameObject.Find ("CutsceneCam").GetComponent<Cutscene> ().StartScene ();
			mode = GameMode.EndGame;
		}
	}

	public class OnDeathHandler : OnScreenDisplay.DeathCallback {

		public void PlayerDied() {
			PlayerDied();
		}
	}

	public enum GameMode {
		StartMenu,
		Paused,
		UnPaused,
		Dead,
		EndGame
	}
}
