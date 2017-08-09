using UnityEngine;

[ExecuteInEditMode]
public class ScaleWidthCamera : MonoBehaviour {

	public int targetWidth = 640;
	public float pixelsToUnits = 100;

	private Camera gameCamera;

	void Start()
	{
		gameCamera = GetComponent<Camera> ();
	}

	
	// Update is called once per frame
	void Update () {
		int height = Mathf.RoundToInt (targetWidth / (float)Screen.width * Screen.height);

		gameCamera.orthographicSize = height / pixelsToUnits / 2;
	}
}
