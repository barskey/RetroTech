﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public float damage = 20f;
	public float duration = 3f; // time in secs before going poof

	// Use this for initialization
	void Start () {
		Destroy (gameObject, duration);
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.CompareTag ("Player")) {
			PlayerHealth health = col.GetComponent <PlayerHealth> ();
			health.TakeDamage (damage);
			Destroy (gameObject);
		} else if (col.CompareTag ("Enemy")) {
			Debug.Log (string.Format ("Hitting enemy {0}", col.name));
			EnemyHealth health = col.GetComponent <EnemyHealth> ();
			health.TakeDamage (damage);
			Destroy (gameObject);
		}
	}
	
}
