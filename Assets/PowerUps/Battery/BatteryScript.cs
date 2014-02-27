using UnityEngine;
using System.Collections;

public class BatteryScript : MonoBehaviour {

	//an angle that determines how much the prefab rotates
	public float RotationAngle;

	/**
	 * handles collisions with the battery, if the player collides with the battery the FlashLight is restored by 50
	 * @param Target - the object that the battery collided with
	 */
	void OnTriggerEnter(Collider Target){
		if(Target.collider.tag == "Player"){
			FlashLight.UpdateBattery(50, true);
			Destroy(this.gameObject);
		}
	}

	/**
	 * used by Unity to rotate the battery prefab, called once per frame
	 */
	void Update(){
		transform.Rotate(RotationAngle*.75f, RotationAngle*.5f, RotationAngle*.25f);
	}
}
