using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class TransformExtension
{
    public static JObject AsJObject(this Transform tr)
    {
        JObject jObj = new JObject();
        jObj.Add("position", JArray.FromObject(tr.position.AsList()));
        jObj.Add("rotation", JArray.FromObject(tr.rotation.eulerAngles.AsList()));
        jObj.Add("localScale", JArray.FromObject(tr.localScale.AsList()));
        return jObj;
    }
    public static Transform FromJObject(this Transform tr, JToken data)
    {
        return FromJObject(tr, data.ToObject<JObject>());
    }
    public static Transform FromJObject(this Transform tr, JObject data)
    {
        tr.position = new Vector3(data["position"][0].ToObject<float>(),
                                    data["position"][1].ToObject<float>(), 
                                    data["position"][2].ToObject<float>());
        tr.eulerAngles = new Vector3(data["rotation"][0].ToObject<float>(),
                                   data["rotation"][1].ToObject<float>(),
                                   data["rotation"][2].ToObject<float>());
        tr.localScale = new Vector3(data["localScale"][0].ToObject<float>(),
                                   data["localScale"][1].ToObject<float>(),
                                   data["localScale"][2].ToObject<float>());
        return tr;
    }
}
