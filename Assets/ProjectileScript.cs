using UnityEngine;
using System.Collections;

public class ProjectileScript : MonoBehaviour {

	private bool active;
	private CapsuleCollider col;
	private float time_made;

	// Use this for initialization
	void Start () {
		active = true;
		col = gameObject.transform.GetComponents<CapsuleCollider> ()[1];
		time_made = Time.timeSinceLevelLoad;
	}
	
	// Update is called once per frame
	void Update () {
		//If knife has basically stopped moving, make it inactive so it can't hurt enemies but can be picked up for ammo
		if (gameObject.rigidbody.velocity.magnitude < 0.05f && Time.timeSinceLevelLoad - time_made > 0.1f) {
			active = false;
			col.radius = 0.05f;
		}
	}

	void OnTriggerEnter(Collider Target) {
		//Hit enemy
		if (Target.collider.tag == "Enemy" && active) {
			Target.transform.GetComponent<MonsterScript>().Hurt(10f, transform.forward);
			active = false;
		}
		//Pick up used sword
		if (Target.collider.tag == "Player" && !active) {
			GameObject.Find("First Person Controller").GetComponent<CharacterScript>().AddAmmo(1);
			OnScreenDisplay.PostMessage("Picked up 1 Throwing Knife", Color.green);
			Destroy(gameObject);
		}
	}
}
