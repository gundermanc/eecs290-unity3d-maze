using UnityEngine;
using System.Collections;
using System;

/**
 * Flashlight control script
 * @author Christian Gunderman
 */
public class FlashLight : MonoBehaviour {
	// sets the flashlight rundown percentage, per second
	private static int dieRate = -2;
	// flashlight life percentage
	private static float batteryLife;
	private static FlashLight instance;
	// stores the time of the last update operation
	private static DateTime lastUpdate = DateTime.Now;
	private static DateTime injuryBegan = DateTime.Now;
	private static bool injured = false;
	private static bool previouslyPaused;

	// Use this for initialization
	void Start () {
		instance = this;
		batteryLife = 100f;
	}
	
	// Update is called once per frame
	void Update () {
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

		// set new life
		//Debug.Log (batteryLife);
		UpdateBattery ((dieRate * sinceLastUpdate), true);
		OnScreenDisplay.SetBatteryLife ((int)batteryLife, false);

		// store update time
		lastUpdate = DateTime.Now;

		UpdateFlashlight ();
	}

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

	private void ResolveInjury() {
		if(injured && DateTime.Now.Subtract(injuryBegan).TotalMilliseconds > 500) {
			instance.light.color = new Color(1.0f, 1.0f, 0.66f);
			injured = false;
		}
	}

	public static void Injury () {
		injured = true;
		injuryBegan = DateTime.Now;
		instance.light.color = new Color (1.0f, 0.35f, 0.35f);
		instance.light.intensity = 14.78f;
	}

	private static void UpdateFlashlight() {
		if(instance == null || injured) {
			return;
		}

		instance.light.intensity = 0.5f + (float)(batteryLife / 7);
	}
}
