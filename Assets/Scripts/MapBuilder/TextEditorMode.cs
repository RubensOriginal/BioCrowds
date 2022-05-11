using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextEditorMode : MonoBehaviour
{

    public MouseScript ms;

    private TMP_Text textMeshPro;

    // Start is called before the first frame update
    void Start()
    {
        textMeshPro = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ms.manipulatorOption == MouseScript.LevelManupulator.Create)
        {
            if (ms.itemOption == MouseScript.ItemList.Spawner)
                textMeshPro.SetText("Manipulation Tool: Create \n Item: Spawner");
            else if (ms.itemOption == MouseScript.ItemList.Goal)
                textMeshPro.SetText("Manipulation Tool: Create \n Item: Goal");
        } 
        else if (ms.manipulatorOption == MouseScript.LevelManupulator.Destroy)
            textMeshPro.SetText("Manipulation Tool: Destroy");
    }
}
