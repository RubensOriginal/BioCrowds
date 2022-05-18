using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;

public class ObjectEditor : MonoBehaviour {

    [HideInInspector]
    public GameObject gameObject;

    public ManagerScript ms;

    private bool isSelected = false;

    public void Awake()
    {
        gameObject = new GameObject();
    }

    public void SelectObject(GameObject gameObject)
    {
        if (gameObject.tag == "Spawner")
        {
            if (isSelected)
                UnselectObject();
            this.gameObject = gameObject;
            this.gameObject.GetComponent<MeshRenderer>().material = ms.spawnerMaterialSelected;
            if (gameObject.GetComponent<SpawnArea>().initialAgentsGoalList.Count != 0)
                this.gameObject.GetComponent<SpawnArea>().initialAgentsGoalList[0].GetComponent<MeshRenderer>().material = ms.goalMaterialSelected;
            isSelected = true;
            ms.numberAgentsInputField.text = gameObject.GetComponent<SpawnArea>().initialNumberOfAgents.ToString();
        }
    }

    public void UnselectObject()
    {
        this.gameObject.GetComponent<MeshRenderer>().material = ms.spawnerMaterial;
        if (gameObject.GetComponent<SpawnArea>().initialAgentsGoalList.Count != 0)
            this.gameObject.GetComponent<SpawnArea>().initialAgentsGoalList[0].GetComponent<MeshRenderer>().material = ms.goalMaterial;
        isSelected = false;
    }

    public void UpdateNumberAgents()
    {
        if (ms.numberAgentsInputField.text != "" && (ms.user.manipulatorOption == MouseScript.LevelManupulator.Edit || ms.user.manipulatorOption == MouseScript.LevelManupulator.Link))
            if (Int32.TryParse(ms.numberAgentsInputField.text, out int numberAgentsInt))
                gameObject.GetComponent<SpawnArea>().initialNumberOfAgents = numberAgentsInt;
            
    }

    public void LinkGoalToSpawner(GameObject gameObject)
    {
        if (isSelected)
        {
            SpawnArea spawnArea = this.gameObject.GetComponent<SpawnArea>();
            if (spawnArea.initialAgentsGoalList.Count == 0)
            {
                spawnArea.initialAgentsGoalList.Add(gameObject);
                this.gameObject.GetComponent<SpawnArea>().initialAgentsGoalList[0].GetComponent<MeshRenderer>().material = ms.goalMaterialSelected;
            } else
            {
                this.gameObject.GetComponent<SpawnArea>().initialAgentsGoalList[0].GetComponent<MeshRenderer>().material = ms.goalMaterial;
                spawnArea.initialAgentsGoalList.RemoveAt(0);
                spawnArea.initialAgentsGoalList.Add(gameObject);
                this.gameObject.GetComponent<SpawnArea>().initialAgentsGoalList[0].GetComponent<MeshRenderer>().material = ms.goalMaterialSelected;
            }
            ms.user.manipulatorOption = MouseScript.LevelManupulator.Edit;
        }
    }
}
