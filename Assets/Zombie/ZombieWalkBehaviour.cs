using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieWalkBehaviour : StateMachineBehaviour
{
    private ZombieController controller;
    private Vector3 eyesPos { get { return controller.target.transform.position + Vector3.up * .8f; } }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (controller == null)
        {
            controller = animator.transform.GetComponentInParent<ZombieController>();
        }
        controller.agent.isStopped = false;
        controller.target = PlayerController.GetClosestPlayer(animator.transform.position);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        controller.SetTarget();
        controller.agent.SetDestination(controller.target.transform.position);
        animator.SetFloat("SpeedFactor", controller.agent.velocity.magnitude / controller.agent.speed);

        if (Vector3.Distance(controller.transform.position, controller.target.transform.position) < controller.hitRange)
        {
            animator.SetBool("AttackRange", true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        controller.agent.isStopped = true;
    }

// OnStateMove is called right after Animator.OnAnimatorMove()
//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
//{
//    // Implement code that processes and affects root motion
//}

// OnStateIK is called right after Animator.OnAnimatorIK()
override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetLookAtPosition(eyesPos);
        animator.SetLookAtWeight(1, 0, .8f, 0);
    }
}
