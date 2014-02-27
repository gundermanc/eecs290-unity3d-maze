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
	private static int level = 0;

	// an array of levels..add new ones to make the game more exciting
	private static LevelParams[] levels = {
		new LevelParams ("<i>Training Wheels</i>", 1, 7),
		new LevelParams ("The <b>Real</b> Effing Deal!", 3, 7, 50),
		new LevelParams ("Just Kidding! This is the <b>Real</b> Effing Deal!", 15, 15, 38),
		new LevelParams ("The Labyrinth", 25, 25, 35, 10, 10),
		new LevelParams ("Down on the Range", 25, 10, 45, 35, 10),
		new LevelParams ("Final Challenge", 2, 75, 100, 75, 10),
	};

	// Use this for initialization
	void Start () {
		instance = this;
		
		/*
		 * The level was just reloaded. If we are reloading, and the level is set up for
		 * level "0", the first level, load the StartMenu. Otherwise, start the game.
		 */
		if(level == 0) {
			mode = GameMode.StartMenu;
		} else {
			mode = GameMode.UnPaused;
		}
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

	public static LevelParams GetCurrentLevelParams() {

		// if there are enough levels
		if(level < levels.Length) {
			return levels[level];
		}
		return null;
	}

	public static void StartGame() {
		// register handler for game over event
		//OnScreenDisplay.RegisterDeathCallback (new OnDeathHandler ());
		UnPause ();
	}

	public static void RestartGame() {

		level = 0;
		
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

		Application.LoadLevel (0);

		// set game mode
		//mode = GameMode.StartMenu;
	}

	public static void PlayerDied() {
		instance.audio.Stop ();
		instance.audio.PlayOneShot (instance.deathSound);
		mode = GameMode.Dead;
	}

	public static void EndLevel() {
		if (GameObject.Find ("CutsceneCam").GetComponent<Cutscene> ().time_to_complete == 0f) {
			GameObject.Find ("CutsceneCam").GetComponent<Cutscene> ().StartScene ();

			// check if there are more levels remaining
			if((level + 1) < levels.Length) {

				// levels remain, show the "Start next level?" screen
				mode = GameMode.EndLevel;
			} else {

				// no more levels. tell user they won
				mode = GameMode.EndGame;
			}
		}
	}

	public static int GetLevel() {
		return level;
	}

	public static void NextLevel() {
		level++;

		// reset flashlight value
		FlashLight.UpdateBattery (100, false);
		
		// reload the level with increased size and monster spawning
		Application.LoadLevel (0);
	}

	public class OnDeathHandler : OnScreenDisplay.DeathCallback {

		public void PlayerDied() {
			PlayerDied();
		}
	}

	/**
	 * An object that contains the parameters for each randomly generated level.
	 */
	public class LevelParams {
		public string LevelName;
		public int Width;
		public int Length;
		public int MedpackSpawnProb;
		public int BatterySpawnProb;
		public Material WallMat, FloorMat;
		public int MonsterSpawnProb;


		public LevelParams(string LevelName, int Width, int Length) 
			: this(LevelName, Width, Length, 5, 5, 20, null, null) {

			// call the other constructor, but fill in some default values

		}

		public LevelParams(string LevelName, int Width, int Length, int MonsterSpawnProb) 
			: this(LevelName, Width, Length, 5, 5, MonsterSpawnProb, null, null) {
			// call the other constructor, but fill in some default values
		}

		public LevelParams(string LevelName, int Width, int Length, int MonsterSpawnProb,
		                   int MedpackSpawnProb, int BatterySpawnProb) 
			: this(LevelName, Width, Length, MedpackSpawnProb, BatterySpawnProb, 
			      MonsterSpawnProb, null, null) {
			// call the other constructor, but fill in some default values
		}

		/**
		 * Complete Constructor...da whole shebang!
		 */
		public LevelParams(string LevelName, int Width, int Length, int MedpackSpawnProb, 
		                   int BatterySpawnProb, int MonsterSpawnProb,
		                   Material WallMat, Material FloorMat) {
			this.LevelName = LevelName;
			this.Width = Width;
			this.Length = Length;
			this.MedpackSpawnProb = MedpackSpawnProb;
			this.BatterySpawnProb = BatterySpawnProb;
			this.MonsterSpawnProb = MonsterSpawnProb;
			this.WallMat = WallMat;
			this.FloorMat = FloorMat;
		}
	}

	public enum GameMode {
		StartMenu,
		Paused,
		UnPaused,
		Dead,
		EndLevel,
		EndGame
	}
}
