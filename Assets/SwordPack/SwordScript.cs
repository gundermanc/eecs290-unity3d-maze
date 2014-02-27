using UnityEngine;
using System.Collections;

public class SwordScript : MonoBehaviour {
	//How much damage the sword does to monsters
	public float Damage;
	//How fast the attack happens (effectively a sword reload time)
	public float AttackSpeed;
	//Timestamp of when the attack was initiated
	private float SwipeTime;
	//True when attacking, false otherwise
	private bool HasAttacked;
	//True if a monster was hit, false otherwise
	private bool Hit;
	//True if the sword is inside a monster, false otherwise
	private bool sword_inside_monster;
	//Stores the enemy to sword is killing
	private Collider Enemy;

	//Sound Effects
	public AudioClip Miss;
	public AudioClip Hit1;
	public AudioClip Hit2;

	//Initializes values
	void Start(){
		HasAttacked = false;
		Hit = true;
		sword_inside_monster = false;
		SwipeTime = 0f;
	}

	//If the sword collides with an enemy and you haven't already attacked, attack
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
	//Turn sword_inside_monster flag off if the sword exits the monster
	void OnTriggerExit(Collider Target) {
		if(Target.collider.tag == "Enemy")
			sword_inside_monster = false;
	}

	void Update(){

		// do nothing if the game is paused
		if(GameManager.IsPaused()) {
			return;
		}

		//If clicked and able to attack, attack
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
				}
			} if(!sword_inside_monster) {
				audio.Stop();
				audio.PlayOneShot(Miss);
			}
		}
		//If you are done attacking, change variables appropriately
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
	//Damages the enemy and plays sound effect
	private void Attack(){
		if(Enemy != null) {
			Enemy.transform.GetComponent<MonsterScript>().Hurt(Damage, transform.forward);
			Hit = true;
			audio.Stop();
			audio.PlayOneShot(Hit1);
		}
	}
}
