using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EditSnap : MonoBehaviour {
	public float gridSize = 1.0f;
	public float phase = 0.0f;
	public float fixedZ = 0.0f;

	void Update() {
		// Only execute in edit mode.
		if(Application.isPlaying)
			return;

		transform.position = new Vector3(
			(Mathf.Floor(transform.position.x / gridSize) + phase) * gridSize,
			(Mathf.Floor(transform.position.y / gridSize) + phase) * gridSize,
			fixedZ
		);
	}
}

