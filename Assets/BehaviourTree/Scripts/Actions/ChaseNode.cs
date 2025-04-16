using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TheKiwiCoder;

public class ChaseNode : ActionNode
{
    private Transform enemy;
    private Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    protected override void OnStart()
    {
        enemy = context.transform;
        agent = context.agent; // context에서 바로 가져올 수 있음
        animator = context.animator;
        player = Chapter1_Mgr.instance.player.transform;
    }

    protected override void OnStop()
    {
        if (agent != null)
        {
            agent.velocity = Vector3.zero;
            agent.ResetPath();
        }
    }

    protected override State OnUpdate()
    {
        if (player == null || agent == null)
        {
            return State.Failure;
        }

        bool lockerDetected = blackboard.Get<bool>("lockerDetected");
        if (lockerDetected)
        {
            return State.Success;
        }

        agent.SetDestination(player.position);

        float distance = Vector3.Distance(enemy.position, player.position);

        if (distance > 20f)
        {
            return State.Success;
        }
        else if (distance < 2.6f)
        {
            // 🥊 어택 애니메이션 트리거 발동
            if (animator != null)
            {
                animator.SetTrigger("Attack");
                Debug.Log("Attack 트리거 발동!");
            }      
        }

        return State.Running;
    }
}
