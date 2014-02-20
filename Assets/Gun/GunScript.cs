using UnityEngine;
using System.Collections;

public class GunScript : MonoBehaviour {
	public Transform Bullet;
	public Transform BulletPosition;
	public float BulletVelocity;
	// Update is called once per frame
	void Update () {
	if(Input.GetMouseButtonDown(0)){
			BulletPosition = BulletPosition.transform;
			Debug.Log("Shot Called");
			Transform ThisBullet;
			ThisBullet = (Transform)Instantiate(Bullet, BulletPosition.localPosition, Quaternion.identity);
			ThisBullet.rigidbody.AddForce(transform.forward * BulletVelocity);
		} 
	}
}
