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

    public CustomPointerHandler confirmLoadSaveSceneButton;
    public CustomPointerHandler confirmLoadLoadAnywayButton;
    public CustomPointerHandler confirmLoadCancelButton;

    public CustomPointerHandler saveFailedContinueButton;

    

    public List<Toggle> actionToggles;
    public TMP_InputField agentNumberInputField;

    [SerializeField] private TMP_Text errorMessage;
    [SerializeField] private World simulationWorld;
    [SerializeField] private RuntimeOBJImporter objImporter;
    [SerializeField] private LevelExporter levelExporter;
    [SerializeField] private LevelImporter levelImporter;
    [SerializeField] private ManagerScript levelEditorManager;

    public RectTransform confirmLoadPanel;
    public RectTransform saveFailedPanel;
    public RectTransform objectsPanel;
    public RectTransform editPanel;

    private void Awake()
    {
        levelEditorManager.world = sceneController.world;
        confirmLoadPanel.gameObject.SetActive(false);
        saveFailedPanel.gameObject.SetActive(false);
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

        confirmLoadSaveSceneButton.OnPointerDownEvent += SaveSceneButton_OnPointerDownEvent;
        confirmLoadLoadAnywayButton.OnPointerDownEvent += ConfirmLoadLoadAnywayButton_OnPointerDownEvent;
        confirmLoadCancelButton.OnPointerDownEvent += ConfirmLoadCancelButton_OnPointerDownEvent;

        saveFailedContinueButton.OnPointerDownEvent += SaveFailedContinueButton_OnPointerDownEvent;
    }

   

    private void Update()
    {
        if (!agentNumberInputField.isFocused && !IsPopUpPanelOpen())
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
            editPanel.gameObject.SetActive(true);
        else
            editPanel.gameObject.SetActive(false);
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
            levelExporter.ExportLevel(sceneController.world, objImporter);
        }
        else
        {
            saveFailedPanel.gameObject.SetActive(true);
        }
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

    private void ConfirmLoadLoadAnywayButton_OnPointerDownEvent(PointerEventData obj)
    {
        levelImporter.ImportLevel(simulationWorld, objImporter);
    }

    private void ConfirmLoadCancelButton_OnPointerDownEvent(PointerEventData obj)
    {
        confirmLoadPanel.gameObject.SetActive(false);
    }
    private void SaveFailedContinueButton_OnPointerDownEvent(PointerEventData obj)
    {
        saveFailedPanel.gameObject.SetActive(false);
    }

    public bool IsPopUpPanelOpen()
    {
        if (confirmLoadPanel.gameObject.activeSelf || saveFailedPanel.gameObject.activeSelf)
            return true;
        return false;
    }
}
