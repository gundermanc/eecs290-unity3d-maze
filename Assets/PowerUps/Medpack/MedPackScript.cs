using UnityEngine;
using System.Collections;

public class MedPackScript : MonoBehaviour {

	public int HealingFactor;
	public float RotationAngle;
	
	void OnTriggerEnter(Collider Target){
		if(Target.collider.tag == "Player"){
			Target.transform.GetComponentInChildren<CharacterScript>().Heal(HealingFactor);
			Destroy(this.gameObject);
		}
	}
	
	void Update(){
		transform.Rotate(0f, RotationAngle*.5f, 0f);
	}
}
