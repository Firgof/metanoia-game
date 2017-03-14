using UnityEngine;
using System.Collections;

public class TriggerObserverSample : MonoBehaviour, ITypewriterObserver
{
    public void OnTypewriterTrigger_Encountered(string trigger)
    {
        Debug.Log(string.Format("Encountered trigger: '{0}'.", trigger));
    }
}
