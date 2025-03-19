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
            // 📌 isDetected가 true이면 ChaseNode 실행
            if ((bool)blackboard["isDetected"])
            {
                // 자식 노드 중 ChaseNode가 있으면 실행
                foreach (var child in children)
                {
                    if (child is ChaseNode chaseNode)
                    {
                        State chaseState = chaseNode.Update();

                        // 만약 추격이 끝났다면(isDetected를 false로 변경)
                        if (chaseState == State.Failure)
                        {
                            blackboard["isDetected"] = false;
                        }

                        return chaseState;
                    }
                }
            }

            // 📌 기존 Selector 동작 유지
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
