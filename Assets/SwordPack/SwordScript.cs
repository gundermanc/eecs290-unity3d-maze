using UnityEngine;
using System.Collections;

public class SwordScript : MonoBehaviour {
	public float Damage;
	public float AttackSpeed;
	private float SwipeTime;
	private bool HasAttacked;
	private bool Hit;
	private bool sword_inside_monster;
	private Collider Enemy;

	//Sound Effects
	public AudioClip Miss;
	public AudioClip Hit1;
	public AudioClip Hit2;

	void Start(){
		HasAttacked = false;
		Hit = true;
		sword_inside_monster = false;
		SwipeTime = 0f;
	}

	void OnTriggerEnter(Collider Target) {
		if(Target.collider.tag == "Enemy"){
			Enemy = Target;
			if (!Hit)
				Attack();
		}
	}
	//In order to deal with attacks where the sword is already in the monster and stays in.
	void OnTriggerStay(Collider Target) {
		if(Target.collider.tag == "Enemy"){
			Enemy = Target;
			sword_inside_monster = true;
		}
	}

	void OnTriggerExit(Collider Target) {
		if(Target.collider.tag == "Enemy")
			sword_inside_monster = false;
	}

	void Update(){

		// do nothing if the game is paused
		if(GameManager.IsPaused()) {
			return;
		}

		if(Input.GetMouseButtonDown(0) && Time.timeSinceLevelLoad - SwipeTime > AttackSpeed){
			Debug.Log("Attacking");
			SwipeTime = Time.timeSinceLevelLoad;
			Hit = false;
			HasAttacked = true;
			if (sword_inside_monster) {
				if (Enemy == null)
					sword_inside_monster = false;
				else{
					Attack();
					//audio.Stop();
					//audio.PlayOneShot(Hit1);
				}
			} if(!sword_inside_monster) {
				audio.Stop();
				audio.PlayOneShot(Miss);
			}
		}
		if (HasAttacked == true && Time.timeSinceLevelLoad - SwipeTime > AttackSpeed){
			HasAttacked = false;
			Hit = true;
		}
		//Smoothly rotate sword down
		if (HasAttacked == true && Time.timeSinceLevelLoad - SwipeTime < AttackSpeed/2f) {
			transform.parent.localRotation = Quaternion.Slerp(new Quaternion(0.1f, 0f, 0.2f, 1f), new Quaternion(0.7f, 0f, 0.2f, 0.7f), (Time.timeSinceLevelLoad - SwipeTime)*2f/AttackSpeed);
		}
		//Smoothly rotate sword back
		if (HasAttacked == true && Time.timeSinceLevelLoad - SwipeTime > AttackSpeed/2f) {
			transform.parent.localRotation = Quaternion.Slerp(new Quaternion(0.7f, 0f, 0.2f, 0.7f), new Quaternion(0.1f, 0f, 0.2f, 1f), (Time.timeSinceLevelLoad - SwipeTime)*2f/AttackSpeed-1f);
		}
	}

	private void Attack(){
		if(Enemy != null) {
			Enemy.transform.GetComponent<MonsterScript>().Hurt(Damage, transform.forward);
			Hit = true;
			audio.Stop();
			audio.PlayOneShot(Hit1);
		}
	}
}
