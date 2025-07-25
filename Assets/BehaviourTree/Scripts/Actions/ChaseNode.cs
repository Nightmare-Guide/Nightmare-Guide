﻿using System.Collections;
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
        if (agent == null)
        {
            Debug.Log("agent is null");
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

        float distance = Vector3.Distance(enemy.position, player.position); //플레이어와의 거리

        if (distance > 25f)
        {
            return State.Success;
        }


        return State.Running;
    }
}
