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

	private Animator anim;
	private Rigidbody2D rb2d;
	private Inventory inv;

	void Start ()
	{
		rb2d = GetComponent <Rigidbody2D> ();
		anim = GetComponent <Animator> ();
		inv = GetComponent <Inventory> ();
	}

	void FixedUpdate ()
	{
		grounded = (true ? rb2d.velocity.y == 0 : false);

		float move = Input.GetAxis ("Horizontal");

		anim.SetFloat ("Speed", Mathf.Abs(move));

		rb2d.velocity = new Vector2 (move * maxSpeed, rb2d.velocity.y);

		if (move > 0 && !facingRight) {
			Flip ();
		} else if (move < 0 && facingRight) {
			Flip ();
		}
	}

	void Update ()
	{
		if (grounded && Input.GetKeyDown (KeyCode.Space)) {
			// set ground bool in anim
			rb2d.AddForce (new Vector2(0, jumpForce));
		}

		if (Input.GetKeyDown (KeyCode.Return)) {
			inv.weapon.Attack (); // shoot or swing the equipped weapon
			//inv.weaponBar.SetAmmoLeft (9); // if successful, decrement the rounds left
		}

		if (Input.GetKeyDown (KeyCode.Alpha1)) {
			inv.EquipWeapon (0); // the first weapon slot
		}

		if (Input.GetKeyDown (KeyCode.Alpha2)) {
			inv.EquipWeapon (1); // the second weapon slot
		}

		if (Input.GetKeyDown (KeyCode.Alpha3)) {
			inv.EquipWeapon (2); // the third weapon slot
		}
	}

	void Flip ()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.gameObject.name.StartsWith ("GridUnit")) {
			currentGrid = col.gameObject;
		}
	}
}
