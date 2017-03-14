using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MTAProgressBar : MonoBehaviour {
	public float timer;
	public float progress;
	public Image timerGraphic;
	public GameObject masterLink;
	public float yOffset;
	public float xOffset;
	public Vector2 anchorPos;
	public Vector2 anchorMin;
	public Vector2 anchorMax;

	void Start(){
		gameObject.GetComponent<RectTransform>().anchorMin = anchorMin;
		gameObject.GetComponent<RectTransform>().anchorMax = anchorMax;

		gameObject.GetComponent<RectTransform>().position = new Vector2(0f,0f);
		gameObject.GetComponent<RectTransform>().anchoredPosition = anchorPos;
		//compile dammit
	}

	void Update () {
		progress += Time.deltaTime;
		if (progress <= timer){ timerGraphic.fillAmount = progress/timer; }
		else{
			var story = masterLink.GetComponent<MTSInk>()._inkStory;
			if (story.canContinue){
				masterLink.GetComponent<MTSInk>().NewText(story.Continue());
			}
			Destroy (gameObject);
		}
		//gameObject.GetComponent<RectTransform>().anchoredPosition = anchorPos;
		//gameObject.GetComponent<RectTransform>().position = new Vector2(xOffset, yOffset);
		//gameObject.GetComponent<RectTransform>().hasChanged = true;
	}
}
