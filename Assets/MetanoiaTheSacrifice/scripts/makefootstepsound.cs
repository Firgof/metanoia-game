using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AudioSource))]

public class makefootstepsound : MonoBehaviour {
	public float stepRate = 0.5f;
	public float stepCoolDown;
	public AudioClip footStep;
	public AudioSource audio;
	
	
	// Update is called once per frame
	void Update () {
		if (stepCoolDown-Time.deltaTime > 0f){
			stepCoolDown -= Time.deltaTime;
		}
		else{
			stepCoolDown = 0;
		}

		if ((Input.GetAxis("Horizontal") != 0f || Input.GetAxis("Vertical") != 0f) && stepCoolDown <= 0f){
			audio.pitch = 1f + Random.Range (-0.2f, 0.2f);
			audio.PlayOneShot (footStep, 0.9f);
			stepCoolDown = stepRate;
		}
	}
}