﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter1Trigger : MonoBehaviour
{
    public GameObject triggerObject;
    public Animator triggerObjectAnimator;
    public AutoDoor autoDoor;
    // 🔸 추가: 텔레포트 인덱스
    public int teleportIndex = -1; // 기본값 -1: 텔레포트 트리거가 아닐 수도 있으니까

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (gameObject.CompareTag("Trigger"))
        {
            Chapter1_Mgr.instance.ActiveTriggerAnimator(triggerObjectAnimator);
        }
        if (gameObject.CompareTag("StrangeRoom1"))
        {
            Chapter1_Mgr.instance.MoveStrangeClass(Chapter1_Mgr.instance.strangeRoom1);
        }
        if (gameObject.CompareTag("StrangeRoom2"))
        {
            Chapter1_Mgr.instance.MoveStrangeClass(Chapter1_Mgr.instance.strangeRoom2);
        }
        if (other.gameObject.CompareTag("Player") && this.gameObject.CompareTag("Teleport"))
        {
            int index = System.Array.IndexOf(Chapter1_Mgr.instance.teleportTriggerPoints, this.gameObject);

            if (index != -1)
            {
                Chapter1_Mgr.instance.Teleport_Enemy(
                    Chapter1_Mgr.instance.Chase_Enemy,
                    index,
                    this.gameObject
                );
            }
            else
            {
                Debug.LogWarning("트리거가 teleportTriggerPoints 배열에 없습니다.");
            }

            if (autoDoor != null && !autoDoor.door.doorState)
            {
                autoDoor.door.Select_Door();
            }
        }
    }
}
