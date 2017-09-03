using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

	public int clipSize = 9;
	public GameObject projectile;
	public float shotForce = 100f; // how fast projectile leaves this weapon
	public float cooldownInSec = 1f;

	private GameObject player;
	private Transform spawnPoint;
	private Animator anim;

	private bool canAttack = true; // can initially attack
	private float cooldown = 0f; // time since last attack

	void Awake ()
	{
		spawnPoint = transform.Find ("ProjectileSpawn");
		player = GameObject.Find ("Player");
		anim = gameObject.GetComponent <Animator> ();
	}

	void Update ()
	{
		cooldown += Time.deltaTime;

		if (cooldown >= cooldownInSec)
			canAttack = true;
	}

	public bool Attack ()
	{
		if (canAttack) {
			if (projectile) {
				GameObject bullet = GameObject.Instantiate (projectile, spawnPoint.position, Quaternion.identity);
				bullet.transform.localScale = player.transform.localScale;
				bullet.GetComponent <Rigidbody2D> ().AddForce (new Vector2 (player.transform.localScale.x, 0f) * shotForce);
			}

			anim.SetTrigger ("Shoot");

			cooldown = 0;
			canAttack = false;

			return true; // did attack
		}

		return false;
	}
}
