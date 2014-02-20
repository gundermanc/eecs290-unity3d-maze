using UnityEngine;
using System.Collections;

public class CharacterScript : MonoBehaviour {

	public AudioClip powerUp;
	private int health;

	// Use this for initialization
	void Start () {
		health = 100;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Harm(int damage){
		FlashLight.Injury ();
		health -= damage;
		OnScreenDisplay.SetHealthPoints ((int)health, false);
		Debug.Log (health);
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

	public void ResetHealth() {
		health = 100;
	}

	public void Die(){
		GameManager.PlayerDied ();
	}
}
