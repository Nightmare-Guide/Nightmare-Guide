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
        player = Chapter1_Mgr.instance.player.transform; // 플레이어 찾기
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

        // 블랙보드에서 lockerDetected 값 가져오기
        bool lockerDetected = blackboard.Get<bool>("lockerDetected");

        // 만약 락커가 감지되었다면 추격을 멈추고 다음 노드로 넘어감
        if (lockerDetected)
        {
            return State.Success;
        }

        // 플레이어를 목표로 이동
        agent.SetDestination(player.position);

        // 플레이어와 너무 멀어졌거나 너무 가까워졌을 경우 추격 종료
        if (Vector3.Distance(enemy.position, player.position) < 0f || Vector3.Distance(enemy.position, player.position) > 20f)
        {
            return State.Success;
        }

        return State.Running; // 계속 추격 중
    }

}
