using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;

public class inkChoicePrefab : MonoBehaviour {
	public string choiceText;
	public int choiceIndex;
	public MTSInk inkMaster;
	public KeyCode input;
	public Color SelectedColor;
	public Color DisabledColor;
	public Button thisButton;
	public AudioClip clickedSound;
	public AudioSource aud;
	public bool playedSound = false;


	void Start () {
		gameObject.transform.FindChild("Text").GetComponent<Text>().text = choiceText;
		thisButton = gameObject.GetComponent<Button>();
		aud = gameObject.AddComponent<AudioSource>();
		aud.clip = clickedSound;
	}

	void Update(){
		if (Input.GetKeyDown(input)){
			Picked (true);
		}
	}

	public void Picked(bool wasPicked){
		if (wasPicked){
			var broadcastList = GameObject.FindGameObjectsWithTag("InkChoice");
			foreach (GameObject choice in broadcastList){
				choice.GetComponent<inkChoicePrefab>().Picked();
			}
			inkMaster._inkStory.ChooseChoiceIndex(choiceIndex);
			if(inkMaster._inkStory.canContinue){ inkMaster.NewText(inkMaster._inkStory.Continue()); }

			thisButton.interactable = false;
			ColorBlock cb = thisButton.colors;
			cb.disabledColor = SelectedColor;
			thisButton.colors = cb;
			thisButton.gameObject.GetComponentInChildren<Text>().color = SelectedColor;
			inkMaster.choiceSetup = false;

			if (playedSound == false){
				aud.Play();
				playedSound = true;
			}
		}
		else{
			thisButton.interactable = false;
			ColorBlock cb = thisButton.colors;
			cb.disabledColor = DisabledColor;
			thisButton.colors = cb;
			thisButton.gameObject.GetComponentInChildren<Text>().color = DisabledColor;
		}
	}

	public void Picked(){
		Picked (false);
	}

	public void KillYourself(){
		Debug.Log ("I was told to kill myself.  How rude!");
		Destroy(gameObject);
	}
}
