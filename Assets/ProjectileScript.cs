using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider Target) {
		if (Target.collider.tag == "Enemy") {
			Target.transform.GetComponent<MonsterScript>().Hurt(10f, transform.forward);
		}
	}
}
