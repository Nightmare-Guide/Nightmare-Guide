using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;
namespace TheKiwiCoder
{
    public class Detected_Check : DecoratorNode
    {
        public float stopDistance = 0.5f;  // 플레이어와 닿았다고 판단할 거리

        private Transform player;
        private Transform enemy;

        protected override void OnStart()
        {
            player = Chapter1_Mgr.instance.player.transform;
            enemy = context.transform;
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            if (player == null || enemy == null)
            {
                Debug.LogWarning("[Detected_Check] 플레이어나 적 트랜스폼이 없습니다!");
                return State.Failure;
            }

            float distance = Vector3.Distance(enemy.position, player.position);

            // 닿았다면 트리 중단
            if (distance <= stopDistance)
            {
                Debug.Log("[Detected_Check] 플레이어와 닿았으므로 트리 중단");
                return State.Failure;
            }

            // isDetected 값 확인
            bool isDetected = blackboard.Get<bool>("isDetected");

            if (isDetected && child != null)
            {
                return child.Update();  // 추적이나 공격 같은 자식 노드 실행
            }

            return State.Failure;  // 감지되지 않았거나 자식이 없으면 실패
        }
    }
}
