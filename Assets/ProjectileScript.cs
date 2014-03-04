using UnityEngine;
using System.Collections;

/**
 * Handles behavior of the throwing knives.
 * @author Kai Smith
 */
public class ProjectileScript : MonoBehaviour {

	/** Knife is active until it stops moving. When inactive, they can't hurt monsters and can be picked back up */
	private bool active;
	/** The knife's trigger collider */
	private CapsuleCollider col;
	/** Timestamp of when the knife was created */
	private float time_made;

	/**
	 * Script initialization. This is called by unity on object creation.
	 */
	void Start () {
		active = true;
		col = gameObject.transform.GetComponents<CapsuleCollider> ()[1];
		time_made = Time.timeSinceLevelLoad;
	}
	
	/**
	 * Called once per frame by unity. Makes knives inactive when appropriate.
	 */
	void Update () {
		//If knife has basically stopped moving, make it inactive so it can't hurt enemies but can be picked up for ammo
		if (gameObject.rigidbody.velocity.magnitude < 0.05f && Time.timeSinceLevelLoad - time_made > 0.1f) {
			active = false;
			col.radius = 0.05f;
		}
	}

	/**
	 * Handles hurting monsters and picking knives back up.
	 * @param Target The collider of the object that entered the trigger
	 */
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
