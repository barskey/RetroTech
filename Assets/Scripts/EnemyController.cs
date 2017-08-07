using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public Transform groundCheckCircle;
	public LayerMask whatIsGound;
	public float jumpForce = 200f;
	public float speed;
	public float speedVar;

	bool facingRight = true;
	bool grounded = false;
	float groundRadius = 0.05f;
	float jumpHeight = 0.35f;

	private Transform player;
	private Rigidbody2D rb2d;

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D> ();
		player = GameObject.Find ("Player").transform;
	}
	
	void FixedUpdate ()
	{
		grounded = Physics2D.OverlapCircle (groundCheckCircle.position, groundRadius, whatIsGound);

		var playerVector = player.position - transform.position; // vector to the player

		rb2d.velocity = new Vector2 (playerVector.x * speed, rb2d.velocity.y);

		if (playerVector.x > 0 && !facingRight)
			Flip ();
		else if (playerVector.x < 0 && facingRight)
			Flip ();

		if (playerVector.y > jumpHeight) { // if enemy is close in x and player is at least a platform above
			RaycastHit2D hit = Physics2D.Raycast (transform.position, Vector2.up, 0.2f, whatIsGound);
			if (grounded && hit) {
				rb2d.AddForce (new Vector2 (0f, jumpForce));
			}
		}
	}

	void Flip()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
