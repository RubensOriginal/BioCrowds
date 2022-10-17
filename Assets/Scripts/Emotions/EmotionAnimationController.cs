using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class EmotionAnimationController : MonoBehaviour
{

    private List<Animator> faceAnimators;

    private bool isIdle;
    private bool isSmiling;

    // Start is called before the first frame update
    void Start()
    {
        isIdle = true;
        isSmiling = false;

        faceAnimators = new List<Animator>();

        foreach(Transform child in transform)
        {
            if (child.GetComponent<Animator>() != null)
                faceAnimators.Add(child.GetComponent<Animator>());
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("[1]"))
        {
            isIdle = true;
            isSmiling = false;

            Debug.Log("Idle");
        }
        else if (Input.GetKeyDown("[2]"))
        {
            isIdle = false;
            isSmiling = true;

            Debug.Log("Smile");
        }

        if (isIdle)
        {
            foreach(Animator animator in faceAnimators)
            {
                if (animator.GetFloat("Smile") > 0.0f)
                    animator.SetFloat("Smile", animator.GetFloat("Smile") - 0.01f);
            }
        }
        else if (isSmiling)
        {
            foreach (Animator animator in faceAnimators)
            {
                if (animator.GetFloat("Smile") < 1.0f)
                    animator.SetFloat("Smile", animator.GetFloat("Smile") + 0.01f);
            }
        }
    }
}
