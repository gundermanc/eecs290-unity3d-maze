using UnityEngine;
using System.Collections;

public class MedPackScript : MonoBehaviour {

	//the amount of health that is restored to the player when they pick up a med pack
	public int HealingFactor;
	//an angle that determines how much the prefab rotates
	public float RotationAngle;

	/**
	 * handles collisions with the med pack, if the player collides with the med pack they are restored HealingFactor of health
	 * @param Target - the object that the med pack collided with
	 */
	void OnTriggerEnter(Collider Target){
		if(Target.collider.tag == "Player"){
			if(Target.GetComponent<CharacterScript>().GetHealth() != 100){
				Target.transform.GetComponentInChildren<CharacterScript>().Heal(HealingFactor);
				Destroy(this.gameObject);
			}
		}
	}

	/**
	 * used by Unity to rotate the med pack prefab, called once per frame
	 */
	void Update(){
		transform.Rotate(0f, RotationAngle*.5f, 0f);
	}
}