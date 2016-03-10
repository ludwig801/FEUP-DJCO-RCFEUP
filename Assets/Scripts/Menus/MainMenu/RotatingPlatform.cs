using UnityEngine;
using System.Collections;

public class RotatingPlatform : MonoBehaviour {

	public float speed = 10f;
	private Vector3 vec = new Vector3 (0, -1, 0);

	// Update is called once per frame
	void Update () {
		transform.Rotate (vec , speed * Time.deltaTime);
	}
}
