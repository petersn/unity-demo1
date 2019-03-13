using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class NinjaScr : MonoBehaviour {
	public EnemyStateScr actionState;
	public GameObject shurikenPrefab;
	public float actionTimerCooldownAfterTransition = 0.8f;
	public float transitionTimerMinimumToAct = 0.3f;
	public float throwCooldown = 1.6f;
	public float forceScaling = 20f;
	public float frictionFactor = 1.0f;
	public float waypointEpsilon = 0.3f;

	// State.
	public bool facingRight = true;
	public bool aggro = false;
	public float transitionTimer = 0;
	public float actionTimer = 0;
	EnemyTransition currentTransition = null;
	EnemyStateScr nextState = null;

	// Fixed references.
	Rigidbody2D rb2D;
	Animator anim;

	void Start() {
		rb2D = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		transitionTimer = actionState.delay;
	}

	void DoTransition() {
		transitionTimer = Mathf.Infinity;
		int transitionIndex = Random.Range(0, actionState.transitions.Length);
		currentTransition = actionState.transitions[transitionIndex];
		Assert.IsNotNull(currentTransition.nextState);
		// Copy in the new state.
		nextState = currentTransition.nextState;
		// Handle teleports.
		if (currentTransition.transitionType == EnemyTransitionType.TRANSITION_TELEPORT) {
			transform.position = actionState.transform.position;
		}
	}

	void DoAction() {
		if (actionState.doThrow) {
			Throw();
			actionTimer = throwCooldown;
		}
	}

	void FixedUpdate() {
		// Adjust the local scale to match our orientation.
		Vector3 newScale = transform.localScale;
		newScale.x = facingRight ? 1 : -1;
		transform.localScale = newScale;

		// Update timers, and possibly transition.
		transitionTimer = Mathf.Max(0, transitionTimer - Time.fixedDeltaTime);
		if (transitionTimer == 0)
			DoTransition();
		actionTimer = Mathf.Max(0, actionTimer - Time.fixedDeltaTime);
		if (actionTimer == 0 && transitionTimer >= transitionTimerMinimumToAct && currentTransition == null)
			DoAction();

		Vector2 force = new Vector2(0, 0);

		// If we're in a run state then run towards our target.
		if (currentTransition != null && currentTransition.transitionType == EnemyTransitionType.TRANSITION_RUN) {		
			Vector3 towardsGoal = actionState.transform.position - transform.position;
			if (towardsGoal.magnitude > waypointEpsilon) {
				anim.SetBool("Running", true);
				// Leave slackness at towardsGoal.x == 0 to make us not have a preferred orientation.
				if (towardsGoal.x > 0)
					facingRight = true;
				if (towardsGoal.x < 0)
					facingRight = false;
				force = new Vector2(towardsGoal.x, towardsGoal.y).normalized * forceScaling;
			} else {
				// Cancel the run if we reach our destination.
				Assert.IsNotNull(nextState);
				actionState = nextState;
				transitionTimer = actionState.delay;
				actionTimer = actionTimerCooldownAfterTransition;
				currentTransition = null;
				anim.SetBool("Running", false);
			}
		} else {
			anim.SetBool("Running", false);
		}

		// If we're not in a transition state then face the player.
		if (currentTransition == null) {
			GameObject player = GameObject.FindWithTag("Player");
			facingRight = player.transform.position.x > transform.position.x;
		}

		force -= rb2D.velocity * frictionFactor;
		rb2D.AddForce(force);
	}

	IEnumerator ThrowCoroutine(float delay) {
		yield return new WaitForSeconds(delay);
		Vector3 createPosition = transform.position + new Vector3(facingRight? 1 : -1, -0.03f, 0);
		GameObject shuriken = Instantiate(shurikenPrefab, createPosition, Quaternion.identity);
		shuriken.GetComponent<ShurikenScr>().facingRight = facingRight;
	}

	void Throw() {
		anim.SetTrigger("Throw");
		// XXX: I'm currently hardcoding this, but I shouldn't.
		StartCoroutine(ThrowCoroutine(0.125f * 3));
	}

	void OnCollisionEnter2D(Collision2D other) {
		// Kill players we collide with.
		if (other.gameObject.tag == "Player")
			other.gameObject.GetComponent<PlayerCon>().KillPlayer();
	}

	void Update() {
//		if (Input.GetKeyDown(KeyCode.Space))
	}
}

