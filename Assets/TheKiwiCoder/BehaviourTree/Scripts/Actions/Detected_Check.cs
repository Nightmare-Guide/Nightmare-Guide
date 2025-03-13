using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class Detected_Check : DecoratorNode
{
    public string keyName = "isDetected"; // 블랙보드에서 사용할 키

    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        bool isDetected = blackboard.Get<bool>(keyName); // 블랙보드에서 불값 가져오기

        if (isDetected)
        {
            return State.Failure; // 첫 번째 시퀀스를 스킵하고 다음 시퀀스로 이동
        }

        return child.Update(); // 정상적으로 자식 노드를 실행
    }
}
