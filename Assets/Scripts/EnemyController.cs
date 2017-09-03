 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public Transform groundCheckCircle; // used to check if ground is overlapping
	public float jumpForce = 200f; // force that determines how high this jumps
	public float maxSpeed = 0.9f; // maximum speed
	public float accel = 0.1f; // how fast this enemy accelerates
	public float speedVar = 0.1f; // how much the speed varies between instantiations of this class
	public float AttackPerSec = 0.5f; // inverse seconds between attacks e.g. 2 = 1/2 = 0.5 secs
	public GameObject projectile;
	public float projectileSpeed = 75f;

	bool facingRight = true; // used to flip transform for running left/right
	bool grounded = false; // used to determine if can jump
	float groundRadius = 0.05f; // radius of overlap circle used to determine if this is grounded

	private GameObject player;
	private Rigidbody2D rb2d;
	private Animator anim;
	private List<PathFind.Point> pathToPlayer = new List<PathFind.Point>();
	private PathFind.Grid grid;

	private float currentSpeed = 0f;
	private Vector2 moveVector = Vector2.zero;

	private GameObject prevPlayerGrid = null;
	//private GameObject prevEnemyGrid = null;
	private GameObject enemyGrid; // grid square enemy is currently in
	private GridUnit enemyScript; // script attached to enemyGrid
	private Vector2 nextGridCoord; // coords of next grid square enemy will move toward
	private GameObject playerGrid; // grid square the player is currently in
	//private int pathIndex = 0;
	private float secsPerAttack;
	private float attackCooldown = 0f;

	int gridRows; // NavGrid # of rows
	int gridCols; // NavGrid # of cols
	bool[,] tilesmap; // tiles in grid used for pathfinding

	// Use this for initialization
	void Awake ()
	{
		rb2d = GetComponent <Rigidbody2D> ();
		player = GameObject.Find ("Player");
		anim = GetComponent <Animator> ();

		secsPerAttack = 1 / AttackPerSec;
		attackCooldown = 0;

		maxSpeed = maxSpeed + Random.Range (-speedVar, speedVar);

		// Set up the pathfinding grid
		GameObject navGrid = GameObject.Find ("NavGrid");
		NavGrid navGridScript = navGrid.GetComponent <NavGrid> (); // get the script attached
		gridRows = navGridScript.gridRows;
		gridCols = navGridScript.gridCols;

		tilesmap = new bool[gridCols, gridRows]; // # of tiles in grid used for pathfinding

		// get all grid squares and read if they are navigable
		var gridUnits = navGrid.GetComponentsInChildren <GridUnit> (true);
		foreach (GridUnit unit in gridUnits) {
			var r = unit.row;
			var c = unit.col;
			tilesmap [c, r] = unit.canNavigateTo;
		}

		// create a grid object for pathfinding
		grid = new PathFind.Grid (gridCols, gridRows, tilesmap);

		InvokeRepeating ("Shoot", secsPerAttack, secsPerAttack);
	}

	void Shoot ()
	{
		GameObject bullet = GameObject.Instantiate (projectile, transform.position, Quaternion.identity) as GameObject;
		Vector3 shotDir = player.transform.position - transform.position; // direction toward player
		shotDir.Normalize (); // make this length 1
		bullet.GetComponent<Rigidbody2D> ().AddForce (shotDir * projectileSpeed); // add force to projectile
		anim.SetTrigger ("Attack");
	}

	void FixedUpdate ()
	{
		// set the flag if player is standing on ground (hence can jump)
		//grounded = Physics2D.OverlapCircle (groundCheckCircle.position, groundRadius, whatIsGound);
		grounded = rb2d.velocity.y == 0 ? true : false;

		// get the current grid the player is on
		playerGrid = player.GetComponent <PlayerController> ().currentGrid;

		// if the player has changed grid squares, update the path from enemy to the player
		if (enemyScript) {
			if (playerGrid != prevPlayerGrid) {
				prevPlayerGrid = playerGrid;
				FindNewPath ();
			}

			// vector toward next path point
			//Debug.Log (string.Format("x:{0}, y:{1}", pathToPlayer[0].x, pathToPlayer[0].y));
			Vector2 curGridCoord = new Vector2 (enemyScript.col, enemyScript.row);
			moveVector = nextGridCoord - curGridCoord;
		}

		float xDist = GetGridPos (nextGridCoord).x - transform.position.x;
		float dir = Mathf.Sign (xDist);
		//Debug.Log (string.Format ("moveVector: {0}, xDist: {1}, dir:{2}", moveVector, xDist, dir));
		if (grounded) {
			if (moveVector.y > 0 && moveVector.x != 0) { // if next point is diagonal up
				if (Mathf.Abs(xDist) > 0.43) {
					// move to current grid center
					Move (dir);
				} else {
					// jump diagonally toward next point
					Jump (dir);
				}
			} else if (moveVector.y < 0 && moveVector.x != 0) { // else if next point is diagonal down
				// move in x toward next point
				Move (dir);
			} else if (moveVector.y > 0 && moveVector.x == 0) { // else if next point is above
				if (Mathf.Abs (xDist) > 0.03) {
					// move to current grid center
					Move (dir);
				} else {
					// jump vertically
					Jump (0);
				}
			} else if (moveVector.y < 0 && moveVector.x == 0) { //else if next point is below
				// move right/left toward next point
				float randDir = Mathf.Sign (Random.Range (-1f, 1f));
				Vector2 newCoord = new Vector2 (enemyScript.col + randDir, enemyScript.row);
				if (GameObject.Find (string.Format ("GridUnit_{0}-{1}", newCoord.y, newCoord.x)))
					nextGridCoord = newCoord;
				else
					nextGridCoord = new Vector2 (enemyScript.col - randDir, enemyScript.row);
				Move (dir);
			} else {
				// move in x toward next point if havent reached attack position
				if (pathToPlayer.Count > 1) {
					Move (dir);
				}
			}
		}
		//Debug.Log (string.Format ("Moving toward w:{0},h:{1}", nextGridCoord.x, nextGridCoord.y));
	}

	void FindNewPath ()
	{
		float dir = Mathf.Sign (transform.position.x - player.transform.position.x);
		GameObject target = GetAdjGrid (playerGrid, dir);
		//Debug.Log (string.Format("New player target: {0}", target.name));
		// get the script component so we can get row,col
		GridUnit playerScript = target.GetComponent <GridUnit> ();

		// create source and target points
		PathFind.Point _from = new PathFind.Point (enemyScript.col, enemyScript.row);
		PathFind.Point _to = new PathFind.Point (playerScript.col, playerScript.row);

		// get path
		// path will either be a list of Points (x, y), or an empty list if no path is found.
		List<PathFind.Point> newList = PathFind.Pathfinding.FindPath (grid, _from, _to);
		//Debug.Log ("FindNewPath Called");

		if (newList.Count > 0) {
			pathToPlayer = newList;
			nextGridCoord = new Vector2 (pathToPlayer [0].x, pathToPlayer [0].y);
			//pathIndex = 0;
			//Debug.Log (string.Format ("Next grid coord; {0}", nextGridCoord.ToString()));
		}
	}

	void Move (float moveDir)
	{
		currentSpeed += accel; // speed up
		float moveSpeed = Mathf.Clamp (currentSpeed, 0f, maxSpeed); // cap speed at maxSpeed

		if (moveDir > 0 && !facingRight)
			Flip ();
		else if (moveDir < 0 && facingRight)
			Flip ();

		rb2d.velocity = new Vector2 (moveSpeed * moveDir, rb2d.velocity.y);
		//Debug.Log (string.Format ("Moving {0}", moveSpeed * moveDir));
	}

	void Jump (float dir)
	{
		//rb2d.velocity.Set (0f, 0f);
		rb2d.AddForce (new Vector2 (50f * dir, jumpForce));
		//Debug.Log (string.Format ("Jumping {0}", dir));
	}

	void Flip ()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	// returns the position in world space of a given game object
	Vector3 GetGridPos (Vector2 gridCoord)
	{
		string gridName = string.Format ("GridUnit_{0}-{1}", gridCoord.y, gridCoord.x);
		GameObject grid = GameObject.Find (gridName);
		if (!grid) // if it exists
			return transform.position; // return the transform position

		return grid.transform.position; // return the gridunit transform position
	}

	// returns the adjacent grid square given a grid object and direction
	GameObject GetAdjGrid (GameObject grid, float dir)
	{
		GridUnit script = grid.GetComponent <GridUnit> ();
		string newName = string.Format ("GridUnit_{0}-{1}", script.row, script.col + dir);
		return GameObject.Find (newName);
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.gameObject.name.StartsWith ("GridUnit")) {
			enemyGrid = col.gameObject;
			enemyScript = enemyGrid.GetComponent <GridUnit> ();
			//pathIndex++;
			//if (pathIndex >= pathToPlayer.Count) {
			//	pathIndex = pathToPlayer.Count;
			//}
			//Debug.Log ("OnTriggerEnter called.");
			if (playerGrid)
				FindNewPath ();
		}
	}
}
