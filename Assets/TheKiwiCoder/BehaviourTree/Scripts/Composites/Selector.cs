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
            chaseNode = new ChaseNode(); // 추격 노드 생성
        }

        protected override void OnStop() { }

        protected override State OnUpdate()
        {
            if (blackboard.isDetected)
            {
                State chaseState = chaseNode.Update();

                // 만약 추격이 끝났다면(isDetected가 false로 변경됨), 순찰로 돌아감
                if (chaseState == State.Failure)
                {
                    blackboard.isDetected = false;
                }

                return chaseState;
            }

            // 기존의 Selector 동작 유지
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
