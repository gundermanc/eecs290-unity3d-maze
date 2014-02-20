﻿using UnityEngine;
using System.Collections;

public class Cutscene : MonoBehaviour {

	public float time_to_zoom;
	public float zoom;
	private Camera cam;
	private Transform cam_transform;
	private static bool animate;
	private Vector3 start;// = new Vector3(5f,20f,5f);
	private Vector3 end;// = new Vector3(5f,40f,5f);

	// Use this for initialization
	void Start () {
		cam = GameObject.Find ("CutsceneCam").GetComponent<Camera> ();
		//cam.orthographicSize = Mathf.Max(x * scaling * 0.55f, z * scaling * 0.5f);
	}

	public void StartScene(){
		float x = GameObject.Find ("Grid").GetComponent<GridCreator> ().Size.x;
		float z = GameObject.Find ("Grid").GetComponent<GridCreator> ().Size.z;
		float scaling = GameObject.Find ("Grid").GetComponent<GridCreator> ().scaling;
		start = GameObject.Find ("Grid").GetComponent<GridCreator> ().Grid[(int)(x / 2f),(int)(z / 2f)].GetComponent<CellScript>().Position + (Vector3.up * 20f);
		end = start + 20*Vector3.up;
		Debug.Log (cam.enabled);
		cam.transform.position = start;
		animate = true;
		RenderSettings.fog = false;
		cam.depth = 10;

	}

	// Update is called once per frame
	void Update () {
		if (animate) {
			cam.transform.position = Vector3.Slerp(start, end, (Time.timeSinceLevelLoad - GridCreator.time_to_complete)/time_to_zoom);
		}
	}
}
