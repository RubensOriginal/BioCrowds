using UnityEngine.EventSystems;
using UnityEngine;

public class MouseScript : MonoBehaviour
{

    public enum LevelManupulator {  Create, Move, Edit, Destroy };
    public enum ItemList { Spawner, Goal };

    [HideInInspector]
    public ItemList itemOption = ItemList.Spawner;
    [HideInInspector]
    public LevelManupulator manipulatorOption = LevelManupulator.Create;
    [HideInInspector]
    public MeshRenderer mr;

    public ManagerScript ms;
    public ObjectEditor oe;

    private Vector3 mousePos;
    private bool colliding;
    private Ray ray;
    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        transform.position = new Vector3(
            Mathf.Clamp(mousePos.x, -50, 50),
            0.75f,
            Mathf.Clamp(mousePos.z, -50, 50));

        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.layer == 9)
            {
                colliding = true;
                // mr.material = badPlace;
            }
            else
            {
                colliding = false;
                // mr.material = goodPlace;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (manipulatorOption == LevelManupulator.Create)
                    CreateObject();
                else if (manipulatorOption == LevelManupulator.Destroy)
                {
                    if (hit.collider.gameObject.layer == 9)
                        Destroy(hit.collider.gameObject);

                } else if (manipulatorOption == LevelManupulator.Edit)
                {
                    if (hit.collider.gameObject.layer == 9)
                        oe.SelectObject(hit.collider.gameObject);
                    else
                        oe.UnselectObject();
                }

            }
        }
    }

    void CreateObject()
    {
        GameObject newObject;
        switch (itemOption)
        {
            case ItemList.Spawner:
                newObject = GameObject.Instantiate(ms.spawnerPrefab);
                newObject.transform.position = transform.position;
                newObject.layer = 9;
                newObject.tag = "Spawner";

                Object spawnerObject = newObject.AddComponent<Object>();

                spawnerObject.data.pos = newObject.transform.position;
                spawnerObject.data.rot = newObject.transform.rotation;
                spawnerObject.data.type = Object.Type.Spawner;
                break;
            case ItemList.Goal:
                newObject = GameObject.Instantiate(ms.goalPrefab);
                newObject.transform.position = transform.position;
                newObject.layer = 9;
                newObject.tag = "Goal";

                Object goalObject = newObject.AddComponent<Object>();

                goalObject.data.pos = newObject.transform.position;
                goalObject.data.rot = newObject.transform.rotation;
                goalObject.data.type = Object.Type.Goal;
                break;

        }
    }

    void EditObject(GameObject editObject)
    {

    }
}
