using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter1Trigger : MonoBehaviour
{
    public GameObject triggerObject;
    public Animator triggerObjectAnimator;

    // 🔸 추가: 텔레포트 인덱스
    public int teleportIndex = -1; // 기본값 -1: 텔레포트 트리거가 아닐 수도 있으니까

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (gameObject.CompareTag("Trigger"))
        {
            Chapter1_Mgr.instance.ActiveTriggerAnimator(triggerObjectAnimator);
        }
        else if (gameObject.CompareTag("StrangeRoom1"))
        {
            Chapter1_Mgr.instance.MoveStrangeClass(Chapter1_Mgr.instance.strangeRoom1);
        }
        else if (gameObject.CompareTag("StrangeRoom2"))
        {
            Chapter1_Mgr.instance.MoveStrangeClass(Chapter1_Mgr.instance.strangeRoom2);
        }
        else if (gameObject.CompareTag("Teleport") && teleportIndex >= 0)
        {
            Chapter1_Mgr.instance.Teleport_Enemy(
                Chapter1_Mgr.instance.Chase_Enemy,
                teleportIndex,
                this.gameObject
            );
        }
    }
}
