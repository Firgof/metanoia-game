using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using AC;

public class AC_FlagManagement : MonoBehaviour {
	public GameObject ClsFlag;
	public GameObject MedFlag;
	public GameObject FarFlag;
	public int stepsRequired;

	List<string> directions = new List<string>{ "Left", "Right", "Forward" };

	void SetupPuzzle(){
		ClsFlag = GameObject.Find("flag-right");
		MedFlag = GameObject.Find("flag-right-med");
		FarFlag = GameObject.Find("flag-right-lng");
		stepsRequired = AC.LocalVariables.GetIntegerValue(8);
		Debug.LogError("Howdy, we're in Snowy Fields and I'm setting up the Puzzle now.");

		//Generate new movement solution if path has been set.
		if (AC.GlobalVariables.GetStringValue(2) != ""){
			for(int i = 0; i < stepsRequired; i++){
				directions = directions.OrderBy( x => Random.value ).ToList ();
				AC.LocalVariables.SetStringValue(i+9, directions.First());
			}
		}
		SetupFlags();
	}

	void ResetPuzzle(){
		for(int i = 0; i < stepsRequired; i++){
			AC.LocalVariables.SetStringValue(i+9, "");
			AC.LocalVariables.SetIntegerValue(7,0);
			AC.LocalVariables.SetStringValue(6,"");
		}
	}

	void SetupFlags(){
		string direction = AC.LocalVariables.GetStringValue(9+AC.LocalVariables.GetIntegerValue(7));
		if (AC.GlobalVariables.GetStringValue(2) == "SnowPilgrim"){
			Debug.LogError("Running Snow Pilgrim Flag Setup");
			//3 Flags Agree --> Go that Way
			//2 Flags Agree --> Go the Opposite Way
			//No Flags Agree --> Go Forward
			if (direction == "Left"){
				Debug.LogError("Left Direction Chosen");
				int rand = Random.Range(0,2);
				if (rand == 0){	ChangeFlag( true,  false, false ); }
				if (rand == 1){ ChangeFlag( false, false, true ); }
				if (rand == 2){ ChangeFlag( true,  true,  true ); }
			}
			else if (direction == "Right"){
				Debug.LogError("Right Direction Chosen");
				int rand = Random.Range(0,2);
				if (rand == 0){ ChangeFlag( false, false, false ); }
				if (rand == 1){ ChangeFlag( true,  true,  false ); }
				if (rand == 2){ ChangeFlag( false, true,  true ); }
			}
			else if (direction == "Forward"){
				Debug.LogError("Forward Direction Chosen");
				int rand = Random.Range(0,1);
				if (rand == 0){ ChangeFlag( false, true,  false ); }
				if (rand == 1){ ChangeFlag( true,  false, true ); }
			}
			else{
				Debug.LogError("INVALID DIRECTION CHOSEN");
			}
		}
		else if (AC.GlobalVariables.GetStringValue(2) == "Scar"){
			//3 Flags Agree --> Go Left
			//2 Flags Agree --> Go Forward
			//No Flags Agree --> Go Right
			if (direction == "Left"){
				int rand = Random.Range(0,1);
				if (rand == 0){ ChangeFlag( false, false, false ); }
				if (rand == 1){ ChangeFlag( true,  true,  true ); }
			}
			if (direction == "Right"){
				int rand = Random.Range(0,1);
				if (rand == 0){ ChangeFlag( false, true,  false ); }
				if (rand == 1){ ChangeFlag( true,  false, true ); }
			}
			if (direction == "Forward"){
				int rand = Random.Range(0,3);
				if (rand == 0){ ChangeFlag( true,  false, false ); }
				if (rand == 1){ ChangeFlag( false, false, true ); }
				if (rand == 2){ ChangeFlag( true,  true,  false ); }
				if (rand == 3){ ChangeFlag( false, true,  true ); }
			}
		}
		else{
			Debug.LogError("ERROR - NO DESTINATION PROVIDED IN WANDERTARGET");
		}

	}
	/*
	void SetupFlags(string direction){
		if (AC.GlobalVariables.GetStringValue(2) == "SnowPilgrim"){
			Debug.LogError("Running Snow Pilgrim Flag Setup");
			//3 Flags Agree --> Go that Way
			//2 Flags Agree --> Go the Opposite Way
			//No Flags Agree --> Go Forward
			if (direction == "Left"){
				Debug.LogError("Left Direction Chosen");
				int rand = Random.Range(0,2);
				if (rand == 0){	ChangeFlag( true,  false, false ); }
				if (rand == 1){ ChangeFlag( false, false, true ); }
				if (rand == 2){ ChangeFlag( true,  true,  true ); }
			}
			else if (direction == "Right"){
				Debug.LogError("Right Direction Chosen");
				int rand = Random.Range(0,2);
				if (rand == 0){ ChangeFlag( false, false, false ); }
				if (rand == 1){ ChangeFlag( true,  true,  false ); }
				if (rand == 2){ ChangeFlag( false, true,  true ); }
			}
			else if (direction == "Forward"){
				Debug.LogError("Forward Direction Chosen");
				int rand = Random.Range(0,1);
				if (rand == 0){ ChangeFlag( false, true,  false ); }
				if (rand == 1){ ChangeFlag( true,  false, true ); }
			}
			else{
				Debug.LogError("INVALID DIRECTION CHOSEN");
			}
		}
		else if (AC.GlobalVariables.GetStringValue(2) == "Scar"){
			//3 Flags Agree --> Go Left
			//2 Flags Agree --> Go Forward
			//No Flags Agree --> Go Right
			if (direction == "Left"){
				int rand = Random.Range(0,1);
				if (rand == 0){ ChangeFlag( false, false, false ); }
				if (rand == 1){ ChangeFlag( true,  true,  true ); }
			}
			if (direction == "Right"){
				int rand = Random.Range(0,1);
				if (rand == 0){ ChangeFlag( false, true,  false ); }
				if (rand == 1){ ChangeFlag( true,  false, true ); }
			}
			if (direction == "Forward"){
				int rand = Random.Range(0,3);
				if (rand == 0){ ChangeFlag( true,  false, false ); }
				if (rand == 1){ ChangeFlag( false, false, true ); }
				if (rand == 2){ ChangeFlag( true,  true,  false ); }
				if (rand == 3){ ChangeFlag( false, true,  true ); }
			}
		}
		else{
			Debug.LogError("ERROR - NO DESTINATION PROVIDED IN WANDERTARGET");
		}
	}*/

	void ChangeFlag(bool ClsLeft, bool MedLeft, bool FarLeft){
		Debug.LogError("Changing facing of the flags in the scene: "+ClsLeft.ToString()+","+MedLeft.ToString()+","+FarLeft.ToString());
		//By default, flags face to the right
		ClsFlag.GetComponent<SpriteRenderer>().flipX = ClsLeft;
		MedFlag.GetComponent<SpriteRenderer>().flipX = MedLeft;
		FarFlag.GetComponent<SpriteRenderer>().flipX = FarLeft;

		//ClsFlag.transform.localScale = new Vector3(1f,1f,1f+(-2f*(System.Convert.ToSingle(ClsLeft))));
		//MedFlag.transform.localScale = new Vector3(1f,1f,1f+(-2f*(System.Convert.ToSingle(MedLeft))));
		//FarFlag.transform.localScale = new Vector3(1f,1f,1f+(-2f*(System.Convert.ToSingle(FarLeft))));
	}

	void EvaluateForward(){
		if (AC.LocalVariables.GetStringValue(9+AC.LocalVariables.GetIntegerValue(7)) == "Forward"){
			AC.LocalVariables.SetBooleanValue(3,true);
		}
		else{ AC.LocalVariables.SetBooleanValue(3,false); }
	}

	void EvaluateLeft(){
		if (AC.LocalVariables.GetStringValue(9+AC.LocalVariables.GetIntegerValue(7)) == "Left"){
			AC.LocalVariables.SetBooleanValue(3,true);
		}
		else{ AC.LocalVariables.SetBooleanValue(3,false); }
	}

	void EvaluateRight(){
		if (AC.LocalVariables.GetStringValue(9+AC.LocalVariables.GetIntegerValue(7)) == "Right"){
			AC.LocalVariables.SetBooleanValue(3,true);
		}
		else{ AC.LocalVariables.SetBooleanValue(3,false); }
	}

	/*
	void Evaluate (string correctDirection, string chosenDirection) {
		//TODO: Make this all function properly
		if (correctDirection == chosenDirection){
			// Go To Destination
			if (AC.LocalVariables.GetIntegerValue(7) == AC.LocalVariables.GetIntegerValue(8)){
				if (AC.GlobalVariables.GetStringValue(2) == "SnowPilgrim"){
					// GOTO: Snow Village
				}
				if (AC.GlobalVariables.GetStringValue(2) == "Scar"){}// GOTO: Scar
			}
			// Else, push pathwalked up one and set up new flags.
			else{
				AC.LocalVariables.SetIntegerValue( 7, AC.LocalVariables.GetIntegerValue(7)+1 );
				SetupFlags( AC.LocalVariables.GetStringValue( 9+AC.LocalVariables.GetIntegerValue(7) ) );
			}
		}
		// Made wrong choice, send them back to the beginning and reset their steps.
		else{
			AC.LocalVariables.SetIntegerValue( 7, 0 );
			AC.GlobalVariables.SetStringValue(2, "");
		}
	}*/
}
