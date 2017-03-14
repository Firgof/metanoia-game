using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class DialogueSample : MonoBehaviour
{
    #region Fields exposed to the Editor

    public Text actorNameLabel;

    public Typewriter dialogueTypeWriter;

    public Image portrait;

    public Text tutorial;

    public List<DialogueNode> nodes = new List<DialogueNode>();

    #endregion  Fields exposed to the Editor

    #region Fields and properties

    private int currentNodeIndex;

    #endregion  Fields and properties

    #region Helpers

    [System.Serializable]
    public class DialogueNode
    {
        public string actorName;

        public string text;

        public Sprite portrait;
    }

    #endregion  Helpers

    #region Unity calls

    void Start()
    {
        StartDialogue();
    }

    #endregion  Unity calls

    private void StartDialogue()
    {
        currentNodeIndex = -1;
        ContinueDialogue();
    }

    private void ContinueDialogue()
    {
        // If the dialogue has ended
        if (++currentNodeIndex >= nodes.Count)
        {
            // Let's restart it
            StartDialogue();
            return;
        }

        DialogueNode node = nodes[currentNodeIndex];

        
        actorNameLabel.text = node.actorName;
        portrait.sprite = node.portrait;
        portrait.gameObject.SetActive(node.portrait != null);

        // Displaying the current dialogue node
        dialogueTypeWriter.BeginDisplayMode(node.text);
    }

    public void OnDialogue_Clicked()
    {
        tutorial.gameObject.SetActive(false);

        // If the Typewriter is still in the middle of the effect
        if (dialogueTypeWriter.IsInProgress)
        {
            // Telling the Typewriter to immediately display the whole text
            dialogueTypeWriter.FinishImmediate();
        }
        else // If the Typewriter is finished
        {
            ContinueDialogue();
        }
    }
}
