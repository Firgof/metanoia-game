using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class buttonClickStay : MonoBehaviour {
	
	public bool ButtonOn = false;
	public Button MyButton;
	
	public void BeenClicked()
	{
		Debug.Log ("BeenClicked was called on a button element");
		ButtonOn = !ButtonOn;
		if(ButtonOn)
		{
			MyButton.image.color = Color.black;
			MyButton.gameObject.GetComponent<BoxCollider2D>().enabled = false;
		}
		else
		{
			MyButton.image.color = Color.white;
			MyButton.gameObject.GetComponent<BoxCollider2D>().enabled = false;
		}
	}
}