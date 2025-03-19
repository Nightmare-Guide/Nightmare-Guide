using System.Collections;
using System.Collections.Generic;
using TheKiwiCoder;
using UnityEngine;

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
        bool isDetected = blackboard.Get<bool>(keyName);// 블랙보드에서 값 가져오기

        // 블랙보드에서 값을 제대로 가져오는지 확인하는 로그
        Debug.Log("[Detected_Check] Retrieved isDetected value from blackboard: " + isDetected);

        if (isDetected)
        {
            return State.Success;  // isDetected가 true이면 Success 반환
        }

        if (child == null)
        {
            Debug.LogWarning("[Detected_Check] Child node is missing!");
            return State.Failure;
        }

        return child.Update();  // 자식 노드가 있으면 자식 노드 실행
    }
}
