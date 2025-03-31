using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class ScanLocker : ActionNode
{
    private GameObject targetLocker;

    protected override void OnStart()
    {
        bool lockerDetected = blackboard.Get<bool>("lockerDetected"); // 🔹 블랙보드에서 lockerDetected 값 가져오기

        if (!lockerDetected)
        {
            return; // 🔹 락커가 탐지되지 않았다면 실행 안 함
        }

        targetLocker = blackboard.Get<GameObject>("detectedLocker"); // 🔹 탐지된 락커 가져오기

        if (targetLocker != null)
        {
            Vector3 lockerFront = targetLocker.transform.position + targetLocker.transform.forward * 1.0f;
            blackboard.Set("moveToPosition", lockerFront); // 🔹 이동 목표 설정
        }
    }


    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        if (targetLocker == null)
        {
            return State.Failure; // 🔹 락커를 찾을 수 없으면 실패
        }

        return State.Success;
    }
}
