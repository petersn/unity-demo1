using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenScr : MonoBehaviour {
	public float moveSpeed = 10.0f;
	public float rotationSpeed = -500.0f;
	public bool facingRight = true;
	public bool killMode = true;

	// Fixed references.
	Rigidbody2D rb2D;

	void Start() {
		rb2D = GetComponent<Rigidbody2D>();
	}

	void Nullify() {
		killMode = false;
		rotationSpeed = 0.0f;
		rb2D.gravityScale = 1.0f;
		rb2D.velocity = -0.3f * rb2D.velocity;
		rb2D.velocity += new Vector2(0, 4.0f);
		GetComponent<CircleCollider2D>().enabled = false;
	}

	void FixedUpdate() {
		if (killMode)
			rb2D.velocity = new Vector2(moveSpeed * (facingRight ? +1 : -1), 0);
		// Figure out our distance from the camera, and if too far, destroy us.
		GameObject mainCamera = GameObject.FindWithTag("MainCamera");
		Vector3 viewportCoordinates = mainCamera.GetComponent<Camera>().WorldToViewportPoint(transform.position);
		Vector2 cameraOffset = new Vector2(viewportCoordinates.x - 0.5f, viewportCoordinates.y - 0.5f);
		if (cameraOffset.magnitude > 1)
			Destroy(gameObject);
	}

	void Update () {
		// Adjust the local scale to match our orientation.
		Vector3 newScale = transform.localScale;
		newScale.x = facingRight ? 1 : -1;
		transform.localScale = newScale;

		// Spin us.
		transform.Rotate(new Vector3(0, 0, facingRight ? 1 : -1) * rotationSpeed * Time.deltaTime);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Player" && killMode) {
			other.gameObject.GetComponent<PlayerCon>().KillPlayer();
		} else if (other.gameObject.layer == LayerMask.NameToLayer("SolidGround")) {
			Nullify();
		}
	}
}

