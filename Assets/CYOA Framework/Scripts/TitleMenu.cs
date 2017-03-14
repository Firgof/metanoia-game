using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.CYOA {

	/// <summary>
	/// Handles the Title Menu.
	/// </summary>
	public class TitleMenu : MonoBehaviour {

		public UnityEngine.UI.Button continueButton;

		private StoryManager storyManager;
		
		private void Start() {
			storyManager = FindObjectOfType<StoryManager>();
			if (storyManager == null) {
				Debug.LogError("Can't find StoryManager on the Dialogue Manager GameObject!");
				return;
			}
			if (continueButton == null) {
				Debug.LogError("The continue button isn't assigned to TitleMenu!", this);
			} else {
				continueButton.gameObject.SetActive(storyManager.HasSavedGame());
			}
		}

		public void Continue() {
			storyManager.Continue();
		}

		public void Restart() {
			storyManager.Restart();
		}

		public void ShowLanguageMenu() {
			storyManager.ShowLanguageMenu();
		}

		public void QuitProgram() {
			storyManager.QuitProgram();
		}

	}

}