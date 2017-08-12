 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public Transform groundCheckCircle; // used to check if ground is overlapping
	public LayerMask whatIsGound; // Layer Mask used for calculating if this is on ground
	public LayerMask whatIsGrid; // Layer Mask used for masking only GridUnits
	public float jumpForce = 200f; // force that determines how high this jumps
	public float maxSpeed = 0.9f; // maximum speed
	public float accel = 0.1f; // how fast this enemy accelerates
	public float speedVar = 0.1f; // how much the speed varies between instantiations of this class

	bool facingRight = true; // used to flip transform for running left/right
	bool grounded = false; // used to determine if can jump
	float groundRadius = 0.05f; // radius of overlap circle used to determine if this is grounded

	private GameObject player;
	private Rigidbody2D rb2d;
	private List<PathFind.Point> pathToPlayer = new List<PathFind.Point>();
	private PathFind.Grid grid;
	private GameObject prevPlayerGrid = null;
	private float currentSpeed = 0f;
	private Vector2 moveVector = Vector2.zero;

	public GameObject currentGrid;
	private GameObject playerGrid;

	int gridRows; // NavGrid # of rows
	int gridCols; // NavGrid # of cols
	bool[,] tilesmap; // tiles in grid used for pathfinding

	// Use this for initialization
	void Start () {
		rb2d = GetComponent<Rigidbody2D> ();
		player = GameObject.Find ("Player");

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

	}

	void FixedUpdate()
	{
		if (currentGrid) {
			// set the flag if player is standing on ground (hence can jump)
			//grounded = Physics2D.OverlapCircle (groundCheckCircle.position, groundRadius, whatIsGound);
			grounded = (true ? rb2d.velocity.y == 0 : false);

			// get the current grid the player is on
			playerGrid = player.GetComponent<PlayerController> ().currentGrid;

			// if the player has changed grid squares, update the path from enemy to the player
			if (playerGrid != prevPlayerGrid) {
				prevPlayerGrid = playerGrid;
				FindNewPath (playerGrid, currentGrid);
			}

			// vector toward next path point
			//Debug.Log (string.Format("x:{0}, y:{1}", pathToPlayer[0].x, pathToPlayer[0].y));
			if (pathToPlayer.Count > 0)
				moveVector = new Vector2 (pathToPlayer [0].x, pathToPlayer [0].y) - GetGridCoord (currentGrid);
			//Debug.Log (moveVector.ToString ());

			if (grounded) {
				//Debug.Log (string.Format("Move Vector: {0},{1}", moveVector.x, moveVector.y));
				Vector3 gridPos = GetGridPosition (GetGridCoord (currentGrid));
				float distToCurrentCenter = gridPos.x - transform.position.x;
				float dir = distToCurrentCenter / Mathf.Abs (distToCurrentCenter);

				if (moveVector.y > 0 && moveVector.x != 0) { // if next point is diagonal up
					if (distToCurrentCenter > 0.02 || distToCurrentCenter < -0.02) {
						Debug.Log (distToCurrentCenter);
						// move to current grid center
						Move (dir);
					} else {
						// jump diagonally toward next point
						Debug.Log ("Jump Diagonal");
						Jump (dir);
					}
				} else if (moveVector.y < 0 && moveVector.x != 0) { // else if next point is diagonal down
					// move in x toward next point
					Debug.Log ("Move in x");
					Move (moveVector.x);
				} else if (moveVector.y > 0 && moveVector.x == 0) { // else if next point is above
					if (distToCurrentCenter > 0.02 || distToCurrentCenter < -0.02) {
						// move to current grid center
						Move (dir);
					} else {
						// jump vertically
						Debug.Log ("Jump Up");
						Jump (0);
					}
				} else if (moveVector.y < 0 && moveVector.x == 0) { //else if next point is below
					// move right/left toward next point
					Debug.Log ("Move below");
				} else {
					// move in x toward next point
					Move (moveVector.x);
				}
			}
		}
	}

	void FindNewPath(GameObject playerGrid, GameObject currentGrid)
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
		List<PathFind.Point> newList = PathFind.Pathfinding.FindPath(grid, _from, _to);

		if (newList.Count > 0)
			pathToPlayer = newList;
	}

	void Move(float moveDir)
	{
		currentSpeed += accel; // speed up
		var moveSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed); // cap speed at maxSpeed
		//Debug.Log(string.Format("moveSpeed: {0}", moveSpeed));

		if (moveDir > 0 && !facingRight)
			Flip ();
		else if (moveDir < 0 && facingRight)
			Flip ();

		rb2d.velocity = new Vector2 (moveSpeed * moveDir, rb2d.velocity.y);
	}

	void Jump(float dir)
	{
		rb2d.velocity.Set (0f, 0f);	
		rb2d.AddForce (new Vector2 (100 * dir, jumpForce));
	}

	// returns position of grid square given coords of grid sqaure in vector2
	Vector3 GetGridPosition(Vector2 grid)
	{
		// get the grid GameObject at those coords - coords are reversed-- width is col#, not x
		string gridName = "GridUnit_" + grid.y.ToString() + grid.x.ToString();
		GameObject gridPos = GameObject.Find (gridName);

		return gridPos.transform.position;
	}

	Vector2 GetGridCoord(GameObject grid)
	{
		string gridName = grid.name;
		int y = int.Parse(gridName [gridName.Length - 2].ToString());
		int x = int.Parse(gridName [gridName.Length - 1].ToString());

		return new Vector2 (x, y);
	}

	public void SetCurrentGrid(GameObject grid)
	{
		currentGrid = grid;
		if (playerGrid)
			FindNewPath (playerGrid, currentGrid);
	}

	void Flip()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

}
