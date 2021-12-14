using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanAnimatorController : MonoBehaviour
{
    [SerializeField] Rigidbody PlayerRB;
    [SerializeField] Animator animator;

    Vector3 oldPosition;

    private void Awake()
    {
        oldPosition = PlayerRB.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        bool moved = oldPosition != PlayerRB.transform.position;
        oldPosition = PlayerRB.transform.position;
        animator.SetBool("Moving", moved);
    }
}
