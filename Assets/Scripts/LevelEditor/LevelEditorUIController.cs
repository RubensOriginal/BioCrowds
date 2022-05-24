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
    public EventSystem eventSystem;

    public CustomPointerHandler importOBJButton;
    public CustomPointerHandler createMarkersButton;
    public CustomPointerHandler saveSceneButton;
    public CustomPointerHandler loadSceneButton;

    public List<Toggle> actionToggles;

    [SerializeField] private TMP_Text errorMessage;
    [SerializeField] private World simulationWorld;
    [SerializeField] private RuntimeOBJImporter objImporter;
    [SerializeField] private LevelExporter levelExporter;
    [SerializeField] private LevelImporter levelImporter;
    [SerializeField] private ManagerScript levelEditorManager;

    public RectTransform objectsPanel;
    public RectTransform editPanel;

    private void Awake()
    {
        levelEditorManager.world = sceneController.world;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (sceneController == null)
            sceneController = FindObjectOfType<SceneController>();

        importOBJButton.OnPointerDownEvent += ImportOBJButton_OnPointerDownEvent;
        createMarkersButton.OnPointerDownEvent += CreateMarkersButton_OnPointerDownEvent;
        saveSceneButton.OnPointerDownEvent += SaveSceneButton_OnPointerDownEvent;
        loadSceneButton.OnPointerDownEvent += LoadSceneButton_OnPointerDownEvent;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            actionToggles[0].isOn = true;
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            actionToggles[1].isOn = true;
        else if(Input.GetKeyDown(KeyCode.Alpha3))
            actionToggles[2].isOn = true;
        else if(Input.GetKeyDown(KeyCode.Alpha4))
            actionToggles[3].isOn = true;

        var mode = levelEditorManager.user.manipulatorOption;

        objectsPanel.gameObject.SetActive(mode == MouseScript.LevelManupulator.Create ? true : false);
        if ((mode == MouseScript.LevelManupulator.Edit || mode == MouseScript.LevelManupulator.Link) &&
            levelEditorManager.user.oe.GetSelected())
            editPanel.gameObject.SetActive(true);
        else
            editPanel.gameObject.SetActive(false);
    }

    private void LoadSceneButton_OnPointerDownEvent(PointerEventData obj)
    {
        eventSystem.SetSelectedGameObject(null);
        levelImporter.ImportLevel(simulationWorld, objImporter);
        actionToggles[0].isOn = true;
        levelEditorManager.user.oe.UnselectObject();
        levelEditorManager.user.mo.isSelected = false;
    }

    private void SaveSceneButton_OnPointerDownEvent(PointerEventData obj)
    {
        eventSystem.SetSelectedGameObject(null);
        levelExporter.ExportLevel(sceneController.world, objImporter);
    }

    private void CreateMarkersButton_OnPointerDownEvent(PointerEventData obj)
    {
        eventSystem.SetSelectedGameObject(null);
        sceneController.LoadSimulationWorld();
    }

    private void ImportOBJButton_OnPointerDownEvent(PointerEventData eventData)
    {
        eventSystem.SetSelectedGameObject(null);
        objImporter.LoadOBJ();
    }
}
