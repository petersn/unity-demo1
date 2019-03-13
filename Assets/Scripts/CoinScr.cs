using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CoinScr : MonoBehaviour {
	public int coinID = 0;
	bool activated = false;
	bool isCollected;

	// Fixed references.
	Animator anim;

	void Start() {
		anim = GetComponent<Animator>();

		isCollected = PersistentStoreScr.mainStore.GetCoinStatus(coinID);
		anim.SetBool("IsCollected", isCollected);
	}

    void OnTriggerEnter2D(Collider2D other) {
    	if (other.gameObject.tag != "Player")
	    	return;

	    // Trigger collection.
       	if (activated)
    		return;
       	activated = true;
       	// Wink us out of existence.
       	GameObject mainCamera = GameObject.FindWithTag("MainCamera");
		mainCamera.GetComponent<RippleEffect>().Emit(transform.position);
       	gameObject.AddComponent<WinkOut>();
       	PersistentStoreScr.mainStore.CollectCoin(coinID);
    }

#if UNITY_EDITOR
	void OnDrawGizmos() {
		GUIStyle style = new GUIStyle();
		style.fontSize = 20;
		style.alignment = TextAnchor.MiddleCenter;
		Handles.Label(transform.position, coinID.ToString(), style);
	}
#endif
}

