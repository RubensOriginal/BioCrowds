using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class LevelEditorUIController : MonoBehaviour
{
    public CustomPointerHandler importOBJButton;

    [SerializeField] private TMP_Text errorMessage;
    [SerializeField] private RuntimeOBJImporter objImporter;


    // Start is called before the first frame update
    void Start()
    {
        importOBJButton.OnPointerDownEvent += ImportOBJButtonClicked;
    }


    private void ImportOBJButtonClicked(PointerEventData eventData)
    {
        Debug.Log("OBJ Clicked");
        objImporter.LoadOBJ();
    }
}
