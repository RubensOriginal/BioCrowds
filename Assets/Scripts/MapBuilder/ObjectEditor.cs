using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UnityEngine;

public class ObjectEditor : MonoBehaviour {

    [HideInInspector]
    public GameObject selectedGameObject;
    public Toggle editToggle;
    public Toggle linkToggle;

    public ManagerScript ms;

    private bool isSelected = false;

    public GameObject editCircle;
    public GameObject goalCircle;

    public void Awake()
    {
        selectedGameObject = null;
    }

    public void Update()
    {
        editCircle.transform.Rotate(Vector3.forward * 90f * Time.deltaTime);
        goalCircle.transform.Rotate(Vector3.forward * 90f * Time.deltaTime);

        if (!isSelected)
        {
            editCircle.transform.position = Vector3.one * -1000f;
            goalCircle.transform.position = Vector3.one * -1000f;
        }
    }

    public bool GetSelected()
    {
        return isSelected;
    }

    public void SelectObject(GameObject go)
    {
        if (go.tag == "Spawner")
        {
            if (isSelected)
                UnselectObject();
            selectedGameObject = go;
            selectedGameObject.GetComponent<MeshRenderer>().material = ms.spawnerMaterialSelected;
            editCircle.transform.position = new Vector3(
                selectedGameObject.transform.position.x,
                0.1f,
                selectedGameObject.transform.position.z);
            SpawnArea sp = go.GetComponent<SpawnArea>();
            if (sp.initialAgentsGoalList.Count != 0)
            {
                sp.initialAgentsGoalList[0].GetComponent<MeshRenderer>().material = ms.goalMaterialSelected;
                goalCircle.transform.position = new Vector3(
                    sp.initialAgentsGoalList[0].transform.position.x,
                    0.1f,
                    sp.initialAgentsGoalList[0].transform.position.z);
            }
            else
            {
                goalCircle.transform.position = Vector3.one * -1000f;
            }
            isSelected = true;
            ms.numberAgentsInputField.text = go.GetComponent<SpawnArea>().initialNumberOfAgents.ToString();
        }
    }

    public void UnselectObject()
    {
        if (selectedGameObject == null)
            return;
        selectedGameObject.GetComponent<MeshRenderer>().material = ms.spawnerMaterial;
        if (selectedGameObject.GetComponent<SpawnArea>().initialAgentsGoalList.Count != 0)
            selectedGameObject.GetComponent<SpawnArea>().initialAgentsGoalList[0].GetComponent<MeshRenderer>().material = ms.goalMaterial;
        isSelected = false;
    }

    public void UpdateNumberAgents()
    {
        if (ms.numberAgentsInputField.text != "" && (ms.user.manipulatorOption == MouseScript.LevelManupulator.Edit || ms.user.manipulatorOption == MouseScript.LevelManupulator.Link))
            if (Int32.TryParse(ms.numberAgentsInputField.text, out int numberAgentsInt))
                selectedGameObject.GetComponent<SpawnArea>().initialNumberOfAgents = numberAgentsInt;
            
    }

    public void LinkGoalToSpawner(GameObject gameObject)
    {
        if (isSelected && gameObject.tag == "Goal")
        {
            SpawnArea spawnArea = this.selectedGameObject.GetComponent<SpawnArea>();
            if (spawnArea.initialAgentsGoalList.Count == 0)
            {
                spawnArea.initialAgentsGoalList.Add(gameObject);
                spawnArea.initialAgentsGoalList[0].GetComponent<MeshRenderer>().material = ms.goalMaterialSelected;
            } 
            else
            {
                spawnArea.initialAgentsGoalList[0].GetComponent<MeshRenderer>().material = ms.goalMaterial;
                spawnArea.initialAgentsGoalList.RemoveAt(0);
                spawnArea.initialAgentsGoalList.Add(gameObject);
                spawnArea.initialAgentsGoalList[0].GetComponent<MeshRenderer>().material = ms.goalMaterialSelected;
            }
            goalCircle.transform.position = new Vector3(
                    spawnArea.initialAgentsGoalList[0].transform.position.x,
                    0.1f,
                    spawnArea.initialAgentsGoalList[0].transform.position.z);
            ms.user.SetLevelManipulator(MouseScript.LevelManupulator.Edit, false);
            editToggle.isOn = true;
        }
    }
}
