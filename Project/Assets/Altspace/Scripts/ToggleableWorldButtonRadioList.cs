using System.Collections.Generic;
using UnityEngine;

/// <summary> A list that un-presses all other buttons when a button in its list is pressed </summary>
class ToggleableWorldButtonRadioList : MonoBehaviour
{
    /// <summary> List of grouped buttons. When one is pressed, the others will be un-pressed if they are down. </summary>
    public List<ToggleableWorldButton> Buttons;

    public void Start()
    {
        // add a delegate to each buttons "OnDownEvent"
        foreach(var button in Buttons)
        {
            // we have to create a new pointer here, otherwise the delegate uses the same one for every button
            // (yay, IEnumerable!)
            var buttonPointer = button; 
            button.OnDownEvent.AddListener(delegate { OnDown(buttonPointer); });
        }
    }

    /// <param name="button"> The button being pressed </param>
    public void OnDown(ToggleableWorldButton button)
    {
        // un-press any buttons that aren't this button
        foreach (var otherButton in Buttons)
        {
            if (button == otherButton)
                continue;

            if (otherButton.IsDown)
            {
                otherButton.SetState(false);
            }
        }
    }
}
