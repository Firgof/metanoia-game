using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ScrollTexture : MonoBehaviour {
	public Vector2 Scroll = new Vector2 (0.05f , 0.05f);
	Vector2 Offset = new Vector2 (0f, 0f);
	
	void Update () {
		Offset +=  Scroll * Time.deltaTime;
		gameObject.GetComponent<MeshRenderer>().material.SetTextureOffset ("_MainTex", Offset);
	}
}
