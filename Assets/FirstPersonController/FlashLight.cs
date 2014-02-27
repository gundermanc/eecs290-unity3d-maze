using UnityEngine;
using System.Collections;
using System;

/**
 * Flashlight control script. Manages the intensity of the main character
 * flashlight and provides red flash functionality for injury feedback.
 * @author Christian Gunderman
 */
public class FlashLight : MonoBehaviour {
	/** The percentage of battery life lost per second */
	private static int dieRate = -2;
	/** The batteryLife in a value 0 to 100 */
	private static float batteryLife;
	/** Stores a static reference to this instance...sloppy. I don't care. */
	private static FlashLight instance;
	/** stores the time of the last update operation */
	private static DateTime lastUpdate = DateTime.Now;
	/** stores the time that an injury flash began */
	private static DateTime injuryBegan = DateTime.Now;
	/** currently in an injury flash */
	private static bool injured = false;
	/** was the game paused last frame */
	private static bool previouslyPaused;

	/**
	 * Initialize flashlight with default value of 100% life.
	 */
	void Start () {
		instance = this;
		batteryLife = 100f;
	}
	
	/**
	 * Called once per frame. Updates flashlight battery life, reducing it in
	 * intensity as time progresses.
	 */
	void Update () {
		/* if flashlight is currently red, due to an injury, check if we should turn
		 * it back normal yet
		 */
		ResolveInjury ();

		// don't run the light down while the game is paused
		if(GameManager.IsPaused()) {
			previouslyPaused = true;
			return;
		} else if(previouslyPaused) {
			lastUpdate = DateTime.Now;
			previouslyPaused = false;
		}

		// seconds since last update
		float sinceLastUpdate = (float)(DateTime.Now.Subtract (lastUpdate).TotalSeconds);

		// set new battery life based upon elapsed time
		UpdateBattery ((dieRate * sinceLastUpdate), true);
		OnScreenDisplay.SetBatteryLife ((int)batteryLife, false);

		// store last updated time
		lastUpdate = DateTime.Now;

		// update the intensity of the directional light on the character
		UpdateFlashlight ();
	}

	/**
	 * Updates the flashlight's battery life.
	 * @param value The value to update the battery life by.
	 * @parma addToExisting If true, value will be added to the current battery
	 * life, but will enforce a maximum battery life of 100f. If false, batteryLife
	 * will be set to value, as long as value does not exceed 100f and is greater 
	 * than 0.
	 */
	public static void UpdateBattery(float value, bool addToExisting) {
		if(addToExisting) {
			if(batteryLife + value > 100) {
				batteryLife = 100;
			} else if (batteryLife + value < 0) {
				batteryLife = 0;
			} else {
				batteryLife += value;
			}
		} else if(value >= 0 && value <= 100) {
			batteryLife = value;
		} else {
			Debug.LogError("Battery life value " + value + " out of range.");
		}
	}

	/**
	 * Should be run every Update, checks if character is currently injured. If so,
	 * if 1/2 second has passed, return flashlight to default intensity and color.
	 */
	private void ResolveInjury() {
		if(injured && DateTime.Now.Subtract(injuryBegan).TotalMilliseconds > 500) {
			instance.light.color = new Color(1.0f, 1.0f, 0.66f);
			injured = false;
		}
	}

	/**
	 * Displays injury feedback in the form of flashing the flashlight red for
	 * 1/2 second.
	 */
	public static void Injury () {
		injured = true;
		injuryBegan = DateTime.Now;
		instance.light.color = new Color (1.0f, 0.35f, 0.35f);
		instance.light.intensity = 14.78f;
	}

	/**
	 * Updates the first person directional light so that it's intensity matches
	 * the remaining battery life.
	 */
	private static void UpdateFlashlight() {
		if(instance == null || injured) {
			return;
		}

		// minimum intensity of 0.5f, maximum around 14.28
		instance.light.intensity = 0.5f + (float)(batteryLife / 7);
	}
}
