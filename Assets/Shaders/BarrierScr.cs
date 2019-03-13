using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierScr : MonoBehaviour {
	public int coinRequirement = 2;
	public float barrierScale = 10f;
	public float textureSize = 1.5f;
	public float peakIntensity = 3.0f;
	public float decay = 0.1f;
	public float propagationRate = 2.0f;
	public Vector2 hitMicroAdjustment = new Vector2(0.05f, 0);

	public Vector2 hitCenter = new Vector2(0, 0);
	public float hitTimeSince = 0;
	public float intensity = 0;

	Material material;

	void Start() {
		material = GetComponent<Renderer>().material;
		material.SetFloat("_Scale", barrierScale);
	}

	public void Hit(Vector2 where) {
		hitCenter = where;
		hitTimeSince = 0;
		intensity = peakIntensity;
	}

	void Update () {
		hitTimeSince += propagationRate * Time.deltaTime;
		intensity *= Mathf.Pow(decay, Time.deltaTime);
		material.SetVector("_Params", new Vector4(
			hitCenter.x,
			hitCenter.y,
			Mathf.Sqrt(hitTimeSince),
			intensity
		));
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (other.gameObject.tag == "Player") {
			Vector3 localContact = transform.InverseTransformPoint(other.gameObject.transform.position);
			// Transform into UV space by dividing by the fact that our texture takes up more space than just (-0.5, 0.5).
			// Then add in (0.5, 0.5) to remap to UVs which range in [0, 1].
			localContact /= textureSize;
			localContact += new Vector3(0.5f, 0.5f, 0);
			Hit(new Vector2(localContact.x, localContact.y) + hitMicroAdjustment);
		}
	}
}

