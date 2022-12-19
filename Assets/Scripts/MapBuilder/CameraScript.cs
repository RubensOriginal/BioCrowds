using UnityEngine.UI;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Slider cameraSpeed;
    public ManagerScript ms;

    private float xAxis;
    private float yAxis;
    private float zoom;
    private Camera cam;

    private Vector3 scPos;


    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();

        scPos = transform.parent.parent.transform.Find("SceneController").position;

        Debug.Log(scPos.x);
    }

    // Update is called once per frame
    void Update()
    {
        if (ms.uiController.IsPopUpPanelOpen())
            return;
        xAxis = Input.GetAxis("Horizontal");
        yAxis = Input.GetAxis("Vertical");
        zoom = Input.GetAxis("Mouse ScrollWheel") * 10;

        transform.Translate(new Vector3 (xAxis * cameraSpeed.value, yAxis * cameraSpeed.value, 0.0f) * Time.deltaTime * 20.0f);
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, 5 + scPos.x, 25 + scPos.x),
            Mathf.Clamp(transform.position.y, 20 + scPos.y, 20 + scPos.y),
            Mathf.Clamp(transform.position.z, 5 + scPos.z, 25 + scPos.z));

        if (zoom < 0 && cam.orthographicSize <= 20)
            cam.orthographicSize += zoom * -cameraSpeed.value;
        if (zoom > 0 && cam.orthographicSize >= 5)
            cam.orthographicSize -= zoom * cameraSpeed.value;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 5, 20);
    }
}
