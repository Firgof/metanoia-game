using UnityEngine;

public class LegacyGUISample : MonoBehaviour
{
    #region Fields exposed to the Editor

    public Rect labelRect;

    public GUIStyle style;

    public AutoStart autoStart = new AutoStart();

    #endregion  Fields exposed to the Editor

    #region Fields and properties

    private string currentText;

    private bool doShow;

    private Typewriter typewriter;

    #endregion  Fields and properties

    #region Helpers
    
    [System.Serializable]
    public class AutoStart
    {
        public bool enabled;
        public string text;
        public Typewriter typewriter;
    }

    #endregion Helpers

    #region Unity calls

    void Start()
    {
        labelRect = new Rect((Screen.width / 2) - (labelRect.width / 2), labelRect.y, labelRect.width, labelRect.height);

        if(autoStart.enabled)
        {
            DisplayText(autoStart.text, autoStart.typewriter);
        }
    }

    void OnGUI()
    {
        if (doShow)
        {
            //GUI.Box(labelRect, "");
            GUI.Label(labelRect, currentText, style);
        }
    }

    #endregion  Unity calls

    public void Hide()
    {
        doShow = false;
    }

    public void DisplayText(string text, Typewriter typewriter)
    {
        this.typewriter = typewriter;

        // Subscribing to events before doing anything else
        typewriter.onTextChanged += OnTypeWriterText_Changed;
        typewriter.onFinished += OnTypeWriter_Finished;
        // Starting the typewriter
        typewriter.BeginCallbackMode(text);

        doShow = true;
    }

    #region Event handlers

    void OnTypeWriterText_Changed(string newText)
    {
        // Refreshing our text with the one we just got from the Typewriter
        currentText = newText;
    }

    void OnTypeWriter_Finished()
    {
        if (typewriter == null)
            return;

        // Typewriter has finished, so it's time to unsubscribe from events to prevent getting subscribed multiple times
        typewriter.onTextChanged -= OnTypeWriterText_Changed;
        typewriter.onFinished -= OnTypeWriter_Finished;
    }

    #endregion Event handlers
}
