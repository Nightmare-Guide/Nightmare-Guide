using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder
{
    public class Selector : CompositeNode
    {
        protected int current;

        protected override void OnStart()
        {
            current = 0; // 실행할 자식 노드 인덱스 초기화
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            bool isDetected = blackboard.Get<bool>("isDetected"); // 🔥 실시간 감지값 가져오기
            Debug.Log($"[Selector] 현재 isDetected 값: {isDetected}");

            // 🔹 isDetected 값이 true면 오른쪽(1), false면 왼쪽(0) 실행
            int targetIndex = isDetected ? 1 : 0;

            // 🔥 예외 처리: 실행할 노드가 존재하는지 확인
            if (targetIndex >= children.Count)
            {
                Debug.LogError($"[Selector] 실행할 노드가 없습니다! (targetIndex: {targetIndex}, childrenCount: {children.Count})");
                return State.Failure;
            }

            // 🔥 선택한 노드 실행 후 상태 체크
            State result = children[targetIndex].Update();
            Debug.Log($"[Selector] 실행된 노드 {targetIndex} 결과: {result}");

            if (result == State.Success)
            {
                // 성공적으로 실행된 경우, 해당 노드를 성공으로 처리
                return State.Success;
            }
            else if (result == State.Failure)
            {
                // 실패한 경우, 다음 노드로 진행
                Debug.Log($"[Selector] 실패, 다른 노드를 시도합니다.");
                current++;
                if (current < children.Count)
                {
                    return State.Running;  // 아직 실행할 자식 노드가 남아있으므로 Running 상태 반환
                }
                else
                {
                    return State.Failure; // 모든 자식 노드가 실패하면 최종적으로 실패 반환
                }
            }

            return State.Running;  // 노드가 아직 실행 중인 경우
        }
    }
}