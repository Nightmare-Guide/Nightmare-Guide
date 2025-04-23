using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder
{
    public class Selector : CompositeNode
    {
        protected int current;

        public float stopDistance = 0.5f;  // 플레이어와 닿았다고 판단할 거리

        private Transform player;
        private Transform enemy;
        private Collider playerCollider;
        private Collider enemyCollider;
        protected override void OnStart()
        {
            current = 0;

            // 트랜스폼 캐싱
            if (Chapter1_Mgr.instance != null)
            {
                player = Chapter1_Mgr.instance.player?.transform;
            }
            enemy = context.transform;
            playerCollider = Chapter1_Mgr.instance.player.GetComponent<CapsuleCollider>();
            enemyCollider = context.transform.GetComponent<CapsuleCollider>();
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (player == null || enemy == null)
            {
                Debug.LogWarning("[Selector] player 또는 enemy 트랜스폼이 없습니다.");
                return State.Failure;
            }

            // 직접 충돌 체크
            if (playerCollider != null && enemyCollider != null &&
                playerCollider.bounds.Intersects(enemyCollider.bounds))
            {
                Debug.Log("[Selector] 충돌 감지됨 → 사망 연출 노드 실행");
                blackboard.Set("isCollided", true);
                if (children.Count > 2)
                {
                    return children[2].Update(); // 죽음 연출용 노드
                }
                return State.Failure;
            }

            // 감지 여부에 따라 자식 노드 선택
            bool isDetected = blackboard.Get<bool>("isDetected");
            Debug.Log($"[Selector] 현재 isDetected 값: {isDetected}");

            int targetIndex = isDetected ? 1 : 0;

            if (targetIndex >= children.Count)
            {
                Debug.LogError($"[Selector] 실행할 노드가 없습니다! (targetIndex: {targetIndex}, childrenCount: {children.Count})");
                return State.Failure;
            }

            State result = children[targetIndex].Update();
            Debug.Log($"[Selector] 실행된 노드 {targetIndex} 결과: {result}");

            if (result == State.Success) return State.Success;

            current++;
            return current < children.Count ? State.Running : State.Failure;
        }
    }
 }
