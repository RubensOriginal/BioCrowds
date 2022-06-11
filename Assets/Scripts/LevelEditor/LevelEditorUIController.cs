using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Biocrowds.Core;
using System.Linq;

public class LevelEditorUIController : MonoBehaviour
{
    public SceneController sceneController;
    public EventSystem eventSystem;

    [SerializeField] private World simulationWorld;
    [SerializeField] private RuntimeOBJImporter objImporter;
    [SerializeField] private LevelExporter levelExporter;
    [SerializeField] private LevelImporter levelImporter;
    [SerializeField] private ManagerScript levelEditorManager;

    //--------------------------------------
    // Save/Load/Run
    [Header("Save/Load/Run")]
    public CustomPointerHandler saveSceneButton;
    public CustomPointerHandler loadSceneButton;
    public CustomPointerHandler runSceneButton;
    public RectTransform        simulationRunningLabel;

    public CustomPointerHandler confirmLoadSaveSceneButton;
    public CustomPointerHandler confirmLoadLoadAnywayButton;
    public CustomPointerHandler confirmLoadCancelButton;

    public CustomPointerHandler saveFailedContinueButton;

    public CustomPointerHandler simulationRunningContinueButton;

    //--------------------------------------
    // Actions
    [Header("Actions")]
    public List<Toggle> actionToggles;

    //--------------------------------------
    // Create Objects/Presets
    [Header("Create Objects/Presets")]
    public CustomPointerHandler importOBJButton;
    public CustomPointerHandler clearOBJButton;
    public CustomPointerHandler confirmPresetButton;
    public CustomPointerHandler cancelPresetButton;
        
    public ToggleGroup  presetToggleGroup;
    public List<Toggle> presetToggles;

    //--------------------------------------
    // Edit Objects
    [Header("Edit Objects")]
    public TMP_InputField agentNumberInputField;

    //--------------------------------------
    // Panels
    [Header("Panels")]
    public RectTransform loadPresetPanel;
    public RectTransform confirmLoadPanel;
    public RectTransform saveFailedPanel;
    public RectTransform simulationRunningPanel;
    public RectTransform objectsPanel;
    public RectTransform editSpawnerPanel;
    public RectTransform editObstaclePanel;

    private void Awake()
    {
        levelEditorManager.world = sceneController.world;
        loadPresetPanel.gameObject.SetActive(false);
        confirmLoadPanel.gameObject.SetActive(false);
        saveFailedPanel.gameObject.SetActive(false);
        simulationRunningPanel.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (sceneController == null)
            sceneController = FindObjectOfType<SceneController>();


        importOBJButton.OnPointerDownEvent += ImportOBJButton_OnPointerDownEvent;
        clearOBJButton.OnPointerDownEvent += ClearOBJButton_OnPointerDownEvent;
        runSceneButton.OnPointerDownEvent += RunSceneButton_OnPointerDownEvent;
        saveSceneButton.OnPointerDownEvent += SaveSceneButton_OnPointerDownEvent;
        loadSceneButton.OnPointerDownEvent += LoadSceneButton_OnPointerDownEvent;

        confirmLoadSaveSceneButton.OnPointerDownEvent += SaveSceneButton_OnPointerDownEvent;
        confirmLoadLoadAnywayButton.OnPointerDownEvent += ConfirmLoadLoadAnywayButton_OnPointerDownEvent;
        confirmLoadCancelButton.OnPointerDownEvent += ConfirmLoadCancelButton_OnPointerDownEvent;

        saveFailedContinueButton.OnPointerDownEvent += SaveFailedContinueButton_OnPointerDownEvent;

        simulationRunningContinueButton.OnPointerDownEvent += SimulationRunningContinueButton_OnPointerDownEvent;

        confirmPresetButton.OnPointerDownEvent += ConfirmPresetButton_OnPointerDownEvent;
        cancelPresetButton.OnPointerDownEvent += CancelPresetButton_OnPointerDownEvent;

        
    }


    private void Update()
    {
        if (!levelEditorManager.HasInputFieldFocused() && !IsPopUpPanelOpen())
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                actionToggles[0].isOn = true;
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                actionToggles[1].isOn = true;
            else if (Input.GetKeyDown(KeyCode.Alpha3))
                actionToggles[2].isOn = true;
            else if (Input.GetKeyDown(KeyCode.Alpha4))
                actionToggles[3].isOn = true;
        }

        if (Input.GetKeyDown(KeyCode.Escape) && IsPopUpPanelOpen())
        {
            confirmLoadPanel.gameObject.SetActive(false);
            saveFailedPanel.gameObject.SetActive(false);
        }

        var mode = levelEditorManager.user.manipulatorOption;

        objectsPanel.gameObject.SetActive(mode == MouseScript.LevelManupulator.Create ? true : false);
        if ((mode == MouseScript.LevelManupulator.Edit || mode == MouseScript.LevelManupulator.Link) &&
            levelEditorManager.user.oe.GetSelected())
        {
            if (levelEditorManager.user.oe.GetSelectedItemType() == MouseScript.ItemList.Spawner)
            {
                editSpawnerPanel.gameObject.SetActive(true);
                editObstaclePanel.gameObject.SetActive(false);
            }
            else if (levelEditorManager.user.oe.GetSelectedItemType() == MouseScript.ItemList.Obstacle)
            {
                editSpawnerPanel.gameObject.SetActive(false);
                editObstaclePanel.gameObject.SetActive(true);
            }
        }
        else
        {
            editSpawnerPanel.gameObject.SetActive(false);
            editObstaclePanel.gameObject.SetActive(false);
        }

        importOBJButton.gameObject.SetActive(objImporter.loadedModels.Count == 0);
        clearOBJButton.gameObject.SetActive(objImporter.loadedModels.Count > 0);
    }


    
    private void ImportOBJButton_OnPointerDownEvent(PointerEventData eventData)
    {
        eventSystem.SetSelectedGameObject(null);
        loadPresetPanel.gameObject.SetActive(true);
    }

    private void ClearOBJButton_OnPointerDownEvent(PointerEventData obj)
    {
        eventSystem.SetSelectedGameObject(null);
        objImporter.ClearLoadedModels();
    }

    private void ConfirmPresetButton_OnPointerDownEvent(PointerEventData obj)
    {

        eventSystem.SetSelectedGameObject(null);
        var selected = presetToggleGroup.ActiveToggles().FirstOrDefault();
        objImporter.LoadPreset(presetToggles.IndexOf(selected));
        loadPresetPanel.gameObject.SetActive(false);
    }

    private void CancelPresetButton_OnPointerDownEvent(PointerEventData obj)
    {
        eventSystem.SetSelectedGameObject(null);
        loadPresetPanel.gameObject.SetActive(false);
    }


    private void LoadSceneButton_OnPointerDownEvent(PointerEventData obj)
    {
        eventSystem.SetSelectedGameObject(null);
        actionToggles[0].isOn = true;
        levelEditorManager.user.oe.UnselectObject();
        levelEditorManager.user.mo.isSelected = false;
        confirmLoadPanel.gameObject.SetActive(true);
    }

    private void SaveSceneButton_OnPointerDownEvent(PointerEventData obj)
    {
        eventSystem.SetSelectedGameObject(null);
        if (levelExporter.IsValidExport(sceneController.world))
        {
            levelExporter.ExportLevel(sceneController.world, objImporter, LevelExporter.ExportType.Download);
        }
        else
        {
            saveFailedPanel.gameObject.SetActive(true);
        }
    }

    private void RunSceneButton_OnPointerDownEvent(PointerEventData obj)
    {
        eventSystem.SetSelectedGameObject(null);

        if (levelExporter.IsValidExport(sceneController.world))
        {
            levelExporter.ExportLevel(sceneController.world, objImporter, LevelExporter.ExportType.RunScene);
            simulationRunningPanel.gameObject.SetActive(true);
            runSceneButton.gameObject.SetActive(false);
            simulationRunningLabel.gameObject.SetActive(true);
        }
        else
        {
            saveFailedPanel.gameObject.SetActive(true);
        }
    }

    private void ConfirmLoadLoadAnywayButton_OnPointerDownEvent(PointerEventData obj)
    {
        levelImporter.ImportLevel(simulationWorld, objImporter);
        confirmLoadPanel.gameObject.SetActive(false);
    }


    private void ConfirmLoadCancelButton_OnPointerDownEvent(PointerEventData obj)
    {
        confirmLoadPanel.gameObject.SetActive(false);
    }
    private void SaveFailedContinueButton_OnPointerDownEvent(PointerEventData obj)
    {
        saveFailedPanel.gameObject.SetActive(false);
    }

    private void SimulationRunningContinueButton_OnPointerDownEvent(PointerEventData obj)
    {
        simulationRunningPanel.gameObject.SetActive(false);
    }

    public void SimulationFinishedRunning(bool finished)
    {
        runSceneButton.gameObject.SetActive(true);
        simulationRunningLabel.gameObject.SetActive(false);
    }

    public bool IsPopUpPanelOpen()
    {
        if (loadPresetPanel.gameObject.activeSelf || confirmLoadPanel.gameObject.activeSelf 
            || saveFailedPanel.gameObject.activeSelf || simulationRunningPanel.gameObject.activeSelf)
            return true;
        return false;
    }
}
