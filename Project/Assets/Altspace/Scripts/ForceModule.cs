using System;
using UnityEngine;

public class ForceModule : MonoBehaviour
{
    // Raycast normal, assuming Selectable has a selection
    public Vector3 RaycastNormal;

    // Raycast point
    public Vector3 RaycastPoint;

    // How hard to push/pull objects
    public float PushForce = 25.0f;

    // How hard to fling objects
    public float FlingForce = 2.0f;

    // Cursor location, world coordinates
    public Vector3 CursorLocation;

    // Used to track cursor movement
    private Vector3 PreviousCursorLocation;

    // The currently grabbed rigid body, if in grab mode
    private Rigidbody CurrentlyGrabbed;

    // All the different possible modes
    public enum Modes
    {
        Push,
        Pull,
        Grab,
        Duplicate
    }

    // The current mode
    public Modes CurrentMode;

    public void SetMode(Modes mode)
    {
        CurrentMode = mode;
        CurrentlyGrabbed = null;
    }

    // Set mode by string
    public void SetMode(string newMode)
    {
        foreach(var mode in (Modes[])Enum.GetValues(typeof(Modes)))
        {
            if (mode.ToString() == newMode)
            {
                SetMode(mode);
                return;
            }
        }
    }

    public void Update()
    {
        switch (CurrentMode)
        {
            case Modes.Push:
            case Modes.Pull:
                if (Selectable.CurrentSelection != null && Input.GetMouseButton(0))
                {
                    var body = Selectable.CurrentSelection.transform.GetComponent<Rigidbody>();
                    if (body != null)
                    {
                        body.AddForceAtPosition(RaycastNormal * (CurrentMode == Modes.Push ? -PushForce : PushForce), RaycastPoint);
                    }
                }

                break;
            case Modes.Grab:
                if (Input.GetMouseButton(0))
                {
                    if(CurrentlyGrabbed == null && Selectable.CurrentSelection != null)
                    {
                        CurrentlyGrabbed = Selectable.CurrentSelection.transform.GetComponent<Rigidbody>();
                    }

                    if (CurrentlyGrabbed != null)
                    {
                        var difference = Camera.main.WorldToScreenPoint(CursorLocation) - Camera.main.WorldToScreenPoint(PreviousCursorLocation);
                        CurrentlyGrabbed.AddForceAtPosition(Camera.main.transform.rotation * new Vector3(difference.x * FlingForce, difference.y * FlingForce, 0.0f), RaycastPoint);
                    }
                }
                else if(Input.GetMouseButtonUp(0))
                {
                    CurrentlyGrabbed = null;
                }
                break;
            case Modes.Duplicate:
                if (Selectable.CurrentSelection != null && Input.GetMouseButtonDown(0))
                {
                    var clone = Instantiate(Selectable.CurrentSelection);
                    clone.transform.position = new Vector3(clone.transform.position.x + UnityEngine.Random.Range(-2.5f, 2.5f), clone.transform.position.y + 5.0f, clone.transform.position.z + UnityEngine.Random.Range(-2.5f, 2.5f));
                    clone.transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(-90.0f, 90.0f), UnityEngine.Random.Range(-90.0f, 90.0f), UnityEngine.Random.Range(-90.0f, 90.0f));
                }
                break;
            default:
                break;
        }

        PreviousCursorLocation = CursorLocation;
    }
}
