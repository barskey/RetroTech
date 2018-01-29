using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

	private Scene hud;
	private JoystickHandler joystickMovement;
	private Vector3 direction;
	private float xMin,xMax,yMin,yMax;

	void Start ()
	{
		rb2d = GetComponent <Rigidbody2D> ();
		anim = GetComponent <Animator> ();
		inv = GetComponent <Inventory> ();

		//Initialization of boundaries
		xMax = Screen.width - 50; // I used 50 because the size of player is 100*100
		xMin = Screen.width; 
		yMax = Screen.height - 50;
		yMin = Screen.height;
	}

	void OnEnable ()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	void OnSceneLoaded (Scene scene, LoadSceneMode mode)
	{
		Debug.Log (string.Format ("Scene {0} loaded.", scene.name));
		if (scene.name == "HUD") {
			GameObject[] rootObj = scene.GetRootGameObjects ();
			foreach (GameObject go in rootObj)
			{
				if (go.transform.Find ("JoystickContainer") != null)
					joystickMovement = go.transform.Find ("JoystickContainer").GetComponent <JoystickHandler> ();
			}
		}
	}

	void FixedUpdate ()
	{
		grounded = (true ? rb2d.velocity.y == 0 : false);

		if(direction.magnitude != 0) {
			transform.position += new Vector3 (direction.x * maxSpeed, 0f);
			//transform.position = new Vector3 (Mathf.Clamp (transform.position.x, xMin, xMax), Mathf.Clamp (transform.position.y, yMin, yMax), 0f);//to restric movement of player
		}    

		//float move = Input.GetAxis ("Horizontal");

		anim.SetFloat ("Speed", Mathf.Abs(direction.x));

		rb2d.velocity = new Vector2 (direction.x * maxSpeed, rb2d.velocity.y);

		if (direction.x > 0 && !facingRight) {
			Flip ();
		} else if (direction.x < 0 && facingRight) {
			Flip ();
		}
	}

	void Update ()
	{
		if (joystickMovement != null) {
			direction = joystickMovement.inputDirection; //InputDirection can be used as per the need of your project
		}

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
