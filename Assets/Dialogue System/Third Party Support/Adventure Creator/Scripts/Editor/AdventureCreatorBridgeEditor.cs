/*
 * This file is no longer used.
 * 
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using AC;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.DialogueSystem.AdventureCreator {
	
	/// <summary>
	/// AdventureCreatorBridge custom inspector.
	/// It was originally written to support custom cursors for Dialogue System conversations.
	/// However, for visual consistency, the Dialogue System currently uses AC's dialog cursor.
	/// </summary>
	[CustomEditor (typeof(AdventureCreatorBridge))]
	public class AdventureCreatorBridgeEditor : Editor {
		
		//--- Unused: private static int DefaultCursor = -1;
		
		private AdventureCreatorBridge bridge = null;
		
		void OnEnable() {
			bridge = target as AdventureCreatorBridge;
		}
		
		public override void OnInspectorGUI() {
			if (bridge == null) bridge = target as AdventureCreatorBridge;
			if (bridge == null) return;
			bridge.useDialogState = (AdventureCreatorBridge.UseDialogState) EditorGUILayout.EnumPopup("Use Dialog State", bridge.useDialogState);
			bridge.takeCameraControl = (AdventureCreatorBridge.UseDialogState) EditorGUILayout.EnumPopup("Take Camera Control", bridge.takeCameraControl);
			
			//--- Disabled. Conversations use the same cursor as AC's DialogOptions game state.
			//bool useDefaultCursor = (bridge.conversationCursor == DefaultCursor);
			//useDefaultCursor = EditorGUILayout.Toggle("Use Default Cursor", useDefaultCursor);
			//CursorManager cursorManager = AdvGame.GetReferences().cursorManager;
			//if (useDefaultCursor) {
			//	bridge.conversationCursor = DefaultCursor;
			//} else {
			//	// Code based on HotspotEditor.cs:
			//	if (cursorManager && cursorManager.cursorIcons.Count > 0) {
			//		int useCursor_int = cursorManager.GetIntFromID (bridge.conversationCursor);
			//		useCursor_int = EditorGUILayout.Popup ("Conversation Cursor", useCursor_int, cursorManager.GetLabelsArray (useCursor_int));
			//		bridge.conversationCursor = cursorManager.cursorIcons[useCursor_int].id;
			//	} else {
			//		bridge.conversationCursor = DefaultCursor;
			//	}
			//}
			//if (!(cursorManager && cursorManager.cursorIcons.Count > 0)) {
			//	EditorGUILayout.HelpBox("If you want to use a custom cursor, you must define at least one cutsom cursor first.", MessageType.Warning);
			//}
			
			bridge.includeSimStatus = EditorGUILayout.Toggle(new GUIContent("Include Sim Status", "Tick if your conversations use SimStatus"), bridge.includeSimStatus);
			bridge.saveToGlobalVariableOnConversationEnd = EditorGUILayout.Toggle(new GUIContent("Save Global On Conversation End", "Tick to save the Lua environment to an AC global variable at the end of every conversation"), bridge.saveToGlobalVariableOnConversationEnd);
		}
		
	}
	
}
*/