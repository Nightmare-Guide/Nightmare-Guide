using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheKiwiCoder
{
    public class Selector : CompositeNode
    {
        protected int current;
        private ChaseNode chaseNode;

        protected override void OnStart()
        {
            current = 0;
            chaseNode = new ChaseNode(); // �߰� ��� ����
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (blackboard.isDetected)
            {
                State chaseState = chaseNode.Update();

                // ���� �߰��� �����ٸ�(isDetected�� false�� �����), ������ ���ư�
                if (chaseState == State.Failure)
                {
                    blackboard.isDetected = false;
                }

                return chaseState;
            }

            // ������ Selector ���� ����
            for (int i = current; i < children.Count; ++i)
            {
                current = i;
                var child = children[current];

                switch (child.Update())
                {
                    case State.Running:
                        return State.Running;
                    case State.Success:
                        return State.Success;
                    case State.Failure:
                        continue;
                }
            }

            return State.Failure;
        }
    }
}
