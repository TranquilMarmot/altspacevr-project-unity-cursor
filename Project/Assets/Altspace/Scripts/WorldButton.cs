using UnityEngine;


/// <summary> A button, in the world </summary>
public abstract class WorldButton : MonoBehaviour
{
    /// <summary> Text rendered above button </summary>
    public string Text = "This is a button";

    /// <summary> Offset to render the text at, in world coordinates </summary>
    public Vector3 TextOffset = new Vector3(0.0f, 0.25f, 0.0f);

    /// <summary> Style text is rendered in, created on Awake </summary>
    private GUIStyle TextStyle;

    public void Awake()
    {
        TextStyle = new GUIStyle();
        TextStyle.fontSize = 20;
        TextStyle.normal.textColor = Color.white;
        TextStyle.alignment = TextAnchor.MiddleCenter;
    }

    /// <summary>
    /// Called when button is pressed
    /// Newcleus - Computer Age (Push the Button) https://youtu.be/OJd22xoSTNM
    /// </summary>
    public abstract void Push();

    /// <summary>
    /// Called when button is released
    /// Elvis Presley - Release Me https://youtu.be/Cv9addszC2s
    /// </summary>
    public abstract void Release();

    public void Update()
    {
        // if the selected item is this button, call the appropriate methods based on mouse state
        if(Selectable.CurrentSelection != null && Selectable.CurrentSelection.gameObject == this.gameObject && Input.GetMouseButtonUp(0))
        {
            if(Input.GetMouseButtonDown(0))
                Push();

            if (Input.GetMouseButtonUp(0))
                Release();
        }
    }

    void OnGUI()
    {
        // draw text above object (plus offset)
        GUI.enabled = true;
        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position + TextOffset);
        if(pos.z > 0.0f)
            GUI.Label(new Rect(pos.x - 75.0f, Screen.height - pos.y - 65.0f, 150.0f, 130.0f), Text, TextStyle);
    }
}
