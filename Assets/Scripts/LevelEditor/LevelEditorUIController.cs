using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Biocrowds.Core;

public class LevelEditorUIController : MonoBehaviour
{
    public SceneController sceneController;

    public CustomPointerHandler importOBJButton;
    public CustomPointerHandler createMarkersButton;
    public CustomPointerHandler saveSceneButton;
    public CustomPointerHandler loadSceneButton;
    public CustomPointerHandler loadSceneTestButton;

    [SerializeField] private TMP_Text errorMessage;
    [SerializeField] private World simulationWorld;
    [SerializeField] private RuntimeOBJImporter objImporter;
    [SerializeField] private LevelExporter levelExporter;
    [SerializeField] private LevelImporter levelImporter;


    // Start is called before the first frame update
    void Start()
    {
        if (sceneController == null)
            sceneController = FindObjectOfType<SceneController>();
        importOBJButton.OnPointerDownEvent += ImportOBJButtonClicked;
        createMarkersButton.OnPointerDownEvent += CreateMarkersButton_OnPointerDownEvent;
        saveSceneButton.OnPointerDownEvent += SaveSceneButton_OnPointerDownEvent;
        loadSceneButton.OnPointerDownEvent += LoadSceneButton_OnPointerDownEvent;
        loadSceneTestButton.OnPointerDownEvent += LoadSceneTestButton_OnPointerDownEvent;
    }

    private void LoadSceneTestButton_OnPointerDownEvent(PointerEventData obj)
    {
        levelImporter.ImportTestLevel(simulationWorld);
    }

    private void LoadSceneButton_OnPointerDownEvent(PointerEventData obj)
    {
        throw new System.NotImplementedException();
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
