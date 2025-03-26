using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder
{
    public class Sequencer : CompositeNode
    {
        protected int current;

        protected override void OnStart()
        {
            current = 0;
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            // isDetected �� üũ
            bool isDetected = blackboard.Get<bool>("isDetected");

            // isDetected�� true�� ��� �ٷ� Failure�� ��ȯ�Ͽ� ���� �������� �ѱ��
            if (isDetected)
            {
                Debug.Log("[Sequencer] isDetected is true, skipping current sequence and moving to next.");
                return State.Failure;
            }

            // ������ ����
            for (int i = current; i < children.Count; ++i)
            {
                current = i;
                var child = children[current];

                switch (child.Update())
                {
                    case State.Running:
                        return State.Running;
                    case State.Failure:
                        return State.Failure;
                    case State.Success:
                        continue;
                }
            }

            return State.Success;
        }
    }
}
