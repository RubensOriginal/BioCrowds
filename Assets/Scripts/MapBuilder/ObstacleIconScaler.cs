using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleIconScaler : MonoBehaviour
{
    void Update()
    {
        var min = Mathf.Min(transform.parent.localScale.x, transform.parent.localScale.z);
        transform.SetGlobalScale(0.8f * min * Vector3.one);
    }
}
