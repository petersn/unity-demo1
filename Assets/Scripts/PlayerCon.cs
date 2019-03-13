using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCon : MonoBehaviour {
	// Configuration.
	public float forceScaling = 20f;
	public float jumpHeight = 6f;
	public int jumpRefractoryFrames = 2;
	public float groundednessCheckRadius = 0.48f;
	public float groundedDistance = 0.56f;
	public float landingVelocityDustThreshold = 10f;
	public float dashDuration = 0.2f;
	public float dashSpeed = 15.0f;
	public float frictionFactor = 1.0f;
	public Vector3 cameraOffset = new Vector3(0, 0, 0);
	public LayerMask groundMask;
	public GameObject dustCloudPrefab;

	// State.
	public bool facingRight = true;
	public bool grounded = false;
	public bool hasDash = false;
	public Vector2 groundedCenter;
	public int jumpRefractory = 0;
	public float dashStatus = 0;
	public int energy = 0;
	public bool dying = false;
	public bool floating = false;

	// Fixed references.
	Rigidbody2D rb2D;
	Animator anim;

	void Start() {
		rb2D = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		PersistentStoreScr.mainStore.SetPinchCenter(transform.position);
	}

	void MakeDust(bool flipOrientation=false) {
		GameObject dust = Instantiate(dustCloudPrefab, new Vector3(groundedCenter.x, groundedCenter.y, 0), Quaternion.identity);
		// Copy our mirroredness status to the dust cloud.
		Vector3 scale = transform.localScale;
		if (flipOrientation)
			scale.x *= -1;
		dust.transform.localScale = scale;
	}

	public void GiveEnergy() {
		energy++;
	}

	bool CanJump() {
		return grounded && jumpRefractory == 0 && !floating;
	}

	void DoJump() {
		jumpRefractory = jumpRefractoryFrames;
		float acceleration = Mathf.Abs(Physics2D.gravity.y);
		float jumpImpulse = Mathf.Sqrt(2 * jumpHeight * acceleration);
		rb2D.AddForce(new Vector2(0, jumpImpulse), ForceMode2D.Impulse);
	}

	bool CanDash() {
		return hasDash && !floating;
	}

	void DoDash() {
		hasDash = false;
		dashStatus = dashDuration;
//		if (grounded)
		MakeDust(flipOrientation: true);
		anim.SetTrigger("InitiateDash");
	}

	public void KillPlayer() {
		if (dying)
			return;
		dying = true;
		//Destroy(gameObject);
		gameObject.AddComponent<WinkOut>();
		PersistentStoreScr.mainStore.ReloadLevelAfter(transform.position, 0.5f);
	}

	public void StartFloating() {
		floating = true;
		rb2D.velocity = new Vector2(0, 0);
	}

	void FixedUpdate() {
		// Compute groundedness.
		//RaycastHit2D hit = Physics2D.CircleCast(transform.position, groundednessCheckRadius, new Vector2(0, -1), distance: Mathf.Infinity, layerMask: groundMask);
		RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(1, 1), 0.0f, new Vector2(0, -1), distance: Mathf.Infinity, layerMask: groundMask);
		bool oldGrounded = grounded;
		grounded = hit.distance <= groundedDistance;
		if (grounded) {
			groundedCenter = hit.point + new Vector2(0, 1);
		} else {
			groundedCenter = transform.position;
		}

		// Decrement counters.
		if (jumpRefractory > 0)
			jumpRefractory--;
		dashStatus = Mathf.Max(0, dashStatus - Time.fixedDeltaTime);

		if (grounded)
			hasDash = true;

		// Apply a force based on horizontal input.
		float horizontal = Input.GetAxis("Horizontal") * forceScaling;
		// No control if floating;
		if (floating)
			horizontal = 0;

		if (dashStatus == 0) {
			Vector2 force = new Vector2(horizontal, 0);
			force -= rb2D.velocity * frictionFactor;
			rb2D.AddForce(force);
			if (horizontal > 0)
				facingRight = true;
			if (horizontal < 0)
				facingRight = false;
		} else {
			rb2D.velocity = new Vector2(dashSpeed * (facingRight ? +1 : -1), 0);
		}

		// If floating accelerate upwards.
		if (floating) {
			float gravityForce = rb2D.mass * Mathf.Abs(Physics2D.gravity.y);
			rb2D.AddForce(new Vector2(0, 2 + gravityForce));
		}

		// Adjust the local scale to match our orientation.
		Vector3 newScale = transform.localScale;
		newScale.x = facingRight ? 1 : -1;
		transform.localScale = newScale;

		// Propagate information to our animator.
		anim.SetFloat("Speed", Mathf.Abs(horizontal));
		anim.SetFloat("VelocityY", rb2D.velocity.y);
		anim.SetBool("Grounded", grounded);
		anim.SetBool("Dashing", dashStatus > 0);
		// If we became grounded then trigger landed.
		if (grounded && !oldGrounded) {
			anim.SetTrigger("Landing");
			if (rb2D.velocity.y < -landingVelocityDustThreshold)
				MakeDust();
		}
	}

	void Update() {
		// Handle jump input.
		if (CanJump() && Input.GetKeyDown(KeyCode.LeftShift))
			DoJump();

		if (CanDash() && Input.GetKeyDown(KeyCode.LeftControl))
			DoDash();

		// XXX: Should this be in LateUpdate?
		if (!dying) {
			GameObject mainCamera = GameObject.FindWithTag("MainCamera");
			mainCamera.transform.position = new Vector3(transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z) + cameraOffset;
			GameObject mainLight = GameObject.Find("MainLight");
			mainLight.transform.position = new Vector3(transform.position.x, transform.position.y, mainLight.transform.position.z);
		}

	}
}

