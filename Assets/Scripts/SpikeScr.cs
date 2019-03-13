using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScr : MonoBehaviour {
	public float shineRefractory = 2.0f;
	public float poissonConstant = 0.1f;

	float refractory = 0;

	// Fixed references.
	Animator anim;

	void Start() {
		anim = GetComponent<Animator>();
	}

	void Shine() {
		if (refractory != 0)
			return;
		refractory = shineRefractory;
		anim.SetTrigger("Shine");
	}

	void Update() {
		refractory = Mathf.Max(0, refractory - Time.deltaTime);

		if (Random.Range(0.0f, 1.0f) < poissonConstant * Time.deltaTime)
			Shine();
	}
}
