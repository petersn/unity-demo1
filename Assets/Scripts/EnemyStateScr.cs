using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyTransitionType {
    TRANSITION_RUN,
    TRANSITION_DASH,
    TRANSITION_TELEPORT
};

[System.Serializable]
public class EnemyTransition : System.Object {
	public EnemyTransitionType transitionType;
	public EnemyStateScr nextState;
}

public class EnemyStateScr : MonoBehaviour {
	public float delay = 0;
	public bool doThrow = false;
	public EnemyTransition[] transitions;

	void DrawArrow(Vector2 a, Vector2 b) {
		Gizmos.DrawLine(a, b);
		float headScale = 0.3f;
		Vector2 back = (a - b).normalized * headScale;
		Vector2 perp = new Vector2(back.y, -back.x);
		Gizmos.DrawLine(b, b + perp + back);
		Gizmos.DrawLine(b, b - perp + back);
	}

#if UNITY_EDITOR
    void OnDrawGizmosSelected() {
    	foreach (EnemyTransition transition in transitions) {
            Gizmos.color = Color.blue;
            DrawArrow(transform.position, transition.nextState.transform.position);
        }
    }
#endif
}
