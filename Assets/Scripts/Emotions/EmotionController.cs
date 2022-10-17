using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EmotionController : MonoBehaviour
{

    public SkinnedMeshRenderer face;
    public SkinnedMeshRenderer eyeLeft;
    public SkinnedMeshRenderer eyeRight;
    public SkinnedMeshRenderer eyelashes;

    // Eyes
    private double timeCloseOpenEye = 0.0;
    private int timeCloseOpenStage = 0;
    private double internalTimeEyes = 0.0;

    private double lastTimestamp = 0.0;

    private System.Random rand;

    // Start is called before the first frame update
    void Start()
    {
        timeCloseOpenEye = Random.Range(0, 3);
        rand = new System.Random();
    }

    // Update is called once per frame
    void Update()
    {
        if (timeCloseOpenStage != 0 || timeCloseOpenEye >= (rand.NextDouble() * 2) + 1)
        {
            if (timeCloseOpenStage == 0) { 
                internalTimeEyes = 0.0;
                Debug.Log("Open and Close Eyes " + timeCloseOpenEye + " Random Example: " + ((rand.NextDouble() * 2) + 1));
                timeCloseOpenEye -= (rand.NextDouble() * 2) + 3;
                timeCloseOpenStage++;
            } else if (timeCloseOpenStage == 1)
            {
                // 0 - Close Left / 7 - Close Right
                face.SetBlendShapeWeight(0, 90);
                eyelashes.SetBlendShapeWeight(0, 90);
                face.SetBlendShapeWeight(7, 90);
                eyelashes.SetBlendShapeWeight(7, 90);
                timeCloseOpenStage++;
            } else if (timeCloseOpenStage == 2)
            {
                if (internalTimeEyes > 0.1) {
                    face.SetBlendShapeWeight(0, 0);
                    eyelashes.SetBlendShapeWeight(0, 0);
                    face.SetBlendShapeWeight(7, 0);
                    eyelashes.SetBlendShapeWeight(7, 0);
                    timeCloseOpenStage = 0;
                } else
                {
                    internalTimeEyes += Time.deltaTime;
                }
            }
            
        } else
        {
            timeCloseOpenEye += Time.deltaTime;
        }
    }


}
