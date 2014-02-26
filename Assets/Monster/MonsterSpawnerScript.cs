using UnityEngine;
using System.Collections;

public class MonsterSpawnerScript : MonoBehaviour {

	private GameObject Monsters;
	public GameObject Monster;
	public float SpawnRate;
	private float TimeAtLastSpawn;
	
	// Update is called once per frame
	void Update () {
		if(Time.timeSinceLevelLoad - TimeAtLastSpawn > SpawnRate){
			GameObject NextMonster;
			NextMonster = Instantiate(Monster, transform.localPosition, Quaternion.identity) as GameObject;
			NextMonster.transform.parent = transform;
			TimeAtLastSpawn = Time.timeSinceLevelLoad;
		}
	}

	public void SetTimeAtLastSpawn(float InitTime){
		TimeAtLastSpawn = InitTime;
	}
}
