using Biocrowds.Core;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class BoundingBoxCalculator : MonoBehaviour
{
    public struct Bound
    {
        public Vector3 min;
        public Vector3 max;

        public Bound(Vector3 min, Vector3 max)
        {
            this.min = min;
            this.max = max;
        }
    }

    public void SaveBoundingBox(List<Agent> agents, Camera camera, int counter)
    {
        using (StreamWriter sw = File.CreateText("Screenshot/sim/" +  counter + ".txt"))
        {
            foreach (Agent agent in agents)
            {
                Bound bound = CalculateBoundingBox(agent._visualAgent.VisualReference.GetComponent<Renderer>().bounds, camera);

                float nMinX = Mathf.Clamp(bound.min.x / camera.pixelWidth, 0.0f ,1.0f);
                float nMaxX = Mathf.Clamp(bound.max.x / camera.pixelWidth, 0.0f, 1.0f);
                float nMinY = Mathf.Clamp(bound.min.y / camera.pixelHeight, 0.0f, 1.0f);
                float nMaxY = Mathf.Clamp(bound.max.y / camera.pixelHeight, 0.0f, 1.0f);

                float width = nMaxX - nMinX;
                float height = nMaxY - nMinY;
                float xCenter = nMinX + (width / 2);
                float yCenter = nMinY + (height / 2);

                //Bounds bounds = agent.gameObject.GetComponent<Renderer>().bounds;

                ////Vector3 center = camera.WorldToScreenPoint(bounds.center);
                //Vector3 max = camera.WorldToScreenPoint(bounds.max);
                //Vector3 min = camera.WorldToScreenPoint(bounds.min);

                //float nMinX = min.x / camera.pixelWidth;
                //float nMaxX = max.x / camera.pixelWidth;
                //float nMinY = 1 - (min.y / camera.pixelHeight);
                //float nMaxY = 1 - (max.y / camera.pixelHeight);

                //float width = Mathf.Abs(nMaxX - nMinX);
                //float height = Mathf.Abs(nMaxY - nMinY);
                //float xCenter = nMinX + (width / 2);
                //float yCenter = nMinY + (height / 2);

                //Debug.Log("0 " + nMinX + " " + nMaxX + " " + nMinY + " " + nMaxY);
                if (width != 0 && height != 0)
                    sw.WriteLine("0 " + xCenter + " " +  yCenter + " " + width + " " + height);
            }
        }
    }

    private Bound CalculateAllBoundingBoxBody(Agent agent,  Camera camera)
    {
        SkinnedMeshRenderer[] meshRenderer = agent.GetComponentsInChildren<SkinnedMeshRenderer>();

        Vector3 min = Vector3.positiveInfinity;
        Vector3 max = Vector3.negativeInfinity;

        foreach (SkinnedMeshRenderer mesh in meshRenderer)
        {
            Bound bound = CalculateBoundingBox(mesh.bounds, camera);

            min = Vector3.Min(bound.min, min);
            max = Vector3.Max(bound.max, max);
        }

        return new Bound(min, max);
    }

    private Bound CalculateBoundingBox(Bounds b, Camera camera)
    {
        Vector3[] bounds = new Vector3[8];
      
        bounds[0] = camera.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
        bounds[1] = camera.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
        bounds[2] = camera.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
        bounds[3] = camera.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));
        bounds[4] = camera.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
        bounds[5] = camera.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
        bounds[6] = camera.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
        bounds[7] = camera.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));

        for (int i = 0; i < bounds.Length; i++) bounds[i].y = Screen.height - bounds[i].y;

        Vector3 min = bounds[0];
        Vector3 max = bounds[0];

        for (int i = 1; i < bounds.Length; i++)
        {
            min = Vector3.Min(min, bounds[i]);
            max = Vector3.Max(max, bounds[i]);
        }

        return new Bound(min, max);
    }
}
