using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GateTriggerScr : MonoBehaviour {
	BarrierScr barrier;
	
	void Start() {
		barrier = transform.parent.Find("EnergyBarrier").GetComponent<BarrierScr>();
		Assert.IsNotNull(barrier);
	}

	void DestroyBarrier() {
		Debug.Log("Destroying barrier!");
		Destroy(gameObject);
		Destroy(barrier.gameObject);
		GameObject mainCamera = GameObject.FindWithTag("MainCamera");
		mainCamera.GetComponent<RippleEffect>().Emit(transform.parent.position);
	}

    void OnTriggerEnter2D(Collider2D other) {
    	if (other.gameObject.tag != "Player")
	    	return;

		// Check the player's coin count.
		if (PersistentStoreScr.mainStore.GetCoinCount() >= barrier.coinRequirement)
			DestroyBarrier();
    }
}
