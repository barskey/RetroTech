using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
	public Transform groundCheckCircle;
	public LayerMask whatIsGound;
	public LayerMask whatIsGrid;
	public float jumpForce = 200f;
	public float speed;
	public float speedVar;

	bool facingRight = true;
	bool grounded = false;
	float groundRadius = 0.05f;
	float jumpHeight = 0.35f;

	private Transform player;
	private Rigidbody2D rb2d;
	private List<PathFind.Point> pathToPlayer;
	PathFind.Grid grid;

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
			var r = int.Parse(unit.name [0].ToString());
			var c = int.Parse(unit.name [1].ToString());
			tilesmap [c, r] = unit.canNavigateTo;
		}

		// create a grid for pathfinding
		grid = new PathFind.Grid(gridCols, gridRows, tilesmap);
	}

	void Update()
	{
	}
	
	void FixedUpdate ()
	{
		// set the flag if player is standing on ground (hence can jump)
		grounded = Physics2D.OverlapCircle (groundCheckCircle.position, groundRadius, whatIsGound);

		// get current grid squares under player and enemy
		Collider2D currentGrid = Physics2D.OverlapCircle (gameObject.transform.position, 0.05f, whatIsGrid);
		Collider2D playerGrid = Physics2D.OverlapCircle (player.position, 0.05f, whatIsGrid);

		// update the path from enemy to the player
		// TODO Should UpdatePath be run once every x seconds for performance?
		UpdatePath(playerGrid, currentGrid);

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

	void UpdatePath(Collider2D playerGrid, Collider2D currentGrid)
	{
		// get the script component so we can get name
		GridUnit playerGridScript = playerGrid.GetComponent<GridUnit> ();
		GridUnit enemyGridScript = currentGrid.GetComponent<GridUnit> ();

		// get grid coords from gameobject belonging to collider
		int pc = int.Parse(playerGridScript.name[1].ToString());
		int pr = int.Parse(playerGridScript.name[0].ToString());
		int ec = int.Parse(enemyGridScript.name[1].ToString());
		int er = int.Parse(enemyGridScript.name[0].ToString());

		// create source and target points
		PathFind.Point _from = new PathFind.Point(ec, er);
		PathFind.Point _to = new PathFind.Point(pc, pr);

		// get path
		// path will either be a list of Points (x, y), or an empty list if no path is found.
		pathToPlayer = PathFind.Pathfinding.FindPath(grid, _from, _to);
	}

}
