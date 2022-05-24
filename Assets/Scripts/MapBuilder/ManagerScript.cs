using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Biocrowds.Core;

public class ManagerScript : MonoBehaviour
{
    public World world;

    [Header("Prefabs")]
    public GameObject spawnerPrefab;
    public GameObject goalPrefab;
    public Transform spawnerContainer;
    public Transform goalContainer;

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

    private void Start()
    {
        spawnerPrefab = world.prefabManager.GetSpawnAreaPrefab();
        goalPrefab = world.prefabManager.GetGoalPrefab();
        spawnerContainer = world.prefabManager.spawnAreaContainer;
        goalContainer = world.prefabManager.goalContainer;
    }

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
        user.SetLevelManipulator(MouseScript.LevelManupulator.Create);
    }

    public void ChooseDestroy()
    {
        user.SetLevelManipulator(MouseScript.LevelManupulator.Destroy);
    }

    public void ChooseEdit()
    {
        if (user.manipulatorOption == MouseScript.LevelManupulator.Link)
            user.SetLevelManipulator(MouseScript.LevelManupulator.Edit, false);
        else
            user.SetLevelManipulator(MouseScript.LevelManupulator.Edit);
    }

    public void ChooseMove(bool value)
    {
        if (value)
            user.SetLevelManipulator(MouseScript.LevelManupulator.Move);
    }

    public void ChooseLink(bool val)
    {
        if (user.manipulatorOption == MouseScript.LevelManupulator.Edit)
            user.SetLevelManipulator(MouseScript.LevelManupulator.Link, false);
        else if (user.manipulatorOption == MouseScript.LevelManupulator.Link)
        {
            if (user.oe.linkToggle.isOn)
            {
                user.SetLevelManipulator(MouseScript.LevelManupulator.Edit, false);
                user.oe.editToggle.isOn = true;
            }

        }
    }
}
