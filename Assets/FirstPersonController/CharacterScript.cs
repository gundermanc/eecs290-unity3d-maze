using UnityEngine;
using System.Collections;

/**
 * Handles player's stats.
 * @authors Kai Smith, John Billingsley, Christian Gunderman
 */
public class CharacterScript : MonoBehaviour {

	/** Health pack sound effect */
	public AudioClip powerUp;
	/** Player's health */
	private int health;
	/** Minimum time between knife throws */
	public float projectileReloadTime;
	/** Highest amount of ammo you can have */
	public int maxAmmo;
	/** Current ammo count */
	public int ammo;
	/** Timestamp of most recent knife throw */
	private float lastShoot;
	/** Prefab for object to throw */
	public GameObject Projectile;

	/**
	 * Script initialization. This is called by unity on object creation.
	 */
	void Start () {
		health = 100;
		lastShoot = 0f;
		OnScreenDisplay.SetMaxAmmoCount (maxAmmo);
		OnScreenDisplay.SetAmmoCount (ammo);
	}
	
	/**
	 * Called once per frame by unity. Handles throwing knives.
	 */
	void Update () {
		if (Input.GetMouseButtonDown (1) && Time.timeSinceLevelLoad - lastShoot> projectileReloadTime && ammo > 0) {
			lastShoot = Time.timeSinceLevelLoad;
			ammo --;
			OnScreenDisplay.SetAmmoCount(ammo);
			Vector3 dir = GameObject.Find("Character View").transform.forward;
			GameObject proj = Instantiate(Projectile, gameObject.transform.position + Vector3.up + gameObject.transform.forward, gameObject.transform.localRotation) as GameObject;
			proj.rigidbody.AddForce(dir*200f);
			proj.rigidbody.AddTorque(new Vector3(-1f*proj.transform.forward.z, 0, proj.transform.forward.x)*-500f);
		}
	}

	/**
	 * Deals damage to the player. Called by attacking monster.
	 * param damage Amount of damage for playr to take.
	 */
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

	/**
	 * Heals some of the player's health.
	 * param Aamage Amount to heal.
	 */
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


	/**
	 * Getter method for player's health
	 * @return players health.
	 */
	public int GetHealth(){
		return(health);
	}

	/**
	 * Resets player's health to 100. Called when new level is created.
	 */
	public void ResetHealth() {
		health = 100;
	}

	/**
	 * Gives the player more ammo.
	 * @param amount_to_add Amount of ammo to add to the player's current stock
	 */
	public void AddAmmo(int ammo_to_add){
		if(ammo + ammo_to_add <= maxAmmo) {
			ammo += ammo_to_add;
		} else {
			ammo = maxAmmo;
		}

		OnScreenDisplay.SetAmmoCount (ammo);
	}

	/**
	 * Handles player's death.
	 */
	public void Die(){
		GameManager.PlayerDied ();
	}
}
