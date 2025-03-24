using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;
namespace TheKiwiCoder
{
    public class Detected_Check : DecoratorNode
    {
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            // 블랙보드에서 isDetected 값 실시간으로 가져오기
            bool isDetected = blackboard.Get<bool>("isDetected");

            if (isDetected)
            {
                return State.Success;  // isDetected가 true이면 Success 반환
            }

            if (child == null)
            {
                Debug.LogWarning("[Detected_Check] Child node is missing!");
                return State.Failure;
            }

            return child.Update();  // 자식 노드가 있으면 자식 노드 실행
        }
    }
}
