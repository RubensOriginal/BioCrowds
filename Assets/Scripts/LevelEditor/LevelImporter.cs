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

public class LevelImporter : MonoBehaviour
{
    [SerializeField]
    private World world;
    private SimulationPrefabManager prefabManager;
    [SerializeField]
    private RuntimeOBJImporter objImporter;

    public ExtensionFilter[] extensions;


    private void Start()
    {
        extensions = new[] {
            new ExtensionFilter("JSON Files", "json"),
        };
    }

    public void ImportLevel(World _world, RuntimeOBJImporter _objLoader)
    {
        world = _world;
        prefabManager = _world.prefabManager;
        objImporter = _objLoader;
#if UNITY_WEBGL && !UNITY_EDITOR
        UploadFile(gameObject.name, "OnFileUpload", ".json", false);
#else
        StartCoroutine(DesktopImportCoroutine());
#endif
    }

    public void ImportTestLevel(World _world, RuntimeOBJImporter _objLoader)
    {
        world = _world;
        prefabManager = _world.prefabManager;
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
            for (int i = 0; i < paths.Length; i++)
                Debug.Log("Now Importing: " + " " + paths[0]);

            using var fs = new FileStream(paths[0], FileMode.Open);
            string content = new StreamReader(fs).ReadToEnd();
            Debug.Log(content);
            LoadContent(content);
        }
        yield return null;

    }
#endif


    private void LoadContent(string text)
    {
        world.ClearWorld();
        objImporter.ClearLoadedModels();

        JObject content = JObject.Parse(text);
        List<Transform> _goals = new List<Transform>();
        // Terrain - Only 1 terrain for now
        //world.CreateCells();
        var _terrain = world.GetTerrain();
        var _t = content["terrains"][0];
        _terrain.transform.FromJObject(_t["transform"]);
        _terrain.terrainData.size = new Vector3(_t["terrain_size"][0].ToObject<float>(),
                                                _t["terrain_size"][1].ToObject<float>(),
                                                _t["terrain_size"][2].ToObject<float>());

        // Goals
        var _goalData = content["goals"] as JArray;
        for (int i = 0; i < _goalData.Count; i++)
        {
            Vector3 _goalPos = Vector3Extensions.FromJObject((_goalData[i]["position"]));
            GameObject newGoal = Instantiate(prefabManager.GetGoalPrefab(), _goalPos, Quaternion.identity, prefabManager.goalContainer);
            newGoal.name = "Goal_" + i.ToString();

            Object goalObject = newGoal.AddComponent<Object>();

            goalObject.data.pos = newGoal.transform.position;
            goalObject.data.rot = newGoal.transform.rotation;
            goalObject.data.type = Object.Type.Goal;
            newGoal.layer = 9;
            _goals.Add(newGoal.transform);
        }

        // Obstacles
        /*var _obstacleData = content["obstacles"] as JArray;
        for (int i = 0; i < _goalData.Count; i++)
        {
            GameObject newObstacle = Instantiate(prefabManager.GetObstaclePrefab(), prefabManager.obstacleContainer);
            newObstacle.name = "Obstacle_" + i.ToString();
            newObstacle.transform.FromJObject(_obstacleData[i]["transform"]);
        }*/

        // SpawnArea
        var _spawnAreaData = content["spawnAreas"] as JArray;
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
            world.spawnAreas.Add(_sp);
        }

        // LoadedModels
        var _loadedModelsData = content["loaded_models"] as JArray;
        for (int i = 0; i < _loadedModelsData.Count; i++)
        {
            LoadedOBJData newSpawnAreaData = objImporter.LoadOBJFromString(_loadedModelsData[i]["data"].ToObject<string>());
            newSpawnAreaData.name = "LoadedModel_" + i.ToString();
            newSpawnAreaData.transform.FromJObject(_loadedModelsData[i]["transform"]);
        }

        // Agents
        /*var _agentsData = content["agents"] as JArray;
        for (int i = 0; i < _agentsData.Count; i++)
        {
            List<GameObject> _goalListGO = new List<GameObject>();
            List<float> _waitList = new List<float>();
            var _goalList = _agentsData[i]["goal_list"] as JArray;
            for (int j = 0; j < _goalList.Count; j++)
            {
                _goalListGO.Add(_goals[_goalList[j].ToObject<int>()].gameObject);
                _waitList.Add(0f);
            }
            Agent newAgent = world.SpawnNewAgent(Vector3Extensions.FromJObject((_agentsData[i]["position"])),
                _agentsData[i]["remove_goal_reach"].ToObject<bool>(),
                _goalListGO);
            newAgent.goalsWaitList = _waitList;
        }*/

        // Auxins
        /*var _auxinsData = content["auxins"] as JArray;
        for (int i = 0; i < _auxinsData.Count; i++)
        {
            GameObject newAuxin = Instantiate(prefabManager.GetAuxinPrefab(), prefabManager.auxinsContainer);
            Auxin _auxin = newAuxin.GetComponent<Auxin>();

            newAuxin.name = (_auxinsData[i]["name"].ToObject<string>());
            newAuxin.transform.position = Vector3Extensions.FromJObject((_auxinsData[i]["position"]));
            string index = newAuxin.name.Split('[')[1].Split(']')[0];
            _auxin.Cell = world.Cells[int.Parse(index)];
            _auxin.Cell.Auxins.Add(_auxin);
            _auxin.Position = newAuxin.transform.position;
            //world.Cells[int.Parse(index)].Auxins.Add(newAuxin);


        }*/

        world.CreateNavMesh();
        //world._isReady = true;

    }
}
