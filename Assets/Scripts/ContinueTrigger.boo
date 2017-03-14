# -Lets you trigger OnContinue events with a key or a mouse click


import UnityEngine
import PixelCrushers.DialogueSystem

class ContinueTrigger (MonoBehaviour): 

	public continueKey as KeyCode
	public useMouseClick as bool = true
	public GUIEffectsContinue as bool = true
	
	[HideInInspector]
	public typingDone as bool = true
	[HideInInspector]
	public movingDone as bool = true
	[HideInInspector]
	public sizingDone as bool = true
	[HideInInspector]
	public fadingDone as bool = true
	[HideInInspector]
	public onContinue as bool = false
	
	dialogueUI as UnityGUI.UnityDialogueUI
	

	def Start():
		dialogueUI = DialogueManager.Instance.displaySettings.dialogueUI.GetComponent(UnityGUI.UnityDialogueUI)

	def Update():
		if typingDone and movingDone and sizingDone and fadingDone:
			if Input.GetKeyDown(continueKey) or (useMouseClick and Input.GetMouseButtonDown(0)) or (GUIEffectsContinue and onContinue):
				onContinue = false
				dialogueUI.SendMessage("OnContinue")