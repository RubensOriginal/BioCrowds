using System.Collections.Generic;
using UnityEngine;
using Biocrowds.Core;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using SFB;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;
using UnityEngine.AI;
using Unity.AI.Navigation;
using System.Threading;
using Assets.Scripts.LevelEditor;

public class LevelExporter : MonoBehaviour
{
    private DateTime timeStamp;
    public SceneController sceneController;
    public enum ExportType { Download, RunScene };

    [DllImport("__Internal")]
    private static extern void RunScene(string message);

    public Material invalidMaterial;

    private void Awake()
    {
        timeStamp = DateTime.UtcNow;
    }

    public void ExportLevel(World world, RuntimeOBJImporter objImporter, ExportType exportType, List<SimulationScenario> testLevels)
    {

        if (exportType == ExportType.Download)
        {
            // string content = GenerateFileContent(world, objImporter, testLevels[0], 0, true);
            var output = GenerateArrayJsonContent(world, testLevels);
            string content = JsonConvert.SerializeObject(output, Formatting.Indented);
#if UNITY_WEBGL && !UNITY_EDITOR
                var bytes = Encoding.UTF8.GetBytes(content);
                DownloadFile(gameObject.name, "OnFileDownload", "SavedScene.json", bytes, bytes.Length);
#else
            StartCoroutine(DesktopExportCoroutine(content));
        #endif
        }
        else if (exportType == ExportType.RunScene)
        {
            // string content = GenerateFileContent(world, objImporter);
            var output = GenerateArrayJsonContent(world, testLevels);
            string content = JsonConvert.SerializeObject(output, Formatting.Indented);

#if UNITY_WEBGL && !UNITY_EDITOR
            RunScene(content);
#else
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
            sceneController.LoadSimulationWorld();
        #endif
        }
    }


#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void DownloadFile(string gameObjectName, string methodName, string filename, byte[] byteArray, int byteArraySize);

    // Called from browser
    public void OnFileDownload() {
        Debug.Log("File Successfully Downloaded");
    }
#else
    IEnumerator DesktopExportCoroutine(string content)
    {
        yield return new WaitForEndOfFrame(); // To re-enable button color
        var path = StandaloneFileBrowser.SaveFilePanel("Title", "", "SavedScene", "json");
        if (!string.IsNullOrEmpty(path))
        {
            File.WriteAllText(path, content);
        }
    }
#endif

    public bool IsValidExport(World world, List<SimulationScenario> simulationScenarios)
    {
        bool valid = true;
        foreach (SimulationScenario tl in simulationScenarios)
        {
            var world_list = tl.GetComponentsInChildren<World>().ToList();
            foreach (World w in world_list)
                w.CreateNavMesh();
        }

        var _spawnAreas = FindObjectsOfType<SpawnArea>().ToList();
        var _goals = GameObject.FindGameObjectsWithTag("Goal").ToList();
        var validMaterialSA = world.prefabManager.GetSpawnAreaPrefab().GetComponent<Renderer>().sharedMaterial;
        var validMaterialGoal = world.prefabManager.GetGoalPrefab().GetComponent<Renderer>().sharedMaterial;

        foreach (SpawnArea sp in _spawnAreas)
        {
            sp.GetMeshRenderer().material = validMaterialSA;
            if (sp.initialAgentsGoalList.Count == 0)
            {
                sp.GetMeshRenderer().material = invalidMaterial;
                valid = false;
            }
            if (sp.initialAgentsGoalList.Count == 1 && sp.initialAgentsGoalList[0] == null)
            {
                sp.GetMeshRenderer().material = invalidMaterial;
                sp.initialAgentsGoalList.Clear();
                valid = false;
            }

            Debug.Log(sp.GetRandomPointInNavmesh().ToString());

            if (sp.GetRandomPointInNavmesh().ToString() == Vector3.negativeInfinity.ToString())
            {
                sp.GetMeshRenderer().material = invalidMaterial;
                valid = false;
            }
        }
        foreach (GameObject go in _goals)
        {
            go.GetComponent<MeshRenderer>().material = validMaterialGoal;
            var point = new Vector3(go.transform.position.x, 0f, go.transform.position.z);
            if (!NavMesh.SamplePosition(point, out NavMeshHit hit, 0.1f, 1 << NavMesh.GetAreaFromName("Walkable")))
            {
                go.GetComponent<MeshRenderer>().material = invalidMaterial;
                valid = false;
            }
        }

        return valid;
    }

    public bool IsValidCassol(World world, List<SimulationScenario> simulationScenarios)
    {
        // var _spawnAreas = FindObjectsOfType<SpawnArea>().ToList();
        
        try
        {
            SceneDataChecker dataChecker = new SceneDataChecker();
            var _gameObjects = new List<GameObject>();
            foreach (SimulationScenario ss in simulationScenarios)
                _gameObjects.Add(ss.gameObject);

            dataChecker.CheckNumberAgentsInTestLevels(_gameObjects)
                .CheckNumberOfGoals(_gameObjects);

            return true;
        }
        catch (Exception e)
        {
            return false;
        }

    }


    private JObject GenerateArrayJsonContent(World world, List<SimulationScenario> testLevels)
    {
        var scenarios = new JArray();
        var cassolValid = IsValidCassol(world, testLevels);

        for (int i = 0; i < testLevels.Count; i++)
        {
            scenarios.Add(SimulationScenarioToJSON(testLevels, i, cassolValid));
        }

        var output = new JObject();
        output.Add("scenarios", scenarios);
        return output;
    }

    private JObject SimulationScenarioToJSON(List<SimulationScenario> scenarios, int scenarioIndex, bool isCassolValid)
    {
        JObject output = new JObject();
        JArray _terrainsArray = new JArray();
        JArray _agentsArray = new JArray();
        JArray _auxinsArray = new JArray();
        JArray _goalsArray = new JArray();
        JArray _pathGoalsArray = new JArray();
        JArray _obstaclesArray = new JArray();
        JArray _spawnAreasArray = new JArray();
        JArray _loadedModelsArray = new JArray();
        Debug.Log("Exporing Scenario " + scenarioIndex);
        UnityEngine.Random.InitState(0);
        var scenario = scenarios[scenarioIndex];

        var _terrains = FindObjectsOfType<Terrain>().ToList();
        var _spawnAreas = scenario.spawnAreasContainer.GetComponentsInChildren<SpawnArea>().ToList();
        var _loadedModels = scenario.importedModelsContainer.GetComponentsInChildren<LoadedOBJData>().ToList();
        var _goals =  GameObject.FindGameObjectsWithTag("Goal").ToList();
        var _obstacles = scenario.obstaclesContainer.FindChildrenWithTag("Obstacle").ToList();
        var _objCollider = new List<GameObject>();
        foreach (Transform obj in scenario.importedModelsContainer.transform)
            _objCollider.AddRange(obj.gameObject.FindChildrenWithTag("OBJCollider").ToList());

        for (int i = 0; i < _terrains.Count; i++) // Terrains (all terrains, including other scenarios)
        {
            JObject _t = new JObject();
            _t.Add("transform", _terrains[i].transform.AsJObject());
            _t.Add("terrain_size", JArray.FromObject(_terrains[i].terrainData.size.AsList()));
            _terrainsArray.Add(_t);
        }
        for (int i = 0; i < _goals.Count; i++) // Goals (all goals, including other scenarios)
        {
            JObject _a = new JObject();
            

            int _goalScenarioIndex = 0;
            for (int j = 0; j < scenarios.Count; j++)
            {
                if (_goals[i].transform.parent == scenarios[j].goalsContainer.transform)
                    _goalScenarioIndex = j;
            }
            _a.Add("position", JArray.FromObject((_goals[i].transform.position - scenario.transform.position).AsList()));
            _a.Add("scenario_index", _goalScenarioIndex);
            _goalsArray.Add(_a);
        }
        for (int i = 0; i < _spawnAreas.Count; i++) // Agents (sampling points)
        {
            for (int j = 0; j < _spawnAreas[i].initialNumberOfAgents; j++)
            {
                
                if (_agentsArray.Count == 100)
                continue;

                JObject _a = new JObject();

                List<int> _goalIndexList = new List<int>();
                for (int l = 0; l < _spawnAreas[i].initialAgentsGoalList.Count; l++)
                    _goalIndexList.Add(_goals.IndexOf(_spawnAreas[i].initialAgentsGoalList[l]));

                NavMeshPath _navMeshPath = new NavMeshPath();

                for (int k = 0; k < 10; k++) // 10 tries to find path
                {
                    var _pos = _spawnAreas[i].GetRandomPointInNavmesh();

                    bool foundPath = NavMesh.CalculatePath(_pos, _goals[_goalIndexList.Last()].transform.position,
                        NavMesh.AllAreas, _navMeshPath);

                    if (foundPath)
                    {
                        Debug.Log("I found a path");
                        _a.Add("position", JArray.FromObject((_pos - scenario.transform.position).AsList()));
                        _a.Add("goal_list", JToken.FromObject(_goalIndexList));
                        _a.Add("remove_goal_reach", JToken.FromObject(_spawnAreas[i].initialRemoveWhenGoalReached));
                        JArray cornerList = new JArray();
                        foreach (var _c in _navMeshPath.corners)
                        {
                            cornerList.Add(JToken.FromObject((_c - scenario.transform.position).AsList()));
                        }
                        _a.Add("path_planning_goals", cornerList);
                        _agentsArray.Add(_a);
                        break;
                    }
                    else if (k == 9)
                    {
                        Debug.Log("9 ... 9 ... 9 ... 9 ... 9 hours ");
                        _a.Add("position", JArray.FromObject((_pos - scenario.transform.position).AsList()));
                        _a.Add("goal_list", JToken.FromObject(_goalIndexList));
                        _a.Add("remove_goal_reach", JToken.FromObject(_spawnAreas[i].initialRemoveWhenGoalReached));

                        JArray cornerList = new JArray();
                        foreach (var _c in _navMeshPath.corners)
                        {
                            cornerList.Add(JToken.FromObject((_c - scenario.transform.position).AsList()));
                        }
                        _a.Add("path_planning_goals", cornerList);
                        _agentsArray.Add(_a);
                        Debug.Log("Error!");
                        break;
                    }
                }
            }
            
        }
        /*for (int i = 0; i < _agents.Count; i++) // Agents (loaded agents)
        {
            JObject _a = new JObject();
            _a.Add("position", JArray.FromObject(_agents[i].transform.position.AsList()));
            List<int> _goalIndexList = new List<int>();
            for (int j = 0; j < _agents[i].goalsList.Count; j++)
                _goalIndexList.Add(_goals.IndexOf(_agents[i].goalsList[j]));
            _a.Add("goal_list", JToken.FromObject(_goalIndexList));
            _a.Add("remove_goal_reach", JToken.FromObject(_agents[i].removeWhenGoalReached));
            _agentsArray.Add(_a);
        }*/
        /*for (int i = 0; i < world.Auxins.Count; i++) // Auxins/Markers
        {
            JObject _a = new JObject();
            _a.Add("position", JArray.FromObject(world.Auxins[i].transform.position.AsList()));
            _a.Add("name", world.Auxins[i].name);
            _auxinsArray.Add(_a);
        }*/
        
        for (int i = 0; i < _obstacles.Count; i++) // Obstacles
        {
            JObject _o = new JObject();
            BoxCollider col = _obstacles[i].GetComponent<BoxCollider>();
            var trans = _obstacles[i].transform;
            var min = col.center - col.size * 0.5f;
            var max = col.center + col.size * 0.5f;
            var P000 = trans.TransformPoint(new Vector3(min.x, min.y, min.z));
            var P001 = trans.TransformPoint(new Vector3(min.x, min.y, max.z));
            var P101 = trans.TransformPoint(new Vector3(max.x, min.y, max.z));
            var P100 = trans.TransformPoint(new Vector3(max.x, min.y, min.z));
            JArray pointList = new JArray();
            pointList.Add(JToken.FromObject((new Vector3(P000.x, P000.z, 0.0f) - scenario.transform.position).AsList()));
            pointList.Add(JToken.FromObject((new Vector3(P001.x, P001.z, 0.0f) - scenario.transform.position).AsList()));
            pointList.Add(JToken.FromObject((new Vector3(P101.x, P101.z, 0.0f) - scenario.transform.position).AsList()));
            pointList.Add(JToken.FromObject((new Vector3(P100.x, P100.z, 0.0f) - scenario.transform.position).AsList()));
            _o.Add("transform", _obstacles[i].transform.AsJObject());
            _o.Add("point_list", pointList);
            _o.Add("from_obj", JToken.FromObject(false));
            _obstaclesArray.Add(_o);
        }

        for (int i = 0; i < _objCollider.Count; i++) // OBJ Colliders
        {
            JObject _o = new JObject();
            BoxCollider col = _objCollider[i].GetComponent<BoxCollider>();
            var trans = _objCollider[i].transform;
            var min = col.center - col.size * 0.5f;
            var max = col.center + col.size * 0.5f;
            var P000 = trans.TransformPoint(new Vector3(min.x, min.y, min.z));
            var P001 = trans.TransformPoint(new Vector3(min.x, min.y, max.z));
            var P101 = trans.TransformPoint(new Vector3(max.x, min.y, max.z));
            var P100 = trans.TransformPoint(new Vector3(max.x, min.y, min.z));
            JArray pointList = new JArray();
            pointList.Add(JToken.FromObject((new Vector3(P100.x, P100.z, 0.0f) - scenario.transform.position).AsList()));
            pointList.Add(JToken.FromObject((new Vector3(P101.x, P101.z, 0.0f) - scenario.transform.position).AsList()));
            pointList.Add(JToken.FromObject((new Vector3(P001.x, P001.z, 0.0f) - scenario.transform.position).AsList()));
            pointList.Add(JToken.FromObject((new Vector3(P000.x, P000.z, 0.0f) - scenario.transform.position).AsList()));
            _o.Add("transform", _objCollider[i].transform.AsJObject());
            _o.Add("point_list", pointList);
            _o.Add("from_obj", JToken.FromObject(true));
            _obstaclesArray.Add(_o);
        }
        for (int i = 0; i < _loadedModels.Count; i++) // Loaded Models
        {
            JObject _lm = new JObject();
            _lm.Add("transform", _loadedModels[i].transform.AsJObject());
            _lm.Add("data", _loadedModels[i].objData);
            _loadedModelsArray.Add(_lm);
        }
        for (int i = 0; i < _spawnAreas.Count; i++) // Spawn Areas
        {
            JObject _sp = new JObject();
            _sp.Add("transform", _spawnAreas[i].transform.AsJObject());
            _sp.Add("agent_count", _spawnAreas[i].initialNumberOfAgents);
            _sp.Add("remove_at_goal", _spawnAreas[i].initialRemoveWhenGoalReached);
            List<int> _goalIndexList = new List<int>();
            for (int j = 0; j < _spawnAreas[i].initialAgentsGoalList.Count; j++)
                _goalIndexList.Add(_goals.IndexOf(_spawnAreas[i].initialAgentsGoalList[j]));
            _sp.Add("goal_list", JToken.FromObject(_goalIndexList));
            _sp.Add("wait_list", JToken.FromObject(_spawnAreas[i].initialWaitList));

            _sp.Add("cycle_lenght", _spawnAreas[i].cycleLenght);
            _sp.Add("cycle_agent_count", _spawnAreas[i].quantitySpawnedEachCycle);
            _sp.Add("cycle_remove_at_goal", _spawnAreas[i].repeatingRemoveWhenGoalReached);
            _goalIndexList = new List<int>();
            for (int j = 0; j < _spawnAreas[i].repeatingGoalList.Count; j++)
                _goalIndexList.Add(_goals.IndexOf(_spawnAreas[i].repeatingGoalList[j]));
            _sp.Add("cycle_goal_list", JToken.FromObject(_goalIndexList));
            _sp.Add("cycle_wait_list", JToken.FromObject(_spawnAreas[i].repeatingWaitList));

            _spawnAreasArray.Add(_sp);
        }

        output.Add("time_stamp", timeStamp.ToLongTimeString() + ":" + (timeStamp.Millisecond + scenarioIndex).ToString());
        output.Add("terrains", _terrainsArray);
        output.Add("goals", _goalsArray);
        output.Add("obstacles", _obstaclesArray);
        output.Add("spawnAreas", _spawnAreasArray);
        output.Add("loaded_models", _loadedModelsArray);
        output.Add("agents", _agentsArray);
        output.Add("auxins", _auxinsArray);
        output.Add("isCassolValid", isCassolValid);

        scenario.world.ClearWorld(false);
        return output;
    }
}
