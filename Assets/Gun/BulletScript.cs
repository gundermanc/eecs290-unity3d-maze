using UnityEngine;
using System.Collections;

public class BulletScript : MonoBehaviour {

	public float Speed;
	public float Damage;

	void OnTriggerEnter(Collider Target){
	if(Target.collider.tag == "Enemy"){
//		Target.transform.GetComponent<MonsterScript>().Hurt(Damage);
		}
		Destroy(this.gameObject);
	}

	public Transform GetTransform(){
		return this.transform;
	}
}
