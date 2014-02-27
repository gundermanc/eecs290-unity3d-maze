using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;

/**
 * Handles the rendering of all GUI elements, HUD elements, pause screen, menus,
 * etc.
 * @author Christian Gunderman
 */
public class OnScreenDisplay : MonoBehaviour {

	// script parameters
	public Texture healthBarTexture;			// the texture file to use for the health bar
	public Texture healthBarBackgroundTexture;	// the texture for the background of the empty health bar
	private Rect healthBarRect;					// the rectangle which the FULL health bar will occupy
	private Rect healthBarBackgroundRect;		// the rectangle which the empty health bar will occupy

	public Texture ammoBarTexture;				// the texture file to use for the ammo bar
	public Texture ammoBarBackgroundTexture;	// the texture for the background of the empty ammo bar
	private Rect ammoBarRect;					// the rectangle which the FULL ammo bar will occupy
	private Rect ammoBarBackgroundRect;			// the rectangle which the empty ammo bar will occupy

	public Texture batteryBarTexture;			// the texture file to use for the battery bar
	public Texture batteryBarBackgroundTexture;	// the texture for the background of the empty battery bar
	private Rect batteryBarRect;				// the rectangle which the FULL battery bar will occupy
	private Rect batteryBarBackgroundRect;		// the rectangle which the empty battery bar will occupy
	public int messageDisplayTime = 5;			// the number of seconds to display a message
	public Texture bloodDecalsTexture;			// the texture for the game over blood graphics
	private const string gameTitle = "Marshmallow Madness"; //text for the game title

	// private constants
	private const string welcomeMessage = "Will you roast the ghosts... or be squished to death by white fluffiness?"; //Text string for the welcome message
	private const string instructions1 = "Left click to swing your sword, right click throw throwing knives"; //Instructions on how to play the game
	private const string instructions2 = "Press Esc to pause and M to view the minimap fullscreen."; //Instructions continued
	private const int pauseMenuMargins = 70; //Coordinates for the rectangle used in the pause menu
	private const int shadowOffset = -2; //Value used to draw a shadow rectangle

	// private fields
	private int healthPoints = 100; //Player character's health points
	private int ammoCount = 100; //Default ammo count for throwing knives
	private int maxAmmoCount = 100; //Maximum amount of ammo the player can have at one time
	private int batteryLife = 100; //The value for the battery life; higher number = brighter battery
	private LinkedList<Message> messageQueue; //A linked list containing all the queue'd messages to be displayed on screen

	// singleton instance reference
	private static OnScreenDisplay instance; 
	private DeathCallback deathCallback = null;			// defines a function called when the player dies


	/**
	 * Run at initialization
	 */
	void Start () {
		/* Initializes the various bars to help you keep track of HP, Ammo, and Battery
		 * The "x" and "y" refer to the top-left co-ordinates of any rectangle
		 * The "width" and "height" refer to the actual size of the rectangle
		 * Each bar has both a rectangle for the actual meter as well as one for the background behind the bar
		 */

		healthBarRect.x = 17;
		healthBarRect.y = 20;
		healthBarRect.width = 155; 
		healthBarRect.height = 25;
		healthBarBackgroundRect.x = 15;
		healthBarBackgroundRect.y = 15;
		healthBarBackgroundRect.width = 160;
		healthBarBackgroundRect.height = 35;

		ammoBarRect.x = 17;
		ammoBarRect.y = 65;
		ammoBarRect.width = 155; 
		ammoBarRect.height = 25;
		ammoBarBackgroundRect.x = 15;
		ammoBarBackgroundRect.y = 60;
		ammoBarBackgroundRect.width = 160;
		ammoBarBackgroundRect.height = 35;

		batteryBarRect.x = 17;
		batteryBarRect.y = 110;
		batteryBarRect.width = 155; 
		batteryBarRect.height = 25;
		batteryBarBackgroundRect.x = 15;
		batteryBarBackgroundRect.y = 105;
		batteryBarBackgroundRect.width = 160;
		batteryBarBackgroundRect.height = 35;
		// class is a singleton. save a static reference
		instance = this;
		messageQueue = new LinkedList<Message> ();
	}

	/**
	 * Draws the GUI elements pertaining to the current game state.
	 */
	void OnGUI () {

		// draw the GUI appropriate for the current game state.
		switch(GameManager.GetGameMode()) {
		case GameManager.GameMode.Paused: //If the game mode is paused
			DrawPauseMenu();			  //Draw the pause menu
			break;
		case GameManager.GameMode.StartMenu:
			DrawGameStart(gameTitle);	  //Draw the start menu
			break;
		case GameManager.GameMode.Dead:   //Draw the "game over" screen
			DrawBloodDecals ();
			DrawWinScreen("You died you <b>SCRUB!</b>");
			break;
		case GameManager.GameMode.UnPaused: //Draw the usual display for when the game is unpaused and is being played
			DrawHUD();
			break;
		case GameManager.GameMode.EndLevel: //Draw the screen for when a level is won (the end of the maze is reached)
			DrawWinLevelScreen();
			break;
		case GameManager.GameMode.EndGame: //Draw the screen that congratulates the player for when all levels have been completed
			DrawWinScreen("Congratulations! You beat all of the levels.");
			break;
		}
	}

	void Update() {

		// don't discard any messages while the game is paused
		if (GameManager.IsPaused ()) {
			return;
		}

		//Otherwise, slowly remove messages from message log
		DiscardMessages ();
	}

	/**
	 * Sets the health points values in the onscreen display. If the player dies
	 * because of this health change, the deathCallback function is called.
	 * It also ensures the health does not go above the max health (100).
	 * @param healthPoints A value between 0 and 100
	 * @param addToExistingValue If true, the function adds healthPoints to
	 * the existing health points. If false, the old value is thrown out and
	 * replaced with the new value.
	 */
	public static void SetHealthPoints(int healthPoints, bool addToExistingValue) {
		if (healthPoints < 0) {
			Debug.LogError ("Invalid health points value: " + healthPoints);
			return;
		}


		// set value
		if (addToExistingValue) {
			if (healthPoints > 100) {
				instance.healthPoints = 100;
			} else {
				instance.healthPoints += healthPoints;
			}
		} else {
			if (healthPoints > 100) {
				instance.healthPoints = 100;
			} else {
			instance.healthPoints = healthPoints;
			}
		}

		// check if the main character died and dispatch handler
		if (instance.healthPoints == 0) {
			if(instance.deathCallback == null) {
				Debug.LogError("Main Character death, but no Death Handler was created.");
				return;
			}

			instance.deathCallback.PlayerDied();
		}
	}

	/**
	 * Sets the ammo count values in the onscreen display.
	 * @param ammo A value between 0 and 100
	 * @param addToExistingValue If true, the function adds healthPoints to
	 * the existing health points. If false, the old value is thrown out and
	 * replaced with the new value.
	 */
	public static void SetAmmoCount(int ammo) {
		if(ammo < 0 || ammo > instance.maxAmmoCount) {
			Debug.LogError("Invalid OSD ammo values.");
		}
		instance.ammoCount = ammo;
	}

	/** 
	 * Sets the max ammo count for the OnScreenDisplay
	 * @param maxAmmoCount The new maximum amount of ammo the player can have
	 */
	public static void SetMaxAmmoCount(int maxAmmoCount) {
		instance.maxAmmoCount = maxAmmoCount;
	}

	/**
	 * Sets the battery life values in the onscreen display.
	 * @param battery points A value between 0 and 100
	 * @param addToExistingValue If true, the function adds battery points to
	 * the existing health points. If false, the old value is thrown out and
	 * replaced with the new value.
	 */
	public static void SetBatteryLife(int batteryPoints, bool addToExistingValue) {
		if(instance == null) {
			return;
		}

		if (batteryPoints < 0) {
			Debug.LogError ("Invalid battery points value: " + batteryPoints);
			return;
		}
		
		
		// set value
		if (addToExistingValue) {
			if (batteryPoints > 100) {
				instance.batteryLife = 100;
			} else {
				instance.batteryLife += batteryPoints;
			}
		} else {
			if (batteryPoints > 100) {
				instance.batteryLife = 100;
			} else if(batteryPoints < 0) {
				instance.batteryLife = 0;
			} else {
				instance.batteryLife = batteryPoints;
			}
		}
	}

	/**
	 * Post a message given only the message to post: simply uses default color (white)
	 * @param message the message to add to the messageQueue, which will be converted to a string
	 */
	public static void PostMessage(object message) {
		PostMessage (message, new Color (1.0f, 1.0f, 1.0f, 1.0f));
	}

	/**
	 * Post a message given both the message to post and a color for the message to be in
	 * @param message the message to add to the messageQueue, which will be converted to a string
	 * @param color the value of the color the text should use
	 */
	public static void PostMessage(object message, Color color) {
		instance.messageQueue.AddLast(new Message(DateTime.Now, message.ToString(),
		                                          color));
	}

	/**
	 * Sets a death callback interface that will be called upon death of the main
	 * character.
	 * @param deathCallback A death callback interface
	 */
	public static void RegisterDeathCallback(DeathCallback deathCallback) {
		instance.deathCallback = deathCallback;
	}

	/**
	 * Draw the game Heads up display.
	 */
	private void DrawHUD() {
		GUIStyle regularStyle = GUI.skin.GetStyle ("Label");
		regularStyle.alignment = TextAnchor.UpperLeft;

		DrawHealthBar ();
		DrawAmmoBar ();
		DrawBatteryBar ();
		DrawMessages ();
		DrawLevelName ();
	}

	/**
	 * Draw the blood decals, used for the game over screen 
	 */
	private void DrawBloodDecals() {
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), bloodDecalsTexture);
	}

	/**
	 * Wraps GUI.Label function to provide shadows.
	 * @param labelPos is the position of the label which is offset by shadowOffset
	 * @param label is the actual string we are drawing the shadow for
	 */
	private void DrawLabelWithShadow(Rect labelPos, string label) {

		// translate rect for shadow
		Rect offsetLabelPos = new Rect (labelPos);
		labelPos.x += shadowOffset;
		labelPos.y += shadowOffset;
		labelPos.xMax += shadowOffset;
		labelPos.yMax += shadowOffset;

		// draw shadow
		GUI.Label (offsetLabelPos, "<color=black>" + label + "</color>");

		// draw text
		GUI.Label (labelPos, label);
	}

	/**
	 * Draws the pause menu	labels and GUI.
	 */
	private void DrawPauseMenu() {

		// create a label GUI style that is centered
		GUIStyle centeredStyle = GUI.skin.GetStyle ("Label");
		centeredStyle.alignment = TextAnchor.UpperCenter;

		// get pause menu rectange, minus the margins specified in the constant pauseMenuMargins
		Rect screenDimensions = new Rect (pauseMenuMargins, pauseMenuMargins, 
		                                  Screen.width - (2 * pauseMenuMargins),
		                                  Screen.height- (2 * pauseMenuMargins));

		DrawLabelWithShadow (screenDimensions, "<size=40><i>Paused</i></size>");

		// restart game button
		screenDimensions.y += 100;
		screenDimensions.yMax = screenDimensions.y + 75;
		if (GUI.Button (screenDimensions, "<size=30>Restart</size>")) {
			GameManager.RestartGame();
		}

		// exit game button
		screenDimensions.y += 100;
		screenDimensions.yMax = screenDimensions.y + 75;
		if (GUI.Button (screenDimensions, "<size=30>Quit</size>")) {
			
			// this works...just not in the editor. you have to actually build the project first
			Application.Quit ();
		}
	}

	/**
	 * Draws the start menu GUI.
	 * @param message is the message displayed on the start screen.
	 */
	private void DrawGameStart(string message) {
		// create a label GUI style that is centered
		GUIStyle centeredStyle = GUI.skin.GetStyle ("Label");
		centeredStyle.alignment = TextAnchor.UpperCenter;
		
		// get pause menu rectange, minus the margins specified in the constant pauseMenuMargins
		Rect screenDimensions = new Rect (pauseMenuMargins, pauseMenuMargins, 
		                                  Screen.width - (2 * pauseMenuMargins),
		                                  Screen.height- (2 * pauseMenuMargins));
		
		DrawLabelWithShadow (screenDimensions, "<size=40><i>" + message + "</i></size>");

		screenDimensions.y += 100;
		DrawLabelWithShadow (screenDimensions, "<size=25><i>" 
		                     + String.Format(welcomeMessage, message) + "</i></size>");

		screenDimensions.y += 100;
		screenDimensions.yMax = screenDimensions.y + 75;
		
		// exit game button
		if (GUI.Button (screenDimensions, "<size=30>Start Game</size>")) {
			
			// this works...just not in the editor. you have to actually build the project first
			GameManager.StartGame();
		}

		
		screenDimensions.y += 100;
		DrawLabelWithShadow (screenDimensions, "<size=15><i>" 
		                     + String.Format(instructions1, message) + "</i></size>");
		screenDimensions.y += 50;
		DrawLabelWithShadow (screenDimensions, "<size=15><i>" 
		                     + String.Format(instructions2, message) + "</i></size>");
	}

	/**
	 * Draws the GUI for winning a level.
	 */
	private void DrawWinLevelScreen() {
		
		// create a label GUI style that is centered
		GUIStyle centeredStyle = GUI.skin.GetStyle ("Label");
		centeredStyle.alignment = TextAnchor.UpperCenter;
		
		// get pause menu rectange, minus the margins specified in the constant pauseMenuMargins
		Rect screenDimensions = new Rect (pauseMenuMargins, pauseMenuMargins, 
		                                  Screen.width - (2 * pauseMenuMargins),
		                                  Screen.height- (2 * pauseMenuMargins));

		GameManager.LevelParams LevelParams = GameManager.GetCurrentLevelParams();
		DrawLabelWithShadow (screenDimensions, "<size=40><i>" +  "Completed Level: " 
		                     + LevelParams.LevelName + "</i></size>");
		

		
		// restart game button
		screenDimensions.y += 150;
		screenDimensions.yMax = screenDimensions.y + 75;
		if (GUI.Button (screenDimensions, "<size=30>Next Level</size>")) {
			
			GameManager.NextLevel();
		}

		// quit game button
		screenDimensions.y += 150;
		screenDimensions.yMax = screenDimensions.y + 75;
		if (GUI.Button (screenDimensions, "<size=30>Quit Game</size>")) {
			
			// this works...just not in the editor. you have to actually build the project first
			Application.Quit();
		}
	}

	/**
	 * Draws the screen shown upon beating all levels or dying.
	 * @param message The message shown on the win screen (e.g. a congratulations message).
	 */
	private void DrawWinScreen(string message) {
		
		// create a label GUI style that is centered
		GUIStyle centeredStyle = GUI.skin.GetStyle ("Label");
		centeredStyle.alignment = TextAnchor.UpperCenter;
		
		// get pause menu rectange, minus the margins specified in the constant pauseMenuMargins
		Rect screenDimensions = new Rect (pauseMenuMargins, pauseMenuMargins, 
		                                  Screen.width - (2 * pauseMenuMargins),
		                                  Screen.height- (2 * pauseMenuMargins));

		DrawLabelWithShadow (screenDimensions, "<size=40><i>" +  message
		                    + "</i></size>");
		
		
		
		// restart game button
		screenDimensions.y += 150;
		screenDimensions.yMax = screenDimensions.y + 75;
		if (GUI.Button (screenDimensions, "<size=30>Restart Game</size>")) {
			
			GameManager.RestartGame();
		}
		
		screenDimensions.y += 150;
		screenDimensions.yMax = screenDimensions.y + 75;
		if (GUI.Button (screenDimensions, "<size=30>Quit Game</size>")) {
			
			// this works...just not in the editor. you have to actually build the project first
			Application.Quit();
		}
	}

	/**
	 * Draws the name of the level across top of screen.
	 */
	private void DrawLevelName() {
		
		// create a label GUI style that is centered
		GUI.color = Color.white;
		GUIStyle centeredStyle = GUI.skin.GetStyle ("Label");
		centeredStyle.alignment = TextAnchor.UpperCenter;
		
		// get pause menu rectange, minus the margins specified in the constant pauseMenuMargins
		Rect screenDimensions = new Rect (15, 15, 
		                                  Screen.width - (2 * 15),
		                                  Screen.height- (2 * 15));
		
		GameManager.LevelParams LevelParams = GameManager.GetCurrentLevelParams();
		DrawLabelWithShadow (screenDimensions, "<size=30><i>Level: " 
		                     + LevelParams.LevelName + "</i></size>");
	}


	/**
	 * Draws the health bar, including the actual value of the health on the 
	 * health bar, and the health bar background 
	 */
	private void DrawHealthBar() {

		// draw background
		GUI.DrawTexture (healthBarBackgroundRect, healthBarBackgroundTexture);

		// draw health bar
		Rect scaledHealthBarRect = new Rect (healthBarRect.x, healthBarRect.y, 
		                                     (int)(((float)healthPoints / 100) * healthBarRect.width),
		                                     healthBarRect.height);
		GUI.DrawTexture (scaledHealthBarRect, healthBarTexture);

		// draw text
		GUI.color = Color.black;
		GUIStyle centeredStyle = GUI.skin.GetStyle ("Label");
		centeredStyle.alignment = TextAnchor.MiddleCenter;
		GUI.Label (healthBarRect, "<b>" + healthPoints + "HP</b>");
		GUI.color = Color.white;
	}

	/**
	 * Draws the battery life percentage bar for the HUD
	 */
	private void DrawBatteryBar() {
		
		// draw background
		GUI.DrawTexture (batteryBarBackgroundRect, batteryBarBackgroundTexture);
		
		// draw battery bar
		Rect scaledbatteryBarRect = new Rect (batteryBarRect.x, batteryBarRect.y, 
		                                     (int)(((float)batteryLife / 100) * batteryBarRect.width),
		                                      batteryBarRect.height);
		GUI.DrawTexture (scaledbatteryBarRect, batteryBarTexture);

		// draw text
		GUI.color = Color.black;
		GUIStyle centeredStyle = GUI.skin.GetStyle ("Label");
		centeredStyle.alignment = TextAnchor.MiddleCenter;
		GUI.Label (batteryBarRect, "<b>" + batteryLife + "% Battery</b>");
		GUI.color = Color.white;
	}

	/**
	 * Draws the ammo bar for the HUD
	 */
	private void DrawAmmoBar() {
		
		// draw background
		GUI.DrawTexture (ammoBarBackgroundRect, ammoBarBackgroundTexture);
		
		// draw battery bar
		Rect scaledAmmoBarRect = new Rect (ammoBarRect.x, ammoBarRect.y, 
		                                      (int)(((float)ammoCount / maxAmmoCount) * ammoBarRect.width),
		                                      ammoBarRect.height);
		GUI.DrawTexture (scaledAmmoBarRect, ammoBarTexture);

		// draw text
		GUI.color = Color.black;
		GUIStyle centeredStyle = GUI.skin.GetStyle ("Label");
		centeredStyle.alignment = TextAnchor.MiddleCenter;
		GUI.Label (ammoBarRect, "<b>" + ammoCount + " Throwing Knives</b>");
		GUI.color = Color.white;
	}

	/**
	 * Draws the messages posted with PostMessage to the screen.
	 */
	private void DrawMessages() {
		float y = 150;

		GUIStyle upperLeft = GUI.skin.GetStyle ("Label");
		upperLeft.alignment = TextAnchor.UpperLeft;
		foreach(Message m in messageQueue) {
			GUI.color = m.color;
			GUI.Label (new Rect (10, y += 20, 1000, 1000),
			           "<size=15>" + m.message + "</size>");
		}
	}

	/**
	 * Iterates through the list of messages posted with PostMessage.
	 * For each message, the message state is updated. If it has been
	 * in the queue for messageDisplayTime, the system begins reducing
	 * the alpha for the current message until it disappears and is removed
	 * from the queue.
	 */
	private void DiscardMessages() {
		if (messageQueue.Count > 0) {
/*			Message message = messageQueue.First.Value;*/

			foreach(Message message in messageQueue) {
				// see if a message has been up a long time, if so, delete it
				if ((DateTime.Now.Subtract (message.submitTime)).TotalSeconds > messageDisplayTime) {
					message.color.a -= 0.03f;
					if(message.color.a <= 0.0f) {
						messageQueue.Remove(message);
						break;
					}
				}
			}
		}
	}

	/**
	 * Defines a message that can be posted to the screen using PostMessage().
	 */
	private class Message {
		/** The time the message was submitted. */
		public DateTime submitTime;
		/** The message to display onscreen. */
		public string message;
		/** The color of the text */
		public Color color;

		/**
		 * Constructs an instance.
		 * @param submitTime The time the message was submitted.
		 * @param message The message text.
		 * @param color The color to draw the text.
		 */
		public Message(DateTime submitTime, string message, Color color) {
			this.submitTime = submitTime;
			this.message = message;
			this.color = color;
		}
	}
}
