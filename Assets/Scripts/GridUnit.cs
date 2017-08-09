using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridUnit : MonoBehaviour {

	public float unitSize = 0.1f;
	public int numUnits = 4;
	public string gName;
	public bool canNavigateTo = true;

	void OnDrawGizmosSelected() {
		if (canNavigateTo)
			Gizmos.color = new Color (0f, 1f, 0f, 0.2f); // green
		else
			Gizmos.color = new Color (1f, 0f, 0f, 0.2f); // red
		Gizmos.DrawCube(transform.position, new Vector3(unitSize * numUnits, unitSize * numUnits, 0));
	}
}
