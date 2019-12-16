using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieIdleBehaviour : StateMachineBehaviour
{
    private ZombieController controller;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (controller == null)
            controller = animator.transform.GetComponentInParent<ZombieController>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        controller.Look();

        //if (PlayerController.AllPlayers != null)
        //{
        //    Debug.DrawRay(controller.transform.position + Vector3.up, ((PlayerController.AllPlayers[0].transform.position + Vector3.down/2) - controller.transform.position).normalized * controller.sightRange, Color.blue);
        //    Debug.DrawRay(controller.transform.position + Vector3.up, Quaternion.AngleAxis(controller.fov / 2, Vector3.up) * controller.transform.forward * controller.sightRange);
        //    Debug.DrawRay(controller.transform.position + Vector3.up, Quaternion.AngleAxis(-controller.fov / 2, Vector3.up) * controller.transform.forward * controller.sightRange);
        //}
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
