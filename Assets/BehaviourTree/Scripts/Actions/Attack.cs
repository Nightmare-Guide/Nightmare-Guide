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
                Debug.LogWarning("[Attack] 플레이어가 존재하지 않습니다!");
                return State.Failure;
            }

            float distance = Vector3.Distance(enemy.position, player.position);

            // 공격 사정거리 밖이면 실패
            if (distance > attackRange)
            {
                return State.Failure;
            }

            // 공격 딜레이 체크
            if (Time.time - lastAttackTime < attackDelay)
            {
                return State.Running;
            }

            // 공격 애니메이션 재생
            if (animator != null)
            {
                animator.SetTrigger("Attack");  // 애니메이터에 Attack 트리거가 있어야 함
            }


            lastAttackTime = Time.time;

            return State.Success;
        }
    }
}