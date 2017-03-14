using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.SequencerCommands {

	/// <summary>
	/// Sequencer command SetSprite(GameObject, Sprite)
	/// 
	/// - GameObject: The name of a GameObject with a Unity UI Image component.
	/// - Sprite: The name of a sprite in a Resources folder.
	/// </summary>
	public class SequencerCommandSetSprite : SequencerCommand {

		public void Start() {
			var go = GetSubject(0);
			var spriteName = GetParameter(1);
			var image = (go == null) ? null : go.GetComponent<UnityEngine.UI.Image>();
			var sprite = string.IsNullOrEmpty(spriteName) ? null : Resources.Load<Sprite>(spriteName);
			if (image == null) {
				if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: SetSprite(" + GetParameters() + ") can't find Image component on " + GetParameter(0));
			} else if (sprite == null) {
				if (DialogueDebug.LogWarnings) Debug.LogWarning("Dialogue System: SetSprite(" + GetParameters() + ") can't load sprite " + GetParameter(1));
			} else {
				if (DialogueDebug.LogInfo) Debug.Log("Dialogue System: SetSprite(" + GetParameters() + ")");
				image.sprite = sprite;
			}
			Stop();
		}
		
	}
}