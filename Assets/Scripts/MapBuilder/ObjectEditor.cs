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
    private MouseScript.ItemList selectedItemType;

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

       
        if (isSelected)
        {
            var maxScale = Mathf.Max(selectedGameObject.transform.localScale.x, selectedGameObject.transform.localScale.z);
            editCircle.transform.localScale = Vector3.one * (maxScale * 1.3f);
            if (selectedItemType != MouseScript.ItemList.Spawner)
                goalCircle.transform.position = Vector3.one * -1000f;
        }
        else
        {
            editCircle.transform.position = Vector3.one * -1000f;
            goalCircle.transform.position = Vector3.one * -1000f;
        }
    }

    public bool GetSelected()
    {
        return isSelected;
    }
    public MouseScript.ItemList GetSelectedItemType()
    {
        return selectedItemType;
    }

    public void SelectObject(GameObject go)
    {
        if (go.tag == "Spawner")
        {
            if (isSelected)
                UnselectObject();
            selectedGameObject = go;
            selectedItemType = MouseScript.ItemList.Spawner;
            selectedGameObject.GetComponent<MeshRenderer>().material = ms.spawnerMaterialSelected;
            editCircle.transform.position = new Vector3(
                selectedGameObject.transform.position.x,
                0.1f,
                selectedGameObject.transform.position.z);
            var maxScale = Mathf.Max(selectedGameObject.transform.localScale.x, selectedGameObject.transform.localScale.z);
            editCircle.transform.localScale = Vector3.one * (maxScale * 1.3f);
            SpawnArea sp = go.GetComponent<SpawnArea>();
            if (sp.initialAgentsGoalList[0] == null)
                sp.initialAgentsGoalList.Clear();

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
        else if (go.tag == "Obstacle")
        {
            if (isSelected)
                UnselectObject();
            selectedGameObject = go;
            selectedItemType = MouseScript.ItemList.Obstacle;
            selectedGameObject.GetComponent<MeshRenderer>().material = ms.obstacleMaterialSelected;
            editCircle.transform.position = new Vector3(
                selectedGameObject.transform.position.x,
                0.1f,
                selectedGameObject.transform.position.z);

            goalCircle.transform.position = Vector3.one * -1000f;
            ms.obstacleWidthInputField.text = selectedGameObject.transform.localScale.x.ToString();
            ms.obstacleHeigthInputField.text = selectedGameObject.transform.localScale.z.ToString();
            ms.obstacleAngleInputField.text = selectedGameObject.transform.localRotation.eulerAngles.y.ToString();
            isSelected = true;
        }

    }

    public void UnselectObject()
    {
        if (selectedGameObject == null)
            return;
        if (selectedItemType == MouseScript.ItemList.Spawner)
        {
            selectedGameObject.GetComponent<MeshRenderer>().material = ms.spawnerMaterial;
            if (selectedGameObject.GetComponent<SpawnArea>().initialAgentsGoalList.Count != 0)
                selectedGameObject.GetComponent<SpawnArea>().initialAgentsGoalList[0].GetComponent<MeshRenderer>().material = ms.goalMaterial;
        }
        else if (selectedItemType == MouseScript.ItemList.Obstacle)
        {
            selectedGameObject.GetComponent<MeshRenderer>().material = ms.obstacleMaterial;
        }
        isSelected = false;
    }

    public void UpdateNumberAgents()
    {
        if (int.TryParse(ms.numberAgentsInputField.text, out int numberAgentsInt))
        {
            int clampValue = Mathf.Clamp(numberAgentsInt, 1, 10);
            selectedGameObject.GetComponent<SpawnArea>().initialNumberOfAgents = clampValue;
            if (clampValue != numberAgentsInt)
                ms.numberAgentsInputField.text = clampValue.ToString();
        }
    }

    public void UpdateObstacleWidth(string text)
    {
        if (float.TryParse(text, out float value))
        {
            float clampValue = Mathf.Clamp(value, 2, 20);

            Vector3 size = selectedGameObject.transform.localScale;
            size.x = clampValue;
            selectedGameObject.transform.localScale = size;

            if (clampValue != value)
                ms.obstacleWidthInputField.text = clampValue.ToString();
        }
    }

    public void UpdateObstacleHeigth(string text)
    {
        if (float.TryParse(text, out float value))
        {
            float clampValue = Mathf.Clamp(value, 2, 20);

            Vector3 size = selectedGameObject.transform.localScale;
            size.z = clampValue;
            selectedGameObject.transform.localScale = size;

            if (clampValue != value)
                ms.obstacleHeigthInputField.text = clampValue.ToString();
        }
    }

    public void UpdateObstacleAngleSlide(float value)
    {
        Vector3 rot = selectedGameObject.transform.localRotation.eulerAngles;
        rot.y = value;
        selectedGameObject.transform.localRotation = Quaternion.Euler(rot);
        ms.obstacleAngleInputField.text = ((int)value).ToString();
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
            selectedGameObject.GetComponent<MeshRenderer>().material = ms.spawnerMaterialSelected;
            goalCircle.transform.position = new Vector3(
                    spawnArea.initialAgentsGoalList[0].transform.position.x,
                    0.1f,
                    spawnArea.initialAgentsGoalList[0].transform.position.z);
            ms.user.SetLevelManipulator(MouseScript.LevelManupulator.Edit, false);
            editToggle.isOn = true;
        }
    }
}
