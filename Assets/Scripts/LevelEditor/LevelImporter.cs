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
        JObject content = JObject.Parse(text);
        List<Transform> _goals = new List<Transform>();
        // Terrain - Only 1 terrain for now
        world.CreateCells();
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
            Transform newGoal = Instantiate(world.goalPrefab, _goalPos, Quaternion.identity, world.goalContainer);
            newGoal.name = "Goal_" + i.ToString();
            _goals.Add(newGoal);
        }

        // Obstacles
        var _obstacleData = content["obstacles"] as JArray;
        for (int i = 0; i < _goalData.Count; i++)
        {
            Transform newObstacle = Instantiate(world.obstaclePrefab, world.obstacleContainer);
            newObstacle.name = "Obstacle_" + i.ToString();
            newObstacle.FromJObject(_obstacleData[i]["transform"]);
        }

        // SpawnArea
        var _spawnAreaData = content["spawnAreas"] as JArray;
        for (int i = 0; i < _spawnAreaData.Count; i++)
        {
            Transform newSpawnArea = Instantiate(world.spawnAreaPrefab, world.spawnAreaContainer);
            newSpawnArea.name = "SpawnArea_" + i.ToString();
            newSpawnArea.FromJObject(_spawnAreaData[i]["transform"]);
            world.spawnAreas.Add(newSpawnArea.GetComponent<SpawnArea>());
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
        var _agentsData = content["agents"] as JArray;
        for (int i = 0; i < _agentsData.Count; i++)
        {
            Transform newAgentT = Instantiate(world.GetRandomAgentPrefab(), world.agentsContainer);
            //newAgentT.name = "Agent [" + world.GetNewAgentID() + "]";
            newAgentT.position = Vector3Extensions.FromJObject((_agentsData[i]["position"]));
            Agent newAgent = newAgentT.GetComponent<Agent>();
            newAgent.goalsList = new List<GameObject>();
            var _goalList = _agentsData[i]["goal_list"] as JArray;
            for (int j = 0; j < _goalList.Count; j++)
            {
                newAgent.goalsList.Add(_goals[_goalList[j].ToObject<int>()].gameObject);
                newAgent.goalsWaitList.Add(0f);
            }
            world.PrepareAgent(newAgent);
            newAgent.Goal = newAgent.goalsList[0];
            newAgent.removeWhenGoalReached = _agentsData[i]["remove_goal_reach"].ToObject<bool>();

            //newAgent.World = world;
            //world.agents.Add(newAgent);
        }
        // Auxins
        var _auxinsData = content["auxins"] as JArray;
        for (int i = 0; i < _auxinsData.Count; i++)
        {
            Auxin newAuxin = Instantiate(world.auxinPrefab, world.auxinsContainer);
            newAuxin.name = (_auxinsData[i]["name"].ToObject<string>());
            newAuxin.transform.position = Vector3Extensions.FromJObject((_auxinsData[i]["position"]));
            string index = newAuxin.name.Split('[')[1].Split(']')[0];
            newAuxin.Cell = world.Cells[int.Parse(index)];
            newAuxin.Cell.Auxins.Add(newAuxin);
            newAuxin.Position = newAuxin.transform.position;
            //world.Cells[int.Parse(index)].Auxins.Add(newAuxin);


        }

        world.CreateNavMesh();
        world._isReady = true;

        /*
        output.Add("goals", _goalsArray);
        output.Add("obstacles", _obstaclesArray);
        output.Add("spawnAreas", _spawnAreasArray);
        output.Add("loaded_models", _loadedModelsArray);
        output.Add("agents", _agentsArray);
        output.Add("auxins", _auxinsArray);
         */

    }
}
