using UnityEngine;
using System.Collections;

public class LockCursor : MonoBehaviour {
    public bool CursorVisible = false;

    public void Start()
    {
        Cursor.lockState = CursorVisible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = CursorVisible;
    }

	public void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
            CursorVisible = !CursorVisible;


        // this has to be set every frame in case the game loses focus
        Cursor.lockState = CursorVisible ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = CursorVisible;
    }
}
