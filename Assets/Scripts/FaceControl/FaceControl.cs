using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceControl : MonoBehaviour
{
    
    public enum FaceAnimation { IDLE, SMILE, FEAR, SAD, SURPRISE };

    // Current
    private FaceAnimation currentAnimation;
    private float value;
    private float coeficientTransition;

    // New Values
    private FaceAnimation nextAnimation;
    private float nextValue;

    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        FaceAnimation currentAnimation = FaceAnimation.IDLE;
        value = 0.0f;
        coeficientTransition = 0.0f;

        nextAnimation = FaceAnimation.IDLE;
        nextValue = 0.0f;

        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (coeficientTransition != 0)
        {
            if (coeficientTransition > 0)
            {
                if (value + coeficientTransition > nextValue)
                {
                    value = nextValue;
                    
                } else
                {
                    value += coeficientTransition;
                }
                
                // Positivo
            } else
            {
                // Negativo
            }

            animator.SetFloat(currentAnimation.ToString().ToLower(), value);
        }
    }

    void setNewFaceAnimation(FaceAnimation animation, int value)
    {
        nextAnimation = animation;
        nextValue = value;

        coeficientTransition = (this.value + value) * Time.deltaTime;
    }
}
