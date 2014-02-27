using UnityEngine;
using System.Collections;

public class Cutscene : MonoBehaviour {

	//When the cutscene starts, how long does it take to zoom all the way out
	public float time_to_zoom;
	//How much thecutscene zooms
	public float zoom;
	//The actual Camera object
	private Camera cam;
	//The transform associated by the camera
	private Transform cam_transform;
	//True when the cutscene is going on, false otherwise
	private static bool animate;
	//Vector3 position for the cutscene camera to start at
	private Vector3 start;
	//Ending position of the cutscene camera
	private Vector3 end;
	//Stores the time at which the maze was completed.
	public float time_to_complete;

	// Use this for initialization
	void Start () {
		cam = GameObject.Find ("CutsceneCam").GetComponent<Camera> ();
		time_to_complete = 0f;
	}
	/*
	 * Called when the maze is complete.
	 * Positions the camera in the appropriate spot and decides the end poition.
	 * Turns off the fog and on a directional light for better viewing.
	 * Turns animate to true so it will move.
	 * Puts the cutscene camera at a high depth so it shows up on the screen.
	 */
	public void StartScene(){
		float x = GameObject.Find ("Grid").GetComponent<GridCreator> ().Size.x;
		float z = GameObject.Find ("Grid").GetComponent<GridCreator> ().Size.z;
		float scaling = GameObject.Find ("Grid").GetComponent<GridCreator> ().scaling;
		start = new Vector3(x*scaling/2f, 20f, z*scaling/2f);
		end = start + 20*Vector3.up;
		cam.transform.position = start;
		time_to_complete = Time.timeSinceLevelLoad;
		GameObject.Find ("Directional light").light.intensity = 0.5f;
		animate = true;
		RenderSettings.fog = false;
		cam.depth = 10;

	}
	/*
	 *Called when the the game starts a new level to hide the cutscene and return the charachters camera
	 *to the main camera and restores appropriate fog and lighting.
	 */
	public void EndScene(){
		cam.depth = -1;
		RenderSettings.fog = true;
		animate = false;
		GameObject.Find ("Directional light").light.intensity = 0f;
		time_to_complete = 0f;
	}

	/*
	 * Update is called once per frame
	 * If animate is true, move the camera for effect.
	*/
	void Update () {
		if (animate) {
			cam.transform.position = Vector3.Slerp(start, end, (Time.timeSinceLevelLoad - time_to_complete)/time_to_zoom);
		}
	}
}
