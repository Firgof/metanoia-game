using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using AC;
using System.Linq;

public class MTSInk : MonoBehaviour {
	public TextAsset inkAsset;
	public Text textPrefab;
	public GameObject panel;
	public GameObject choicebox;
	public GameObject choiceMaster;
	public GameObject timerPrefab;
	public GameObject thisTimer;
	public bool choiceSetup = false;

	public AudioSource bgsPlayer;
	public AudioClip bgsTrack;

	public AudioSource sePlayer;
	public AudioClip seClip;

	public GameObject[] choiceArray;

	//public List<GameObject> choiceList;

	public Image background;
	public bool autoAdvance = false;
	int maxChoices = 5;
	public Story _inkStory;

	public string curKnot;

	private IEnumerator timingCoroutine;
	private float timeToWait = 0.08f;  // Per character in string

	//public GameObject[] textObjects;
	//public GameObject[] choiceObjects;

	public List<GameObject> textObjects;
	public List<GameObject> dummyPlug;
	public List<GameObject> choiceObjects;
	public List<string> currentTags;

	void Awake(){
		_inkStory = new Story(inkAsset.text);
		NewPage();
		DummyText();
		bgsPlayer = gameObject.GetComponent<AudioSource>();
		bgsTrack = gameObject.GetComponent<AudioClip>();
		bgsPlayer.clip = bgsTrack;
		NewText(_inkStory.Continue());
	}

	void Update(){
		if (_inkStory.canContinue){
			//Debug.Log("Can Continue.");
			if (Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)){
				currentTags = _inkStory.currentTags;
				NewText(_inkStory.Continue());
				//if (_inkStory.currentTags != null){
					Debug.Log("Processing new tags");
					processTags();
				//}
			}
		}

		if(_inkStory.currentChoices.Count > 0){
			if (choiceSetup == true){}
			else{
				Debug.Log("New choices encountered");
				var myMaster = Instantiate(choiceMaster, panel.transform);
				for (int i = 0; i< _inkStory.currentChoices.Count; ++i){
					var newbox = Instantiate(choicebox, myMaster.transform);
					var newboxprefab = newbox.GetComponent<inkChoicePrefab>();
					newboxprefab.choiceText = _inkStory.currentChoices[i].text;
					newboxprefab.choiceIndex = _inkStory.currentChoices[i].index;
					newboxprefab.inkMaster = gameObject.GetComponent<MTSInk>();

					switch(i){
					case 0: newboxprefab.input = KeyCode.Alpha1; break;
					case 1: newboxprefab.input = KeyCode.Alpha2; break;
					case 2: newboxprefab.input = KeyCode.Alpha3; break;
					case 3: newboxprefab.input = KeyCode.Alpha4; break;
					case 4: newboxprefab.input = KeyCode.Alpha5; break;
					}

					choiceArray[i] = newbox;
					choiceSetup = true;
				}
			}
		}
	}

	public void DummyText(){
		Text _dummy1 = Instantiate(textPrefab,panel.transform);
		Text _dummy2 = Instantiate(textPrefab,panel.transform);

		_dummy1.GetComponent<Text>().text = "If you're reading this,";
		_dummy2.GetComponent<Text>().text = "something has gone very, very, very wrong.";

		dummyPlug.Add(_dummy1.gameObject);
		dummyPlug.Add(_dummy2.gameObject);

		StartCoroutine("waitThing");
	}

	public void NewText(string newText){
		Text _newTextLine = Instantiate(textPrefab, panel.transform);
		_newTextLine.GetComponent<Text>().text = newText;

		if(GameObject.FindGameObjectWithTag("WaitBar") != null){ Destroy(thisTimer); thisTimer = null; }
		thisTimer = Instantiate(timerPrefab, panel.transform.parent.transform.parent.transform);
		thisTimer.transform.SetParent(panel.transform.parent.transform.parent.transform, false);
		thisTimer.GetComponent<MTAProgressBar>().timer = CalcTextTime(newText);
		thisTimer.GetComponent<MTAProgressBar>().masterLink = gameObject;
		//thisTimer.transform.localPosition = new Vector3(thisTimer.transform.localPosition.x, 0f, thisTimer.transform.localPosition.z);
		//thisTimer.
	}

	void NewPage(){
		CullObjects(false);
	}

	void NewPage(Sprite newBG){
		Debug.Log("Swapping background and clearing present sprite-backing");
		NewPage();
		background.sprite = newBG;
	}

	void NewPage(string thisScene){
		Debug.Log("Switching to new scene: "+thisScene);
		NewPage();
		ActionListAsset actList = (ActionListAsset)Resources.Load("actions/MTS_FadeSceneChange");
		actList.parameters[0].SetValue(thisScene);
		AdvGame.RunActionListAsset(actList);
	}

	float CalcTextTime(string evalText){
		return timeToWait*evalText.ToCharArray().Length;
	}

	void CullObjects(bool allOrRecent){
		var all = 0;
		if (allOrRecent == true){ all = 1; }
		else{ all = 0; }

		Debug.Log("Clearing page of old text entries");
		for(int i = 0; i < maxChoices; i++){
			Debug.Log("Culling entry in choices");
			choiceArray[i] = null;
		}
		Debug.Log("Attempting to cull text and choice objects");
		
		textObjects.AddRange(GameObject.FindGameObjectsWithTag("InkText").ToList());
		choiceObjects.AddRange(GameObject.FindGameObjectsWithTag("InkChoice").ToList());
		
		Debug.Log(textObjects.Count.ToString());
		for(int i = 0; i < textObjects.Count-all; i++){
			//var targ = textObjects.FindIndex(i);
			Debug.Log("Destroying Text Entry "+i.ToString());
			Destroy (textObjects[i]);
		}
		textObjects.Clear();
		
		Debug.Log(choiceObjects.Count.ToString());
		for(int i = 0; i < choiceObjects.Count-all; i++){
			//var targ = choiceObjects.FindIndex(i);
			Debug.Log("Destroying Choice Entry "+i.ToString());
			Destroy (choiceObjects[i]);
		}
		choiceObjects.Clear();

		if(all == 1){
			Destroy(gameObject);
		}
	}

	IEnumerator waitThing(){
		yield return 2;
		dummyPlug[0].BroadcastMessage("KillYourself");
		dummyPlug[1].BroadcastMessage("KillYourself");
		Debug.Log ("Sent Death Threats to dummyPlugs");
	}

	void processTags(){
		for(int i = 0; i < _inkStory.currentTags.Count; i++){
			var thisTag = _inkStory.currentTags[i];
			// Split each tag up into type/param by their colon
			string[] splitTag = thisTag.Split(':');
			if (splitTag.GetLength(0) == 2){
				var type = splitTag[0];
				var param = splitTag[1];
				var track = 99;
				switch(type){
				case "Music": 
					switch(param){
					case "A Twisted Tail": track = 0; break;
					case "Fantascape": track = 1; break;
					case "Drafty Places": track = 2; break;
					case "Impossible Decision": track = 3; break;
					case "Surreal": track = 4; break;
					case "Puzzles": track = 5; break;
					case "Ocean": track = 6; break;
					case "Dark": track = 7; break;
					case "WhisperLow": track = 8; break;
					case "WhisperHigh": track = 9; break;
					}
					var mus = KickStarter.stateHandler.GetMusicEngine();
					if (mus != null){
						mus.Play( track, true, false, 3.0f );
					}
					break;
				case "NewPage": 
					Debug.Log("Encountered NewPage symbol");
					switch(param){
					case "New": NewPage(); break;
					case "Close": NewPage(); CullObjects(true); break;
					default: NewPage (param); break;
					}
				break;
				case "SE":
					seClip = (AudioClip)Resources.Load ("se/"+param);
					sePlayer.clip = seClip;
					sePlayer.Play();
				break;
				case "BGS":
					Debug.Log("Loading new audio track [BGS]");
					switch(param){
					case "Snow": 
						Debug.Log("Attemping to load Snow track");
						bgsTrack = Resources.Load<AudioClip>("bgs/blizzard");
						bgsPlayer.clip = bgsTrack;
						bgsPlayer.Play();
						break;
					}
					bgsPlayer.Play();
				break;
				case "CloseCYOA":
					Debug.Log("CYOA closing down...");
					CullObjects(true);
				break;
				}
			}
		}
	}
}