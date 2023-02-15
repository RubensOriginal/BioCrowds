using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    public static T[] GetComponentsInChildrenWithTag<T>(this GameObject gameObject, string tag)
         where T : Component
    {
        List<T> results = new List<T>();

        if (gameObject.CompareTag(tag))
            results.Add(gameObject.GetComponent<T>());

        foreach (Transform t in gameObject.transform)
            results.AddRange(t.gameObject.GetComponentsInChildrenWithTag<T>(tag));

        return results.ToArray();
    }

    public static GameObject[] FindChildrenWithTag(this GameObject gameObject, string tag)
    {
        List<GameObject> results = new List<GameObject>();

        foreach (Transform t in gameObject.transform)
            if (t.CompareTag(tag))
                results.Add(t.gameObject);

        return results.ToArray();
    }
}
