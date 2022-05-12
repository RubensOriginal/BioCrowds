using System.Collections.Generic;
using UnityEngine;
using Biocrowds.Core;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

public class LevelImporter : MonoBehaviour
{
    [SerializeField]
    private World world;
    [SerializeField]
    private RuntimeOBJImporter objImporter;

    public void ImportLevel(World _world, RuntimeOBJImporter _objLoader)
    {
        world = _world;
        objImporter = _objLoader;
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

    private void LoadContent(string text)
    {
        JObject content = JObject.Parse(text);

        // Terrain - Only 1 terrain for now
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
        }

        // LoadedModels
        var _loadedModelsData = content["loaded_models"] as JArray;
        for (int i = 0; i < _loadedModelsData.Count; i++)
        {
            LoadedOBJData newSpawnAreaData = objImporter.LoadOBJFromString(_loadedModelsData[i]["data"].ToObject<string>());
            newSpawnAreaData.name = "LoadedModel_" + i.ToString();
            newSpawnAreaData.transform.FromJObject(_loadedModelsData[i]["transform"]);
            //Transform newSpawnArea = Instantiate(world.spawnAreaPrefab, world.spawnAreaContainer);
            //newSpawnArea.name = "SpawnArea_" + i.ToString();
            //newSpawnArea.FromJObject(_loadedModelsData[i]["transform"]);
        }



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
