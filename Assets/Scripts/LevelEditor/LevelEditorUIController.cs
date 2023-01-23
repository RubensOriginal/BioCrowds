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
    [SerializeField] private ObjectEditor  objEditor;

    //--------------------------------------
    // Save/Load/Run
    [Header("Save/Load/Run")]
    public CustomPointerHandler saveSceneButton;
    public CustomPointerHandler loadSceneButton;
    public CustomPointerHandler runSceneButton;
    public RectTransform        simulationRunningLabel;
    public CustomPointerHandler createAlternativesButton;
    public CustomPointerHandler removeAlternativeButton;

    public CustomPointerHandler confirmLoadSaveSceneButton;
    public CustomPointerHandler confirmLoadLoadAnywayButton;
    public CustomPointerHandler confirmLoadCancelButton;

    public CustomPointerHandler saveFailedContinueButton;
    public CustomPointerHandler loadFailedContinueButton;

    public CustomPointerHandler simulationRunningContinueButton;

    //--------------------------------------
    // Clear
    public CustomPointerHandler clearSceneButton;
    public CustomPointerHandler confirmClearButton;
    public CustomPointerHandler cancelClearButton;

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
    public RectTransform confirmClearPanel;
    public RectTransform saveFailedPanel;
    public RectTransform loadFailedPanel;
    public RectTransform simulationRunningPanel;
    public RectTransform objectsPanel;
    public RectTransform editSpawnerPanel;
    public RectTransform editObstaclePanel;

    //--------------------------------------
    // Hints
    public RectTransform editObjectHint;
    public RectTransform editGoalHint;

    [Header("Cameras")]
    public List<Camera> cameras;
    public Camera currrentCamera;

    [Header("Test Level")]
    public GameObject mainTestLevel;
    public List<GameObject> testLevels;

    [HideInInspector]
    public bool isZoom { get; private set; }


    private void Awake()
    {
        levelEditorManager.world = sceneController.world;
        loadPresetPanel.gameObject.SetActive(false);
        confirmLoadPanel.gameObject.SetActive(false);
        confirmClearPanel.gameObject.SetActive(false);
        saveFailedPanel.gameObject.SetActive(false);
        loadFailedPanel.gameObject.SetActive(false);
        simulationRunningPanel.gameObject.SetActive(false);
        removeAlternativeButton.gameObject.SetActive(false);

        editObjectHint.gameObject.SetActive(false);
        editGoalHint.gameObject.SetActive(false);
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
        createAlternativesButton.OnPointerDownEvent += CreateAlternativesButton_OnPointerDownEvent;
        removeAlternativeButton.OnPointerDownEvent += RemoveAlternativeButton_OnPointerDownEvent;

        confirmLoadSaveSceneButton.OnPointerDownEvent += SaveSceneButton_OnPointerDownEvent;
        confirmLoadLoadAnywayButton.OnPointerDownEvent += ConfirmLoadLoadAnywayButton_OnPointerDownEvent;
        confirmLoadCancelButton.OnPointerDownEvent += ConfirmLoadCancelButton_OnPointerDownEvent;

        clearSceneButton.OnPointerDownEvent += ClearSceneButton_OnPointerDownEvent;
        confirmClearButton.OnPointerDownEvent += ConfirmClearButton_OnPointerDownEvent;
        cancelClearButton.OnPointerDownEvent += CancelClearButton_OnPointerDownEvent;

        saveFailedContinueButton.OnPointerDownEvent += SaveFailedContinueButton_OnPointerDownEvent;
        loadFailedContinueButton.OnPointerDownEvent += LoadFailedContinueButton_OnPointerDownEvent;

        simulationRunningContinueButton.OnPointerDownEvent += SimulationRunningContinueButton_OnPointerDownEvent;

        confirmPresetButton.OnPointerDownEvent += ConfirmPresetButton_OnPointerDownEvent;
        cancelPresetButton.OnPointerDownEvent += CancelPresetButton_OnPointerDownEvent;

        if (!testLevels.Contains(mainTestLevel))
            testLevels.Add(mainTestLevel);

        if (!cameras.Contains(mainTestLevel.GetComponentInChildren<Camera>()))
            cameras.Add(mainTestLevel.GetComponentInChildren<Camera>());

        isZoom = false;

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
            confirmClearPanel.gameObject.SetActive(false);
            simulationRunningPanel.gameObject.SetActive(false);
            loadFailedPanel.gameObject.SetActive(false);
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


        if (mode == MouseScript.LevelManupulator.Edit && !objEditor.GetSelected())
        {
            editObjectHint.gameObject.SetActive(true);
        }
        else
        {
            editObjectHint.gameObject.SetActive(false);
        }

        editGoalHint.gameObject.SetActive(mode == MouseScript.LevelManupulator.Link ? true : false);
        


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
        if (levelExporter.IsValidExport(sceneController.world, testLevels))
        {
            levelExporter.ExportLevel(sceneController.world, objImporter, LevelExporter.ExportType.Download, testLevels);
        }
        else
        {
            saveFailedPanel.gameObject.SetActive(true);
        }
    }

    private void RunSceneButton_OnPointerDownEvent(PointerEventData obj)
    {
        eventSystem.SetSelectedGameObject(null);

        if (levelExporter.IsValidExport(sceneController.world, testLevels))
        {
            levelExporter.ExportLevel(sceneController.world, objImporter, LevelExporter.ExportType.RunScene, testLevels);
            simulationRunningPanel.gameObject.SetActive(true);
            runSceneButton.gameObject.SetActive(false);
            simulationRunningLabel.gameObject.SetActive(true);
        }
        else
        {
            saveFailedPanel.gameObject.SetActive(true);
        }
    }

    private void CreateAlternativesButton_OnPointerDownEvent(PointerEventData obj)
    {
        if (testLevels.Count == 4)
            throw new System.Exception("It is not possible to create more alternatives");

        GameObject newTestLevel = Instantiate(mainTestLevel, new Vector3(testLevels.Count * 100, 0, 0), new Quaternion());
        newTestLevel.name = "TestLevel " + (testLevels.Count + 1);
        testLevels.Add(newTestLevel);

        cameras.Add(newTestLevel.GetComponentInChildren<Camera>());
        ResizeCameras();

        removeAlternativeButton.gameObject.SetActive(true);
        if (testLevels.Count == 4)
            createAlternativesButton.gameObject.SetActive(false);
    }

    private void RemoveAlternativeButton_OnPointerDownEvent(PointerEventData obj)
    {
        GameObject removedTestLevel = testLevels[testLevels.Count - 1];
        cameras.Remove(cameras[cameras.Count - 1]);
        testLevels.Remove(removedTestLevel);
        Destroy(removedTestLevel);

        ResizeCameras();

        createAlternativesButton.gameObject.SetActive(true);
        if (testLevels.Count == 1)
            removeAlternativeButton.gameObject.SetActive(false);
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

    private void LoadFailedContinueButton_OnPointerDownEvent(PointerEventData obj)
    {
        loadFailedPanel.gameObject.SetActive(false);
    }

    private void SimulationRunningContinueButton_OnPointerDownEvent(PointerEventData obj)
    {
        simulationRunningPanel.gameObject.SetActive(false);
    }

    public void SimulationFinishedRunning()
    {
        runSceneButton.gameObject.SetActive(true);
        simulationRunningLabel.gameObject.SetActive(false);
    }

    public void InvalidImport()
    {
        loadFailedPanel.gameObject.SetActive(true);
    }

    private void ClearSceneButton_OnPointerDownEvent(PointerEventData obj)
    {
        eventSystem.SetSelectedGameObject(null);
        confirmClearPanel.gameObject.SetActive(true);
    }
    private void ConfirmClearButton_OnPointerDownEvent(PointerEventData obj)
    {
        actionToggles[0].isOn = true;
        eventSystem.SetSelectedGameObject(null);
        sceneController.world.ClearWorld(true);
        objImporter.ClearLoadedModels();
        confirmClearPanel.gameObject.SetActive(false);
    }
    private void CancelClearButton_OnPointerDownEvent(PointerEventData obj)
    {
        eventSystem.SetSelectedGameObject(null);
        confirmClearPanel.gameObject.SetActive(false);
    }

    public bool IsPopUpPanelOpen()
    {
        if (loadPresetPanel.gameObject.activeSelf || confirmLoadPanel.gameObject.activeSelf 
            || saveFailedPanel.gameObject.activeSelf || simulationRunningPanel.gameObject.activeSelf 
            || loadFailedPanel.gameObject.activeSelf || confirmClearPanel.gameObject.activeSelf)
            return true;
        return false;
    }

    public void ResizeCameras()
    {
        isZoom = false;

        foreach (Camera camera in cameras) {
            if (!camera.enabled)
                camera.enabled = true;
                
        }

        switch (cameras.Count)
        {
            case 1:
                ResizeCamera(cameras[0], 0.0f, 0.0f, 1.0f, 1.0f);
                break;
            case 2:
                ResizeCamera(cameras[0], 0.0f, 0.0f, 0.5f, 1.0f);
                ResizeCamera(cameras[1], 0.5f, 0.0f, 0.5f, 1.0f);
                break;
            case 3:
                ResizeCamera(cameras[0], 0.0f, 0.5f, 0.5f, 0.5f);
                ResizeCamera(cameras[1], 0.5f, 0.5f, 0.5f, 0.5f);
                ResizeCamera(cameras[2], 0.0f, 0.0f, 1.0f, 0.5f);
                break;
            case 4:
                ResizeCamera(cameras[0], 0.0f, 0.5f, 0.5f, 0.5f);
                ResizeCamera(cameras[1], 0.5f, 0.5f, 0.5f, 0.5f);
                ResizeCamera(cameras[2], 0.0f, 0.0f, 0.5f, 0.5f);
                ResizeCamera(cameras[3], 0.5f, 0.0f, 0.5f, 0.5f);
                break;
        }
    }

    private void ResizeCamera(Camera camera, float x, float y, float width, float height)
    {
        camera.rect = new Rect(x, y, width, height);
    }

    public void ZoomCamera(Camera camera)
    {
        isZoom = true;

        camera.rect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);

        foreach (Camera cameraref in cameras)
        {
            if (camera != cameraref)
                cameraref.enabled = false;
        }
    }

}
