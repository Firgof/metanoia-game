using UnityEngine;

public class DestroyYourself : MonoBehaviour {
	void Awake () {
		DestroyImmediate(gameObject);
	}
}
