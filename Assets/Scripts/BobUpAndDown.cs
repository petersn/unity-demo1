using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobUpAndDown : MonoBehaviour {
	public float bobSpeed = 0.5f;
	public float bobAmplitude = 1.5f;

	public float bobPhase = 0;
	public int lastBobPixels = 0;

	void Update() {
		bobPhase += bobSpeed * Time.deltaTime;
		bobPhase %= 2 * Mathf.PI;
		int newBobPixels = (int)(bobAmplitude * Mathf.Sin(bobPhase));
		if (newBobPixels != lastBobPixels) {
			float deltaY = 0.125f * (newBobPixels - lastBobPixels);
			transform.position = new Vector3(transform.position.x, transform.position.y + deltaY, transform.position.z);
			lastBobPixels = newBobPixels;
		}
	}
}

