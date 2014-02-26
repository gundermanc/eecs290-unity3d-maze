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
			transform.parent = Target.transform;
		} else if(Target.collider.tag == "Player"){
			Physics.IgnoreCollision(Target.collider, collider);
		} else {
			Destroy(gameObject);
		}
	}
}
