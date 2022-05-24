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
    public void ExportLevel(RuntimeOBJImporter objImporter)
    {
        string content = GenerateFileContent(objImporter);

#if UNITY_WEBGL && !UNITY_EDITOR
        var bytes = Encoding.UTF8.GetBytes(content);
        DownloadFile(gameObject.name, "OnFileDownload", "SavedScene.json", bytes, bytes.Length);
#else
        StartCoroutine(DesktopExportCoroutine(content));
#endif
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


    private string GenerateFileContent(RuntimeOBJImporter objImporter)
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

        var _terrains = FindObjectsOfType<Terrain>().ToList();
        var _agents = FindObjectsOfType<Agent>().ToList();
        var _auxins = FindObjectsOfType<Auxin>().ToList();
        var _spawnAreas = FindObjectsOfType<SpawnArea>().ToList();
        var _loadedModels = FindObjectsOfType<LoadedOBJData>().ToList();
        var _goals = GameObject.FindGameObjectsWithTag("Goal").ToList();
        var _obstacles = GameObject.FindGameObjectsWithTag("Obstacle").ToList();

        Debug.Log(_agents.Count + " " + _auxins.Count);

        for (int i = 0; i < _terrains.Count; i++) // Terrains
        {
            JObject _t = new JObject();
            _t.Add("transform", _terrains[i].transform.AsJObject());
            _t.Add("terrain_size", JArray.FromObject(_terrains[i].terrainData.size.AsList()));
            _terrainsArray.Add(_t);
        }
        for (int i = 0; i < _agents.Count; i++) // Agents
        {
            JObject _a = new JObject();
            _a.Add("position", JArray.FromObject(_agents[i].transform.position.AsList()));
            List<int> _goalIndexList = new List<int>();
            for (int j = 0; j < _agents[i].goalsList.Count; j++)
                _goalIndexList.Add(_goals.IndexOf(_agents[i].goalsList[j]));
            _a.Add("goal_list", JToken.FromObject(_goalIndexList));
            _a.Add("remove_goal_reach", JToken.FromObject(_agents[i].removeWhenGoalReached));
            _agentsArray.Add(_a);
        }
        for (int i = 0; i < _auxins.Count; i++) // Auxins/Markers
        {
            JObject _a = new JObject();
            _a.Add("position", JArray.FromObject(_auxins[i].transform.position.AsList()));
            _a.Add("name", _auxins[i].name);
            _auxinsArray.Add(_a);
        }
        for (int i = 0; i < _goals.Count; i++) // Goals
        {
            JObject _a = new JObject();
            _a.Add("position", JArray.FromObject(_goals[i].transform.position.AsList()));
            _goalsArray.Add(_a);
        }
        for (int i = 0; i < _obstacles.Count; i++) // Obstacles
        {
            JObject _o = new JObject();
            _o.Add("transform", _obstacles[i].transform.AsJObject());
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

        return JsonConvert.SerializeObject(output, Formatting.Indented);
    }
}
