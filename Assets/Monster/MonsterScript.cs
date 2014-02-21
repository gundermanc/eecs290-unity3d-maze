using UnityEngine;
using System.Collections;

public class MonsterScript : MonoBehaviour {
	public float health;
	public float speed;
	public float sight;
	public float attack_dist;
	public float attack_reload_time;
	public float power;
	private float last_attack;
	public float YaxisOffset;
	public float AttackKnockback;
	private GameObject target;
	//private LineRenderer lineRender;
	private Color c1 = Color.yellow;
	private Color c2 = Color.red;
	private Transform MouthTransform;
	private bool MouthOpen = false;
	
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
	
	void Update () {
		if(MouthOpen && Time.timeSinceLevelLoad - last_attack > .3f){
			MouthTransform.Rotate(-30f,0f,0f);
			MouthOpen = false;
		}
		Vector3 origin = transform.position;
		RaycastHit SeenObject;
		Vector3 dir = ((new Vector3(transform.position.x,transform.position.y +YaxisOffset, transform.position.z)) - target.transform.position).normalized*-1;
		Vector3 endPoint = origin + dir * sight;
		//lineRender.SetPosition(0, origin);
		if(Physics.Raycast(transform.position, dir, out SeenObject, sight)){
			transform.LookAt(target.transform.position);
			endPoint = SeenObject.point;
		//	lineRender.SetPosition(1,endPoint);
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
