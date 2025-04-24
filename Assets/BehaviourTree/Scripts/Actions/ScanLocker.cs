using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class ScanLocker : ActionNode
{
    private GameObject targetLocker;

    protected override void OnStart()
    {
        targetLocker = null; // 🔹 항상 초기화

        bool lockerDetected = blackboard.Get<bool>("lockerDetected");

        if (!lockerDetected)
        {
            return;
        }

        targetLocker = blackboard.Get<GameObject>("detectedLocker");

        if (targetLocker != null)
        {
            Vector3 lockerFront = targetLocker.transform.position + targetLocker.transform.forward * 1.0f;
            blackboard.Set("moveToPosition", lockerFront);
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
