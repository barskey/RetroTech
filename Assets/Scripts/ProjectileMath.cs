using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMath : MonoBehaviour
{
	public float MaxSpeed (float x)
	{
		float g = Mathf.Abs (Physics2D.gravity.y);
		float v = Mathf.Sqrt (x * g);

		Debug.Log (string.Format ("MaxSpeed: {0}", v));

		return v;
	}

	public float InitialSpeed (float ang, float x, float y)
	{
		float g = Mathf.Abs (Physics2D.gravity.y);
		float x2 = Mathf.Pow (x, 2);
		float theta = Mathf.Deg2Rad * ang;
		float costheta2 = Mathf.Pow (Mathf.Cos (theta), 2);

		float v = Mathf.Sqrt ((x2 * g) / (x * Mathf.Sin (2 * theta) - 2 * y * costheta2));

		Debug.Log (string.Format ("Vo to hit ({0},{1}): {2}", x, y, v));

		return v;
	}
}
