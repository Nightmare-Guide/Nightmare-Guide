using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder
{
    public class Wait : ActionNode
    {
        public float waitTime = 3f;
        private float timer = 0f;

        protected override void OnStart()
        {
            timer = 0f;
        }

        protected override void OnStop()
        {
        }

        // Wait에서 isDetected 체크
        protected override State OnUpdate()
        {
            // blackboard에서 isDetected 값을 가져옵니다.
            bool isDetected = blackboard.Get<bool>("isDetected");

            // isDetected가 true이면 바로 실패 상태로 반환하여 다음 시퀀스로 넘어가게 함
            if (isDetected)
            {
                return State.Failure;
            }

            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                return State.Success;
            }

            return State.Running;
        }

    }
}
