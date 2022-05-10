using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class LevelEditorUIController : MonoBehaviour
{
    public SceneController sceneController;

    public CustomPointerHandler importOBJButton;
    public CustomPointerHandler createMarkersButton;
    public CustomPointerHandler saveSceneButton;

    [SerializeField] private TMP_Text errorMessage;
    [SerializeField] private RuntimeOBJImporter objImporter;
    [SerializeField] private LevelExporter levelExporter;


    // Start is called before the first frame update
    void Start()
    {
        if (sceneController == null)
            sceneController = FindObjectOfType<SceneController>();
        importOBJButton.OnPointerDownEvent += ImportOBJButtonClicked;
        createMarkersButton.OnPointerDownEvent += CreateMarkersButton_OnPointerDownEvent;
        saveSceneButton.OnPointerDownEvent += SaveSceneButton_OnPointerDownEvent;
    }

    private void SaveSceneButton_OnPointerDownEvent(PointerEventData obj)
    {
        levelExporter.ExportLevel(objImporter);
    }

    private void CreateMarkersButton_OnPointerDownEvent(PointerEventData obj)
    {
        sceneController.LoadSimulationWorld();
    }

    private void ImportOBJButtonClicked(PointerEventData eventData)
    {
        Debug.Log("OBJ Clicked");
        objImporter.LoadOBJ();
    }
}
