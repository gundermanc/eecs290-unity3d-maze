using UnityEngine; 
using System.Collections;
using System.Collections.Generic;

/**
 * Creates a grid of specified dimensions and generates a procedural maze using a
 * modified form of Prim's Algorithm.
 * @author Timothy Sesler
 * @author tds45
 * @date 4 February 2014
 * 
 * Adapted from work provided online by Austin Takechi 
 * Contact: MinoruTono@Gmail.com
 */ 
public class GridCreator : MonoBehaviour {
	// This is the prefab being used to create the maze out of cell blocks
	public Transform CellPrefab;
	// This is the maze grid comprised of an array of cell blocks
	public Transform[,] Grid;
	// This is a grid comprised of an array of cell blocks used to create the minimap structure
	public static Transform[,] MiniMap;
	// Used to toggle full screen mode for the minimap
	public bool MMFullScreen = false;
	// Specific location of the block that ends the level
	public static Transform GoalBlock;
	// This is a container to put the minimap cells into so the hierarchy doesnt get too cluttered
	public GameObject MMContainer;
	// This is the monster prefab that will be spawned in the levels as the badguys
	public GameObject Monster;
	// This is the container for all the monsters so the hierarchy doesnt get too cluttered
	public GameObject Monsters;
	// This is a container for all powerups so the hierarchy doesnt get too cluttered
	public GameObject PowerUps;
	// This is the Battery prefab that will be used to spawn batterys in the map
	public GameObject Battery;
	// This is the Medpack prefab that will be used to spawn medpacks in the map
	public GameObject Medpack;

	// This is the closest distance monsters are allowed to spawn to the player
	public float monsterStartingDistance = 5;

	// This is the size of the grid
	public Vector3 Size;
	// This is the probability a medpack will spawn
	public int MedpackSpwanProb;
	// This is the probability a battery will spawn
	public int BatterySpawnProb;
	// These are the various matterials that will be used for the wall and floors of the map
	public Material WallMat, FloorMat;
	// This is the proability a monster will spawn
	public int MonsterSpawnProb;
	// This is the scaling for the level prefabs and grid creator
	public float scaling;

	// This is the randomized probability for things
	private int DiceRoll;

	// Use this for initialization
	void Start () {

		// set params from game manager
		GameManager.LevelParams levelParams = GameManager.GetCurrentLevelParams ();
		Size = new Vector3 (levelParams.Width, 1, levelParams.Length);
		MedpackSpwanProb = levelParams.MedpackSpawnProb;
		BatterySpawnProb = levelParams.BatterySpawnProb;
		MonsterSpawnProb = levelParams.MonsterSpawnProb;

		if(levelParams.WallMat != null) {
			WallMat = levelParams.WallMat;
		}

		if(levelParams.FloorMat != null) {
			FloorMat = levelParams.FloorMat;
		}

		CreateGrid();
		SetRandomNumbers();
		SetAdjacents();
		SetStart();
		FindNext();
	}

	// Creates the grid by instantiating provided cell prefabs.
	void CreateGrid () {
		Grid = new Transform[(int)Size.x,(int)Size.z];
		MiniMap = new Transform[(int)Size.x,(int)Size.z];
		
		// Places the cells and names them according to their coordinates in the grid.
		for (int x = 0; x < Size.x; x++) {
			for (int z = 0; z < Size.z; z++) {
				Transform newCell;
				newCell = (Transform)Instantiate(CellPrefab, new Vector3(((float)x)*scaling, 0, ((float)z)*scaling), Quaternion.identity);
				newCell.localScale = new Vector3(scaling, newCell.localScale.y, scaling);
				newCell.name = string.Format("({0},0,{1})", x, z);
				newCell.parent = transform;
				newCell.GetComponent<CellScript>().Position = new Vector3(((float)x)*scaling, 0, ((float)z)*scaling);
				newCell.GetComponentInChildren<TextMesh>().renderer.enabled = false;
				Grid[x,z] = newCell;

				//Create physical minimap
				Transform MMCell;
				MMCell = (Transform)Instantiate(CellPrefab, new Vector3(((float)x)*scaling, -40, ((float)z)*scaling), Quaternion.identity);
				MMCell.localScale = new Vector3(scaling, MMCell.localScale.y, scaling);
				MMCell.name = string.Format("mm({0},0,{1})", x, z);
				MMCell.parent = MMContainer.transform;
				//MMCell.GetComponent<CellScript>().Position = new Vector3(x, -40, z);
				MMCell.GetComponentInChildren<TextMesh>().renderer.enabled = false;
				MiniMap[x,z] = MMCell;
			}
		}
		// Centers the camera on the maze.
		// Feel free to adjust this as needed.
		Camera.main.transform.position = Grid[(int)(Size.x / 2f),(int)(Size.z / 2f)].position - Vector3.up * 20f;
		Camera.main.orthographicSize = Mathf.Max(Size.x * scaling * 0.6f, Size.z * scaling * 0.4f);

		//adds borders around the Grid
		Transform border1, border2, border3, border4;
		border1 = (Transform)Instantiate (CellPrefab, new Vector3 (((Size.x / 2) * scaling) - 1.5f, 0, -1 * (scaling / 2) - 0.5f), Quaternion.identity); //Vector3 is position
		border1.localScale = new Vector3 (scaling * Size.x, 5, 1);
		border1.GetComponentInChildren<TextMesh>().renderer.enabled = false;
		border1.renderer.material = WallMat;
		border1.transform.tag = "Wall";
		border1.parent = transform;
		border2 = (Transform)Instantiate (CellPrefab, new Vector3(Size.x * scaling - 1f, 0, ((Size.z / 2) * scaling) - 1.5f), Quaternion.identity);
		border2.localScale = new Vector3 (1, 5, scaling * Size.z + 2f);
		border2.GetComponentInChildren<TextMesh>().renderer.enabled = false;
		border2.renderer.material = WallMat;
		border2.transform.tag = "Wall";
		border2.parent = transform;
		border3 = (Transform)Instantiate (CellPrefab, new Vector3(((Size.x / 2) * scaling) - 1.5f, 0, Size.z * scaling - 1f), Quaternion.identity);
		border3.localScale = new Vector3 (scaling * Size.x, 5, 1);
		border3.GetComponentInChildren<TextMesh>().renderer.enabled = false;
		border3.renderer.material = WallMat;
		border3.transform.tag = "Wall";
		border3.parent = transform;
		border4 = (Transform)Instantiate(CellPrefab, new Vector3(-2, 0, ((Size.z / 2) * scaling) - 1.5f), Quaternion.identity);
		border4.localScale = new Vector3 (1, 5, scaling * Size.z + 2f);
		border4.GetComponentInChildren<TextMesh>().renderer.enabled = false;
		border4.renderer.material = WallMat;
		border4.transform.tag = "Wall";
		border4.parent = transform;
	}

	// Sets a random weight to each cell.
	void SetRandomNumbers () {
		foreach (Transform child in transform) {
			int weight = Random.Range(0,10);
			child.GetComponentInChildren<TextMesh>().text = weight.ToString();
			child.GetComponent<CellScript>().Weight = weight;
		}
	}

	// Determines the adjacent cells of each cell in the grid.
	void SetAdjacents () {
		for(int x = 0; x < Size.x; x++){
			for (int z = 0; z < Size.z; z++) {
				Transform cell;
				cell = Grid[x,z];
				CellScript cScript = cell.GetComponent<CellScript>();
				
				if (x - 1 >= 0) {
					cScript.Adjacents.Add(Grid[x - 1, z]);
				}
				if (x + 1 < Size.x) {
					cScript.Adjacents.Add(Grid[x + 1, z]);
				}
				if (z - 1 >= 0) {
					cScript.Adjacents.Add(Grid[x, z - 1]);
				}
				if (z + 1 < Size.z) {
					cScript.Adjacents.Add(Grid[x, z + 1]);
				}
				
				cScript.Adjacents.Sort(SortByLowestWeight);
			}
		}
	}

	// Sorts the weights of adjacent cells.
	// Check the link for more info on custom comparators and sorting.
	// http://msdn.microsoft.com/en-us/library/0e743hdt.aspx
	int SortByLowestWeight (Transform inputA, Transform inputB) {
		int a = inputA.GetComponent<CellScript>().Weight;
		int b = inputB.GetComponent<CellScript>().Weight;
		return a.CompareTo(b);
	}

	/*********************************************************************
	 * Everything after this point pertains to generating the actual maze.
	 * Look at the Wikipedia page for more info on Prim's Algorithm.
	 * http://en.wikipedia.org/wiki/Prim%27s_algorithm
	 ********************************************************************/ 
	public List<Transform> PathCells;			// The cells in the path through the grid.
	public List<List<Transform>> AdjSet;		// A list of lists representing available adjacent cells.
	/** Here is the structure:
	 *  AdjSet{
	 * 		[ 0 ] is a list of all the cells
	 *      that have a weight of 0, and are
	 *      adjacent to the cells in the path
	 *      [ 1 ] is a list of all the cells
	 *      that have a weight of 1, and are
	 * 		adjacent to the cells in the path
	 *      ...
	 *      [ 9 ] is a list of all the cells
	 *      that have a weight of 9, and are
	 *      adjacent to the cells in the path
	 * 	}
	 *
	 * Note: Multiple entries of the same cell
	 * will not appear as duplicates.
	 * (Some adjacent cells will be next to
	 * two or three or four other path cells).
	 * They are only recorded in the AdjSet once.
	 */  

	// Initializes the sets and the starting cell.
	void SetStart () {
		PathCells = new List<Transform>();
		AdjSet = new List<List<Transform>>();
		
		for (int i = 0; i < 10; i++) {
			AdjSet.Add(new List<Transform>());	
		}
		
		Grid[0, 0].renderer.material.color = Color.green;
		Grid[0, 0].transform.localScale = new Vector3(Grid[0, 0].transform.localScale.x,1,Grid[0, 0].transform.localScale.z);
		Grid[0, 0].transform.localPosition = new Vector3(Grid[0, 0].transform.localPosition.x, -2f, Grid[0, 0].transform.localPosition.z);
		AddToSet(Grid[0, 0]);
	}

	// Adds a cell to the set of visited cells.
	void AddToSet (Transform cellToAdd) {
		PathCells.Add(cellToAdd);
		
		foreach (Transform adj in cellToAdd.GetComponent<CellScript>().Adjacents) {
			adj.GetComponent<CellScript>().AdjacentsOpened++;
			
			if (!PathCells.Contains(adj) && !(AdjSet[adj.GetComponent<CellScript>().Weight].Contains(adj))) {
				AdjSet[adj.GetComponent<CellScript>().Weight].Add(adj);
			}
		}
	}

	// Determines the next cell to be visited.
	void FindNext () {
		Transform next;

		do {
			bool isEmpty = true;
			int lowestList = 0;

			// We loop through each sub-list in the AdjSet list of lists, until we find one with a count of more than 0.
			// If there are more than 0 items in the sub-list, it is not empty.
			// We've found the lowest sub-list, so there is no need to continue searching.
			for (int i = 0; i < 10; i++) {
				lowestList = i;
				
				if (AdjSet[i].Count > 0) {
					isEmpty = false;
					break;
				}
			}

			// The maze is complete.
			if (isEmpty) { 
				Debug.Log("Generation completed in " + Time.timeSinceLevelLoad + " seconds."); 
				CancelInvoke("FindNext");
				PathCells[PathCells.Count - 1].renderer.material.color = Color.red;
				PathCells[PathCells.Count - 1].transform.localScale = new Vector3(PathCells[PathCells.Count - 1].transform.localScale.x,1,PathCells[PathCells.Count - 1].transform.localScale.z);
				GoalBlock = PathCells[PathCells.Count - 1];

				foreach (Transform cell in Grid) {
					// Removes displayed weight
					cell.GetComponentInChildren<TextMesh>().renderer.enabled = false;

					if (!PathCells.Contains(cell)) {
						cell.renderer.material.color = Color.gray;
						cell.renderer.material = WallMat;
						cell.transform.tag = "Wall";
					}
				}
				return;
			}
			// If we did not finish, then:
			// 1. Use the smallest sub-list in AdjSet as found earlier with the lowestList variable.
			// 2. With that smallest sub-list, take the first element in that list, and use it as the 'next'.
			next = AdjSet[lowestList][0];
			// Since we do not want the same cell in both AdjSet and Set, remove this 'next' variable from AdjSet.
			AdjSet[lowestList].Remove(next);
		} while (next.GetComponent<CellScript>().AdjacentsOpened >= 2);	// This keeps the walls in the grid, otherwise Prim's Algorithm would just visit every cell

		/*
		 * This section is for spawning objects in the maze
		 * We are spwaning objects only over path cells, which is why this code is going here where path cells are being created one by one
		 * The first thing we do is create a random variable that we will use to compare results against spawning probablities set earlier
		 * We then create a game object which will be the next monster if a monster will spawn, and a transform for a positional location
		 */
		DiceRoll = Random.Range(0,100);
		GameObject NextMonster;
		Vector3 location = new Vector3(next.transform.localPosition.x, 3f, next.transform.localPosition.z);

		/*
		 * We also need to starting position of the player so monsters will not spawn directly on the player
		 * or very close to the player, so they can not be imediately attacked.
		 */
		Vector3 startingPos = new Vector3 (0, 0, 0);

		/*
		 * We compare the dice roll to the probability a monster will spawn AND
		 * we need to check the distance is further away than the monsterStartingDistance we set eariler AND
		 * the monsters are not in line of sight of the player
		 */
		if((DiceRoll > (100 - MonsterSpawnProb))
		   && Vector3.Distance(location, startingPos) >= monsterStartingDistance
		   && !lineOfSight(location, startingPos)) {
			// If all of these conditions are satisfied, then we can spawn a monster in this tile position
			NextMonster = Instantiate(Monster, location, Quaternion.identity) as GameObject;
			// We put the monster as a child of the Monsters object so we dont get a bunch of clones in the build hierarchy
			NextMonster.transform.parent = Monsters.transform;
			// If a monster doesnt spawn, we reroll a dice and attempt to create a battery in a similar manner
		} else {
			DiceRoll = Random.Range(0,100);
			GameObject NextBattery;
			if(DiceRoll > (100 - BatterySpawnProb)){
				NextBattery = Instantiate(Battery, next.transform.position, Quaternion.identity) as GameObject;
				NextBattery.transform.parent = PowerUps.transform;
				// If the battery doesnt spawn, we reroll again and attempt to spawn a medpack
			} else {
				DiceRoll = Random.Range(0,100);
				GameObject NextMedpack;
				if(DiceRoll > (100 - MedpackSpwanProb)){
					NextMedpack = Instantiate(Medpack, next.transform.position, Quaternion.identity) as GameObject;
					NextMedpack.transform.parent = PowerUps.transform;
				}
			}
		}

		// The 'next' transform's material color becomes white. And the scale is as big as we want it
		next.renderer.material = FloorMat;
		next.renderer.material.color = Color.white;
		next.transform.localScale = new Vector3(next.transform.localScale.x,1,next.transform.localScale.z);
		next.transform.localPosition = new Vector3(next.transform.localPosition.x, -2f, next.transform.localPosition.z);
		// We add this 'next' transform to the Set our function.
		AddToSet(next);
		// Recursively call this function as soon as it finishes.
		Invoke("FindNext", 0);
	}

	public static void MazeComplete(){
		GameManager.EndLevel ();
	}

	// Called once per frame.
	void Update() {

		// Pressing 'F1' will generate a new maze.
		if (Input.GetKeyDown(KeyCode.F1)) {
			Application.LoadLevel(0);	
		}

		if (Input.GetKeyDown(KeyCode.M)) {
			if(!MMFullScreen){
				Camera.main.rect = new Rect(0,0,1,1);
				MMFullScreen = true;
			}
			else{
				Camera.main.rect = new Rect(.9f,.8f,.1f,.2f);
				MMFullScreen = false;
			}
		}
		if (GameManager.IsPaused()) 
			Camera.main.depth = -1;
		else
			Camera.main.depth = 1;
	}

	/**
	 * Checks to see if two positions are in the same row or column to
	 * naively check to see if they can be line of sight.
	 */
	bool lineOfSight(Vector3 vector1, Vector3 vector2) {
		const float cellWidth = 1f;
		
		if (Mathf.Abs (vector1.x - vector2.x) <= cellWidth
		    || Mathf.Abs (vector1.z - vector2.z) <= cellWidth) {
			return true;
		}
		return false;
	}
}
