using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetParentsTransform : MonoBehaviour {
	void Start(){
		StartCoroutine(DoThing());
	}

	IEnumerator DoThing(){
		yield return 12;
		gameObject.transform.localScale = gameObject.GetComponentInParent<Transform>().transform.localScale;
		DestroyImmediate(gameObject.GetComponent<GetParentsTransform>());
	}
}