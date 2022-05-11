using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerScript : MonoBehaviour
{
    public GameObject spawnerPrefab;
    public GameObject goalPrefab;

    public MeshFilter mouseObject;

    public MouseScript user;

    private MapBuilder mapBuider;

    // Update is called once per frame
    void Update()
    {
        CreateEditor();
    }

    MapBuilder CreateEditor()
    {
        mapBuider = new MapBuilder();
        mapBuider.objects = new List<Object.Data>();
        return mapBuider;
    }

    // ItemList Options

    public void ChooseSpawner()
    {
        user.itemOption = MouseScript.ItemList.Spawner;
        /*
        GameObject spawner = GameObject.Instantiate(spawnerPrefab);
        mouseObject.mesh = spawner.GetComponent<MeshFilter>().mesh;
        Destroy(spawner);
        */
    }

    public void ChooseGoal()
    {
        user.itemOption = MouseScript.ItemList.Goal;
        /*
        GameObject goal = GameObject.Instantiate(goalPrefab);
        mouseObject.mesh = goal.GetComponent<MeshFilter>().mesh;
        Destroy(goal);
        */
    }

    // LevelManupulator Options

    public void ChooseCreate()
    {
        user.manipulatorOption = MouseScript.LevelManupulator.Create;
    }

    public void ChooseDestroy()
    {
        user.manipulatorOption = MouseScript.LevelManupulator.Destroy;
    }
}
