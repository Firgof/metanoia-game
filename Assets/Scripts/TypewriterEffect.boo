# -This script is simply an extension of UnityGUI.TypewriterEffect to add some simple functionnality.

# -It will work with the ContinueTrigger script if it is present on the DialogueManager

# -You are welcomed to add functionnality and share!


import UnityEngine
import System.Collections
import PixelCrushers.DialogueSystem

class TypewriterEffect (UnityGUI.TypewriterEffect): 

	public speedUpKey as KeyCode = KeyCode.Space
	public speedUpOnClick as bool = true
	public speedUpFactor as single = 2
	
	dialogueUI as UnityGUI.UnityDialogueUI
	continueScript as ContinueTrigger
	initCharPerSec as single
	isPlaying as bool
	counter as single

	def Start():
		dialogueUI = DialogueManager.Instance.displaySettings.dialogueUI.GetComponent(UnityGUI.UnityDialogueUI)
		continueScript = DialogueManager.Instance.GetComponent(ContinueTrigger)
		initCharPerSec = charactersPerSecond

	def Update():
		if isPlaying:
			if Input.GetKey(speedUpKey) or (speedUpOnClick and Input.GetMouseButton(0)):
				charactersPerSecond = initCharPerSec * speedUpFactor
			else:
				charactersPerSecond = initCharPerSec
			
	def Play() as IEnumerator:
		isPlaying = true
		for i in super.Play():
			if continueScript:
				continueScript.typingDone = false
			yield i
		isPlaying = false
		if continueScript:
			continueScript.typingDone = true