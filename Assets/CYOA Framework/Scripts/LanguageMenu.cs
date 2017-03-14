using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.CYOA {

	/// <summary>
	/// This script handles the Language Menu.
	/// </summary>
	public class LanguageMenu : MonoBehaviour {

		public string languagePlayerPrefsKey = "Language";

		public void Close() {
			gameObject.SetActive(false);
		}

		public void SetLanguage(string language) {
			Debug.Log ("Set language: " + language);
			PlayerPrefs.SetString(languagePlayerPrefsKey, language);
			PixelCrushers.DialogueSystem.Localization.Language = language;
			DialogueManager.DisplaySettings.localizationSettings.language = language;
			foreach (var localizeUiText in FindObjectsOfType<LocalizeUIText>()) {
				localizeUiText.LocalizeText();
			}
			Close();
		}
		
	}

}