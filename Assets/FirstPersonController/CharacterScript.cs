using UnityEngine;
using System.Collections;

public class CharacterScript : MonoBehaviour {

	public AudioClip powerUp;
	private int health;
	public float projectileReloadTime;
	public int ammo;
	private float lastShoot;
	public GameObject Projectile;

	// Use this for initialization
	void Start () {
		health = 100;
		lastShoot = 0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (1) && Time.timeSinceLevelLoad - lastShoot> projectileReloadTime && ammo > 0) {
			lastShoot = Time.timeSinceLevelLoad;
			ammo --;
			Vector3 dir = GameObject.Find("Character View").transform.forward;
			GameObject proj = Instantiate(Projectile, gameObject.transform.position + Vector3.up + gameObject.transform.forward, gameObject.transform.localRotation) as GameObject;
			proj.rigidbody.AddForce(dir*200f);
			proj.rigidbody.AddTorque(new Vector3(-1f*proj.transform.forward.z, 0, proj.transform.forward.x)*-500f);
		}
	}

	public void Harm(int damage){
		FlashLight.Injury ();
		health -= damage;
		OnScreenDisplay.SetHealthPoints ((int)health, false);
		//Debug.Log (health);
		if (health <= 0) {
			Die();	
		} else {
			audio.Play();
			OnScreenDisplay.PostMessage("Taking Damage!", Color.red);
		}
	}

	public void Heal(int Amount){
		OnScreenDisplay.SetHealthPoints (health + (int)Amount, false);
		OnScreenDisplay.PostMessage ("Plus " + Amount + " health!", Color.green);
		if (health + Amount > 100) {
			health = 100;
		} else {
			audio.PlayOneShot(powerUp);
			health += Amount;
		}
	}

	public int GetHealth(){
		return(health);
	}

	public void ResetHealth() {
		health = 100;
	}

	public void AddAmmo(int ammo_to_add){
		ammo += ammo_to_add;
	}

	public void Die(){
		GameManager.PlayerDied ();
	}
}
