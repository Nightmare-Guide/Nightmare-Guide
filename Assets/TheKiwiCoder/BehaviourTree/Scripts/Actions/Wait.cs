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

        // Wait���� isDetected üũ
        protected override State OnUpdate()
        {
            // blackboard���� isDetected ���� �����ɴϴ�.
            bool isDetected = blackboard.Get<bool>("isDetected");

            // isDetected�� true�̸� �ٷ� ���� ���·� ��ȯ�Ͽ� ���� �������� �Ѿ�� ��
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
