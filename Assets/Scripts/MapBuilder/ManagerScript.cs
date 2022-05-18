using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ManagerScript : MonoBehaviour
{

    [Header("Prefabs")]
    public GameObject spawnerPrefab;
    public GameObject goalPrefab;

    [Header("Materials")]
    public Material spawnerMaterial;
    public Material spawnerMaterialSelected;
    public Material goalMaterial;
    public Material goalMaterialSelected;

    [Header("Other Gameobjects")]
    public MeshFilter mouseObject;
    public MouseScript user;
    public TMP_InputField numberAgentsInputField;

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

    public void ChooseEdit()
    {
        user.manipulatorOption = MouseScript.LevelManupulator.Edit;
    }

    public void ChooseMove()
    {
        user.manipulatorOption = MouseScript.LevelManupulator.Move;
    }

    public void ChooseLink()
    {
        if (user.manipulatorOption == MouseScript.LevelManupulator.Edit)
            user.manipulatorOption = MouseScript.LevelManupulator.Link;
        else if (user.manipulatorOption == MouseScript.LevelManupulator.Link)
            user.manipulatorOption = MouseScript.LevelManupulator.Edit;
    }
}
