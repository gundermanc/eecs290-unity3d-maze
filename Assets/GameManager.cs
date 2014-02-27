using UnityEngine;
using System.Collections;

/**
 * Handles game level loading, StartGame, EndGame, Pause, and GameModes
 * @author Christian Gunderman
 */
public class GameManager : MonoBehaviour {

	/** The sound played when the player dies */
	public AudioClip deathSound;

	/** Saves a static reference to instance ... sloppy I know, I don't care */
	private static GameManager instance;
	/** Stores the current GameMode (pause, play, dead, menu etc. */
	private static GameMode mode;
	/** The current game level. This will persist between level loads. Change to change level */
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

	/**
	 * Script initialization. This is called by unity on object creation.
	 */
	void Start () {
		instance = this;
		
		/*
		 * The level was just reloaded. If we are reloading, and the level is set up for
		 * level "0", the first level, load the StartMenu. Otherwise, start the game.
		 */
		if(level == 0) {
			mode = GameMode.StartMenu;
		} else {
			mode = GameMode.UnPaused;  // normal game execution
		}
	}
	
	/**
	 * Called once per frame by unity. Handles toggling of paused and play.
	 */
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

	/**
	 * Checks if the game is paused.
	 * @return Returns true if paused, false if not.
	 */
	public static bool IsPaused() {
		return mode != GameMode.UnPaused;
	}

	/**
	 * Gets the GameMode of the current game. See GameMode enum definition
	 * for possible states.
	 * @return The current game mode.
	 */
	public static GameMode GetGameMode() {
		return mode;
	}

	/**
	 * Pauses the game.
	 */
	public static void Pause() {
		mode = GameMode.Paused;
	}

	/**
	 * Unpauses the game.
	 */
	public static void UnPause() {
		Debug.Log ("Unpaused.");
		mode = GameMode.UnPaused;
	}

	/**
	 * Gets the parameters for the grid for the current level. This 
	 * is used by the grid creator at level construction.
	 * @return Gets the current level's parameters, or null if we have
	 * finished all levels.
	 */
	public static LevelParams GetCurrentLevelParams() {

		// if there are enough levels
		if(level < levels.Length) {
			return levels[level];
		}
		return null;
	}

	/**
	 * Starts the execution of the game.
	 */
	public static void StartGame() {
		// register handler for game over event
		//OnScreenDisplay.RegisterDeathCallback (new OnDeathHandler ());
		UnPause ();
	}

	/**
	 * Restarts the game at the first level with full health, battery, and ammo.
	 */
	public static void RestartGame() {

		// configure for first level
		level = 0;

		// reset flashlight value
		FlashLight.UpdateBattery (100, false);

		// Switch camera to player view
		GameObject.Find ("CutsceneCam").GetComponent<Cutscene> ().EndScene ();

		// load first level
		Application.LoadLevel (0);
	}

	/**
	 * Called when the player died. Displays end game UIs.
	 */
	public static void PlayerDied() {
		instance.audio.Stop ();
		instance.audio.PlayOneShot (instance.deathSound);
		mode = GameMode.Dead;
	}

	/**
	 * Called when player reaches the end of a level. Dispatches EndLevel GUIs
	 * and loads next level.
	 */
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

	/**
	 * Gets an integer value that represents the current level.
	 * @return The index of the current level. 0 is level one.
	 */
	public static int GetLevel() {
		return level;
	}

	/**
	 * Ends the current level and advances to the next level by reloading the
	 * current scene with new parameters from the levels array.
	 */
	public static void NextLevel() {
		// increment level index
		level++;

		// reset flashlight value
		FlashLight.UpdateBattery (100, false);
		
		// reload the level with increased size and monster spawning
		Application.LoadLevel (0);
	}

	/**
	 * An object that contains the parameters for each randomly generated level.
	 * These are stored in an array, levels, at the top of the script. Add more levels
	 * by adding additional instances.
	 */
	public class LevelParams {
		/** The name of the level */
		public string LevelName;
		/** The width of the grid in blocks */
		public int Width;
		/** The width of the grid in blocks */
		public int Length;
		/** The percentage chance that a medpack will spawn */
		public int MedpackSpawnProb;
		/** The percentage chance that a medpack will spawn */
		public int BatterySpawnProb;
		/** The material for the walls and floor of this level */
		public Material WallMat, FloorMat;
		/** The percentage chance that monster will spawn in a block */
		public int MonsterSpawnProb;

		/**
		 * Constructs a LevelParams with some default params.
		 * @param LevelName The name of the level.
		 * @param Width The width of the grid in blocks.
		 * @param Length The length the grid in blocks.
		 */
		public LevelParams(string LevelName, int Width, int Length) 
			: this(LevelName, Width, Length, 5, 5, 20, null, null) {

			// call the other constructor, but fill in some default values

		}

		/**
		 * Constructs a LevelParams with some default params.
		 * @param LevelName The name of the level.
		 * @param Width The width of the grid in blocks.
		 * @param Length The length the grid in blocks.
		 * @param MonsterSpawnProb The percentage chance that monsters will spawn.
		 */
		public LevelParams(string LevelName, int Width, int Length, int MonsterSpawnProb) 
			: this(LevelName, Width, Length, 5, 5, MonsterSpawnProb, null, null) {
			// call the other constructor, but fill in some default values
		}

		/**
		 * Constructs a LevelParams with some default params.
		 * @param LevelName The name of the level.
		 * @param Width The width of the grid in blocks.
		 * @param Length The length the grid in blocks.
		 * @param MonsterSpawnProb The percentage chance that monsters will spawn.
		 * @param MedpackSpawnProb The percentage chance that medpacks will spawn.
		 * @param BatterySpawnProb The probability that batteries will spawn in each cell.
		 */
		public LevelParams(string LevelName, int Width, int Length, int MonsterSpawnProb,
		                   int MedpackSpawnProb, int BatterySpawnProb) 
			: this(LevelName, Width, Length, MedpackSpawnProb, BatterySpawnProb, 
			      MonsterSpawnProb, null, null) {
			// call the other constructor, but fill in some default values
		}

		/**
		 * Complete constructor...de whole shebang!
		 * Constructs a LevelParams with some default params.
		 * @param LevelName The name of the level.
		 * @param Width The width of the grid in blocks.
		 * @param Length The length the grid in blocks.
		 * @param MonsterSpawnProb The percentage chance that monsters will spawn.
		 * @param MedpackSpawnProb The percentage chance that medpacks will spawn.
		 * @param BatterySpawnProb The probability that batteries will spawn in each cell.
		 * @param WallMat A material for the wall.
		 * @param FloorMat Material for the floor.
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

	/**
	 * An enum of possible game states.
	 */
	public enum GameMode {
		StartMenu,			// at the start screen
		Paused,				// at the pause screen
		UnPaused,			// in normal game play
		Dead,				// at the death screen
		EndLevel,			// at the end level screen
		EndGame				// at the end game screen
	}
}
