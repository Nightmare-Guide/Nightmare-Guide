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
            // isDetected �� ��������
            bool isDetected = blackboard.Get<bool>("isDetected");

            // isDetected�� false�� ��� ���� �������� ������ ó��
            if (!isDetected)
            {
                Debug.Log("[ChaseSequencer] isDetected is false, moving to left sequence.");
                // ���� ������ ���� (���� �������� �ڽ� ��� 0���� �ִٰ� ����)
                return children[0].Update(); // ���� �������� ����
            }

            // isDetected�� true�� ��쿡�� �⺻������ Success�� ��ȯ
            return State.Success;
        }
    }
}