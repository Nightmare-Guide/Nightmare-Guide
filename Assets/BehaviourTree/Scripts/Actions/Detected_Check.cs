using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;
namespace TheKiwiCoder
{
    public class Detected_Check : DecoratorNode
    {
        private Transform player;
        private Transform enemy;
        private Collider playerCollider;
        private Collider enemyCollider;

        protected override void OnStart()
        {
            player = Chapter1_Mgr.instance.player.transform;
            enemy = context.transform;
            playerCollider = Chapter1_Mgr.instance.player.GetComponent<CapsuleCollider>();
            enemyCollider = context.transform.GetComponent<CapsuleCollider>();
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            if (player == null || enemy == null || playerCollider == null || enemyCollider == null)
            {
                Debug.LogWarning("[Detected_Check] 필수 트랜스폼 또는 콜라이더가 없습니다!");
                return State.Success;
            }

            float distance = Vector3.Distance(enemy.position, player.position);

            // 🔸 충돌 여부 체크
            if (playerCollider.bounds.Intersects(enemyCollider.bounds))
            {
                Debug.Log("[Detected_Check] 플레이어와 적이 충돌했습니다. 사망 연출 실행!");
                blackboard.UpdateCollisionStatus(true);

                if (blackboard.IsCollidedWithPlayer())
                {
                    return child.Update();
                }
                return State.Failure;
            }

            bool isDetected = blackboard.Get<bool>("isDetected");
            if (isDetected && child != null)
            {
                return child.Update();
            }

            return State.Failure;
        }
    }
}
