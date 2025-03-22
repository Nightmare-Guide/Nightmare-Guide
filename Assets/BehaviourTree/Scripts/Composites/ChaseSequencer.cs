using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

namespace TheKiwiCoder
{
    public class ChaseSequencer : CompositeNode
    {
        protected int current;
        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            // isDetected 값 가져오기
            bool isDetected = blackboard.Get<bool>("isDetected");

            // isDetected가 false일 경우 왼쪽 시퀀스로 가도록 처리
            if (!isDetected)
            {
                Debug.Log("[ChaseSequencer] isDetected is false, moving to left sequence.");
                // 왼쪽 시퀀스 실행 (왼쪽 시퀀스가 자식 노드 0번에 있다고 가정)
                return children[0].Update(); // 왼쪽 시퀀스를 실행
            }

            // isDetected가 true일 경우에는 기본적으로 Success를 반환
            return State.Success;
        }
    }
}