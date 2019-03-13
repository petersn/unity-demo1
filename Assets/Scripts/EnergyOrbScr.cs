using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyOrbScr : MonoBehaviour {
	bool activated = false;

    void OnTriggerEnter2D(Collider2D other) {
    	if (other.gameObject.tag != "Player")
	    	return;

	    // Trigger collection.
       	if (activated)
    		return;
       	activated = true;
       	// Wink us out of existence.
       	gameObject.AddComponent<WinkOut>();
       	other.gameObject.GetComponent<PlayerCon>().GiveEnergy();
    }
}
