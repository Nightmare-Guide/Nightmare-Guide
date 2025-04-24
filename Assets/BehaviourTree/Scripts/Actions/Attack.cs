using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

namespace TheKiwiCoder
{
    public class Attack : ActionNode
    {
        private Animator animator;
        private Transform player;
        private Transform enemy;

        public float attackRange = 1.5f;
        public float attackDelay = 1.0f;
        private float lastAttackTime;

        protected override void OnStart()
        {
            enemy = context.transform;
            animator = context.animator;
            player = Chapter1_Mgr.instance.player.transform;
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            if (player == null)
            {
                Debug.LogWarning("[Attack] �÷��̾ �������� �ʽ��ϴ�!");
                return State.Failure;
            }

            float distance = Vector3.Distance(enemy.position, player.position);

            // ���� �����Ÿ� ���̸� ����
            if (distance > attackRange)
            {
                return State.Failure;
            }

            // ���� ������ üũ
            if (Time.time - lastAttackTime < attackDelay)
            {
                return State.Running;
            }

            // ���� �ִϸ��̼� ���
            if (animator != null)
            {
                animator.SetTrigger("Attack");  // �ִϸ����Ϳ� Attack Ʈ���Ű� �־�� ��
            }


            lastAttackTime = Time.time;

            return State.Success;
        }
    }
}