using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public Transform groundCheck;
	public LayerMask whatIsGound;
	public float jumpForce = 200f;
	public float speed;

	bool facingRight = true;
	bool grounded = false;
	float groundRadius = 0.1f;

	private Transform player;
	private Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D> ();
		player = GameObject.Find ("Player").transform;
	}
	
	void FixedUpdate () {

		float move = player.position.x - transform.position.x;

		rb2d.velocity = new Vector2 (move * speed, rb2d.velocity.y);

		if (move > 0 && !facingRight)
			Flip ();
		else if (move < 0 && facingRight)
			Flip ();
	}

	void Flip()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
