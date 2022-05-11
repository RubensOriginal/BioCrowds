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


    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        xAxis = Input.GetAxis("Horizontal");
        yAxis = Input.GetAxis("Vertical");
        zoom = Input.GetAxis("Mouse ScrollWheel") * 10;

        transform.Translate(new Vector3 (xAxis * - cameraSpeed.value, yAxis * -cameraSpeed.value, 0.0f));
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -50, 50),
            Mathf.Clamp(transform.position.y, 20, 20),
            Mathf.Clamp(transform.position.z, -50, 50));

        if (zoom < 0 && cam.orthographicSize >= -25)
            cam.orthographicSize -= zoom * -cameraSpeed.value;
        if (zoom > 0 && cam.orthographicSize <= -5)
            cam.orthographicSize += zoom * cameraSpeed.value;
    }
}
