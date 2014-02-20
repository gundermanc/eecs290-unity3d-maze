using UnityEngine;
using System.Collections;

public class BatteryScript : MonoBehaviour {
	public float BatteryCharge;
	public float BatteryRangeIncrease;
	public float RotationAngle;

	void OnTriggerEnter(Collider Target){
		if(Target.collider.tag == "Player"){
			FlashLight.UpdateBattery(50, true);
			Destroy(this.gameObject);
		}
	}

	void Update(){
		transform.Rotate(RotationAngle*.75f, RotationAngle*.5f, RotationAngle*.25f);
	}
}
