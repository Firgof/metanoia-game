using UnityEngine;
using UnityEngine.UI;

public class WallOfTextSample : MonoBehaviour
{
    #region Fields exposed to the Editor

    public Typewriter typewriter;

    public string demoText;

    #endregion  Fields exposed to the Editor

    #region Fields and properties

    private LegacyGUISample legacyGUI;

    #endregion  Fields and properties

    #region Unity calls

    void Awake()
    {
        legacyGUI = GetComponent<LegacyGUISample>();
    }

    #endregion  Unity calls

    public void OnLegacyButton_Clicked()
    {
        // Calling Reset removes the text previously displayed by the Typewriter
        typewriter.GetComponent<Text>().text = string.Empty;
        typewriter.Reset();
        legacyGUI.DisplayText(demoText, typewriter);
    }

    public void OnNewGUIButton_Clicked()
    {
        legacyGUI.Hide();
        typewriter.BeginDisplayMode(demoText);
    }

}
