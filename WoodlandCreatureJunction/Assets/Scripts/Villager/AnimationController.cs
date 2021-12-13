using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationController : MonoBehaviour
{
    [Header("Animation")]
    public Animation CurrentAnimation;

    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        animator.SetInteger("animation_state", (int)CurrentAnimation);
    }

    public enum Animation
    {
        IDLE,
        MOVING,
        THINKING,
        TALKING
    }
}
