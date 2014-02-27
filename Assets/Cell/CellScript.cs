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

	/** List of each cell's adjacent cells */
	public List<Transform> Adjacents;
	/** Cell's position in space */
	public Vector3 Position;
	/** Cell's weight */
	public int Weight;
	/** Number of adjacent cells that are part of the path */
	public int AdjacentsOpened = 0;
	/** True if player steps on cell, false otherwise */
	public bool IsPlayer = false;
	/** The grid's scaling factor */
	private float scaling;
	
	/**
	 * Script initialization. This is called by unity on object creation.
	 */
	void Start(){
		scaling = GameObject.Find ("Grid").GetComponent<GridCreator> ().scaling;
	}

	/**
	 * Recognizes that the player entered the cell and turns the corresponding sqare on the minimap red.
	 * @param Target Collider of the object that entered the trigger.
	 */
	void OnTriggerEnter(Collider Target){
		if(Target != null && Target.tag == "Player"){
			IsPlayer = true;
			Transform mmCell = GridCreator.MiniMap[(int) (gameObject.transform.position.x/scaling), (int) (gameObject.transform.position.z/scaling)];
			mmCell.renderer.material.color = Color.red;
			//If square is the goal square, the maze has been completed.
			if (mmCell.transform.position.x == GridCreator.GoalBlock.transform.position.x 
			    && mmCell.transform.position.z == GridCreator.GoalBlock.transform.position.z) {
				GridCreator.MazeComplete();	
			}
		}
	}

	/**
	 * Recognizes that the player left the cell and turns the corresponding sqare on the minimap white.
	 */
	void OnTriggerExit(){
		if(IsPlayer){
			Transform mmCell = GridCreator.MiniMap[(int) (gameObject.transform.position.x/scaling), (int) (gameObject.transform.position.z/scaling)];
			mmCell.renderer.material.color = Color.white;
			IsPlayer = false;
		}
	}
	
}
