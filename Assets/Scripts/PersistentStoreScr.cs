using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentStoreScr : MonoBehaviour {
	public static PersistentStoreScr mainStore = null;

	Dictionary<int, bool> coinStatuses = new Dictionary<int, bool>();

	Vector2 pinchCenter;
	float pinchPhase = 0;
	float pinchRate = 0;

	void Awake() {
		if (mainStore == null) {
			mainStore = this;
			DontDestroyOnLoad(this);
		} else {
			Destroy(gameObject);
		}
	}

	public bool GetCoinStatus(int coinID) {
		if (!coinStatuses.ContainsKey(coinID)) {
			coinStatuses[coinID] = false;
		}
		return coinStatuses[coinID];
	}

	public void CollectCoin(int coinID) {
		coinStatuses[coinID] = true;
	}

	public int GetCoinCount() {
		int total = 0;
		foreach (KeyValuePair<int, bool> entry in coinStatuses) {
			if (entry.Value)
				total++;
		}
		return total;
	}

	IEnumerator ReloadCoroutine(float delay) {
		Debug.Log("Starting coroutine.");
		yield return new WaitForSeconds(delay);
		Debug.Log("Reloading.");
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		pinchRate *= -1;
	}

	void Update() {
		pinchPhase = Mathf.Max(0, Mathf.Min(1, pinchPhase + pinchRate * Time.deltaTime));
		GameObject mainCamera = GameObject.FindWithTag("MainCamera");
		mainCamera.GetComponent<RippleEffect>().SetPinch(pinchCenter, pinchPhase);
	}

	public void SetPinchCenter(Vector2 _pinchCenter) {
		pinchCenter = _pinchCenter;
	}

	public void ReloadLevelAfter(Vector2 _pinchCenter, float delay) {
		pinchCenter = _pinchCenter;
		pinchPhase = 0.0f;
		pinchRate = 1.0f / delay;
		StartCoroutine(ReloadCoroutine(delay));
	}
}

