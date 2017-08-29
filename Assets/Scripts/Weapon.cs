using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

	public GameObject projectile;
	public float shotForce = 100f;

	private GameObject player;
	private Transform spawnPoint;

	void Awake ()
	{
		spawnPoint = transform.Find ("ProjectileSpawn");
		player = GameObject.Find ("Player");
	}

	public void Attack ()
	{
		if (projectile) {
			GameObject bullet = GameObject.Instantiate (projectile, spawnPoint.position, Quaternion.identity);
			bullet.transform.localScale = player.transform.localScale;
			bullet.GetComponent <Rigidbody2D> ().AddForce (new Vector2 (player.transform.localScale.x, 0f) * shotForce);
		}
	}
}
