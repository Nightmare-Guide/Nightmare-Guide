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

    protected override void OnStart()
    {
        enemy = context.transform; // AI의 Transform 가져오기
        agent = enemy.GetComponent<NavMeshAgent>(); // NavMeshAgent 가져오기
        player = GameObject.FindGameObjectWithTag("Player")?.transform; // 플레이어 찾기
    }

    protected override void OnStop()
    {
        // 추격을 멈출 때, AI의 속도를 0으로 설정
        if (agent != null)
        {
            agent.velocity = Vector3.zero;
            agent.ResetPath();
        }
    }

    protected override State OnUpdate()
    {
        // 플레이어가 없거나, 네비게이션 에이전트가 없으면 실패 반환
        if (player == null || agent == null)
        {
            return State.Failure;
        }

        // 플레이어를 목표로 이동
        agent.SetDestination(player.position);

        // 플레이어와 너무 가까워졌다면 성공 반환
        if (Vector3.Distance(enemy.position, player.position) < 2f)
        {
            return State.Success;
        }

        return State.Running; // 계속 추격 중
    }
}
