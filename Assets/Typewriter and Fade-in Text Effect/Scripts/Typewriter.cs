//-------------------------------------
//  Typewriter & Fade-in Text Effect
//  Copyright © 2014 Kalandor Studio
//-------------------------------------

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;

/// <summary>
///	Main script of Typewriter & Fade-in Text Effect.
///	Displays characters gradually or let them fade-in over time.
/// </summary>
public class Typewriter : MonoBehaviour
{
    #region Fields exposed to the Editor

    /// <summary>
    /// How many characters should be displayed in a second.
    /// </summary>
    public int charactersPerSecond = 20;

    /// <summary>
    /// Decides whether the effect should be real time, or if changing the TimeScale should also affect its speed.
    /// </summary>
    public bool ignoreTimeScale = true;

    /// <summary>
    /// Whether non-alphanumeric characters should be instantly displayed.
    /// </summary>
    public bool instantDisplayNonAlphanumeric = true;

    /// <summary>
    /// Whether the Typewriter should handle the wrapping of the text in cases when it would otherwise overflow horizontally.
    /// Only works with UGUI.
    /// </summary>
    public bool wrapContent = true;

    public TextAlignment alignment = TextAlignment.Left;

    /// <summary>
    /// Set to true if you'd like the Text component to always have a fix size instead of growing as the text appears.
    /// </summary>
    public bool fixLabelWidth = false;

    /// <summary>
    /// Whether the effect should translate Rich Text tags and display the text accordingly.
    /// </summary>
    public bool richText = true;

    public AutoStartOptions autoStartOptions = new AutoStartOptions();

    /// <summary>
    /// "Triggers, when encountered, invoke the onTriggerEncounter event.
    /// </summary>
    public bool useTriggers;

    /// <summary>
    /// The list of triggers to look for in the text.
    /// </summary>
    public List<string> triggers = new List<string>();

    /// <summary>
    /// Game objects that should receive the OnTypewriterTrigger_Encountered message.
    /// This is the same as subscribing to the onTriggerEncountered event, 
    /// and is useful when starting the Typewriter automatically, and not from an external script.
    /// </summary>
    public List<GameObject> triggerObservers = new List<GameObject>();

    #endregion  Fields exposed to the Editor

    #region Fields and properties

    private string currentText;
    /// <summary>
    /// The currently displayed text (content of the label).
    /// </summary>
    public string CurrentText
    {
        get
        {
            return currentText;
        }
        set
        {
            currentText = value;

            if (writerMode == WriterMode.ImmediateDisplay && label != null)
            {
                label.text = value;

                if (fixLabelWidth)
                    SetFixLabelWidth();
            }

            if (onTextChanged != null)
            {
                onTextChanged(value);
            }
        }
    }

    /// <summary>
    /// Which character in currentChars should be displayed next.
    /// </summary>
    int currentIndex;

    /// <summary>
    /// The list of characters-to-be-displayed.
    /// </summary>
    char[] currentChars;

    /// <summary>
    /// The original text.
    /// </summary>
    string fullText;

    private string defaultColorHex;
    private string CurrentColor
    {
        get
        {
            string currentColor = richTextHandler.GetCurrentColorTag(currentIndex);
            return currentColor == string.Empty ? defaultColorHex : currentColor;
        }
    }


    /// <summary>
    /// The UGUI label that should be used for displaying the scrolling text.
    /// </summary>
    private Text label;

    /// <summary>
    /// How much time has passed since the last Thick (displaying of a character).
    /// </summary>
    float timeSinceLastThick;

    /// <summary>
    /// When was the last thick (in realtime).
    /// </summary>
    private float lastThickTime;

    /// <summary>
    /// How often the Thick should occur.
    /// </summary>
    float ThickTimePeriod
    {
        get
        {
            if (charactersPerSecond == 0)
            {
                return 0;
            }
            else
            {
                return 1 / (float)charactersPerSecond;
            }
        }
    }

    private WriterState state;
    /// <summary>
    /// The current state of the effect.
    /// </summary>
    public WriterState State
    {
        get
        {
            return state;
        }
        set
        {
            state = value;

            if (state == WriterState.Finished && onFinished != null)
            {
                onFinished();
            }

            if (typeWriterSound != null)
            {
                if (state == WriterState.InProgress)
                {
                    typeWriterSound.PlaySound();
                }
                else
                {
                    typeWriterSound.StopSound();
                }
            }
        }
    }

    public bool IsInProgress
    {
        get
        {
            return State == WriterState.InProgress || State == WriterState.Paused;
        }
    }

    private WriterMode writerMode;

    /// <summary>
    /// Gets called when the typewriter effect finishes.
    /// </summary>
    public event Action onFinished;

    /// <summary>
    /// Gets called when the typewriter proceeds with the text.
    /// </summary>
    public event Action<string> onTextChanged;

    /// <summary>
    /// Gets called when the typewriter encounters a specified trigger.
    /// </summary>
    public event Action<string> onTriggerEncountered;

    private TypewriterSound typeWriterSound;

    /// <summary>
    /// Helps with the Rich Text formatting.
    /// </summary>
    private RichTextHandler richTextHandler = new RichTextHandler();
    public AlphaEntryManager fadeManager;
    private Vector2 preferredLabelSize = new Vector2();

    private const string noTextLabelFound_Error = "[Typewriter]: Couldn't start Typewriter because there is no Text component attached. Add a Text component to the Typewriter's game object, '{0}'.";

    private LayoutElement textLayout = null;
    public LayoutElement TextLayout
    {
        get
        {
            if (textLayout == null)
                textLayout = gameObject.AddComponent<LayoutElement>();

            return textLayout;
        }
    }

    #endregion  Fields and properties

    #region Helpers

    /// <summary>
    /// This determines whether the display should be handled by the TypeWriter (e.g. uGUI),
    /// or if it should only forward the text changes via a callback, and not display anything.
    /// </summary>
    public enum WriterMode
    {
        ImmediateDisplay,
        ForwardChanges
    }

    public enum WriterState
    {
        None,
        InProgress,
		Paused,
		PausedWithFade,
        Finished,
        FinishedWaitingAlpha
    }

    public enum TextAlignment
    {
        Left,
        Middle
        // Right - right alignment is currently unsupported
    }

    private sealed class RichTextHandler
    {
        /// <summary>
        /// The list of tags affecting the text.
        /// </summary>
        public List<TagRange> tagRanges = new List<TagRange>();

        /// <summary>
        /// Checks if there are any tags affecting the current character.
        /// </summary>
        /// <param name="index">The current character's index.</param>
        /// <returns>The list of tags</returns>
        public List<TagRange> CheckForTag(int index)
        {
            return tagRanges.Where(t => t.startIndex <= index && index <= t.endIndex).OrderByDescending(t => t.order).Select(t => t).ToList();
        }

        public int GetLastClosingTagLocation(int index, TagRange tag)
        {
            if (!tag.hasPlacedFirstClosing)
            {
                // We don't need to step back if it is the first character, without a closing tag
                return -1;
            }

            // Adding all the closing tags before the current one. 
            // (E.g. when looking for </i> in <b><i>e</i>m</b> the result would be the length of the last </b>
            int previousClosingTagsLength = CheckForTag(index).Where(t => t.order > tag.order).Sum(t => t.closingTag.Length);

            // Returning the steps needed to take from the current index to the last occurence of this closing tag's location
            return previousClosingTagsLength + tag.closingTag.Length;
        }

        public void ModifyClosingTagIndexes(int affectedMinIndex, int modifier)
        {
            IEnumerable<TagRange> affectedRanges = tagRanges.Where(t => t.closingTagIndex >= affectedMinIndex);

            foreach (TagRange range in affectedRanges)
            {
                range.closingTagIndex += modifier;
            }
        }

        public string GetCurrentColorTag(int index)
        {
            TagRange currentColorTag = CheckForTag(index).Where(t => t.isColor).OrderBy(t => t.startIndex).LastOrDefault();

            if (currentColorTag != null)
            {
                return RichTextUtils.ExtractColorTags(currentColorTag.openingTag).FirstOrDefault();
            }
            else
            {
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// This class stores the information about tags.
    /// </summary>
    private sealed class TagRange
    {
        /// <summary>
        /// The first index from where the tag should be inserted.
        /// </summary>
        public int startIndex;

        /// <summary>
        /// The last index where the tag should be inserted.
        /// </summary>
        public int endIndex;

        public string openingTag;

        /// <summary>
        /// The tag's closing part (e.g. </b>).
        /// </summary>
        public string closingTag;

        public int closingTagIndex;

        /// <summary>
        /// The inserting order of tags: it helps inserting them according to the LIFO (Last In First Out) algorythm.
        /// </summary>
        public int order;

        public bool isColor;

        public bool hasPlacedFirstClosing = false;

        public TagRange()
        { }

        public TagRange(int startIndex, int endIndex, string openingTag, string closingTag, int order)
        {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
            this.openingTag = openingTag;
            this.closingTag = closingTag;
            this.order = order;
            this.isColor = openingTag.Contains("color=");
        }
    }

    [Serializable]
    public class AlphaEntryManager
    {
        public bool enabled;

        public List<AlphaEntry> entries = new List<AlphaEntry>();

        public string currentColor = "00FF00";

        public float fadeInTime = 3f;

        public float timeSinceLastProcess;
        public float lastProcessTime;

        public bool IsFadingInProgress
        {
            get
            {
                return entries.Any(e => e.alpha <= 1);
            }
        }

        /// <summary>
        /// Determines how often the alpha should be processed. The lower the value, the smoother the effect is.
        /// However, setting the alpha is a bit demanding, so be careful with extremely low values.
        /// </summary>
        public float processTimePeriod = 0.001f;

        public void Reset()
        {
            entries.Clear();
            timeSinceLastProcess = 0;
            lastProcessTime = 0;
        }

        public string AddEntry(int index, string currentColor)
        {
            this.currentColor = currentColor;
			// string openingTag = string.Format("<color=#{0}{1}>", currentColor, RichTextUtils.NormalizedToHex(0.5f));
			string rgba = "";

			if(currentColor.Length > 6) // If the current color contains an alpha entry
			{
				// We remove the alpha entry and replace it with the one assigned by the FadeManager
				rgba = currentColor.Substring(0, 6) + RichTextUtils.NormalizedToHex(0f);
			}
			else
			{
				// We add the fade value to the end of the color
				rgba = currentColor + RichTextUtils.NormalizedToHex(0f);
			}
            
            string openingTag = string.Format("<color=#{0}>", rgba);
            entries.Add(new AlphaEntry(index, 0, rgba));
            return openingTag;
        }

        public string CloseEntry(int index, int textLength)
        {
            return "</color>";
        }

        /// <summary>
        /// Refreshes the alpha information and display of all entries.
        /// </summary>
        public string ProcessAlpha(string text, string colorOverride)
        {
            currentColor = colorOverride;
            string retVal = text;

            // Incrementing the alpha value of entries
            for (int i = 0; i < entries.Count; i++)
            {
                entries[i].alpha += Time.deltaTime / fadeInTime;
            }

            List<string> colorTags = RichTextUtils.ExtractColorTags(text);

            Dictionary<string, string> oldNewColors = new Dictionary<string, string>();

            foreach (string tag in colorTags.Distinct())
            {
                // If the collection already has this item
                if (oldNewColors.ContainsKey(tag))
                    continue;

                AlphaEntry match = entries.Find(e => e.openingTag == tag);

                if (match != null)
                {
                    // Adding the old color value, and pairing it with the one with refreshed alpha
                    oldNewColors.Add(tag, match.RefreshOpeningTag());
                }
            }

            foreach (var item in oldNewColors)
            {
                retVal = retVal.Replace(item.Key, item.Value);
            }

            //return RemoveUnused(retVal);
			return retVal;
		}

        public void ModifyIndexes(int affectedMinIndex, int modifier)
        {
            IEnumerable<AlphaEntry> affectedRanges = entries.Where(e => e.index >= affectedMinIndex);

            foreach (AlphaEntry entry in affectedRanges)
            {
                entry.index += modifier;
            }
        }

        /// <summary>
        /// Removes finished alpha entries, like:
        /// '<color=#000000FF>A</color>' => 'A'
        /// </summary>
        /// <param name="text"></param>
        private string RemoveUnused(string text)
        {
            //IEnumerable<string> differentColors = entries.Where(e => e.alpha >= 1).Select(e => e.openingTag).Distinct();
			IEnumerable<AlphaEntry> entriesToFinish = entries.Where(e => e.alpha >= 1 && !e.isRemoved);

			foreach (AlphaEntry entry in entriesToFinish)
            {
				// Removing the ending part, FF>
				//string colorTag = color.Substring(0, color.Length - 2) + "FF";
				//string colorTag = string.Format("<color=#{0}>", entry.openingTag);

				//int index = entry.index;
				//// Removing </color>
				//text = text.Remove(index + 18, 8);
				//// Removing the alpha entry
				//text = text.Remove(index, 17);

				int index = entry.index;

				if (index > -1 && text.Length > index + 26)
				{
					// Removing </color>
					text = text.Remove(index + 18, 8);
					// Removing the alpha entry
					text = text.Remove(index, 17);

					entry.isRemoved = true;
					ModifyIndexes(index, 17 + 8);
				}
			}

            return text;
        }

		/// <summary>
		/// Removes the alpha entries from all the color tags from the source string.
		/// </summary>
		public void RemoveColorAlphas(ref string text)
		{
			foreach(string colorTag in RichTextUtils.ExtractColorTags(text))
			{
				if(colorTag.Length > 6) // If the color HEX contains alpha entries
				{
					// Removing the alpha entries from all occurences
					text.Replace(colorTag, colorTag.Substring(0, 6));
				}
			}
		}
    }

    /// <summary>
    /// Stores the alpha information of a character.
    /// </summary>
    [Serializable]
    public class AlphaEntry
    {
        public int index;
        public float alpha;
        public string openingTag;
        public bool isRemoved = false;

        public AlphaEntry(int index, float alpha, string openingTag)
        {
            this.index = index;
            this.alpha = alpha;
            this.openingTag = openingTag;
        }

        public string RefreshOpeningTag()
        {
            string rgb = openingTag.Substring(0, 6);
            openingTag = rgb + RichTextUtils.NormalizedToHex(alpha);
            return openingTag;
        }
    }

    /// <summary>
    /// If you’d like the Typewriter to start as soon as it’s activated, specify these options.
    /// </summary>
    [Serializable]
    public class AutoStartOptions
    {
        /// <summary>
        /// Whether the effect should start without having to call it from code.
        /// </summary>
        public bool startAutomatically;

        /// <summary>
        /// Should the Typewriter restart upon finish?
        /// </summary>
        public bool loop;

        /// <summary>
        /// How much time should the effect wait before restart? Requires Loop to be checked.
        /// </summary>
        public float loopDelay = 1;

        [HideInInspector]
        public string text;
    }

    #endregion  Helpers

    #region Unity calls

    void Awake()
    {
        typeWriterSound = GetComponent<TypewriterSound>();
    }

    void Start()
    {
        if (autoStartOptions.startAutomatically)
        {
            label = GetComponent<Text>();

            if (label)
            {
                autoStartOptions.text = label.text;
                BeginDisplayMode(autoStartOptions.text);
            }
            else
            {
                Debug.LogError(string.Format(noTextLabelFound_Error, name));
            }
        }
    }

    void Update()
    {
        // Running the timer
        if (State == WriterState.InProgress)
        {
            timeSinceLastThick = ignoreTimeScale ? Time.realtimeSinceStartup - lastThickTime : timeSinceLastThick += Time.deltaTime;

            // Thicking if it is time for the next character
            if (timeSinceLastThick >= ThickTimePeriod)
            {
                timeSinceLastThick -= ThickTimePeriod;
                lastThickTime = Time.realtimeSinceStartup;
                Thick();
            }
            else if (instantDisplayNonAlphanumeric && currentChars.Length > currentIndex && IsAlphanumeric(currentChars[currentIndex]))
            {
                Thick();
            }
        }

		if (fadeManager.enabled && State == WriterState.InProgress || State == WriterState.FinishedWaitingAlpha || State == WriterState.PausedWithFade)
        {
            // Setting the alpha
            fadeManager.timeSinceLastProcess += Time.realtimeSinceStartup - fadeManager.lastProcessTime;

            if (fadeManager.timeSinceLastProcess >= fadeManager.processTimePeriod)
            {
                CurrentText = fadeManager.ProcessAlpha(CurrentText, CurrentColor);
            }

            // Setting the state to finished if the alpha effect is finished
            if(State != WriterState.PausedWithFade && !fadeManager.IsFadingInProgress)
                State = WriterState.Finished;
        }
    }

    #endregion  Unity calls

    #region Public methods

    /// <summary>
    /// Displays the Typewriter text immediately while handling triggers if set.
    /// </summary>
    /// <param name="text">The text to display.</param>
    /// <param name="clips">Specify audio clips if you want sound effect to be played while the effect is in progress</param>
    public void DisplayImmediate(string text, AudioClip[] clips = null)
    {
        BeginDisplayMode(text, ConvertClipsToSoundEntries(clips));
        FinishImmediate();
    }

    /// <summary>
    /// Starts the Typewriter effect, displaying the characters on the specified label.
    /// </summary>
    /// <param name="text">The text to display.</param>
    /// <param name="clips">Specify audio clips if you want sound effect to be played while the effect is in progress</param>
    public void BeginDisplayMode(string text, AudioClip[] clips = null)
    {
        BeginDisplayMode(text, ConvertClipsToSoundEntries(clips));
    }

    /// <summary>
    /// Starts the Typewriter effect, displaying the characters on the specified label.
    /// </summary>
    /// <param name="text">The text to display.</param>
    /// <param name="soundEntries">Specify sound entries if you want sound effect to be played while the effect is in progress</param>
    public void BeginDisplayMode(string text, List<TypewriterSound.SoundEntry> soundEntries)
    {
        writerMode = WriterMode.ImmediateDisplay;
        label = GetComponent<Text>();

		if (label == null)
        {
            Debug.LogError(string.Format(noTextLabelFound_Error, name));
            return;
        }

        defaultColorHex = RichTextUtils.ColorToRGBHex(label.color);
		// The Unity Inspector automatically escapes the newline character with an extra \, so \n becomes \\n if supplied into the Text component. This line fixes this issue.
		text = text.Replace("\\n", "\n");

		if (wrapContent)
        {
            label.horizontalOverflow = HorizontalWrapMode.Overflow;
            text = PlaceLineBreaks(text);
        }

        if (richText)
        {
            label.supportRichText = richText;
        }

        Initialize(text, soundEntries);
    }

    /// <summary>
    /// Begins the Typewriter effect, but instead of displaying the text in a specified label,
    /// it uses callbacks to notify the caller about every change in the text.
    /// Use this if planning to display text via GUI systems other than uGUI.
    /// </summary>
    /// <param name="text">The text to display.</param>
    /// <param name="clips">Specify audio clips if you want sound effect to be played while the effect is in progress</param>
    public void BeginCallbackMode(string text, AudioClip[] clips = null, Color? defaultColor = null)
    {
        BeginCallbackMode(text, ConvertClipsToSoundEntries(clips), defaultColor);
    }

    /// <summary>
    /// Begins the Typewriter effect, but instead of displaying the text in a specified label,
    /// it uses callbacks to notify the caller about every change in the text.
    /// Use this if planning to display text via GUI systems other than uGUI.
    /// </summary>
    /// <param name="text">The text to display.</param>
    /// <param name="soundEntries">Specify sound entries if you want sound effect to be played while the effect is in progress</param>
    public void BeginCallbackMode(string text, List<TypewriterSound.SoundEntry> soundEntries, Color? defaultColor = null)
    {
        if (!defaultColor.HasValue)
            defaultColor = Color.black;

        defaultColorHex = RichTextUtils.ColorToRGBHex(defaultColor.Value);

        writerMode = WriterMode.ForwardChanges;
        Initialize(text, soundEntries);
    }

    /// <summary>
    /// Pauses the effect. Call UnPause to continue.
    /// </summary>
    public void Pause()
    {
        label.CalculateLayoutInputHorizontal();
        State = WriterState.Paused;
    }

	/// <summary>
	/// Pauses the effect. Call UnPause to continue.
	/// </summary>
	public void PauseFinishFade()
	{
		label.CalculateLayoutInputHorizontal();
		State = WriterState.PausedWithFade;
	}

    /// <summary>
    /// Continues the effect if it was paused.
    /// </summary>
    public void UnPause()
    {
        if (State == WriterState.Paused || State == WriterState.PausedWithFade)
            State = WriterState.InProgress;
    }

    /// <summary>
    /// Stops the effect and immediately displays the rest of the text.
    /// </summary>
    public void FinishImmediate()
    {
        while (State != WriterState.Finished && State != WriterState.FinishedWaitingAlpha)
        {
            Thick();
        }

        State = WriterState.Finished;
        CurrentText = CheckRemoveTrigger(fullText);
    }

    #endregion Public methods

    /// <summary>
    /// Prepares the Typewriter for execution.
    /// </summary>
    private void Initialize(string text, List<TypewriterSound.SoundEntry> soundEntries = null)
    {
        Reset();

		if (fadeManager.enabled)
		{
			fadeManager.RemoveColorAlphas(ref text);
		}

		if(label != null)
			label.alignment = TextAnchor.UpperLeft;

		fullText = text;
        currentChars = text.ToCharArray();
        State = WriterState.InProgress;

        // Initing TypewriterSound if clips were specified
        if (soundEntries != null && typeWriterSound != null)
        {
            if (typeWriterSound == null)
            {
                typeWriterSound = gameObject.AddComponent<TypewriterSound>();
            }

            typeWriterSound.soundClips = soundEntries;
        }

    }

    private List<TypewriterSound.SoundEntry> ConvertClipsToSoundEntries(AudioClip[] clips)
    {
        List<TypewriterSound.SoundEntry> soundEntries = null;

        if (clips != null)
        {
            soundEntries = new List<TypewriterSound.SoundEntry>();

            foreach (AudioClip clip in clips)
            {
                soundEntries.Add(new TypewriterSound.SoundEntry(clip));
            }
        }

        return soundEntries;
    }

    /// <summary>
    /// Resets the effect to its starting state.
    /// </summary>
    public void Reset()
    {
        CurrentText = string.Empty;
        lastThickTime = 0;
        timeSinceLastThick = 0;
        currentIndex = 0;
        richTextHandler = new RichTextHandler();

        if (fadeManager != null)
            fadeManager.Reset();

        if (fixLabelWidth && GetComponent<LayoutElement>() == null)
            gameObject.AddComponent<LayoutElement>();
    }

    /// <summary>
    /// Occurs when it's time for processing the next character.
    /// </summary>
    void Thick()
    {
        if (richText)
        {
            CheckForRichText(currentChars[currentIndex]);
        }
        else if (fadeManager.enabled)
        {
            CurrentText += fadeManager.AddEntry(currentIndex, CurrentColor);
        }

        DisplayNextCharacter();
    }

    /// <summary>
    /// Method that runs when the timer ticks.
    /// This is when we display a new character.
    /// </summary>
    void DisplayNextCharacter()
    {
        bool hasTag = richTextHandler.CheckForTag(currentIndex).Count > 0;

        if (currentIndex >= currentChars.Length)
        {
            SetEndState();
            return;
        }

        if (richText && hasTag)
        {
            DisplayCharWithRichTextTags();
        }
        else
        {
            if (fadeManager.enabled)
            {
                CurrentText += currentChars[currentIndex] + fadeManager.CloseEntry(currentIndex, CurrentText.Length);
            }
            else
            {
                CurrentText += currentChars[currentIndex];
            }
        }

        currentIndex++;

        // If it was the last character, we're finished
        if (currentIndex >= currentChars.Length)
        {
            SetEndState();
        }
    }

    private void SetEndState()
    {
        if (autoStartOptions.startAutomatically && autoStartOptions.loop)
        {
            State = WriterState.FinishedWaitingAlpha;
            StartCoroutine(CO_WaitAndRestart(autoStartOptions.loopDelay));
        }
        else
        {
            State = fadeManager.IsFadingInProgress ? WriterState.FinishedWaitingAlpha : WriterState.Finished;
        }
    }

    /// <summary>
    /// Checks if the supplied character is alphanumeric.
    /// </summary>
    bool IsAlphanumeric(char character)
    {
        return Regex.IsMatch(character.ToString(), @"\W|_");
    }

    /// <summary>
    /// Coroutine that waits for some time and then restarts the effect.
    /// </summary>
    private IEnumerator CO_WaitAndRestart(float waitTime)
    {
        float startTime = Time.realtimeSinceStartup;

        while (Time.realtimeSinceStartup < startTime + waitTime || fadeManager.IsFadingInProgress)
        {
            yield return null;
        }

        State = WriterState.Finished;
        BeginDisplayMode(autoStartOptions.text);
    }

    private void SetFixLabelWidth()
    {
        TextLayout.preferredWidth = preferredLabelSize.x;
        TextLayout.preferredHeight = preferredLabelSize.y;

        #region Old approach
        //if (string.IsNullOrEmpty(fullText))
        //    return;

        //char[] remainingChars = fullText.Substring(currentIndex).ToCharArray();

        //foreach (char c in remainingChars)
        //{
        //    label.text += string.Format("<color={0}>{1}</color>", RichTextUtils.namedColorValues["invisible"], c);
        //}
        #endregion Old approach
    }

    #region Rich Text formatting

    /// <summary>
    /// Checks if the currently displayed character is the part of an RTF tag, and if so it handles it.
    /// </summary>
    /// <param name="character">The current character</param>
    private void CheckForRichText(char character)
    {
        // We check if it's an opening tag
        if (character != '<')
        {
            if (fadeManager.enabled)
                CurrentText += fadeManager.AddEntry(currentIndex, CurrentColor);
            return;
        }
        else if (currentChars[currentIndex + 1] == '/')
        {
            if (fadeManager.enabled)
                CurrentText += fadeManager.AddEntry(currentIndex, CurrentColor);
            return;
        }

		List<TagRange> newTagRanges = new List<TagRange>();

        while (currentChars[currentIndex] == '<')
        {
            int expressionStartingIndex = 0;
            // Getting the string after the current character
            string fromOpeningTag = fullText.Substring(currentIndex);
            string tag;

            if (fromOpeningTag.Contains(">"))
            {
                tag = RichTextUtils.GetTagContent(fromOpeningTag.Substring(0, fromOpeningTag.IndexOf(">") + 1));
            }
            else
            {
                return;
            }
            
            // Handling triggers
            if (useTriggers && triggers.Contains(tag))
            {
                currentIndex += tag.Length + 2;

                if (onTriggerEncountered != null)
                {
                    onTriggerEncountered(tag);
                }

                foreach (GameObject observer in triggerObservers)
                {
                    observer.SendMessage("OnTypewriterTrigger_Encountered", tag, SendMessageOptions.DontRequireReceiver);
                }

                if (currentIndex == currentChars.Length)
                {
                    // If a trigger is in the end, we don't want to place the alpha opening tag
                    return;
                }
                else
                {
                    break;
                }
            }

            string closingTag = "</" + tag;

            // Checking for a closing tag
            if (fromOpeningTag.Contains(closingTag))
            {
                // Substring starting with the closing tag ( "</" )
                string fromClosingTag = fromOpeningTag.Substring(fromOpeningTag.IndexOf(closingTag));

                // Expanding the closing tag to the last character
                closingTag = fromClosingTag.Substring(0, fromClosingTag.IndexOf('>') + 1);

                // The ending index of the closing tag ( ">" )
                int closingTagIndex = fromClosingTag.IndexOf(closingTag) + closingTag.Length;

                string currentOpeningTag = fromOpeningTag.Substring(0, fromOpeningTag.IndexOf(">") + 1);

                // Getting the expression with opening and closing tags (i.e. <b>expression</b>
                string expressionWithTags = fromOpeningTag.Substring(0, fromOpeningTag.IndexOf(closingTag) + closingTagIndex);
                string expressionWithoutTags = RichTextUtils.ExtractExpression(expressionWithTags);
				
				// Instantly inserting the opening tag
				CurrentText += currentOpeningTag;
                currentIndex += currentOpeningTag.Length;
                expressionStartingIndex = currentIndex;

				int expressionLastIndex = currentIndex + expressionWithoutTags.Length;
				newTagRanges.Add(new TagRange(expressionStartingIndex, expressionLastIndex, currentOpeningTag, closingTag, newTagRanges.Count));
            }
            else
            {
                return;
            }
        }


		// Modifying the endIndex of tagRanges that have additional tags between them and their expressions. E.g. <color=#432435FF><b>a</b></color>
		for (int i = 0; i < newTagRanges.Count; i++)
		{
			TagRange current = newTagRanges[i];
			current.endIndex += newTagRanges.Where(t => t != current && t.startIndex > current.startIndex).Sum(t => t.openingTag.Length);
		}

		// Adding the new RTF tags to the collection
		richTextHandler.tagRanges.AddRange(newTagRanges);

		if (fadeManager.enabled)
            CurrentText += fadeManager.AddEntry(currentIndex, CurrentColor);
    }

    /// <summary>
    /// Checks if the current character-to-be-displayed has any RTF tags associated, and if so, it adds those tags.
    /// </summary>
    void DisplayCharWithRichTextTags()
    {
		// Iterating through all the tags affecting the current character
		for (int i = 0; i < richTextHandler.CheckForTag(currentIndex).Count; i++)
		{
			TagRange tag = richTextHandler.CheckForTag(currentIndex)[i];

			if (tag.endIndex == currentIndex) // In case the Rich Text expression is finished
            {
                int lastExpressionIndex = currentIndex;

				// Ending tags that end with the current index
				foreach (TagRange tagInProgress in richTextHandler.CheckForTag(lastExpressionIndex).Where(t => t.endIndex == lastExpressionIndex))
				{
					int closingTagLength = tagInProgress.closingTag.Length;

					if (currentIndex + closingTagLength < currentChars.Length - 1)
					{
						currentIndex += closingTagLength;
					}
					else
					{
						// If it's the end of the text and we have an alpha opening tag inserted
						if (fadeManager.enabled && CurrentText.EndsWith(fadeManager.entries.Last().openingTag))
						{
							// Remove the alpha opening tag
							CurrentText = CurrentText.Remove(CurrentText.LastIndexOf(fadeManager.entries.Last().openingTag), fadeManager.entries.Last().openingTag.Length);
						}

						currentIndex = currentChars.Length - 1;
					}
				}

				bool isTextEnd = currentIndex >= currentChars.Length - 1;

				if (fadeManager.enabled)
				{
					string currentCharacter = isTextEnd ? string.Empty : currentChars[currentIndex].ToString();
					CurrentText += currentCharacter + fadeManager.CloseEntry(currentIndex, CurrentText.Length);
				}
				else if (!isTextEnd)
				{
					CurrentText += currentChars[currentIndex];
				}
			}
			else // If the Rich Text expression is still affecting the current characters
			{
				TryRemoveClosingTag(tag);

				if (i == 0)  // If it's the first tag we're about to add, then let's start with the original expression's character
				{
					if (fadeManager.enabled)
					{
						CurrentText += currentChars[currentIndex] + fadeManager.CloseEntry(currentIndex, CurrentText.Length) + tag.closingTag;
					}
					else
					{
						CurrentText += currentChars[currentIndex] + tag.closingTag;
					}

					tag.closingTagIndex = CurrentText.Length - tag.closingTag.Length;
				}
				else   // We've already added the expression and a tag, so let's just append the additional tags
				{
					CurrentText += tag.closingTag;
					tag.closingTagIndex = CurrentText.Length - tag.closingTag.Length;
				}

				tag.hasPlacedFirstClosing = true;
			}
		}
	}

	/// <summary>
	/// Checks for the last occurence of the given tag and removes it.
	/// </summary>  
	/// <param name="tag">The tag to remove</param>
	private void TryRemoveClosingTag(TagRange tag)
	{
		string text = CurrentText;

		int lastIndex = tag.closingTagIndex;

		// If the current text doesn't end with the closing tag, we return
		if (CurrentText.Length < lastIndex + tag.closingTag.Length ||
			CurrentText.Substring(lastIndex, tag.closingTag.Length) != tag.closingTag)
		{
			return;
		}
		// If the current text ends with the closing tag
		else if (lastIndex > -1 && lastIndex > tag.startIndex)// && lastIndex < tag.endIndex)
		{
			// Removing the closing tag
			text = text.Remove(lastIndex, tag.closingTag.Length);
			richTextHandler.ModifyClosingTagIndexes(currentIndex, tag.closingTag.Length * -1);
			fadeManager.ModifyIndexes(currentIndex, tag.closingTag.Length * -1);
		}

		//if (tag.closingTag == "</color>" && fadeManager.closingTagIndexes.Contains(lastIndex - 1) && currentIndex == tag.endIndex - 1)
		//{
		//    return;
		//}

		CurrentText = text;
	}

	#endregion Rich Text formatting

	#region Line breaking

	/// <summary>
	/// Processes the input text in a way that it checks when would the text overflow the current line,
	/// and then places a line break, and continues with the next one. Only works with uGUI.
	/// </summary>
	/// <param name="text"></param>
	private string PlaceLineBreaks(string text)
    {
        // Preparing the operation: separating the text, splitting it by empty spaces
        string[] spaceSeparated = text.Split(' ');
        spaceSeparated = SplitByNewLine(spaceSeparated);
        StringBuilder pendingText = new StringBuilder();
        string line = "";
        text = "";
        string firstExpression = string.Empty;
        int progress = 0;

        while (progress < spaceSeparated.Length)
        {
            // We always add a space to the expression, except when it's the first word
            string prefix = line == firstExpression ? string.Empty : " ";

            // If the current expression is a trigger, we ignore it
            if (useTriggers)
            {
                label.text = CheckRemoveTrigger(pendingText.Append(prefix + spaceSeparated[progress]).ToString());
            }
            else
            {
                label.text = pendingText.Append(prefix + spaceSeparated[progress]).ToString();
            }

            firstExpression = spaceSeparated[progress] + " ";

			bool doesOverflow = label.preferredWidth > label.rectTransform.sizeDelta.x;
			bool endsWithNewline = spaceSeparated[progress].EndsWith("\n");
			
			if (doesOverflow || endsWithNewline)
			{
				if(doesOverflow) // In case of overflow
				{
					text = HandleAlignment(text, line);

					// In case of a line break, we need the align the new line as well
					if (endsWithNewline)
						firstExpression = HandleAlignment("", firstExpression.Remove(firstExpression.Length - 2, 2));
                }
				else // If there is no overflow, but the new line's first expression ends with a newline character
				{
					line += prefix + firstExpression;
					text = HandleAlignment(text, line.Remove(line.Length - 2, 2));
					firstExpression = spaceSeparated[++progress] + " ";
				}

				// Starting a new line
				line = firstExpression.TrimStart();

				// If it is the end of the last line
				if (progress + 1 >= spaceSeparated.Length)
                {
					text = HandleAlignment(text, line);
                }
				else
				{
					pendingText = new StringBuilder();
					pendingText.Append(firstExpression);
				}
            }
            else
            {
                // Continuing the current line
                line += prefix + spaceSeparated[progress];

				// If it is the end of the last line
				if (progress + 1 >= spaceSeparated.Length)
                {
                    text = HandleAlignment(text, line);
                }
            }

            progress++;
        }

		// Saving the preferred size to use later in case fixLabelWidth is true
		if (fixLabelWidth)
		{
			label.text = text;
			preferredLabelSize.x = label.preferredWidth;
			preferredLabelSize.y = label.preferredHeight;
		}

		return text;
    }

    /// <summary>
    /// Inspects all elements in the array and when it finds a newline character it splits that element into two.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private string[] SplitByNewLine(string[] input)
    {
        List<string> separated = input.ToList();

        for (int i = 0; i < separated.Count; i++)
        {
            int newLineIndex = separated[i].IndexOf("\n");

            if(newLineIndex != -1)
            {
                string untilNewLine = separated[i].Substring(0, newLineIndex + 1);
                separated.Insert(i + 1, separated[i].Substring(untilNewLine.Length, separated[i].Length - untilNewLine.Length));
                separated[i] = untilNewLine;
            }
        }

        return separated.ToArray();
    }

    private string CheckRemoveTrigger(string text)
    {
        if (!text.Contains("<") || !text.Contains(">"))
            return text;

        foreach (string trigger in triggers)
        {
            string triggerTag = "<" + trigger + ">";

            if (text.Contains(triggerTag))
            {
                int triggerIndex = text.IndexOf(triggerTag);
                text = text.Remove(triggerIndex, triggerTag.Length);
            }
        }

        return text;
    }


	private string HandleAlignment(string fullText, string currentLine)
	{
		if (useTriggers)
			currentLine = CheckRemoveTrigger(currentLine);
			
		switch (alignment)
		{
			case TextAlignment.Left:
				// Adding the current line to the return value
				fullText += currentLine + "\n";
				break;

			case TextAlignment.Middle:
				fullText = AlignLineMiddle(fullText, currentLine);
				break;
		}

		return fullText;
	}

	/// <summary>
	/// Aligns the line to the middle by placing spaces at the beginning of the lines until it overflows the line.
	/// </summary>
	/// <returns></returns>
	private string AlignLineMiddle(string fullText, string currentLine)
    {
		// The label should represent the whole line for the width calculation
        label.text = currentLine;
        int spaceCount = 0;

        // We add spaces until they fit in the same line, and stop when no more characters can fit
        for (int i = 1; label.preferredWidth <= label.rectTransform.sizeDelta.x; i++)
        {
            spaceCount = i;
            label.text = PrefixSpaces(currentLine, i);
        }

        spaceCount = spaceCount > 0 ? (spaceCount - 1) : 0;

		// Adding the actual spaces and concatenating the new line to the full text
        fullText += PrefixSpaces(currentLine, spaceCount, true) + "\n";
        return fullText;
    }

	/// <summary>
	/// Places the given number of spaces in front of the specified text.
	/// </summary>
	/// <param name="text"></param>
	/// <param name="spaceCount"></param>
	/// <returns></returns>
    private string PrefixSpaces(string text, int spaceCount, bool single = false)
	{
		string prefix = single ? " " : "  ";

		for (int i = 0; i < spaceCount; i++)
        {
            text = prefix + text;
        }

        return text;
    }

    #endregion Line breaking

}

public interface ITypewriterObserver
{
    void OnTypewriterTrigger_Encountered(string trigger);
}