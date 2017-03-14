# -This script can be used with any UnityGUI.GUIVisibleControl

# -If the ContinueTrigger script is present on the DialogueManager, this script will automatically 
# trigger OnContinue events when TweenOut is done (the option can be deactivated on the ContinueTrigger script)

# -You are welcomed to add functionnality and share!


import UnityEngine
import System.Collections
import iTween
import PixelCrushers.DialogueSystem

class GUIEffects (UnityGUI.GUIEffect): 
	public positionEffects as PositionEffect
	public sizeEffects as SizeEffect
	public colorEffects as ColorEffect
	
	cameraToUse as Camera
	dialogueManager as DialogueSystemController
	dialogueUI as UnityGUI.UnityDialogueUI
	guiStyle as GUIStyle
	guiVisibleControl as UnityGUI.GUIVisibleControl
	
	isPlaying as bool
	
	def Awake():
		cameraToUse = Camera.main
		dialogueManager = DialogueManager.Instance
		dialogueUI = dialogueManager.displaySettings.dialogueUI.GetComponent(UnityGUI.UnityDialogueUI)
		guiSkin = dialogueUI.guiRoot.guiSkin
		guiVisibleControl = GetComponent(UnityGUI.GUIVisibleControl)
		for gs in guiSkin.customStyles:
			if gs.name == guiVisibleControl.guiStyleName:
				guiStyle = gs
			
	def Play() as IEnumerator:
		if not isPlaying:
			isPlaying = true
		else:
			positionEffects.Start(guiVisibleControl, gameObject, cameraToUse)
			sizeEffects.Start(guiStyle, gameObject)
			colorEffects.Start(guiStyle, gameObject)
			while true:
				positionEffects.Update()
				sizeEffects.Update()
				colorEffects.Update()
				guiVisibleControl.Refresh()
				yield WaitForSeconds(0)

	def TweenPosition(p as Vector2):
		positionEffects.tweened = p
	
	def TweenSize(s as single):
		sizeEffects.tweened = s
	
	def TweenColor(c as Color):
		colorEffects.tweened = c			










class PositionEffect:
	public follow as Follower
	public clampToScreen as ClampToScreen
	public tween as TweenPosition
	
	[HideInInspector]
	public tweened as Vector2
	
	guiVisibleControl as UnityGUI.GUIVisibleControl
	gameObject as GameObject
	cameraToUse as Camera
	
	def Start(GVC as UnityGUI.GUIVisibleControl, GO as GameObject, cam as Camera):
		guiVisibleControl = GVC
		gameObject = GO
		cameraToUse = cam
		
		# The order of the Start() methods is important
		follow.Start(guiVisibleControl, cameraToUse)
		tween.Start(guiVisibleControl, gameObject)
		clampToScreen.Start(guiVisibleControl, cameraToUse)
			
	def Update():
		# The order of the Update() methods is important
		follow.Update()
		tween.Update(tweened)
		clampToScreen.Update()

class Follower:
	enum Targets:
		Actor
		Conversant
		Custom
	public enable as bool
	public target as Targets
	public customTarget as Transform
	public offset as Vector2 = Vector2(0, 0)
	public damping as single = 1
	
	guiVisibleControl as UnityGUI.GUIVisibleControl
	cameraToUse as Camera
	currentTarget as Transform
	position as Vector2
	
	def Start(GVC as UnityGUI.GUIVisibleControl, cam as Camera):
		guiVisibleControl = GVC
		cameraToUse = cam
		
		if enable:
			guiVisibleControl.autoSize.autoSizeWidth = true
			guiVisibleControl.autoSize.autoSizeHeight = true
			guiVisibleControl.scaledRect.origin = UnityGUI.ScaledRectAlignment.TopLeft
			guiVisibleControl.scaledRect.alignment = UnityGUI.ScaledRectAlignment.MiddleCenter
			guiVisibleControl.scaledRect.x.scale = UnityGUI.ValueScale.Normalized
			guiVisibleControl.scaledRect.y.scale = UnityGUI.ValueScale.Normalized
			guiVisibleControl.scaledRect.width.scale = UnityGUI.ValueScale.Normalized
			guiVisibleControl.scaledRect.height.scale = UnityGUI.ValueScale.Normalized
			if target == Targets.Actor:
				currentTarget = DialogueManager.CurrentActor.transform
			elif target == Targets.Conversant:
				currentTarget = DialogueManager.CurrentConversant.transform
			elif target == Targets.Custom:
				currentTarget = customTarget
				
			position = cameraToUse.WorldToViewportPoint(currentTarget.position)
			guiVisibleControl.scaledRect.x.value = position.x + offset.x
			guiVisibleControl.scaledRect.y.value = 1 - (position.y + offset.y)
				
	def Update():
		if enable:
			position = Vector2.Lerp(position, cameraToUse.WorldToViewportPoint(currentTarget.position), damping)
			guiVisibleControl.scaledRect.x.value = position.x + offset.x
			guiVisibleControl.scaledRect.y.value = 1 - (position.y + offset.y)
		
class ClampToScreen:
	public enable as bool
	public borders as Vector2 = Vector2(0.01, 0)
	
	guiVisibleControl as UnityGUI.GUIVisibleControl
	cameraToUse as Camera
	
	def Start(GVC as UnityGUI.GUIVisibleControl, cam as Camera):
		guiVisibleControl = GVC
		cameraToUse = cam
		if enable:
			rect = guiVisibleControl.rect
			rectSize = cameraToUse.ScreenToViewportPoint(Vector2(rect.width, rect.height))
			bounds = borders + (rectSize / 2)
			guiVisibleControl.scaledRect.x.value = Mathf.Clamp(guiVisibleControl.scaledRect.x.value, bounds.x, 1 - bounds.x)
			guiVisibleControl.scaledRect.y.value = Mathf.Clamp(guiVisibleControl.scaledRect.y.value, bounds.y, 1 - bounds.y)
	
	def Update():
		if enable:
			rect = guiVisibleControl.rect
			rectSize = cameraToUse.ScreenToViewportPoint(Vector2(rect.width, rect.height))
			bounds = borders + (rectSize / 2)
			guiVisibleControl.scaledRect.x.value = Mathf.Clamp(guiVisibleControl.scaledRect.x.value, bounds.x, 1 - bounds.x)
			guiVisibleControl.scaledRect.y.value = Mathf.Clamp(guiVisibleControl.scaledRect.y.value, bounds.y, 1 - bounds.y)
			
class TweenPosition:
	public enable as bool
	public tweenIn as TweenInPosition
	public tweenOut as TweenOutPosition
	public waitDuration as single
	public waitForKey as KeyCode
	public waitForClick as bool
	
	guiVisibleControl as UnityGUI.GUIVisibleControl
	continueScript as ContinueTrigger
	gameObject as GameObject
	currentState as callable
	counter as single
	
	def Start(GVC as UnityGUI.GUIVisibleControl, GO as GameObject):
		guiVisibleControl = GVC
		gameObject = GO
		counter = 0
		
		if enable:
			continueScript = DialogueManager.Instance.GetComponent(ContinueTrigger)
			if continueScript:
				continueScript.movingDone = false
			tweenIn.Start( gameObject)
			currentState = TweenIn
			guiVisibleControl.scaledRect.x.value += tweenIn.startPosition.x
			guiVisibleControl.scaledRect.y.value -= tweenIn.startPosition.y
			
	def Update(tweened as Vector2):
		if enable:
			currentState()
			guiVisibleControl.scaledRect.x.value += tweened.x
			guiVisibleControl.scaledRect.y.value -= tweened.y
			
	def TweenIn():
		if counter >= tweenIn.duration:
			counter = 0
			currentState = Wait
		else:
			counter += Time.deltaTime
	def Wait():
		if waitForKey or waitForClick:
			if Input.GetKeyDown(waitForKey) or (waitForClick and Input.GetMouseButtonDown(0)):
				if not continueScript.typingDone:
					return
				tweenOut.Start(gameObject, tweenIn.endPosition)
				counter = 0
				currentState = TweenOut
		elif counter >= waitDuration:
			tweenOut.Start(gameObject, tweenIn.endPosition)
			counter = 0
			currentState = TweenOut
		else:
			counter += Time.deltaTime
	def TweenOut():
		if counter >= tweenOut.duration:
			counter = 0
			if continueScript:
				continueScript.movingDone = true
				if continueScript.sizingDone and continueScript.fadingDone:
					continueScript.onContinue = true
		else:
			counter += Time.deltaTime
class TweenInPosition:
	public startPosition as Vector2
	public endPosition as Vector2
	public duration as single = 1
	public delay as single
	public easeType as EaseType = EaseType.spring
	public arguments as Hashtable = Hashtable()
	
	def Start(gameObject as GameObject):
		arguments['name'] = "Position"
		arguments['from'] = startPosition
		arguments['to'] = endPosition
		arguments['time'] = duration
		arguments['delay'] = delay
		arguments['easetype'] = easeType
		arguments["onupdate"] = "TweenPosition"
		ValueTo(gameObject, arguments)
class TweenOutPosition:
	public endPosition as Vector2
	public duration as single = 1
	public delay as single
	public easeType as EaseType = EaseType.spring
	public arguments as Hashtable = Hashtable()
	
	def Start(gameObject as GameObject, startPosition as Vector2):
		arguments['name'] = "Position"
		arguments['from'] = startPosition
		arguments['to'] = endPosition
		arguments['time'] = duration
		arguments['delay'] = delay
		arguments['easetype'] = easeType
		arguments["onupdate"] = "TweenPosition"
		ValueTo(gameObject, arguments)


	
		
			
				
					
						
							
								
class SizeEffect:
	public tween as TweenSize
	
	[HideInInspector]
	public tweened as single
	
	guiStyle as GUIStyle
	gameObject as GameObject
	
	def Start(GS as GUIStyle, GO as GameObject):
		guiStyle = GS
		gameObject = GO
		
		tween.Start(guiStyle, gameObject)
	
	def Update():
		tween.Update(tweened)
		
class TweenSize:
	public enable as bool
	public fontSize as single = 16
	public tweenIn as TweenInSize
	public tweenOut as TweenOutSize
	public waitDuration as single
	public waitForKey as KeyCode
	public waitForClick as bool
	
	guiStyle as GUIStyle
	continueScript as ContinueTrigger
	gameObject as GameObject
	currentState as callable
	counter as single
	
	def Start(GS as GUIStyle, GO as GameObject):
		guiStyle = GS
		gameObject = GO
		counter = 0
		
		if enable:
			continueScript = DialogueManager.Instance.GetComponent(ContinueTrigger)
			if continueScript:
				continueScript.sizingDone = false
			tweenIn.Start(gameObject)
			currentState = TweenIn
			guiStyle.fontSize = tweenIn.startSize * fontSize
			
	def Update(tweened as single):
		if enable:
			currentState()
			guiStyle.fontSize = tweened * fontSize
			
	def TweenIn():
		if counter >= tweenIn.duration:
			counter = 0
			currentState = Wait
		else:
			counter += Time.deltaTime
	def Wait():
		if waitForKey or waitForClick:
			if Input.GetKeyDown(waitForKey) or (waitForClick and Input.GetMouseButtonDown(0)):
				if not continueScript.typingDone:
					return
				tweenOut.Start(gameObject, tweenIn.endSize)
				counter = 0
				currentState = TweenOut
		elif counter >= waitDuration:
			tweenOut.Start(gameObject, tweenIn.endSize)
			counter = 0
			currentState = TweenOut
		else:
			counter += Time.deltaTime
	def TweenOut():
		if counter >= tweenOut.duration:
			counter = 0
			if continueScript:
				continueScript.sizingDone = true
				if continueScript.movingDone and continueScript.fadingDone:
					continueScript.onContinue = true
				
		else:
			counter += Time.deltaTime
class TweenInSize:
	public startSize as single = 0
	public endSize as single = 1
	public duration as single = 1
	public delay as single
	public easeType as EaseType = EaseType.spring
	public arguments as Hashtable = Hashtable()
	
	def Start(gameObject as GameObject):
		arguments['name'] = "Size"
		arguments['from'] = startSize
		arguments['to'] = endSize
		arguments['time'] = duration
		arguments['delay'] = delay
		arguments['easetype'] = easeType
		arguments["onupdate"] = "TweenSize"
		ValueTo(gameObject, arguments)
class TweenOutSize:
	public endSize as single = 0
	public duration as single = 1
	public delay as single
	public easeType as EaseType = EaseType.spring
	public arguments as Hashtable = Hashtable()
	
	def Start(gameObject as GameObject, startSize as single):
		arguments['name'] = "Size"
		arguments['from'] = startSize
		arguments['to'] = endSize
		arguments['time'] = duration
		arguments['delay'] = delay
		arguments['easetype'] = easeType
		arguments["onupdate"] = "TweenSize"
		ValueTo(gameObject, arguments)










class ColorEffect:
	public setColor as SetColor
	#public fade as Fader
	public randomColor as RandomColor
	public tween as TweenColor

	[HideInInspector]
	public tweened as Color

	guiStyle as GUIStyle
	gameObject as GameObject
	
	def Start(GS as GUIStyle, GO as GameObject):
		guiStyle = GS
		gameObject = GO
		
		setColor.Start(guiStyle)
		tween.Start(guiStyle, gameObject)
		randomColor.Start(guiStyle)

	def Update():
		tween.Update(tweened)
		randomColor.Update()

class SetColor:
	public enable as bool
	public color as Color = Color(1, 1, 1, 1)
	
	def Start(GS as GUIStyle):
		if enable:
			GS.normal.textColor = color

class RandomColor:
	public enable as bool
	public rate as single = 2
	public damping as single = 0.1
	public constraints as Constraints
	
	guiStyle as GUIStyle
	targetColor as Color
	previousColor as Color
	counter as single
	
	def Start(GS as GUIStyle):
		guiStyle = GS
		
		if enable:
			previousColor = guiStyle.normal.textColor
			targetColor = GenerateColor()
			guiStyle.normal.textColor = targetColor
		
	def Update():
		if enable:
			if counter > 1 / rate:
				targetColor = GenerateColor()
				counter = 0
			currentColor = Color.Lerp(previousColor, targetColor, damping)
			if constraints.redOrHue.enable:
				guiStyle.normal.textColor.r = currentColor.r
			if constraints.greenOrSaturation.enable:
				guiStyle.normal.textColor.g = currentColor.g
			if constraints.blueOrValue.enable:
				guiStyle.normal.textColor.b = currentColor.b
			if constraints.alpha.enable:
				guiStyle.normal.textColor.a = currentColor.a
			previousColor = currentColor
			counter += Time.deltaTime
	
	def GenerateColor():
		color = Color(0, 0, 0, 0)
		color.r = Random.Range(constraints.redOrHue.min, constraints.redOrHue.max)
		color.g = Random.Range(constraints.greenOrSaturation.min, constraints.greenOrSaturation.max)
		color.b = Random.Range(constraints.blueOrValue.min, constraints.blueOrValue.max)
		color.a = Random.Range(constraints.alpha.min, constraints.alpha.max)
		if constraints.colorMode == Constraints.ColorMode.RGB:
			return color
		elif constraints.colorMode == Constraints.ColorMode.HSV:
			return color.HSVToRGB()
class Constraints:
	enum ColorMode:
		RGB
		HSV
	public colorMode as ColorMode
	public redOrHue as ColorValue
	public greenOrSaturation as ColorValue
	public blueOrValue as ColorValue
	public alpha as ColorValue
class ColorValue:
	public enable as bool = true
	public min as single = 0
	public max as single = 1
	
class TweenColor:
	public enable as bool
	public tweenIn as TweenInColor
	public tweenOut as TweenOutColor
	public waitDuration as single
	public waitForKey as KeyCode
	public waitForClick as bool
	
	guiStyle as GUIStyle
	continueScript as ContinueTrigger
	gameObject as GameObject
	currentState as callable
	counter as single
	
	def Start(GS as GUIStyle, GO as GameObject):
		guiStyle = GS
		gameObject = GO
		counter = 0
		
		if enable:
			continueScript = DialogueManager.Instance.GetComponent(ContinueTrigger)
			if continueScript:
				continueScript.fadingDone = false
			tweenIn.Start(gameObject)
			currentState = TweenIn
			guiStyle.normal.textColor = tweenIn.startColor
			
	def Update(tweened as Color):
		if enable:
			currentState()
			guiStyle.normal.textColor = tweened
			
	def TweenIn():
		if counter >= tweenIn.duration:
			counter = 0
			currentState = Wait
		else:
			counter += Time.deltaTime
	def Wait():
		if waitForKey or waitForClick:
			if Input.GetKeyDown(waitForKey) or (waitForClick and Input.GetMouseButtonDown(0)):
				if not continueScript.typingDone:
					return
				tweenOut.Start(gameObject, tweenIn.endColor)
				counter = 0
				currentState = TweenOut
		elif counter >= waitDuration:
			tweenOut.Start(gameObject, tweenIn.endColor)
			counter = 0
			currentState = TweenOut
		else:
			counter += Time.deltaTime
	def TweenOut():
		if counter >= tweenOut.duration:
			counter = 0
			if continueScript:
				continueScript.fadingDone = true
				if continueScript.movingDone and continueScript.sizingDone:
					continueScript.onContinue = true
		else:
			counter += Time.deltaTime
class TweenInColor:
	public startColor as Color
	public endColor as Color = Color(1, 1, 1, 1)
	public duration as single = 1
	public delay as single
	public easeType as EaseType = EaseType.spring
	public arguments as Hashtable = Hashtable()
	
	def Start(gameObject as GameObject):
		arguments['name'] = "Color"
		arguments['from'] = startColor
		arguments['to'] = endColor
		arguments['time'] = duration
		arguments['delay'] = delay
		arguments['easetype'] = easeType
		arguments["onupdate"] = "TweenColor"
		ValueTo(gameObject, arguments)
class TweenOutColor:
	public endColor as Color
	public duration as single = 1
	public delay as single
	public easeType as EaseType = EaseType.spring
	public arguments as Hashtable = Hashtable()
	
	def Start(gameObject as GameObject, startColor as Color):
		arguments['name'] = "Color"
		arguments['from'] = startColor
		arguments['to'] = endColor
		arguments['time'] = duration
		arguments['delay'] = delay
		arguments['easetype'] = easeType
		arguments["onupdate"] = "TweenColor"
		ValueTo(gameObject, arguments)