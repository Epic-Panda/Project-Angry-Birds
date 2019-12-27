using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Mouse Texture")]
    public Texture2D select;
    public Texture2D click;
    public Texture2D grab;
    public Texture2D hold;

    [Header("Camera constraints")]
    public Vector2 xConstraints;
    public Vector2 yConstraints;

    [Header("Target to follow")]
    public GameObject target;

    [Header("Camera settings")]
    public float speed = 10f;
    public bool frezeZ = true;
    public float cameraDragSpeed = .01f;

    [HideInInspector]
    public bool EogGoToTarget = false;

    [HideInInspector]
    public bool goToTarget = true;
    [HideInInspector]
    public bool mouseDrag = false;
    Vector3 startingMousePos;
    Vector3 startingCameraPos;
    Vector3 distance;

    GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.instance;

        if (!target && gameManager.bird.Count > 0)
            target = gameManager.bird[0];
    }

    void LateUpdate()
    {
        if (gameManager.forceQuit)
            return;

        // at the end of game go to birds position
        if (EogGoToTarget)
            SmothMotion(target.transform.position);

        if (gameManager.EOG || gameManager.pause)
            return;

        if (gameManager.cameraDragEnabled)
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouseDrag = true;
                startingMousePos = Input.mousePosition;
                startingCameraPos = transform.position;
                goToTarget = false;
            }
            else if (Input.GetMouseButtonUp(0))
                mouseDrag = false;
        }

        if (mouseDrag)
        {
            distance = startingMousePos - Input.mousePosition;
            distance = distance * cameraDragSpeed;
            SmothMotion(startingCameraPos + distance);

            return;
        }
        
        if (!target || !goToTarget)
            return;

        SmothMotion(target.transform.position);
    }

    // for mouse drag
    void GoToPosition(Vector2 pos)
    {
        SmothMotion(new Vector3(pos.x, pos.y, -10));
    }

    void SmothMotion(Vector3 destination)
    {
        // clamp target position
        destination.x = Mathf.Clamp(destination.x, xConstraints.x, xConstraints.y);

        if (yConstraints.y != yConstraints.x)
            destination.y = Mathf.Clamp(destination.y, yConstraints.x, yConstraints.y);
        else
            destination.y = yConstraints.x;

        if (frezeZ)
            destination.z = transform.position.z;

        Vector3 smothPosition = Vector3.Lerp(transform.position, destination, speed * Time.deltaTime);

        transform.position = smothPosition;
    }
}
