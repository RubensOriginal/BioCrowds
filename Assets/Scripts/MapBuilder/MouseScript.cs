using UnityEngine.EventSystems;
using UnityEngine;
using UnityEditor;

public class MouseScript : MonoBehaviour
{

    public enum LevelManupulator {  Create, Move, Edit, Destroy, Link };
    public enum ItemList { Spawner, Goal, Obstacle };

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
    private RaycastHit terrainHit;

    public MoveObject mo;

    // Start is called before the first frame update
    void Start()
    {
        mr = GetComponent<MeshRenderer>();
        mo = GetComponent<MoveObject>();
    }

    // Update is called once per frame
    void Update()
    {

        for (int i = 0; i < ms.uiController.cameras.Count; i++)
        {
            Camera camera = ms.uiController.cameras[i];
            GameObject testLevel = ms.uiController.testLevels[i];

            ray = camera.ScreenPointToRay(Input.mousePosition);

            colliding = false;
            hitTerrain = false;

            RaycastHit[] hits = Physics.RaycastAll(ray);
            if (hits.Length > 0)
            {
                for (int j = 0; j < hits.Length; j++)
                {
                    if (hits[j].collider.gameObject.layer == 9)
                    {
                        colliding = true;
                        hit = hits[j];
                    }
                    else if (hits[j].transform.tag == "Terrain")
                    {
                        hitTerrain = true;
                        terrainHit = hits[j];
                    }
                }
            }

            if (hitTerrain)
            {
                ms.uiController.currrentCamera = terrainHit.collider.gameObject.transform.parent.parent.GetComponentInChildren<Camera>();

                mousePos = camera.ScreenToWorldPoint(Input.mousePosition);

                transform.position = new Vector3(
                    Mathf.Clamp(mousePos.x, -50 + testLevel.transform.position.x, 50 + testLevel.transform.position.x),
                    0.50f,
                    Mathf.Clamp(mousePos.z, -50 + testLevel.transform.position.z, 50 + testLevel.transform.position.z));
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

            case ItemList.Obstacle:
                newObject = GameObject.Instantiate(ms.obstaclePrefab, ms.obstacleContainer);
                newObject.transform.position = transform.position;
                newObject.layer = 9;
                newObject.tag = "Obstacle";

                Object obstacleObject = newObject.AddComponent<Object>();

                obstacleObject.data.pos = newObject.transform.position;
                obstacleObject.data.rot = newObject.transform.rotation;
                obstacleObject.data.type = Object.Type.Obstacle;
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
