using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Biocrowds.Core;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

public class LevelExporter : MonoBehaviour
{

    public void ExportLevel(RuntimeOBJImporter objImporter)
    {
        JObject output = new JObject();
        JArray _terrainsArray = new JArray();
        JArray _agentsArray = new JArray();
        JArray _auxinsArray = new JArray();
        JArray _goalsArray = new JArray();
        JArray _obstaclesArray = new JArray();
        JArray _spawnAreasArray = new JArray();
        Debug.Log("Exporing Level");

        var _terrains = FindObjectsOfType<Terrain>().ToList();
        var _agents = FindObjectsOfType<Agent>().ToList();
        var _auxins = FindObjectsOfType<Auxin>().ToList();
        var _spawnAreas = FindObjectsOfType<SpawnArea>().ToList(); 
        var _goals = GameObject.FindGameObjectsWithTag("Goal").ToList();
        var _obstacles = GameObject.FindGameObjectsWithTag("Obstacle").ToList();

        Debug.Log(_agents.Count + " " + _auxins.Count);

        for (int i = 0; i < _terrains.Count; i++) // Terrains
        {
            JObject _a = new JObject();
            _a.Add("size", JArray.FromObject(_terrains[i].terrainData.size.AsList()));
            _terrainsArray.Add(_a);
        }
        for (int i = 0; i < _agents.Count; i ++) // Agents
        {
            JObject _a = new JObject();
            _a.Add("position", JArray.FromObject(_agents[i].transform.position.AsList()));
            _a.Add("goal", JArray.FromObject(_agents[i].GetFinalGoalPositionAsList()));
            _agentsArray.Add(_a);
        }
        for (int i = 0; i < _auxins.Count; i++) // Auxins/Markers
        {
            JObject _a = new JObject();
            _a.Add("position", JArray.FromObject(_auxins[i].transform.position.AsList()));
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
            _o.Add("position", JArray.FromObject(_spawnAreas[i].transform.position.AsList()));
            _o.Add("rotation", JArray.FromObject(_spawnAreas[i].transform.rotation.eulerAngles.AsList()));
            _o.Add("scale", JArray.FromObject(_spawnAreas[i].transform.localScale.AsList()));
            _obstaclesArray.Add(_o);
        }
        for (int i = 0; i < _spawnAreas.Count; i++) // Spawn Areas
        {
            JObject _sp = new JObject();
            _sp.Add("position", JArray.FromObject(_spawnAreas[i].transform.position.AsList()));
            _sp.Add("rotation", JArray.FromObject(_spawnAreas[i].transform.rotation.eulerAngles.AsList()));
            _sp.Add("scale", JArray.FromObject(_spawnAreas[i].transform.localScale.AsList()));

            _spawnAreasArray.Add(_sp);
        }

        output.Add("terrains", _terrainsArray);
        output.Add("agents", _agentsArray);
        output.Add("goals", _goalsArray);
        output.Add("obstacles", _obstaclesArray);
        output.Add("spawnAreas", _spawnAreasArray);
        output.Add("auxins", _auxinsArray);
        var fileFinalName = "test";
        StreamWriter writer = new StreamWriter("Assets/Resources/SavedLevels/" + fileFinalName + ".json", false);
        writer.Write(JsonConvert.SerializeObject(output, Formatting.Indented));
        writer.Close();
    }
}
