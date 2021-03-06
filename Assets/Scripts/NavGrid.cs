﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NavGrid : MonoBehaviour {
	public int gridRows;
	public int gridCols;
	public float gridSize;
	public float groundOffset = 0.1f;
	public Camera gameCam;
	public GameObject gridUnit;
	public bool drawAlways = false;

	private float wd;
	private float ht;
	private float startX;
	private float startY;

	void Awake()
	{
		ht = gameCam.orthographicSize * 2;
		wd = ((float)gameCam.pixelWidth / (float)gameCam.pixelHeight) * ht;
		gridRows = (int)Mathf.Round (ht / gridSize) - 1;
		gridCols = (int)Mathf.Round (wd / gridSize);

		if (gridCols % 2 == 0) { // even number of cols
			startX = (float)gridCols / 2 * gridSize * -1 + gridSize / 2; // add half-grid square offset
		} else {
			startX = (float)gridCols / 2 * gridSize * -1;
		}

		if (gridRows % 2 == 0) { // even number of rows
			startY = (float)gridRows / 2 * gridSize * -1 + gridSize / 2 + groundOffset;
		} else {
			startY = (float)gridRows / 2 * gridSize * -1 + gridSize / 2 + groundOffset - gridSize / 2; // add half-grid square offset
		}
	}

	public void CreateGrid()
	{
		for (int r = 0; r < gridRows; r++) {
			for (int c = 0; c < gridCols; c++) {
				string thisName = string.Format ("GridUnit_{0}-{1}", r, c);
				float x = startX + ((float)c * gridSize);
				float y = startY + ((float)r * gridSize);
				Vector3 pos = new Vector3 (x, y, 0f);
				GameObject unit = GameObject.Instantiate (gridUnit, pos, Quaternion.identity, gameObject.transform);
				unit.name = thisName;
				GridUnit script = unit.GetComponent<GridUnit> ();
				script.row = r;
				script.col = c;
			}
		}
	}
}
