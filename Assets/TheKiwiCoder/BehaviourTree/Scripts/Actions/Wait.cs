using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder {
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

        protected override State OnUpdate()
        {
            // isDetected가 true이면 기다리지 않고 실패 상태로 반환하여 시퀀서를 넘김
            if (blackboard.Get<bool>("isDetected"))
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
