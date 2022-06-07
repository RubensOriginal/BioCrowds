using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Biocrowds.Core;

public class ManagerScript : MonoBehaviour
{
    public World world;
    public LevelEditorUIController uiController;

    [Header("Prefabs")]
    public GameObject spawnerPrefab;
    public GameObject goalPrefab;
    public GameObject obstaclePrefab;
    public Transform spawnerContainer;
    public Transform goalContainer;
    public Transform obstacleContainer;

    [Header("Materials")]
    public Material spawnerMaterial;
    public Material spawnerMaterialSelected;
    public Material goalMaterial;
    public Material goalMaterialSelected;
    public Material obstacleMaterial;
    public Material obstacleMaterialSelected;

    [Header("Other Gameobjects")]
    public MeshFilter mouseObject;
    public MouseScript user;
    public TMP_InputField numberAgentsInputField;
    public TMP_InputField obstacleWidthInputField;
    public TMP_InputField obstacleHeigthInputField;
    public TMP_InputField obstacleAngleInputField;

    private MapBuilder mapBuider;

    private void Start()
    {
        spawnerPrefab = world.prefabManager.GetSpawnAreaPrefab();
        goalPrefab = world.prefabManager.GetGoalPrefab();
        obstaclePrefab = world.prefabManager.GetObstaclePrefab();
        spawnerContainer = world.prefabManager.spawnAreaContainer;
        goalContainer = world.prefabManager.goalContainer;
        obstacleContainer = world.prefabManager.obstacleContainer;
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
    public void ChooseItemOption(int item)
    {
        user.itemOption = (MouseScript.ItemList)item;
    }
    public void ChooseItemOption(MouseScript.ItemList item)
    {
        user.itemOption = item;
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

    public bool HasInputFieldFocused()
    {
        if (numberAgentsInputField.isFocused)
            return true;
        if (obstacleWidthInputField.isFocused)
            return true;
        if (obstacleHeigthInputField.isFocused)
            return true;
        if (obstacleAngleInputField.isFocused)
            return true;
        return false;
    }
}
