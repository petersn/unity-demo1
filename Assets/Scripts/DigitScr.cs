using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DigitScr : MonoBehaviour {
	public Sprite[] digitSprites;

	void Start() {
		Assert.IsTrue(digitSprites.Length == 10);
	}

	public void SetValue(int digit, Color color) {
		Assert.IsTrue(0 <= digit && digit <= 9);
		SpriteRenderer renderer = GetComponent<SpriteRenderer>();
		renderer.sprite = digitSprites[digit];
		renderer.color = color;
	}
}

