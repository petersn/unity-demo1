using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinkOut : MonoBehaviour {
	float duration = 0.2f;
	float verticalStretch = 5.0f;
	float exponent = 3.0f;
	float t = 0;

	void Start() {
		//GameObject mainCamera = GameObject.FindWithTag("MainCamera");
		//mainCamera.GetComponent<RippleEffect>().Emit(transform.position);
	}

	// Update is called once per frame
	void Update() {
		t += Time.deltaTime;
		// Compute the new transform.
		float completion = Mathf.Pow(t / duration, exponent);
		transform.localScale = new Vector3(1 - completion, 1 + (verticalStretch - 1) * completion, 1);
		// Destroy us if relevant.
		if (t > duration)
			Destroy(gameObject);
	}
}
