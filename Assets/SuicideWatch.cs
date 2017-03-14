using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuicideWatch : MonoBehaviour {
	void KillYourself(){
		Debug.Log ("I was told to die!  Well, fine.");
		Destroy(gameObject);
	}
}