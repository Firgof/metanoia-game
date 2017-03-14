//-------------------------------------
//  Typewriter & Fade-in Text Effect
//  Copyright © 2014 Kalandor Studio
//-------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

/// <summary>
/// Contains useful methods for working with Rich Text Format and HTML tags.
/// </summary>
public class RichTextUtils
{
	/// <summary>
	/// Contains all the named colors and their hexadecimal values.
	/// </summary>

	public static Dictionary<string, string> namedColorValues = new Dictionary<string, string>()
    {
        { "aqua",       "#00ffff"},
        { "black",      "#000000"},
        { "blue",       "#0000ff"},
        { "brown",      "#a52a2a"},
        { "cyan",       "#00ffff"},
        { "darkblue",   "#0000a0"},
        { "fuchsia",    "#ff00ff"},
        { "green",      "#008000"},
        { "grey",       "#808080"},
        { "lightblue",  "#add8e6"},
        { "lime",       "#00ff00"},
        { "magenta",    "#ff00ff"},
        { "maroon",     "#800000"},
        { "navy",       "#000080"},
        { "olive",      "#808000"},
        { "orange",     "#ffa500"},
        { "purple",     "#800080"},
        { "red",        "#ff0000"},
        { "silver",     "#c0c0c0"},
        { "teal",       "#008080"},
        { "white",      "#ffffff"},
        { "yellow",     "#ffff00"},
        { "invisible",  "#00000000"}
    };

    /// <summary>
    /// Extracts the expression from between (potentially multiple) RTF tags,
	/// but only if it ends with the closing tags.
    /// </summary>
    /// <param name="source">The expression with tags</param>
    /// <returns>The expression without tags</returns>
    public static string ExtractExpression(string source)
    {
        while (source.Trim().EndsWith(">"))
        {
            int endTagStartPosition = source.LastIndexOf("</");
            if (endTagStartPosition >= 0)
                source = source.Substring(0, endTagStartPosition);

            if (source.Trim().StartsWith("<"))
            {
                int openTagEndPosition = source.IndexOf(">");
                if (openTagEndPosition >= 0)
                    source = source.Substring(openTagEndPosition + 1);
            }
        }

        return source;
    }

	/// <summary>
	/// Removes all RTF tags from the given string.
	/// </summary>
	/// <param name="source">The expression with tags</param>
	/// <returns>The expression without tags</returns>
	public static string RemoveRichTextTags(string source)
	{
		return Regex.Replace(source, "<.*?>", string.Empty);
	}

    public class TagRange
    {
        int startingIndex;
        int endingIndex;

        public TagRange(int startingIndex, int endingIndex)
        {
            this.startingIndex = startingIndex;
            this.endingIndex = endingIndex;
        }

        public bool IsInRange(int index)
        {
            return startingIndex <= index && endingIndex >= index;
        }
    }

    public static string RemoveNonRichText(string source, int index, int length)
    {
        // The ranges of characters to ignore (since these are RTF tags)
        List<TagRange> ignoreRanges = new List<TagRange>();
        //Dictionary<int, int> ignoreRanges = new Dictionary<int,int>();

        string rtfSource = source;

        while(rtfSource.Contains("<") && rtfSource.Contains(">") && rtfSource.Contains("</"))
        {
            int startIndex = rtfSource.IndexOf("<");
            int closeIndex = rtfSource.IndexOf(">");

            // If the closing tag has a closing bracket as well
            if( rtfSource.Substring(closeIndex + 1).Contains(">"))
            {
                // Adding the opening tag to the ignore list
                ignoreRanges.Add(new TagRange(startIndex, closeIndex));

                // Adding the closing tag to the ignore list
                rtfSource = rtfSource.Substring(closeIndex + 1);
                startIndex = rtfSource.IndexOf("</");
                closeIndex = rtfSource.IndexOf(">");
                ignoreRanges.Add(new TagRange(startIndex, closeIndex));
            }
            else
            {
                break;
            }
        }

        List<int> indexesToRemove = new List<int>();

        for (int i = index; i < source.Length; i++)
        {
            // If the character is not inside any of the ignore ranges (meaning it not part of an RTF tag
            if(ignoreRanges.Where(r => r.IsInRange(i)).Count() == 0)
            {
                indexesToRemove.Add(i);
            }

            if (indexesToRemove.Count >= length)
                break;
        }

        for (int i = 0; i < indexesToRemove.Count; i++)
        {
            source = source.Remove(indexesToRemove[i] - i, 1);
        }

        return source;
    }

    /// <summary>
    /// Gets the content of an RTF tag (e.g. color from </color>).
    /// </summary>
    /// <param name="tag">The full tag</param>
    /// <returns>The tag content</returns>
    public static string GetTagContent(string tag)
    {
        // Stripping the first characters
        if (tag.StartsWith("<"))
        {
            tag = tag.Remove(0, 1);

            if (tag.Contains("="))   // If the tag contains = (e.g. in color='Red')
            {
                tag = tag.Remove(tag.IndexOf('='));
            }
        }
        else if (tag.StartsWith("</"))
        {
            tag = tag.Remove(0, 2);
        }

        if (tag.EndsWith(">"))
        {
            tag = tag.Remove(tag.Length - 1);
        }

        return tag;
    }

    /// <summary>
    /// Removes all the specified color tags from the referenced text.
    /// </summary>
    public static void RemoveColorTagOccurences(ref string sourceText, string tag)
    {
        while (sourceText.Contains(tag))
        {
            sourceText = sourceText.Substring(sourceText.IndexOf(tag));
            sourceText = sourceText.Remove(0, tag.Length);
            sourceText = sourceText.Remove(sourceText.IndexOf("</color>"), 8);
        }
    }

    /// <summary>
    /// Converts a color's RGB values to hexadecimal.
    /// </summary>
    public static string ColorToRGBHex(Color32 color)
    {
        return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
    }

    /// <summary>
    /// Converts a color's RGB values to hexadecimal.
    /// </summary>
    public static string ColorToRGBAHex(Color32 color)
    {
        return color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2") + color.a.ToString("X2");
    }

    /// <summary>
    /// Converts hexadecimal values to RGB.
    /// </summary>
    Color HexToColor(string hex)
    {
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color32(r, g, b, 255);
    }


    public static string NormalizedToHex(float value)
    {
        int deci = Mathf.Clamp(Mathf.RoundToInt(value * 255f), 0, 255);
        return deci.ToString("X2");
    }

    public static List<string> ExtractColorTags(string source)
    {
        List<string> retVal = new List<string>();
        MatchCollection match = Regex.Matches(source, @"<color=.*?>");

        string text = "";

        foreach (System.Text.RegularExpressions.Capture c in match)
        {
            text = c.Value;

            if (text.Contains("#")) // Normal HEX color starting with #
            {
                // Removing the '#' from the beginning
                text = text.Substring(text.IndexOf('#') + 1, text.Length - text.IndexOf('#') - 2);
            }
            else // Named color
            {
                text = GetNamedColorRGB(c.Value.Substring(text.IndexOf('=') + 1, text.Length - text.IndexOf('=') - 2));

                if (!string.IsNullOrEmpty(text))
                {
                    // Removing the '#' from the beginning
                    text = text.Remove(0, 1);
                }
            }

            if (!retVal.Contains(text) && !string.IsNullOrEmpty(text))
            {
                retVal.Add(text);
            }
        }

        return retVal;
    }

    /// <summary>
    /// Takes a named color (e.g. 'yellow' and translates it into its respective HEX code.
    /// </summary>
    public static string GetNamedColorRGB(string namedColor)
    {
        string retVal = "";

        if (namedColorValues.ContainsKey(namedColor))
        {
            retVal = namedColorValues[namedColor];
        }

        return retVal;
    }
}
