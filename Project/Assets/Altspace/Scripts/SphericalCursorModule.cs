using UnityEngine;

public class SphericalCursorModule : MonoBehaviour {
    /// <summary> This is a sensitivity parameter that should adjust how sensitive the mouse control is. </summary>
    public float Sensitivity;

    /// <summary> This is a scale factor that determines how much to scale down the cursor based on its collision distance. </summary>
    public float DistanceScaleFactor;

    /// <summary> Color to draw sphere in </summary>
    public Color SphereColor;

    /// <summary> Use the force! </summary>
    public ForceModule ForceModule;
    
    /// <summary>
    /// This is the layer mask to use when performing the ray cast for the objects. 
    /// The furniture in the room is in layer 8, everything else is not.
    /// </summary>
    private const int ColliderMask = (1 << 8);

    /// <summary> This is the Cursor game object. </summary>
    private GameObject Cursor;

    /// <summary> Transform of mesh, used for scaling based on distance </summary>
    private Transform MeshTransform;

    /// <summary> Initial scale of the cursor's mesh </summary>
    private Vector3 InitialMeshScale;

    /// <summary> This is the Cursor mesh. (The sphere.) </summary>
	private Renderer CursorMeshRenderer;

    /// <summary> This is the scale to set the cursor to if no ray hit is found. </summary>
	private Vector3 DefaultCursorScale = new Vector3(10.0f, 10.0f, 10.0f);

    /// <summary> Maximum distance to ray cast. </summary>
	private const float MaxDistance = 100.0f;

    /// <summary> Sphere radius to project cursor onto if no raycast hit. </summary>
	private const float SphereRadius = 1000.0f;

    /// <summary> The rotation of the cursor on a sphere around the player, changed by mouse movement </summary>
    private Quaternion LookingAt;

    /// <summary> Used to rotate cursor with camera </summary>
    private Quaternion PreviousCameraRotation;

    void Start()
    {
        CursorMeshRenderer.material.color = SphereColor;
        CursorMeshRenderer.material.EnableKeyword("_EMISSION"); // have to enable the keyword to actually set the emission color
        CursorMeshRenderer.material.SetColor("_EmissionColor", SphereColor);
    }

    void Awake()
    {
		Cursor = transform.Find("Cursor").gameObject;
        MeshTransform = Cursor.transform.FindChild("CursorMesh");
        CursorMeshRenderer = Cursor.transform.GetComponentInChildren<MeshRenderer>().GetComponent<Renderer>();
        InitialMeshScale = MeshTransform.localScale;

        LookingAt = Quaternion.LookRotation(Cursor.transform.position - Camera.main.transform.position);
        PreviousCameraRotation = Camera.main.transform.rotation;
    }	

	void Update()
	{
        // only want to do logic if we have the cursor locked
        if (UnityEngine.Cursor.lockState == CursorLockMode.Locked)
        {
            // Handle mouse movement to update cursor position.
            var dx = Input.GetAxis("Mouse X") * Sensitivity;
            var dy = Input.GetAxis("Mouse Y") * Sensitivity;

            // ignore movement if any of these are down (since they need to be down to move the mouse)
            // ideally, this would use Unity's input manager
            if(Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) || Input.GetMouseButton(1))
            {
                dx = 0.0f;
                dy = 0.0f;
            }

            // rotate with the camera
            var cameraDifference = Camera.main.transform.rotation * Quaternion.Inverse(PreviousCameraRotation);

            // rotate with mouse movement
            LookingAt = cameraDifference * (LookingAt * Quaternion.Euler(-dy, dx, 0.0f));

            // Perform ray cast to find object cursor is pointing at.
            var cursorHit = new RaycastHit();
            if (Physics.Raycast(new Ray(Camera.main.transform.position, LookingAt * Vector3.forward), out cursorHit, MaxDistance, ColliderMask))
            {
                // scale cursor based on distance to hit
                var distanceToObject = Vector3.Distance(Camera.main.transform.position, cursorHit.point);
                var scale = (distanceToObject * DistanceScaleFactor + 1.0f) / 2.0f;
                MeshTransform.localScale = new Vector3(scale * InitialMeshScale.x, scale * InitialMeshScale.y, scale * InitialMeshScale.z);

                Cursor.transform.position = cursorHit.point;

                Selectable.CurrentSelection = cursorHit.collider.gameObject;
                ForceModule.RaycastNormal = cursorHit.normal;
                ForceModule.RaycastPoint = cursorHit.point;
            }
            else
            {
                // set the cursor at the edge of a mighty big sphere
                MeshTransform.localScale = DefaultCursorScale;
                Cursor.transform.position = LookingAt * new Vector3(0.0f, 0.0f, SphereRadius);

                Selectable.CurrentSelection = null;
            }
        }

        PreviousCameraRotation = Camera.main.transform.rotation;
        ForceModule.CursorLocation = Cursor.transform.position;

        // set the cursor's color (each frame, just in case it changes)
        CursorMeshRenderer.material.color = SphereColor;
        CursorMeshRenderer.material.SetColor("_EmissionColor", SphereColor);
    }
}
