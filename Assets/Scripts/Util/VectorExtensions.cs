using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class Vector3Extensions
{
    public static List<float> AsList(this Vector3 pos)
    {
        return new List<float> { pos.x, pos.y, pos.z };
    }

    public static Vector3 FromJObject(this Vector3 pos, JToken data)
    {
        Debug.Log("data token" + data);
        pos.FromJObject(data.ToObject<JObject>());
        return pos;
    }
    public static Vector3 FromJObject(this Vector3 pos, JArray data)
    {
        Debug.Log("data array" + data);
        pos.x = data[0].ToObject<float>();
        pos.y = data[1].ToObject<float>();
        pos.z = data[2].ToObject<float>();
        return pos;
    }

    public static Vector3 FromJObject(this Vector3 pos, JObject data)
    {
        Debug.Log("data obj" + data);
        pos.x = data[0].ToObject<float>();
        pos.y = data[1].ToObject<float>();
        pos.z = data[2].ToObject<float>();
        return pos;
    }
    public static Vector3 FromJObject(JToken data)
    {
        return FromJObject(data as JArray);
    }

    public static Vector3 FromJObject(JObject data)
    {
        var pos = new Vector3();
        pos.x = data[0].ToObject<float>();
        pos.y = data[1].ToObject<float>();
        pos.z = data[2].ToObject<float>();
        return pos;
    }

    public static Vector3 FromJObject(JArray data)
    {
        var pos = new Vector3();
        pos.x = data[0].ToObject<float>();
        pos.y = data[1].ToObject<float>();
        pos.z = data[2].ToObject<float>();
        return pos;
    }
}