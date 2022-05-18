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
            isSelected = true;
            ms.numberAgentsInputField.text = gameObject.GetComponent<SpawnArea>().initialNumberOfAgents.ToString();
        }
    }

    public void UnselectObject()
    {
        this.gameObject.GetComponent<MeshRenderer>().material = ms.spawnerMaterial;
        isSelected = false;
    }

    public void UpdateNumberAgents()
    {
        Debug.Log("N Agents: " + ms.numberAgentsInputField.text);
        if (ms.numberAgentsInputField.text == "")
            if (Int32.TryParse(ms.numberAgentsInputField.text, out int numberAgentsInt))
                gameObject.GetComponent<SpawnArea>().initialNumberOfAgents = numberAgentsInt;


    }
}
