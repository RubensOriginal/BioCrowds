using UnityEngine;
using System;

public class Object : MonoBehaviour
{
    public enum Type { Spawner, Goal };

    public struct Data
    {
        public Vector3 pos;
        public Quaternion rot;
        public Type type;
    }

    public Data data;


}
