using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using AC;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.AdventureCreator {

	[AddComponentMenu("Dialogue System/Third Party/Adventure Creator/Dialogue System Saver (on PersistentEngine prefab)")]
	public class DialogueSystemSaver : Remember, ISave, ISaveOptions {

		public bool saveWhenChangingLevels = false;

		/// <summary>
		/// Tells the Dialogue System to save its state into an AC global variable
		/// prior to changing levels (or saving a game).
		/// </summary>
		public override string SaveData() {
			if (saveWhenChangingLevels) {
				if (DialogueDebug.LogInfo) Debug.Log("Saving Dialogue System state to Adventure Creator.");
				AdventureCreatorBridge.SaveDialogueSystemToGlobalVariable();
			}
			return string.Empty;
		}
		
		/// <summary>
		/// Tells the Dialogue System to save its state into an AC global variable prior
		/// to saving a game, unless DialogueSystemSceneSync has already saved it.
		/// </summary>
		public void PreSave() {
			if (!saveWhenChangingLevels) {
				if (DialogueDebug.LogInfo) Debug.Log("Adding Dialogue System state to the saved game.");
				AdventureCreatorBridge.SaveDialogueSystemToGlobalVariable();
			}
		}

		/// <summary>
		/// Tells the Dialogue System to restore its state from an AC global
		/// variable after loading a game.
		/// </summary>
		public void PostLoad() {
			if (DialogueDebug.LogInfo) Debug.Log("Restoring Dialogue System state from Adventure Creator.");
			AdventureCreatorBridge.LoadDialogueSystemFromGlobalVariable();
		}

        /// <summary>
        /// After changing options, update the Dialogue System's AC-synchronized settings, for example
        /// in case the player changed AC's language or toggled subtitles.
        /// </summary>
        public void PreSaveOptions() {
			AdventureCreatorBridge.UpdateSettingsFromAC();
		}

        /// <summary>
        /// After loading options, update the Dialogue System's AC-synchronized settings, for example
        /// in case the player changed AC's language or toggled subtitles.
        /// </summary>
        public void PostLoadOptions() {
			AdventureCreatorBridge.UpdateSettingsFromAC();
		}

	}

}
