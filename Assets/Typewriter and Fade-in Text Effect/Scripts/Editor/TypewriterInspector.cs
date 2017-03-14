//-------------------------------------
//  Typewriter & Fade-in Text Effect
//  Copyright © 2014 Kalandor Studio
//-------------------------------------

using UnityEngine;
#if UNITY_5_3_OR_NEWER
using UnityEditor.SceneManagement;
#endif

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UI;

[CustomEditor(typeof(Typewriter))]
public class TypewriterInspector : Editor
{
    private bool hasSoundSource;
    private bool hasTextLabel;

    private string indentL1 = "   ";
    private const string triggerHelp = "Triggers, when encountered, invoke the onTriggerEncounter event. To add a trigger, place it anywhere in the input text between <> (e.g. <sampleTrigger>), and also define it in the list below.\n(See the documentation for details.)";
    private const string autoStartHelp = "To auto-display a text, supply it to the Text component on this game object!";
    private const string typewriterEditorNoTextError = "Couldn't save the RichText, because there is no Text component attached to the game object '{0}'. The output text was copied to your clipboard.";

    Typewriter typewriter;

    public void OnEnable()
    {
        typewriter = target as Typewriter;

		if (Selection.activeGameObject)
		{
			hasSoundSource = Selection.activeGameObject.GetComponent<TypewriterSound>() != null;
			hasTextLabel = Selection.activeGameObject.GetComponent<Text>() != null;
		}
    }

    public override void OnInspectorGUI()
    {
#region General options

        typewriter.charactersPerSecond = EditorGUILayout.IntField(new GUIContent("Characters Per Second", "How many characters should be displayed in a second."), typewriter.charactersPerSecond);
        typewriter.ignoreTimeScale = EditorGUILayout.Toggle(new GUIContent("Ignore Timescale", "Decides whether the effect should be real time, or if changing the TimeScale should also affect its speed."), typewriter.ignoreTimeScale);
        typewriter.wrapContent = EditorGUILayout.Toggle(new GUIContent("Wrap Content (uGUI only)", "Whether the Typewriter should handle the wrapping of the text in cases when it would otherwise overflow horizontally. Only works with UGUI."), typewriter.wrapContent);
        typewriter.fixLabelWidth = EditorGUILayout.Toggle(new GUIContent("Fix Text Width", "Set to true if you'd like the Text component to always have a fix size instead of growing as the text appears."), typewriter.fixLabelWidth);
        typewriter.instantDisplayNonAlphanumeric = EditorGUILayout.Toggle("Skip Non-Alphanumeric", typewriter.instantDisplayNonAlphanumeric);
        typewriter.alignment = (Typewriter.TextAlignment)EditorGUILayout.EnumPopup("Alignment", typewriter.alignment);

#endregion General options

#region Triggers

        typewriter.useTriggers = EditorGUILayout.Toggle("Use Triggers", typewriter.useTriggers);

        if (typewriter.useTriggers)
        {
            EditorGUILayout.HelpBox(triggerHelp, MessageType.Info);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("triggers"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("triggerObservers"), true);
            serializedObject.ApplyModifiedProperties();
        }

#endregion Triggers

#region Fading

        if (typewriter.fadeManager == null)
        {
            typewriter.fadeManager = new Typewriter.AlphaEntryManager();
            EditorUtility.SetDirty(target);
        }

        typewriter.fadeManager.enabled = EditorGUILayout.Toggle("Fade-In Effect", typewriter.fadeManager.enabled);

        if (typewriter.fadeManager.enabled)
        {
            typewriter.fadeManager.fadeInTime = EditorGUILayout.FloatField(indentL1 + "Fade-In Time", typewriter.fadeManager.fadeInTime);
            EditorUtility.SetDirty(target);
        }

#endregion Fading

#region Auto-start

        typewriter.autoStartOptions.startAutomatically = EditorGUILayout.Toggle("Auto-Start (uGUI only)", typewriter.autoStartOptions.startAutomatically);

        if (typewriter.autoStartOptions.startAutomatically)
        {
            if(!hasTextLabel)
            {
                typewriter.gameObject.AddComponent<Text>();
                hasTextLabel = true;
            }

            EditorGUILayout.HelpBox(autoStartHelp, MessageType.Info);
            typewriter.autoStartOptions.loop = EditorGUILayout.Toggle(indentL1 + "Loop", typewriter.autoStartOptions.loop);
            typewriter.autoStartOptions.loopDelay = EditorGUILayout.FloatField(indentL1 + "Loop Delay", typewriter.autoStartOptions.loopDelay);
        }

#endregion Auto-start

#region Typewriter Sound

        if (!hasSoundSource)
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                {
                    if (GUILayout.Button("Add Sound Effect", GUILayout.Height(20), GUILayout.Width(180)))
                    {
                        Selection.activeGameObject.AddComponent<TypewriterSound>();
                        hasSoundSource = true;
                    }
                }
                GUILayout.FlexibleSpace();
            }
            GUILayout.EndHorizontal();
        }

#endregion Typewriter Sound

#region RichText Editor

        GUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            {
                if (GUILayout.Button("RichText Editor", GUILayout.Height(20), GUILayout.Width(180)))
                {
                    RichTextEditor editorWindow = (RichTextEditor)EditorWindow.GetWindow(typeof(RichTextEditor));
                    string initText = typewriter.GetComponent<Text>() != null ? typewriter.gameObject.GetComponent<Text>().text : string.Empty;
                    editorWindow.Init(initText);
                }

                if(RichTextEditor.isClosing && RichTextEditor.needSaving)
                {
                    if(typewriter.GetComponent<Text>() != null)
                    {
                        typewriter.GetComponent<Text>().text = RichTextEditor.text;
                    }
                    else
                    {
                        string message = string.Format(typewriterEditorNoTextError, typewriter.gameObject.name);
                        EditorUtility.DisplayDialog("No Text Component", message, "Ok");
                        EditorGUIUtility.systemCopyBuffer = RichTextEditor.text;
                        Debug.LogError(message);
                    }

                    RichTextEditor.needSaving = false;
                    RichTextEditor.isClosing = false;
                }
            }
            GUILayout.FlexibleSpace();
        }
        GUILayout.EndHorizontal();

#endregion RichText Editor

        if (GUI.changed) 
        { 
            EditorUtility.SetDirty(typewriter);
#if UNITY_5_3_OR_NEWER
			EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
#endif
		}
    }
}

#endif