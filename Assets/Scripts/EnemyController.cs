 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public Transform groundCheckCircle; // used to check if ground is overlapping
	public LayerMask whatIsGound; // Layer Mask used for calculating if this is on ground
	public LayerMask whatIsGrid; // Layer Mask used for masking only GridUnits
	public float jumpForce = 200f; // force that determines how high this jumps
	public float maxSpeed = 1f; // maximum speed
	public float accel = 0.1f; // how fast this enemy accelerates
	public float speedVar = 0.1f; // how much the speed varies between instantiations of this class

	bool facingRight = true; // used to flip transform for running left/right
	bool grounded = false; // used to determine if can jump
	float groundRadius = 0.05f; // radius of overlap circle used to determine if this is grounded

	private Transform player;
	private Rigidbody2D rb2d;
	private List<PathFind.Point> pathToPlayer;
	private PathFind.Grid grid;
	private float currentSpeed = 0f;

	int gridRows; // NavGrid # of rows
	int gridCols; // NavGrid # of cols
	bool[,] tilesmap; // tiles in grid used for pathfinding

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D> ();
		player = GameObject.Find ("Player").transform;

		// Set up the pathfinding grid
		GameObject navGrid = GameObject.Find("NavGrid");
		NavGrid navGridScript = navGrid.GetComponent<NavGrid> (); // get the script attached
		gridRows = navGridScript.gridRows;
		gridCols = navGridScript.gridCols;

		tilesmap = new bool[gridCols, gridRows]; // # of tiles in grid used for pathfinding

		// get all grid squares and read if they are navigable
		var gridUnits = navGrid.GetComponentsInChildren<GridUnit> (true);
		foreach (GridUnit unit in gridUnits) {
			var r = int.Parse(unit.gName [0].ToString());
			var c = int.Parse(unit.gName [1].ToString());
			tilesmap [c, r] = unit.canNavigateTo;
		}

		// create a grid object for pathfinding
		grid = new PathFind.Grid(gridCols, gridRows, tilesmap);
	}

	void Update()
	{
		// set the flag if player is standing on ground (hence can jump)
		grounded = Physics2D.OverlapCircle (groundCheckCircle.position, groundRadius, whatIsGound);

		// get current grid squares under player and enemy
		GameObject currentGrid = Physics2D.OverlapCircle (gameObject.transform.position, 0.05f, whatIsGrid).gameObject;
		GameObject playerGrid = Physics2D.OverlapCircle (player.position, 0.05f, whatIsGrid).gameObject;

		// update the path from enemy to the player
		// TODO Should UpdatePath be run once every x seconds for performance?
		UpdatePath(playerGrid, currentGrid);

		if (pathToPlayer.Count > 0) { // if there is a list of path points
			Vector3 moveVector = GetNextCoord() - currentGrid.transform.position;
			if (moveVector.y != 0) { // if vector to next point has a y component
				if (moveVector.y > 0) { // if y vector is up
					if (Mathf.Abs (moveVector.x) < 0.01) { // if x distance to current grid center is small
						Jump ();
					} else {
						Move (moveVector); // move in x toward next path point
					}
				} else { // (y vector is down)
					Move (moveVector); // move in x toward next path point
				}
			} else { // else (next path point is only in the x direction)
				Move(moveVector); // move toward next position
			}
		} else {
			Vector3 playerVector = playerGrid.transform.position - currentGrid.transform.position;
			Move(playerVector); //   move in x toward player
		}
	}

	void UpdatePath(GameObject playerGrid, GameObject currentGrid)
	{
		// get the script component so we can get name
		GridUnit playerGridScript = playerGrid.GetComponent<GridUnit> ();
		GridUnit enemyGridScript = currentGrid.GetComponent<GridUnit> ();

		// get grid coords from gameobject belonging to collider
		int pc = int.Parse(playerGridScript.gName[1].ToString());
		int pr = int.Parse(playerGridScript.gName[0].ToString());
		int ec = int.Parse(enemyGridScript.gName[1].ToString());
		int er = int.Parse(enemyGridScript.gName[0].ToString());

		// create source and target points
		PathFind.Point _from = new PathFind.Point(ec, er);
		PathFind.Point _to = new PathFind.Point(pc, pr);

		// get path
		// path will either be a list of Points (x, y), or an empty list if no path is found.
		pathToPlayer = PathFind.Pathfinding.FindPath(grid, _from, _to); // TODO dont set pathToPlayer if no path is found
	}

	void Move(Vector3 moveTo)
	{
		var moveDir = moveTo.x / Mathf.Abs(moveTo.x); // get the sign of x component of vector

		currentSpeed += accel; // speed up
		var moveSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed); // cap speed at maxSpeed
		Debug.Log(string.Format("moveSpeed: {0}", moveSpeed));

		if (moveDir > 0 && !facingRight)
			Flip ();
		else if (moveDir < 0 && facingRight)
			Flip ();

		rb2d.velocity = new Vector2 (moveSpeed * moveDir, rb2d.velocity.y);

		/*
		if (moveVector.y > jumpHeight) { // if enemy is close in x and player is at least a platform above
			RaycastHit2D hit = Physics2D.Raycast (transform.position, Vector2.up, 0.2f, whatIsGound);
			if (grounded && hit) {
				rb2d.AddForce (new Vector2 (0f, jumpForce));
			}
		}
		*/

		//Debug.Log (string.Format("Next Point: {0},{1}", nextPoint.x, nextPoint.y));
	}

	void Jump()
	{
		if (grounded)
			rb2d.AddForce (new Vector2 (0f, jumpForce));
	}

	Vector3 GetNextCoord()
	{
		var nextPoint = pathToPlayer [0]; // next path point with x,y coords of the grid square

		// get the grid GameObject at those coords - coords are reversed-- width is col#, not x
		string gridName = "GridUnit_" + nextPoint.y.ToString() + nextPoint.x.ToString();
		GameObject grid = GameObject.Find (gridName);

		return grid.transform.position;
	}

	void Flip()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

}
