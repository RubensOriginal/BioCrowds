using System.Collections.Generic;
using UnityEngine;
using Biocrowds.Core;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;
using UnityEngine.Events;

public class LevelImporter : MonoBehaviour
{
    [SerializeField]
    private RuntimeOBJImporter objImporter;

    public ExtensionFilter[] extensions;
    [SerializeField]
    private LevelEditorUIController editorUIController;


    [SerializeField]
    private UnityEvent OnInvalidLoad;


    private void Start()
    {
        extensions = new[] {
            new ExtensionFilter("JSON Files", "json"),
        };
    }

    public void ImportLevel(RuntimeOBJImporter _objLoader)
    {
        objImporter = _objLoader;
#if UNITY_WEBGL && !UNITY_EDITOR
        UploadFile(gameObject.name, "OnFileUpload", ".json", false);
#else
        StartCoroutine(DesktopImportCoroutine());
#endif
    }

    public void ImportTestLevel(RuntimeOBJImporter _objLoader)
    {
        objImporter = _objLoader;
        var fileFinalName = "test";
        StreamReader reader = new StreamReader("Assets/Resources/SavedLevels/" + fileFinalName + ".json", false);
        var text = reader.ReadToEnd();
        reader.Close();
        LoadContent(text);
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    
	// WebGL
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    // Called from browser
    public void OnFileUpload(string url) {
        StartCoroutine(WebGLImportCoroutine(url));
    }

	private IEnumerator WebGLImportCoroutine(string url)
    {
        var loader = new WWW(url);
        yield return loader;
        if (!IsValidImport(loader.text))
        {
            OnInvalidLoad.Invoke();
            yield break;
        }
        LoadContent(loader.text);
        yield break;
    }
#else
    IEnumerator DesktopImportCoroutine()
    {
        yield return new WaitForEndOfFrame(); // To re-enable button color
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        if (paths.Length > 0)
        {
            using var fs = new FileStream(paths[0], FileMode.Open);
            string content = new StreamReader(fs).ReadToEnd();

            if (!IsValidImport(content))
            {
                OnInvalidLoad.Invoke();
                yield break;
            }
            LoadContent(content);
        }
        yield return null;

    }
#endif

    public bool IsValidImport(string text)
    {
        if (!text.Contains("terrains"))
            return false;
        if (!text.Contains("goals"))
            return false;
        if (!text.Contains("obstacles"))
            return false;
        if (!text.Contains("spawnAreas"))
            return false;
        if (!text.Contains("loaded_models"))
            return false;
        return true;
    }

    private void LoadSimulationScenario(JObject data, int alternativeIndex)
    {
        var scenario = editorUIController.CreateNewAlternative();
        SimulationPrefabManager prefabManager = scenario.world.prefabManager;
        List<Transform> _goals = new List<Transform>();
        List<Transform> _goalsToRemove = new List<Transform>();
        
        var _terrain = scenario.world.GetTerrain();
        var _t = data["terrains"][alternativeIndex];
        //_terrain.terrainData.size = ;
        scenario.world.UpateTerrainSize(new Vector3(_t["terrain_size"][0].ToObject<float>(),
                                                _t["terrain_size"][1].ToObject<float>(),
                                                _t["terrain_size"][2].ToObject<float>()));
        // Goals
        var _goalData = data["goals"] as JArray;
        for (int i = 0; i < _goalData.Count; i++)
        {
            Vector3 _goalPos = Vector3Extensions.FromJObject((_goalData[i]["position"]));
            GameObject newGoal = Instantiate(prefabManager.GetGoalPrefab(), _goalPos, Quaternion.identity, prefabManager.goalContainer);
            newGoal.name = "Goal_" + i.ToString();
            newGoal.transform.localPosition = _goalPos;

            Object goalObject = newGoal.AddComponent<Object>();

            goalObject.data.pos = newGoal.transform.position;
            goalObject.data.rot = newGoal.transform.rotation;
            goalObject.data.type = Object.Type.Goal;
            newGoal.layer = 9;
            _goals.Add(newGoal.transform);

            if (_goalData[i]["scenario_index"].ToObject<int>() != alternativeIndex)
                _goalsToRemove.Add(newGoal.transform);
        }

        // Obstacles
        var _obstacleData = data["obstacles"] as JArray;
        for (int i = 0; i < _obstacleData.Count; i++)
        {
            if (_obstacleData[i]["from_obj"].ToObject<bool>())
                continue;

            GameObject newObstacle = Instantiate(prefabManager.GetObstaclePrefab(), prefabManager.obstacleContainer);
            newObstacle.name = "Obstacle_" + i.ToString();
            newObstacle.transform.FromJObject(_obstacleData[i]["transform"]);

            Object goalObject = newObstacle.AddComponent<Object>();

            goalObject.data.pos = newObstacle.transform.position;
            goalObject.data.rot = newObstacle.transform.rotation;
            goalObject.data.type = Object.Type.Goal;
            newObstacle.layer = 9;
        }

        // SpawnArea
        var _spawnAreaData = data["spawnAreas"] as JArray;
        for (int i = 0; i < _spawnAreaData.Count; i++)
        {
            GameObject newSpawnArea = Instantiate(prefabManager.GetSpawnAreaPrefab(), prefabManager.spawnAreaContainer);
            newSpawnArea.name = "SpawnArea_" + i.ToString();
            newSpawnArea.transform.position = Vector3Extensions.FromJObject(_spawnAreaData[i]["transform"]["position"]);

            SpawnArea _sp = newSpawnArea.GetComponent<SpawnArea>();
            _sp.initialNumberOfAgents = _spawnAreaData[i]["agent_count"].ToObject<int>();
            _sp.initialRemoveWhenGoalReached = _spawnAreaData[i]["remove_at_goal"].ToObject<bool>();
            var _goalIndex = _spawnAreaData[i]["goal_list"].ToObject<List<int>>();
            _sp.initialAgentsGoalList = new List<GameObject>();
            
            for (int j = 0; j < _goalIndex.Count; j++)
                _sp.initialAgentsGoalList.Add(_goals[_goalIndex[j]].gameObject);
            _sp.initialWaitList = _spawnAreaData[i]["wait_list"].ToObject<List<float>>();

            
            _sp.cycleLenght = _spawnAreaData[i]["cycle_lenght"].ToObject<float>();
            _sp.quantitySpawnedEachCycle = _spawnAreaData[i]["cycle_agent_count"].ToObject<int>();
            _sp.repeatingRemoveWhenGoalReached = _spawnAreaData[i]["cycle_remove_at_goal"].ToObject<bool>();
            _goalIndex = _spawnAreaData[i]["goal_list"].ToObject<List<int>>();
            _sp.repeatingGoalList = new List<GameObject>();
            for (int j = 0; j < _goalIndex.Count; j++)
                _sp.repeatingGoalList.Add(_goals[_goalIndex[j]].gameObject);

            _sp.repeatingWaitList = _spawnAreaData[i]["cycle_wait_list"].ToObject<List<float>>();

            Object spawnerObject = newSpawnArea.AddComponent<Object>();

            spawnerObject.data.pos = newSpawnArea.transform.position;
            spawnerObject.data.rot = newSpawnArea.transform.rotation;
            spawnerObject.data.type = Object.Type.Spawner;
            newSpawnArea.layer = 9;
            //world.spawnAreas.Add(_sp);
        }
        // LoadedModels
        var _loadedModelsData = data["loaded_models"] as JArray;
        for (int i = 0; i < _loadedModelsData.Count; i++)
        {
            LoadedOBJData newSpawnAreaData = objImporter.LoadOBJFromString(_loadedModelsData[i]["data"].ToObject<string>());
            newSpawnAreaData.name = "LoadedModel_" + i.ToString();
            newSpawnAreaData.transform.FromJObject(_loadedModelsData[i]["transform"]);
        }

        foreach (Transform t in _goalsToRemove)
            Destroy(t.gameObject);
        scenario.world.CreateNavMesh();
    }

    private void LoadContent(string text)
    {
        editorUIController.RemoveAllAlternatives();

        JObject content = JObject.Parse(text);
        JArray scenarios = (JArray)content["scenarios"];

        Debug.Log("Loading scenarios. Quantity:" + scenarios.Count);
        objImporter.ClearAllLoadedModels();
        for (int i = 0; i < scenarios.Count; i++)
            LoadSimulationScenario((JObject)scenarios[i], i);
    }

    
}
