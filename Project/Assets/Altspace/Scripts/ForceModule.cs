using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Manipulates objects in the world based on a mode </summary>
public class ForceModule : MonoBehaviour
{
    public SphericalCursorModule Cursor;

    /// <summary> Raycast normal, assuming Selectable has a selection </summary>
    public Vector3 RaycastNormal;

    /// <summary> Point that raycast hit selected item at </summary>
    public Vector3 RaycastPoint;

    /// <summary> How hard to push/pull objects </summary>
    public float PushForce = 25.0f;

    /// <summary> How hard to fling objects </summary>
    public float FlingForce = 2.0f;

    /// <summary> Radius to explode </summary>
    public float ExplosionRadius = 10.0f;

    /// <summary> Force to explode with </summary>
    public float ExplosionForce = 500.0f;

    /// <summary> Particle system instantiated to cause explosion effect </summary>
    public ParticleSystem ExplosionParticles;

    /// <summary> Cursor location, world coordinates </summary>
    public Vector3 CursorLocation;

    /// <summary> Text that displays current mode on screen </summary>
    public Text ModeText;

    /// <summary> Used to track cursor movement </summary>
    private Vector3 PreviousCursorLocation;

    /// <summary> The currently grabbed rigid body, if in grab mode </summary>
    private Rigidbody CurrentlyGrabbed;

    /// <summary> Used to turn gravity back on </summary>
    private Vector3 OriginalGravity;

    /// <summary> All the different possible modes </summary>
    public enum Modes
    {
        None,
        Push,
        Pull,
        Fling,
        Grab,
        Clone,
        Explode
    }

    /// <summary> The current mode </summary>
    public Modes CurrentMode = Modes.None;

    public void SetMode(Modes mode)
    {
        CurrentMode = mode;
        CurrentlyGrabbed = null;

        ModeText.text = "Current Mode: " + mode;
    }

    /// <summary>
    /// Set mode by string; searches enum for matching mode and sets it
    /// If matching mode is not found, sets mode to "None"
    /// </summary>
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

        SetMode(Modes.None);
    }

    public void Awake()
    {
        OriginalGravity = new Vector3(Physics.gravity.x, Physics.gravity.y, Physics.gravity.z);
    }

    public void Update()
    {
        switch (CurrentMode)
        {
            // Push/pull are the same, just in opposite directions
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
            
            case Modes.Fling:
                if (Input.GetMouseButton(0))
                {
                    // if we haven't grabbed anything yet, grab whatever is selected
                    if(CurrentlyGrabbed == null && Selectable.CurrentSelection != null)
                        CurrentlyGrabbed = Selectable.CurrentSelection.transform.GetComponent<Rigidbody>();

                    if (CurrentlyGrabbed != null)
                    {
                        // we want to use screen coordinates here, since the cursor gets set to *really* far away when not focused on anything
                        var difference = Camera.main.WorldToScreenPoint(CursorLocation) - Camera.main.WorldToScreenPoint(PreviousCursorLocation);
                        CurrentlyGrabbed.AddForceAtPosition(Camera.main.transform.rotation * new Vector3(difference.x * FlingForce, difference.y * FlingForce, 0.0f), RaycastPoint);
                    }
                }
                else if(Input.GetMouseButtonUp(0))
                {
                    // if the mouse button was let go, when can stop grabbing whatever we're moving around
                    CurrentlyGrabbed = null;
                }
                break;

            case Modes.Grab:
                if (Input.GetMouseButton(0))
                {
                    // if we haven't grabbed anything yet, grab whatever is selected
                    if (CurrentlyGrabbed == null && Selectable.CurrentSelection != null)
                        CurrentlyGrabbed = Selectable.CurrentSelection.transform.GetComponent<Rigidbody>();

                    if (CurrentlyGrabbed != null)
                    {
                        CurrentlyGrabbed.transform.position = Camera.main.transform.position + (Cursor.LookingAt * new Vector3(0.0f, 0.0f, 10.0f));
                        var body = CurrentlyGrabbed.GetComponent<Rigidbody>();
                        if(body != null)
                        {
                            body.useGravity = false;
                            body.velocity = Vector3.zero;
                        }
                            
                    }
                }

                // if the mouse button was let go, when can stop grabbing whatever we're moving around
                else if (Input.GetMouseButtonUp(0))
                {
                    if (CurrentlyGrabbed != null)
                    {
                        var body = CurrentlyGrabbed.GetComponent<Rigidbody>();
                        if (body != null)
                            body.useGravity = true;

                        CurrentlyGrabbed = null;
                    }
                }

                break;
            
            case Modes.Clone:
                // only clone selectables that DON'T have buttons
                if (Selectable.CurrentSelection != null && Input.GetMouseButtonDown(0) && Selectable.CurrentSelection.transform.GetComponent<WorldButton>() == null)
                {
                    var clone = Instantiate(Selectable.CurrentSelection);
                    clone.transform.position = new Vector3(clone.transform.position.x + UnityEngine.Random.Range(-2.5f, 2.5f), clone.transform.position.y + 5.0f, clone.transform.position.z + UnityEngine.Random.Range(-2.5f, 2.5f));
                    clone.transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(-90.0f, 90.0f), UnityEngine.Random.Range(-90.0f, 90.0f), UnityEngine.Random.Range(-90.0f, 90.0f));
                }
                break;

            case Modes.Explode:
                if (Selectable.CurrentSelection != null && Input.GetMouseButtonUp(0) && Selectable.CurrentSelection.transform.GetComponent<WorldButton>() == null)
                {
                    var selectedBody = Selectable.CurrentSelection.GetComponent<Rigidbody>();
                    if(selectedBody != null)
                        selectedBody.AddExplosionForce(ExplosionForce, Selectable.CurrentSelection.transform.position, ExplosionRadius, 2.5f);

                    // sphere cast, and add an explosion force
                    foreach (var hit in Physics.SphereCastAll(Selectable.CurrentSelection.transform.position, ExplosionRadius, Vector3.forward, 100.0f, (1 << 8)))
                    {
                        try
                        {
                            var body = hit.collider.GetComponent<Rigidbody>();
                            if (body != null)
                            {
                                body.AddExplosionForce(ExplosionForce, Selectable.CurrentSelection.transform.position, ExplosionRadius);
                            }
                        }
                        catch (MissingComponentException) { }
                    }

                    // add particle system at explosion site
                    var particles = Instantiate(ExplosionParticles);
                    particles.transform.position = Selectable.CurrentSelection.transform.position;
                    particles.transform.parent = this.transform;
                }
                

                break;

            default:
                break;
        }

        PreviousCursorLocation = CursorLocation;

        // remove any finished particle systems
        foreach(var particleSystem in transform.GetComponentsInChildren<ParticleSystem>())
        {
            if(!particleSystem.IsAlive())
            {
                particleSystem.transform.parent = null;
                Destroy(particleSystem.gameObject);
                Destroy(particleSystem);
            }
        }
    }

    public void FlipGravity()
    {
        OriginalGravity = new Vector3(-OriginalGravity.x, -OriginalGravity.y, -OriginalGravity.z);
        if (Physics.gravity != Vector3.zero)
        {
            Physics.gravity = new Vector3(OriginalGravity.x, OriginalGravity.y, OriginalGravity.z);
        }
    }

    public void SetGravityEnabled(bool enabled)
    {
        if (enabled)
            Physics.gravity = OriginalGravity;
        else
            Physics.gravity = Vector3.zero;
    }
}
