﻿using UnityEngine;
using System.Collections.Generic;
using System;
using System.Text;

/**
 * A singleton class for the on screen display for our game.
 * @author Christian Gunderman
 */
public class OnScreenDisplay : MonoBehaviour {

	// script parameters
	public Texture healthBarTexture;			// the texture file to use for the health bar
	public Texture healthBarBackgroundTexture;	// the texture for the background of the empty health bar
	private Rect healthBarRect;					// the rectangle which the FULL health bar will occupy
	private Rect healthBarBackgroundRect;		// the rectangle which the empty health bar will occupy

	public Texture batteryBarTexture;			// the texture file to use for the health bar
	public Texture batteryBarBackgroundTexture;	// the texture for the background of the empty health bar
	private Rect batteryBarRect;					// the rectangle which the FULL health bar will occupy
	private Rect batteryBarBackgroundRect;		// the rectangle which the empty health bar will occupy
	public int messageDisplayTime = 5;			// the number of seconds to display a message
	public Texture bloodDecalsTexture;
	private const string gameTitle = "Marshmallow Madness";

	// private constants
	private const string welcomeMessage = "Will you roast the ghosts... or be squished to death by white fluffiness?";
	private const int pauseMenuMargins = 70;
	private const int shadowOffset = -3;

	// private fields
	private int healthPoints = 100;
	private int batteryLife = 100;
	private LinkedList<Message> messageQueue;

	// singleton instance reference
	private static OnScreenDisplay instance;
	private DeathCallback deathCallback = null;			// defines a function called when the player dies


	// Use this for initialization
	void Start () {
		healthBarRect.x = 17;
		healthBarRect.y = 20;
		healthBarRect.width = 155; 
		healthBarRect.height = 25;
		healthBarBackgroundRect.x = 15;
		healthBarBackgroundRect.y = 15;
		healthBarBackgroundRect.width = 160;
		healthBarBackgroundRect.height = 35;

		batteryBarRect.x = 17;
		batteryBarRect.y = 65;
		batteryBarRect.width = 155; 
		batteryBarRect.height = 25;
		batteryBarBackgroundRect.x = 15;
		batteryBarBackgroundRect.y = 60;
		batteryBarBackgroundRect.width = 160;
		batteryBarBackgroundRect.height = 35;
		// class is a singleton. save a static reference
		instance = this;
		messageQueue = new LinkedList<Message> ();
	}

	void OnGUI () {

		// draw the GUI appropriate for the current situation
		switch(GameManager.GetGameMode()) {
		case GameManager.GameMode.Paused:
			DrawPauseMenu();
			break;
		case GameManager.GameMode.StartMenu:
			DrawGameStart(gameTitle);
			break;
		case GameManager.GameMode.Dead:
			DrawGameStart("You Died!");
			DrawBloodDecals ();
			break;
		case GameManager.GameMode.UnPaused:
			DrawHUD();
			break;
		case GameManager.GameMode.EndGame:
			DrawWinScreen();
			break;
		}
	}

	void Update() {

		// don't discard any messages while the game is paused
		if (GameManager.IsPaused ()) {
			return;
		}

		DiscardMessages ();
	}

	/**
	 * Sets the health points values in the onscreen display. If the player dies
	 * because of this health change, the deathCallback function is called.
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

	public static void PostMessage(object message) {
		PostMessage (message, new Color (1.0f, 1.0f, 1.0f, 1.0f));
	}

	public static void PostMessage(object message, Color color) {
		instance.messageQueue.AddLast(new Message(DateTime.Now, message.ToString(),
		                                          color));
	}

	/**
	 * Sets a death callback interface that will be called upon death of the main
	 * character.
	 */
	public static void RegisterDeathCallback(DeathCallback deathCallback) {
		instance.deathCallback = deathCallback;
	}

	private void DrawHUD() {
		// create a label GUI style that is upper left justified
		GUIStyle regularStyle = GUI.skin.GetStyle ("Label");
		regularStyle.alignment = TextAnchor.UpperLeft;

		DrawHealthBar ();
		DrawBatteryBar ();
		DrawMessages ();
	}

	private void DrawBloodDecals() {
		GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), bloodDecalsTexture);
	}

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

		
		screenDimensions.y += 150;
		screenDimensions.yMax = screenDimensions.y + 75;
		
		// exit game button
		if (GUI.Button (screenDimensions, "<size=30>Start Game</size>")) {
			
			// this works...just not in the editor. you have to actually build the project first
			GameManager.StartGame();
		}
	}

	private void DrawWinScreen() {
		
		// create a label GUI style that is centered
		GUIStyle centeredStyle = GUI.skin.GetStyle ("Label");
		centeredStyle.alignment = TextAnchor.UpperCenter;
		
		// get pause menu rectange, minus the margins specified in the constant pauseMenuMargins
		Rect screenDimensions = new Rect (pauseMenuMargins, pauseMenuMargins, 
		                                  Screen.width - (2 * pauseMenuMargins),
		                                  Screen.height- (2 * pauseMenuMargins));
		
		DrawLabelWithShadow (screenDimensions, "<size=40><i>" +  "You Win!"  +"</i></size>");
		

		
		// restart game button
		screenDimensions.y += 150;
		screenDimensions.yMax = screenDimensions.y + 75;
		if (GUI.Button (screenDimensions, "<size=30>Next Level</size>")) {
			
			// this works...just not in the editor. you have to actually build the project first
			Application.LoadLevel(0);
		}

		screenDimensions.y += 150;
		screenDimensions.yMax = screenDimensions.y + 75;
		if (GUI.Button (screenDimensions, "<size=30>Quit Game</size>")) {
			
			// this works...just not in the editor. you have to actually build the project first
			Application.Quit();
		}
	}

	private void DrawHealthBar() {

		// draw background
		GUI.DrawTexture (healthBarBackgroundRect, healthBarBackgroundTexture);

		// draw health bar
		Rect scaledHealthBarRect = new Rect (healthBarRect.x, healthBarRect.y, 
		                                     (int)(((float)healthPoints / 100) * healthBarRect.width),
		                                     healthBarRect.height);
		GUI.DrawTexture (scaledHealthBarRect, healthBarTexture);
	}

	private void DrawBatteryBar() {
		
		// draw background
		GUI.DrawTexture (batteryBarBackgroundRect, batteryBarBackgroundTexture);
		
		// draw battery bar
		//Debug.Log (batteryLife + "h");
		//Debug.Log ((int)(((float)batteryLife / 100) * batteryBarRect.width));
		Rect scaledbatteryBarRect = new Rect (batteryBarRect.x, batteryBarRect.y, 
		                                     (int)(((float)batteryLife / 100) * batteryBarRect.width),
		                                      batteryBarRect.height);
		GUI.DrawTexture (scaledbatteryBarRect, batteryBarTexture);
	}

	private void DrawMessages() {
		float y = 100;

		foreach(Message m in messageQueue) {
			GUI.color = m.color;
			GUI.Label (new Rect (10, y += 20, 1000, 1000),
			           "<size=15>" + m.message + "</size>");
		}
	}

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

	public interface DeathCallback {
		void PlayerDied();
	}

	private class Message {
		public DateTime submitTime;
		public string message;
		public Color color;

		public Message(DateTime submitTime, string message, Color color) {
			this.submitTime = submitTime;
			this.message = message;
			this.color = color;
		}
	}
}
