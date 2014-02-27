using UnityEngine;
using System.Collections;

public class MonsterScript : MonoBehaviour {
	// Monster initial health
	public float health;
	// Monster move speed
	public float speed;
	// the range the monster will see you from
	public float sight;
	// the range the monster will attack from
	public float attack_dist;
	// how much time is taken in between attacks
	public float attack_reload_time;
	// the amount of damage each attack does
	public float power;
	// the time the last attack took place
	private float last_attack;
	// there is a slight offset added to the raycast position, this makes the monsters target the player better than targeting the 0 y position of the player capsule
	public float YaxisOffset;
	// the force the monsters push the player back when they attack
	public float AttackKnockback;
	// the player gameObject
	private GameObject target;
	// This is the transform of the bottom jaw of the monster. used for "animation"
	private Transform MouthTransform;
	// checks if the mouth is open
	private bool MouthOpen = false;

	/** 
	 * How the monster variables are initialized
	 */
	void Start(){
		last_attack = 0f;
		target = GameObject.Find("First Person Controller");
		//lineRender = gameObject.AddComponent<LineRenderer>();
		//lineRender.material = new Material(Shader.Find("Particles/Additive"));
		//lineRender.SetColors(c1,c2);
		//lineRender.SetWidth(0.2f, 0.2f);
		//lineRender.SetVertexCount(2);
		MouthTransform = transform.Find("Mouth/MouthBottom");
	}

	/**
	 * What the monster is doing each frame of gameplay
	 */
	void Update () {
		if(GameManager.IsPaused()) {
			return;
		}

		if(MouthOpen && Time.timeSinceLevelLoad - last_attack > .3f){
			MouthTransform.Rotate(-30f,0f,0f);
			MouthOpen = false;
		}
		Vector3 origin = transform.position;
		RaycastHit SeenObject;
		Vector3 dir = ((new Vector3(transform.position.x,transform.position.y +YaxisOffset, transform.position.z)) - target.transform.position).normalized*-1;
		Vector3 endPoint = origin + dir * sight;

		if(Physics.Raycast(transform.position, dir, out SeenObject, sight)){
			transform.LookAt(target.transform.position);
			endPoint = SeenObject.point;

			if(SeenObject.collider.tag == "Player"){
				if(Dist(gameObject, target) > attack_dist)
					rigidbody.AddForce(dir*speed);
				else
					Attack();
			} else if(SeenObject.collider.tag == "Wall"){
				Patrol(SeenObject);
			}
		}
		
	}
	
	private float Dist(GameObject a, GameObject b){
		return((a.transform.position - b.transform.position).magnitude);
	}

	/**
	 * This is the attack Method for the monster
	 * It is called when the player collider enters the monster collider
	 * It 
	 */
	private void Attack(){
		if (Time.timeSinceLevelLoad - last_attack > attack_reload_time) {
			target.GetComponent<CharacterScript> ().Harm ((int)power);
			last_attack = Time.timeSinceLevelLoad;
			MouthTransform.Rotate(30f,0f,0f);
			MouthOpen = true;
		}
	}
	
	private void Patrol(RaycastHit SeenObject){
		rigidbody.AddForce(transform.forward*speed);
	}
	
	public void Hurt(float Amount, Vector3 PushDirection){
		//Debug.Log(PushDirection);
		health -= Amount;
		OnScreenDisplay.PostMessage ("Hit!", Color.yellow);
		rigidbody.AddForce(PushDirection * AttackKnockback);
		if(health <= 0){
			Destroy(gameObject);
		}
	}
}
