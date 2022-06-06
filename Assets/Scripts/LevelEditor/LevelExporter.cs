using System.Collections.Generic;
using UnityEngine;
using Biocrowds.Core;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using SFB;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;


public class LevelExporter : MonoBehaviour
{
    public enum ExportType { Download, RunScene };

    [DllImport("__Internal")]
    private static extern void RunScene(string message);

    public Material invalidMaterial;
    public void ExportLevel(World world, RuntimeOBJImporter objImporter, ExportType exportType)
    {
        string content = GenerateFileContent(world, objImporter);

        if (exportType == ExportType.Download)
        {

        #if UNITY_WEBGL && !UNITY_EDITOR
                var bytes = Encoding.UTF8.GetBytes(content);
                DownloadFile(gameObject.name, "OnFileDownload", "SavedScene.json", bytes, bytes.Length);
        #else
                    StartCoroutine(DesktopExportCoroutine(content));
        #endif
        }
        else if (exportType == ExportType.RunScene)
        {
            RunScene(content);
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

    public bool IsValidExport(World world)
    {
        var _spawnAreas = FindObjectsOfType<SpawnArea>().ToList();
        bool valid = true;
        foreach (SpawnArea sp in _spawnAreas)
        {
            if (sp.initialAgentsGoalList.Count == 0)
            {
                sp.GetMeshRenderer().material = invalidMaterial;
                valid = false;
            }
        }

        return valid;
    }

    private string GenerateFileContent(World world, RuntimeOBJImporter objImporter)
    {
        JObject output = new JObject();
        JArray _terrainsArray = new JArray();
        JArray _agentsArray = new JArray();
        JArray _auxinsArray = new JArray();
        JArray _goalsArray = new JArray();
        JArray _obstaclesArray = new JArray();
        JArray _spawnAreasArray = new JArray();
        JArray _loadedModelsArray = new JArray();
        Debug.Log("Exporing Level");

        //objImporter
        world.CreateNavMesh();
        world.CreateCells();
        world.SetMarkerSpawner();
        StartCoroutine(world._markerSpawner.CreateMarkers(world.prefabManager.GetAuxinPrefab(), world.prefabManager.auxinsContainer, world.Cells, world.Auxins));
        //world.markerSpawnMethod.

        var _terrains = FindObjectsOfType<Terrain>().ToList();
        var _agents = FindObjectsOfType<Agent>().ToList();
        var _spawnAreas = FindObjectsOfType<SpawnArea>().ToList();
        var _loadedModels = FindObjectsOfType<LoadedOBJData>().ToList();
        var _goals = GameObject.FindGameObjectsWithTag("Goal").ToList();
        var _obstacles = GameObject.FindGameObjectsWithTag("Obstacle").ToList();

        Debug.Log(_agents.Count + " " + world.Auxins.Count);

        for (int i = 0; i < _terrains.Count; i++) // Terrains
        {
            JObject _t = new JObject();
            _t.Add("transform", _terrains[i].transform.AsJObject());
            _t.Add("terrain_size", JArray.FromObject(_terrains[i].terrainData.size.AsList()));
            _terrainsArray.Add(_t);
        }
        
        for (int i = 0; i < _spawnAreas.Count; i++) // Agents (sampling points)
        {
            for(int j = 0; j < _spawnAreas[i].initialNumberOfAgents; j++)
            {
                JObject _a = new JObject();
                _a.Add("position", JArray.FromObject(_spawnAreas[i].GetRandomPoint().AsList()));
                List<int> _goalIndexList = new List<int>();
                for (int k = 0; k < _spawnAreas[i].initialAgentsGoalList.Count; k++)
                    _goalIndexList.Add(_goals.IndexOf(_spawnAreas[i].initialAgentsGoalList[k]));
                _a.Add("goal_list", JToken.FromObject(_goalIndexList));
                _a.Add("remove_goal_reach", JToken.FromObject(_spawnAreas[i].initialRemoveWhenGoalReached));
                _agentsArray.Add(_a);
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
        for (int i = 0; i < _goals.Count; i++) // Goals
        {
            JObject _a = new JObject();
            _a.Add("position", JArray.FromObject(_goals[i].transform.position.AsList()));
            _goalsArray.Add(_a);
        }
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
            pointList.Add(JToken.FromObject(new Vector3(P000.x, P000.z, 0.0f).AsList()));
            pointList.Add(JToken.FromObject(new Vector3(P001.x, P001.z, 0.0f).AsList()));
            pointList.Add(JToken.FromObject(new Vector3(P101.x, P101.z, 0.0f).AsList()));
            pointList.Add(JToken.FromObject(new Vector3(P100.x, P100.z, 0.0f).AsList()));
            _o.Add("transform", _obstacles[i].transform.AsJObject());
            _o.Add("point_list", pointList);
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

        output.Add("terrains", _terrainsArray);
        output.Add("goals", _goalsArray);
        output.Add("obstacles", _obstaclesArray);
        output.Add("spawnAreas", _spawnAreasArray);
        output.Add("loaded_models", _loadedModelsArray);
        output.Add("agents", _agentsArray);
        output.Add("auxins", _auxinsArray);

        world.ClearWorld(false);

        return JsonConvert.SerializeObject(output, Formatting.Indented);
    }
}
