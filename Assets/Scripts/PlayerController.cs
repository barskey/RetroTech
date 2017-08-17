using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
	public Transform groundCheck;
	public LayerMask whatIsGound;
	public float jumpForce = 200f;
	public float maxSpeed = 5f;

	public GameObject currentGrid;

	bool facingRight = true;
	bool grounded = false;
	float groundRadius = 0.01f;
	Animator anim;

	private Rigidbody2D rb2d;

	void Start()
	{
		rb2d = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();

	}

	void FixedUpdate () {
		grounded = (true ? rb2d.velocity.y == 0 : false);

		float move = Input.GetAxis ("Horizontal");

		anim.SetFloat ("Speed", Mathf.Abs(move));

		rb2d.velocity = new Vector2 (move * maxSpeed, rb2d.velocity.y);
		anim.SetFloat ("vSpeed", rb2d.velocity.y);

		if (move > 0 && !facingRight)
			Flip ();
		else if (move < 0 && facingRight)
			Flip ();
	}

	void Update()
	{
		if (grounded && Input.GetKeyDown (KeyCode.Space))
		{
			// set ground bool in anim
			rb2d.AddForce(new Vector2(0, jumpForce));
		}

		//if (Input.GetMouseButtonDown(0))
		//	Attack();
	}

	void Flip()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		currentGrid = col.gameObject;
	}
}
