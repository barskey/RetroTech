using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public Transform groundCheck;
	public LayerMask whatIsGound;
	public float jumpForce = 200f;
	public float speed;
	public float speedVar;
	public LayerMask whatIsPlatform;

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
	
	void FixedUpdate ()
	{
		grounded = Physics2D.OverlapCircle (groundCheck.position, groundRadius, whatIsGound);

		float move = player.position.x - transform.position.x;

		rb2d.velocity = new Vector2 (move * speed, rb2d.velocity.y);

		if (move > 0 && !facingRight)
			Flip ();
		else if (move < 0 && facingRight)
			Flip ();
	}

	void Update()
	{
		float dist_y = player.position.y - transform.position.y;
		if (grounded && dist_y > 0.1f)
		{
			if (PlatformAbove ())
			{
				rb2d.AddForce (new Vector2 (0, jumpForce));
				Debug.Log ("Jumped");
			}
		}

	}

	bool PlatformAbove()
	{
		RaycastHit2D hit = Physics2D.Raycast (transform.position, Vector2.up, 0.2f, whatIsPlatform);
		return hit;
	}

	void Flip()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
