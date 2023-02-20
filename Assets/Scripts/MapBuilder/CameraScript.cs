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
    [SerializeField]
    private Terrain _terrain;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();

        scPos = transform.parent.parent.transform.Find("SceneController").position;
    }

    // Update is called once per frame
    void Update()
    {
        if (ms.uiController.IsPopUpPanelOpen() || ms.uiController.currrentCamera != cam || !cam.enabled)
            return;
        xAxis = Input.GetAxis("Horizontal");
        yAxis = Input.GetAxis("Vertical");
        zoom = Input.GetAxis("Mouse ScrollWheel") * 10;
        Vector3 _terrainSize = _terrain.terrainData.size;
        float largestTerrainAxis = Mathf.Max(_terrainSize.x, _terrainSize.z);
        float maxOrthoSize = (largestTerrainAxis / 2f) + 5f;

        transform.Translate(new Vector3 (xAxis * cameraSpeed.value, yAxis * cameraSpeed.value, 0.0f) * Time.deltaTime * 20.0f);
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, 5 + scPos.x, _terrainSize.x - 5f + scPos.x),
            Mathf.Clamp(transform.position.y, 20 + scPos.y, 20 + scPos.y),
            Mathf.Clamp(transform.position.z, 5 + scPos.z, _terrainSize.z - 5f + scPos.z));

        cam.orthographicSize += zoom * -cameraSpeed.value;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, 5, maxOrthoSize);
    }
}
