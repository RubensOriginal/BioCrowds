using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    public static List<float> AsList(this Vector3 pos)
    {
        return new List<float> { pos.x, pos.y, pos.z };
    }
}