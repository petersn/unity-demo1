using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndScr : MonoBehaviour {
	public float lightupRate = 4.0f;
	public float glowMaxAlpha = 0.1f;
	public float glowAlphaRate = 0.05f;
	public float youWinAlphaRate = 0.3f;
	public float youWinTextFadeInTime = 5.0f;
	public GameObject[] obelisks;
	public GameObject light;
	public GameObject glow;
	public GameObject youWinText;

	bool active = false;
	float winTime = 0;
	float startingIntensity;

	void Start() {
		// Save the initial intensity to restore later.
		startingIntensity = light.GetComponent<Light>().intensity;
		light.GetComponent<Light>().intensity = 0;
		SetAlpha(glow, 0);
		SetAlpha(youWinText, 0);
	}

	void SetAlpha(GameObject obj, float alpha) {
		SpriteRenderer renderer = obj.GetComponent<SpriteRenderer>();
		Color color = renderer.color;
		color.a = alpha;
		renderer.color = color;
	}

	void WinLevel() {
		GameObject.Find("Player").GetComponent<PlayerCon>().StartFloating();
		// Light up all the obelisks.
		foreach (GameObject obelisk in obelisks)
			obelisk.GetComponent<Animator>().SetTrigger("LightUp");
		// Turn on the light.
		active = true;
		FlickerLight flicker = light.AddComponent<FlickerLight>();
		flicker.hueLow = 0.7f;
		flicker.hueHigh = 0.75f;
	}

	void Update() {
		if (active) {
			winTime += Time.deltaTime;

			float intensity = light.GetComponent<Light>().intensity;
			intensity = Mathf.Min(startingIntensity, intensity + lightupRate * Time.deltaTime);
			light.GetComponent<Light>().intensity = intensity;

			float alpha = glow.GetComponent<SpriteRenderer>().color.a;
			alpha = Mathf.Min(glowMaxAlpha, alpha + glowAlphaRate * Time.deltaTime);
			SetAlpha(glow, alpha);
		}
		if (winTime > youWinTextFadeInTime) {
			float alpha = youWinText.GetComponent<SpriteRenderer>().color.a;
			alpha = Mathf.Min(1, alpha + youWinAlphaRate * Time.deltaTime);
			SetAlpha(youWinText, alpha);
		}
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player")
			WinLevel();
	}
}
