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

    public void ImportLevel(World _world)
    {
        world = _world;
    }

    public void ImportTestLevel(World _world)
    {
        world = _world;
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
        _terrain.terrainData.size = _t["terrain_size"].ToObject<Vector3>();

    }
}
