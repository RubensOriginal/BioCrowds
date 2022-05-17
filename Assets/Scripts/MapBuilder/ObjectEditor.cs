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

    public void selectObject(GameObject gameObject)
    {
        if (gameObject.tag == "Spawner")
        {
            if (isSelected)
                unselectObject();
            this.gameObject = gameObject;
            this.gameObject.GetComponent<MeshRenderer>().material = ms.spawnerMaterialSelected;
            isSelected = true;
        }
    }

    public void unselectObject()
    {
        this.gameObject.GetComponent<MeshRenderer>().material = ms.spawnerMaterial;
        isSelected = false;
    }

    public void updateNumberAgents(string numberAgents)
    {
        if (Int32.TryParse(numberAgents, out int numberAgentsInt))
            gameObject.GetComponent<SpawnArea>().initialNumberOfAgents = numberAgentsInt;

        
    }
}
