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
            Debug.Log("[ChaseSequencer] ���� ��...");

            foreach (var child in children)
            {
                var result = child.Update();
                Debug.Log($"[ChaseSequencer] {child.name} ����: {result}");

                if (result == State.Running)
                {
                    return State.Running;
                }
            }

            return State.Success;
        }

    }
}