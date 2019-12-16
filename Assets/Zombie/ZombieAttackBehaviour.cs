using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttackBehaviour : StateMachineBehaviour
{
    private ZombieController controller;
    private bool hasHit = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (controller == null)
        {
            controller = animator.transform.GetComponentInParent<ZombieController>();
        }
        controller.agent.isStopped = true;
        hasHit = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetFloat("HitCurve") > 0 && !hasHit)
        {
            Debug.Log("Hajå?");
            hasHit = true;
            controller.Attack();
        }

        if (Vector3.Distance(controller.transform.position, controller.target.transform.position) > controller.hitRange / 2)
        {
            animator.SetBool("AttackRange", false);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        hasHit = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
