using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedZ : MonoBehaviour {
	public float fixedZ = 0.0f;

	void Start() {
		transform.position = new Vector3(transform.position.x, transform.position.y, fixedZ);
	}
}

