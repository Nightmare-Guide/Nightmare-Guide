using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

namespace TheKiwiCoder
{
    public class IsInAttackRange : ActionNode
    {
        public float attackRange = 1.5f; // ���� ������ �Ÿ�

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            if (Chapter1_Mgr.instance == null || Chapter1_Mgr.instance.player == null)
            {
                Debug.LogWarning("[IsInAttackRange] �÷��̾ �������� �ʽ��ϴ�!");
                return State.Failure;
            }

            Transform enemy = context.transform;
            Transform player = Chapter1_Mgr.instance.player.transform;

            float distance = Vector3.Distance(enemy.position, player.position);

            if (distance <= attackRange)
            {
                return State.Success; // ���� ���� �ȿ� ����
            }
            else
            {
                return State.Failure; // ���� ���� ��
            }
        }
    }
}