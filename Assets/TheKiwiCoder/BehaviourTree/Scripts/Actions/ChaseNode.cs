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
        enemy = context.transform; // AI�� Transform ��������
        agent = enemy.GetComponent<NavMeshAgent>(); // NavMeshAgent ��������
        player = GameObject.FindGameObjectWithTag("Player")?.transform; // �÷��̾� ã��
    }

    protected override void OnStop()
    {
        // �߰��� ���� ��, AI�� �ӵ��� 0���� ����
        if (agent != null)
        {
            agent.velocity = Vector3.zero;
            agent.ResetPath();
        }
    }

    protected override State OnUpdate()
    {
        // �÷��̾ ���ų�, �׺���̼� ������Ʈ�� ������ ���� ��ȯ
        if (player == null || agent == null)
        {
            return State.Failure;
        }

        // �÷��̾ ��ǥ�� �̵�
        agent.SetDestination(player.position);

        // �÷��̾�� �ʹ� ��������ٸ� ���� ��ȯ
        if (Vector3.Distance(enemy.position, player.position) < 2f)
        {
            return State.Success;
        }

        return State.Running; // ��� �߰� ��
    }
}
