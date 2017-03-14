using UnityEngine;
using System.Collections;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.CYOA {

	/// <summary>
	/// This is the main coordination script for the CYOA framework. It records
	/// the current dialogue entry, reconnects the dialogue UI when switching to
	/// the Story scene, and provides the guts of menu item functionality.
	/// </summary>
	public class StoryManager : MonoBehaviour {

		public string startScene = "Start";
		public string storyScene = "Story";
		public string storyConversation = "Story";

		public bool autoSaveEveryLine = false;

		public string savedGameKey = "SavedGame";

		public string gameSavedMessage = "Game saved.";

		public string restartMessage = "Start from the beginning?";
		
		public LocalizedTextTable localizedText = null;

		public LanguageMenu languageMenu = null;

		public GameObject confirmationPanel = null;

		public UnityEngine.UI.Text confirmationText = null;

		private System.Action confirmationAction = null;

		private int conversationID;
		private int dialogueEntryID;

		public void OnConversationLine(Subtitle subtitle) {
			DialogueLua.SetVariable("conversationID", subtitle.dialogueEntry.conversationID);
			DialogueLua.SetVariable("dialogueEntryID", subtitle.dialogueEntry.id);
			if (autoSaveEveryLine) SaveToPlayerPrefs();
		}

		private void SaveToPlayerPrefs() {
			var saveData = PersistentDataManager.GetSaveData();
			PlayerPrefs.SetString(savedGameKey, saveData);
			Debug.Log("SAVING: " + saveData);
		}
		
		public void Save() {
			SaveToPlayerPrefs();
			var message = ((localizedText != null) && localizedText.ContainsField(gameSavedMessage))
				? localizedText[gameSavedMessage] : gameSavedMessage;
			DialogueManager.ShowAlert(message);
		}

		public bool HasSavedGame() {
			return PlayerPrefs.HasKey(savedGameKey);
		}

		public void ClearSavedGame() {
			PlayerPrefs.DeleteKey(savedGameKey);
		}

		public void Continue() {
			StartCoroutine(ContinueCoroutine());
		}

		public void Restart() {
			Confirm(restartMessage, ConfirmRestart);
		}

		public void ConfirmRestart() {
			StartCoroutine(RestartCoroutine());
		}

		private IEnumerator ContinueCoroutine() {
			DialogueManager.StopConversation();
			Application.LoadLevel(storyScene);
			yield return null;
			ConnectDialogueUI();
			PersistentDataManager.ApplySaveData(PlayerPrefs.GetString(savedGameKey));
			var conversationID = DialogueLua.GetVariable("conversationID").AsInt;
			var dialogueEntryID = DialogueLua.GetVariable("dialogueEntryID").AsInt;
			var conversation = DialogueManager.MasterDatabase.GetConversation(conversationID);
			if (conversation == null) {
				Debug.LogError("Can't find a conversation with ID " + conversationID);
				DialogueManager.ShowAlert("Sorry, can't load the story conversation!");
			} else {
				DialogueManager.StartConversation(conversation.Title, null, null, dialogueEntryID);
			}
		}
		
		private IEnumerator RestartCoroutine() {
			DialogueManager.StopConversation();
			ClearSavedGame();
			Application.LoadLevel(storyScene);
			yield return null;
			ConnectDialogueUI();
			DialogueManager.StartConversation(storyConversation);
			if (!DialogueManager.IsConversationActive) {
				Debug.Log("Wasn't able to start conversation " + storyConversation);
				DialogueManager.ShowAlert("Sorry, can't start the story conversation!");
			}
		}

		private void ConnectDialogueUI() {
			DialogueManager.UseDialogueUI(GameObject.Find("Dialogue UI"));
		}

		public void QuitToTitle() {
			DialogueManager.StopConversation();
			Application.LoadLevel(startScene);
		}

		public void QuitProgram() {
			Application.Quit();
		}

		public void ShowLanguageMenu() {
			languageMenu.gameObject.SetActive(true);
		}

		public void Confirm(string question, System.Action confirmationAction) {
			this.confirmationAction = confirmationAction;
			var message = ((localizedText != null) && localizedText.ContainsField(question)) ? localizedText[question] : question;
			if (confirmationPanel == null || confirmationText == null) {
				Debug.LogError("The confirmation panel isn't assigned to StoryManager!", this);
				OkConfirm();
			} else {
				confirmationPanel.gameObject.SetActive(true);
				confirmationText.text = message;
			}
		}

		public void OkConfirm() {
			if (confirmationPanel != null) confirmationPanel.gameObject.SetActive(false);
			if (confirmationAction != null) confirmationAction();
		}

		public void CancelConfirm() {
			if (confirmationPanel != null) confirmationPanel.gameObject.SetActive(false);
		}

	}

}