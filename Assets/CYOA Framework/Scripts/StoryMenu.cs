using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.CYOA {

	/// <summary>
	/// Handles the Story Menu.
	/// </summary>
	public class StoryMenu : MonoBehaviour {

		public GameObject menuPanel = null;

		private StoryManager storyManager = null;
		
		private void Start() {
			storyManager = FindObjectOfType<StoryManager>();
			if (storyManager == null) {
				Debug.LogError("Can't find StoryManager on the Dialogue Manager GameObject!");
			}
			if (menuPanel == null) {
				Debug.LogError("The menu panel isn't assigned to StoryMenu!", this);
			} else {
				menuPanel.SetActive(false);
			}
		}

		public void ToggleMenu() {
			menuPanel.SetActive(!menuPanel.activeInHierarchy);
		}

		public void Continue() {
			ToggleMenu();
		}

		public void Save() {
			storyManager.Save();
			ToggleMenu();
		}
		
		public void Restart() {
			storyManager.Restart();
		}

		public void ShowLanguageMenu() {
			storyManager.ShowLanguageMenu();
		}
		
		public void QuitToTitle() {
			Save();
			storyManager.QuitToTitle();
		}

		public void QuitProgram() {
			Save();
			storyManager.QuitProgram();
		}

	}

}