using System;
using UnityEngine;
using UnityEngine.Events;

class ToggleableWorldButton : WorldButton
{
    /// <summary> Whether or not this button is toggled down </summary>
    public bool IsDown { get; set; }

    /// <summary> Scale to multiply base scale by when button is toggled down </summary>
    public Vector3 PressedScaleModifier = new Vector3(1.0f, 0.13f, 1.0f);

    /// <summary> Called when button is toggled down </summary>
    public UnityEvent OnDownEvent;

    /// <summary> Called when button is toggled up </summary>
    public UnityEvent OnUpEvent;

    /// <summary> Original scale of object, used to go back to up state from down </summary>
    private Vector3 OriginalScale;

    /// <summary> Original position of object, used to go back to up state from down </summary>
    private Vector3 OriginalPosition;

    public new void Awake()
    {
        base.Awake();

        OriginalScale = transform.localScale;
        OriginalPosition = transform.position;
    }

    public override void Push() { }

    public override void Release()
    {
        IsDown = !IsDown;
        SetState(IsDown);

        if(IsDown)
            OnDownEvent.Invoke();
        else
            OnUpEvent.Invoke();
    }

    /// <summary> Set the state of this button, without calling its OnDownEvent or OnUpEvent </summary>
    /// <param name="down"> Whether or not the button is down </param>
    public void SetState(bool down)
    {
        IsDown = down;

        if(down)
        {
            var newScale = new Vector3(OriginalScale.x * PressedScaleModifier.x, OriginalScale.y * PressedScaleModifier.y, OriginalScale.z * PressedScaleModifier.z);
            transform.localScale = newScale;

            // we also want to move the button by the difference in scale, so that it looks like it is just being pressed
            transform.position = new Vector3(OriginalPosition.x - (OriginalScale.x - newScale.x), OriginalPosition.y - (OriginalScale.y - newScale.y), OriginalPosition.z - (OriginalScale.z - newScale.z));
        }
        else
        {
            // just set the object back to its original transform
            transform.localScale = OriginalScale;
            transform.position = OriginalPosition;
        }
    }
}
