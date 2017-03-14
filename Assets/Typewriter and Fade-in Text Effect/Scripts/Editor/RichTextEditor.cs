//-------------------------------------
//  Typewriter & Fade-in Text Effect
//  Copyright © 2014 Kalandor Studio
//-------------------------------------

#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1
#define PRE_UNITY_5_2
#endif

using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class RichTextEditor : EditorWindow
{
    public static string text = "";
    private bool togglePreview = true;
    private Color textColor = Color.white;

    int textSize = 10;

    private float margin = 8;
    private float buttonHeight = 18;
    private float lineSpacing = 4f;

    public static bool isClosing;
    public static bool needSaving;

    float currentHeight = 8;
    Vector2 previewScrollPosition;

    private bool isPasting = false;
    private bool isCopying = false;

    public TextEditor TextField
    {
        get
        {
            return (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
        }
    }

    [MenuItem("Window/RichText Editor", false)]
    static void Init()
    {
        var window = (RichTextEditor)EditorWindow.GetWindow(typeof(RichTextEditor));
#if PRE_UNITY_5_2
        window.title = "RichText Editor";
#else
		window.titleContent = new GUIContent("RichText Editor");
#endif
        window.minSize = new Vector2(600f, 250f);
        isClosing = false;
        needSaving = false;
    }

    public void Init(string initialText)
    {
        text = initialText;
        Init();
    }

    void OnGUI()
    {
        CheckHandleKeys();

        TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);

        currentHeight = 8;

        ShowFormattingButtons(editor);
        ShowTextAreas();
        ShowBottomButtonBar();
    }

    private void CheckHandleKeys()
    {
        Event e = Event.current;

        if (e.type == EventType.KeyUp)
        {
            if (e.control && e.keyCode == KeyCode.V)
            {
                if (!isPasting)
                {
                    isPasting = true;
                    Paste();
                }
            }

            if (e.control && e.keyCode == KeyCode.C)
            {
                if (!isCopying)
                {
                    isCopying = false;
                    CopyToClipboard(TextField.SelectedText);
                }
            }

            if (e.control && e.keyCode == KeyCode.A)
            {
                TextField.SelectAll();
                Repaint();
            }
        }

        if (!e.control)
        {
            isPasting = false;
            isCopying = false;
        }
    }

    #region GUI Display

    private void ShowFormattingButtons(TextEditor editor)
    {
        EditorGUILayout.BeginHorizontal();

        string newExp = "";

        if (GUILayout.Button("B", GUILayout.Width(50), GUILayout.Height(buttonHeight)))
        {
            newExp = SetBold(editor.SelectedText);
        }
        else if (GUILayout.Button("I", GUILayout.Width(50), GUILayout.Height(buttonHeight)))
        {
            newExp = SetItalian(editor.SelectedText);
        }
        else if (GUILayout.Button("Set Size", GUILayout.Width(70), GUILayout.Height(buttonHeight)))
        {
            newExp = SetSize(textSize, editor.SelectedText);
        }

        textSize = EditorGUILayout.IntField("", textSize, GUILayout.Width(50));

        if (GUILayout.Button("Set Color", GUILayout.Width(70), GUILayout.Height(buttonHeight)))
        {
            newExp = SetColor(textColor, editor.SelectedText);
        }

        textColor = EditorGUILayout.ColorField("", textColor, GUILayout.Width(50));

        GUILayout.FlexibleSpace();

        GUILayout.Label("Live Preview");
        togglePreview = EditorGUILayout.Toggle("", togglePreview, GUILayout.Width(20));

        if (GUILayout.Button("Clear Formatting", GUILayout.Width(120), GUILayout.Height(buttonHeight)))
        {
            text = RemoveFormatting();
        }

        ReplaceSelectedText(newExp);

        EditorGUILayout.EndHorizontal();

        currentHeight += buttonHeight + lineSpacing;
    }

    private void ShowTextAreas()
    {
        float textAreaWidth = position.width / 2 - margin;
        float bottomElementsHeight = (margin + buttonHeight) * 3;
        float textAreaHeight = position.height - bottomElementsHeight - lineSpacing;

        GUIStyle textStyle = new GUIStyle();
        textStyle.richText = true;
        textStyle.normal.textColor = Color.black;
        textStyle.wordWrap = true;

        text = GUI.TextArea(new Rect(margin, currentHeight, togglePreview ? textAreaWidth : textAreaWidth * 2, textAreaHeight), text);

        if (togglePreview)
        {
            Rect previewRect = new Rect(textAreaWidth + margin * 2, currentHeight, textAreaWidth, textAreaHeight);
            GUILayout.BeginArea(previewRect);

            previewScrollPosition = GUILayout.BeginScrollView(previewScrollPosition, GUILayout.Width(previewRect.width), GUILayout.Height(previewRect.height));

            GUILayout.Label(text, textStyle);
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        currentHeight += textAreaHeight + lineSpacing;
    }

    private void ShowBottomButtonBar()
    {
        if (GUI.Button(new Rect(position.width - 260 - margin, position.height - buttonHeight - margin, 140, buttonHeight), "Copy to Clipboard"))
        {
            CopyToClipboard(text);
        }

        if (GUI.Button(new Rect(position.width - 100 - margin, position.height - buttonHeight - margin, 100, buttonHeight), "Close"))
        {
            needSaving = true;
            this.Close();
        }
    }

    #endregion GUI Display

    #region Textfield operations

    private void Paste()
    {
        if (!string.IsNullOrEmpty(TextField.SelectedText))
        {
            ReplaceSelectedText(EditorGUIUtility.systemCopyBuffer);
        }
        else
        {
            text += EditorGUIUtility.systemCopyBuffer;
        }

        Repaint();
    }

    private void ReplaceSelectedText(string newExp)
    {
        if (newExp != string.Empty)
        {
#if PRE_UNITY_5_2
            int textStartPos = TextField.selectPos > TextField.pos ? TextField.pos : TextField.selectPos;
            string textToReplace = text.Substring(textStartPos, Mathf.Abs(TextField.pos - TextField.selectPos));
#else
            int textStartPos = TextField.selectIndex > TextField.cursorIndex ? TextField.cursorIndex : TextField.selectIndex;
            string textToReplace = text.Substring(textStartPos, Mathf.Abs(TextField.cursorIndex - TextField.selectIndex));
#endif
            text = text.Replace(textToReplace, newExp);
            TextField.DeleteSelection();
        }
    }

    private void CopyToClipboard(string textToCopy)
    {
        EditorGUIUtility.systemCopyBuffer = textToCopy;
    }

#endregion Textfield operations

    #region RTF Functionality

    private string SetBold(string selectedText)
    {
        return string.Format("<b>{0}</b>", selectedText);
    }

    private string SetItalian(string selectedText)
    {
        return string.Format("<i>{0}</i>", selectedText);
    }

    private string SetColor(Color color, string selectedText)
    {
        return string.Format("<color=#{0}>{1}</color>", RichTextUtils.ColorToRGBAHex(color), selectedText);
    }

    private string RemoveFormatting()
    {
        return RichTextUtils.RemoveRichTextTags(text);
    }

    private string SetSize(int size, string selectedText)
    {
        return string.Format("<size={0}>{1}</size>", size, selectedText);
    }

    #endregion RTF Functionality

    #region  Event handlers

    public void OnDestroy()
    {
        isClosing = true;
    }

    #endregion  Event handlers

}
