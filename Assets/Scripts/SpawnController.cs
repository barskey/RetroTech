using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour {

	public GameObject enemy; // Which enemy is spawned
	public float spawnDelay = 3f; // Seconds between spawning new enemies
	public int maxSpawn = 3; // max # of enemies allowed on screen
	public float health = 100f;

	private Animator anim;

	void Awake ()
	{
		anim = gameObject.GetComponent<Animator> ();

		float randTime = Random.Range (0, spawnDelay);

		InvokeRepeating ("TrySpawnEnemy", randTime, spawnDelay);
	}

	void TakeDamage (float dmg)
	{
		health -= dmg;

		anim.SetFloat ("Health", health);

		// if health < 0, destroy itself
	}

	void TrySpawnEnemy ()
	{
		if (GetEnemyCount () < maxSpawn) {
			Debug.Log ("Spawning enemy...");
			anim.SetTrigger ("Open");
		}
	}

	void SpawnEnemy ()
	{
		GameObject newEnemy = GameObject.Instantiate (enemy, gameObject.transform.position, Quaternion.identity);
	}

	int GetEnemyCount ()
	{
		int i = 0;

		var enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		foreach (GameObject e in enemies)
		{
			if (e.name.StartsWith (enemy.name))
				i++;
		}

		return i;
	}
}
