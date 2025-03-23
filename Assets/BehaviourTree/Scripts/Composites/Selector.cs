using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder
{
    public class Selector : CompositeNode
    {
        protected int current;
        private Detected_Check detectedCheck; // 🔹 Detected_Check 노드 저장

        protected override void OnStart()
        {
            current = 0; // 실행할 자식 노드 인덱스 초기화

            // 🔹 Detected_Check 노드를 찾아서 저장
            foreach (var child in children)
            {
                if (child is Detected_Check checkNode)
                {
                    detectedCheck = checkNode;
                    break;
                }
            }
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            // 🔹 Detected_Check에서 isDetected 값을 실시간으로 갱신하여 가져오기
            bool isDetected = blackboard.Get<bool>("isDetected");

            // 🔹 isDetected 값에 따라 실행할 시퀀스 선택 (true이면 두 번째, false이면 첫 번째 시퀀스)
            int targetIndex = isDetected ? 1 : 0;

            // 🔹 시퀀스가 부족하면 실패 반환
            if (targetIndex >= children.Count)
            {
                Debug.LogError("[Selector] 시퀀스 노드 부족!");
                return State.Failure;
            }

            // 🔹 선택된 자식 노드 실행
            return children[targetIndex].Update();
        }
    }
}
