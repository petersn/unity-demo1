using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickerLight : MonoBehaviour {
	public float hueLow = 0.05f;
	public float hueHigh = 0.1f;
	public float saturationLow = 0.6f;
	public float saturationHigh = 0.8f;
	public float valueLow = 0.8f;
	public float valueHigh = 1.0f;
	public float changeCoef = 2.0f;
	public float newColorInterval = 0.1f;

	float newColorTimer = 0.0f;
	Color newColor;

	// Fixed references.
	Light light;

	void RandomizeTargetColor() {
		newColor = Color.HSVToRGB(Random.Range(hueLow, hueHigh), Random.Range(saturationLow, saturationHigh), Random.Range(valueLow, valueHigh));
	}

	void Start() {
		light = GetComponent<Light>();
	}

	void Update() {
		light.color = Color.Lerp(light.color, newColor, changeCoef * Time.deltaTime);
		newColorTimer = Mathf.Max(0, newColorTimer - Time.deltaTime);
		if (newColorTimer == 0) {
			newColorTimer = newColorInterval;
			RandomizeTargetColor();
		}
	}
}

