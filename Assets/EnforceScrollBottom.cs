using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnforceScrollBottom : MonoBehaviour {
	public Scrollbar tgtScrollbar;
	public NondragScrollRect thisRect;
	public bool over_ride = false;

	public float timeToScroll = 0.05f;
	public float vel;
	public float curPos;
	public float scrlBarSize;
	public float threshold = 0.25f;
	public float nearness = 0.01f;

	void Awake(){
		thisRect = gameObject.GetComponent<NondragScrollRect>();
	}

	void Update () {
		vel = thisRect.velocity.y;
		curPos = tgtScrollbar.value;
		scrlBarSize = tgtScrollbar.size;
		float scrlValue = scrlBarSize*threshold;

		float mouseScrl = Input.GetAxis("Mouse ScrollWheel");

		if (mouseScrl != 0f){ over_ride = true;}

		if (!over_ride){ Scroll(); }

		if (over_ride && mouseScrl < 0f && tgtScrollbar.value < nearness){ over_ride = false;}
	}

	void Scroll(){
		tgtScrollbar.value = Mathf.SmoothDamp(curPos,0,ref vel,timeToScroll);
	}
}