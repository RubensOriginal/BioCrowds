using System;
using UnityEngine;

namespace Biocrowds.Core
{
    public class GoalArea : MonoBehaviour
    {
        public float speed;

        private void Update()
        {
            Quaternion q = this.transform.rotation;
            
            // Debug.Log("X:" + q.x + " | Y: " + q.y + " | Z: " + q.z + " | W: " + q.w);
        }
    }
}