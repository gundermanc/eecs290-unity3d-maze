using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * The variables of a single maze cell.
 * @author Timothy Sesler
 * @author tds45
 * @date 4 February 2014
 * 
 * Adapted from work provided online by Austin Takechi 
 * Contact: MinoruTono@Gmail.com
 */ 
public class CellScript : MonoBehaviour {
	
	public List<Transform> Adjacents;
	public Vector3 Position;
	public int Weight;
	public int AdjacentsOpened = 0;
	public bool IsPlayer = false;
	private float scaling;
	
	void Start(){
		scaling = GameObject.Find ("Grid").GetComponent<GridCreator> ().scaling;
	}
	
	void OnTriggerEnter(Collider Target){
		if(Target.tag == "Player"){
			IsPlayer = true;
			Transform mmCell = GridCreator.MiniMap[(int) (gameObject.transform.position.x/scaling), (int) (gameObject.transform.position.z/scaling)];
			mmCell.renderer.material.color = Color.red;
			if (mmCell.transform.position.x == GridCreator.GoalBlock.transform.position.x 
			    && mmCell.transform.position.z == GridCreator.GoalBlock.transform.position.z) {
				GridCreator.MazeComplete();	
			}
		}
	}
	
	void OnTriggerExit(){
		if(IsPlayer){
			Transform mmCell = GridCreator.MiniMap[(int) (gameObject.transform.position.x/scaling), (int) (gameObject.transform.position.z/scaling)];
			mmCell.renderer.material.color = Color.white;
			IsPlayer = false;
		}
	}
	
}
