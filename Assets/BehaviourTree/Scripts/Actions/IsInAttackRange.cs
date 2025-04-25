using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

namespace TheKiwiCoder
{
    public class IsInAttackRange : ActionNode
    {
        public float attackRange = 1.5f; // 공격 가능한 거리

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
                Debug.LogWarning("[IsInAttackRange] 플레이어가 존재하지 않습니다!");
                return State.Failure;
            }

            Transform enemy = context.transform;
            Transform player = Chapter1_Mgr.instance.player.transform;

            float distance = Vector3.Distance(enemy.position, player.position);

            if (distance <= attackRange)
            {
                return State.Success; // 공격 범위 안에 있음
            }
            else
            {
                return State.Failure; // 공격 범위 밖
            }
        }
    }
}