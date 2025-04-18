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

        protected override void OnStart()
        {
            current = 0;

            // 트랜스폼 캐싱
            if (Chapter1_Mgr.instance != null)
            {
                player = Chapter1_Mgr.instance.player?.transform;
            }
            enemy = context.transform;
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            // 1. 유효성 검사
            if (player == null || enemy == null)
            {
                Debug.LogWarning("[Selector] player 또는 enemy 트랜스폼이 없습니다.");
                return State.Failure;
            }

            // 2. 거리 체크
            float distance = Vector3.Distance(player.position, enemy.position);
            if (distance <= stopDistance)
            {
                Debug.Log("[Selector] 플레이어와 닿음 - 트리 중단 (Failure)");
                return State.Failure;
            }

            // 3. 감지 여부 확인
            bool isDetected = blackboard.Get<bool>("isDetected");
            Debug.Log($"[Selector] 현재 isDetected 값: {isDetected}");

            int targetIndex = isDetected ? 1 : 0;

            if (targetIndex >= children.Count)
            {
                Debug.LogError($"[Selector] 실행할 노드가 없습니다! (targetIndex: {targetIndex}, childrenCount: {children.Count})");
                return State.Failure;
            }

            // 4. 선택된 자식 노드 실행
            State result = children[targetIndex].Update();
            Debug.Log($"[Selector] 실행된 노드 {targetIndex} 결과: {result}");

            if (result == State.Success)
            {
                return State.Success;
            }
            else if (result == State.Failure)
            {
                current++;
                if (current < children.Count)
                {
                    return State.Running;
                }
                else
                {
                    return State.Failure;
                }
            }

            return State.Running;
        }
    }
}
