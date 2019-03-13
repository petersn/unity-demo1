using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxElement : MonoBehaviour {
	public float parallaxFactorX = 0.5f;
	public float parallaxFactorY = 0.5f;
	
	Vector3 parallaxOffset;
	Vector3 startingPosition;

	void Start() {
		startingPosition = transform.position;
		GameObject mainCamera = GameObject.FindWithTag("MainCamera");
		parallaxOffset = mainCamera.transform.position - startingPosition;
	}

	void LateUpdate() {
		GameObject mainCamera = GameObject.FindWithTag("MainCamera");
		Vector3 cameraOffset = (mainCamera.transform.position - startingPosition) - parallaxOffset;
		transform.position = new Vector3(
			startingPosition.x + (1 - parallaxFactorX) * cameraOffset.x,
			startingPosition.y + (1 - parallaxFactorY) * cameraOffset.y,
			startingPosition.z
		);
	}
}

