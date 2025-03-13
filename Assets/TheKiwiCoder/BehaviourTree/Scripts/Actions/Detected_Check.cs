using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class Detected_Check : DecoratorNode
{
    public string keyName = "isDetected"; // �����忡�� ����� Ű

    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        bool isDetected = blackboard.Get<bool>(keyName); // �����忡�� �Ұ� ��������

        if (isDetected)
        {
            return State.Failure; // ù ��° �������� ��ŵ�ϰ� ���� �������� �̵�
        }

        return child.Update(); // ���������� �ڽ� ��带 ����
    }
}
