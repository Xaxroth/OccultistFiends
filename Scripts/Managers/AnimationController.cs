using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public static AnimationController Instance;
    public Animator CreatureAnimator;
    public Animator CultistAnimator;
    public Animator ShadowAnimator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        CreatureAnimator = BeastPlayerController.Instance.CreatureAnimator;
        CultistAnimator = PlayerController.Instance.CultistAnimator;
    }

    public void Update()
    {
        //switch (creatureState)
        //{
        //    case CreatureAnimationState.Idle:
        //        CreatureAnimator.SetBool(Animator.StringToHash("Idle"), true);
        //        break;
        //}
    }

    //public void PlayAnimation(Animator animator, string animation, bool looping)
    //{
    //    StartCoroutine(AnimationSequence(animator, animation, looping));
    //}

    public void SetAnimatorBool(Animator animator, string parameter, bool value)
    {
        animator.SetBool(Animator.StringToHash(parameter), value);
    }

    public void SetAnimatorInt(Animator animator, string animation, int value)
    {
        animator.SetInteger(Animator.StringToHash(animation), value);
    }

    public void SetAnimatorFloat(Animator animator, string animation, float value)
    {
        animator.SetFloat(Animator.StringToHash(animation), value);
    }

    private IEnumerator AnimationSequence(Animator animator, string animation, bool looping)
    {
        if (!looping)
        {
            animator.SetBool(animation, true);
            yield return new WaitForSeconds(0.1f); // clip.length?
            animator.SetBool(animation, false);
        }
        else
        {

        }
    }



}
