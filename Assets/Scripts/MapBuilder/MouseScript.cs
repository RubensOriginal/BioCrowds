using UnityEngine.EventSystems;
using UnityEngine;

public class MouseScript : MonoBehaviour
{

    public enum LevelManupulator {  Create, Move, Edit, Destroy, Link };
    public enum ItemList { Spawner, Goal };

    [HideInInspector]
    public ItemList itemOption = ItemList.Spawner;
    //[HideInInspector]
    public LevelManupulator manipulatorOption = LevelManupulator.Create;
    [HideInInspector]
    public MeshRenderer mr;

    public ManagerScript ms;
    public ObjectEditor oe;


    private Vector3 mousePos;
    private bool colliding;
    private bool hitTerrain;
    private Ray ray;
    private RaycastHit hit;

    private MoveObject mo;

    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mo = GetComponent<MoveObject>();
    }

    // Update is called once per frame
    void Update()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        colliding = false;
        hitTerrain = false;

        RaycastHit[] hits = Physics.RaycastAll(ray);
        if (hits.Length > 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.gameObject.layer == 9)
                {
                    colliding = true;
                    hit = hits[i];
                }
                else if (hits[i].transform.tag == "Terrain")
                {
                    hitTerrain = true;
                }
            }
        }

        if (hitTerrain)
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            transform.position = new Vector3(
                Mathf.Clamp(mousePos.x, -50, 50),
                0.75f,
                Mathf.Clamp(mousePos.z, -50, 50));
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                if (manipulatorOption == LevelManupulator.Create)
                {
                    if (hitTerrain && !colliding)
                        CreateObject();
                }
                else if (manipulatorOption == LevelManupulator.Destroy)
                {
                    if (colliding)
                        Destroy(hit.collider.gameObject);

                }
                else if (manipulatorOption == LevelManupulator.Edit)
                {
                    if (colliding)
                        oe.SelectObject(hit.collider.gameObject);
                    else
                        oe.UnselectObject();
                }
                else if (manipulatorOption == LevelManupulator.Link)
                {
                    if (colliding || hit.collider.gameObject.CompareTag("Goal"))
                        oe.LinkGoalToSpawner(hit.collider.gameObject);
                }
                else if (manipulatorOption == LevelManupulator.Move)
                {
                    if (colliding)
                    {
                        if (!mo.isSelected)
                            mo.SelectObject(hit.collider.gameObject);
                        else
                            mo.isSelected = false;
                    }
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
                newObject = GameObject.Instantiate(ms.spawnerPrefab, ms.spawnerContainer);
                newObject.transform.position = transform.position;
                newObject.layer = 9;
                newObject.tag = "Spawner";

                Object spawnerObject = newObject.AddComponent<Object>();

                spawnerObject.data.pos = newObject.transform.position;
                spawnerObject.data.rot = newObject.transform.rotation;
                spawnerObject.data.type = Object.Type.Spawner;
                break;
            case ItemList.Goal:
                newObject = GameObject.Instantiate(ms.goalPrefab, ms.goalContainer);
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

    public void SetLevelManipulator(LevelManupulator _manipulator, bool _deselect = true)
    {
        if (_manipulator == manipulatorOption)
            return;
        manipulatorOption = _manipulator;
        if (_deselect)
            DeselectObject();
    }

    public void DeselectObject()
    {
        mo.isSelected = false;
        oe.UnselectObject();
    }
}
