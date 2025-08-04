using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chapter1Trigger : MonoBehaviour
{
    public GameObject triggerObject;
    public Animator triggerObjectAnimator;
    public AutoDoor autoDoor;
    // 🔸 추가: 텔레포트 인덱스
    public int teleportIndex = -1; // 기본값 -1: 텔레포트 트리거가 아닐 수도 있으니까
    public Transform enemyTpPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (gameObject.CompareTag("Trigger"))
        {
            Chapter1_Mgr.instance.ActiveTriggerAnimator(triggerObjectAnimator);

            if (triggerObject.name.Contains("Locker") && SoundManager.instance != null) { SoundManager.instance.LockerFallSound(); }
            else if (triggerObject.name.Contains("Wall") || triggerObject.name.Contains("MovePillar")) { SoundManager.instance.WallMoveSound(); }
            else if (triggerObject.name.Contains("OpenElevatorDoor Trigger")) { SoundManager.instance.ElevatorOpenSound(); }
        }
        if (gameObject.CompareTag("StrangeRoom1"))
        {
            Chapter1_Mgr.instance.MoveStrangeClass(Chapter1_Mgr.instance.strangeRoom1);
            if (SoundManager.instance != null) { SoundManager.instance.LockerFallSound(); }

        }
        if (gameObject.CompareTag("StrangeRoom2"))
        {
            Chapter1_Mgr.instance.MoveStrangeClass(Chapter1_Mgr.instance.strangeRoom2);
            if (SoundManager.instance != null)
            {
                SoundManager.instance.LockerFallSound();
            }
            if (other.gameObject.CompareTag("Player") && this.gameObject.CompareTag("Teleport"))
            {
                SchoolUIManager schoolUIManager = CommonUIManager.instance.uiManager as SchoolUIManager;

                schoolUIManager.lastEnemy.gameObject.transform.position = enemyTpPoint.position;
                schoolUIManager.lastEnemy.gameObject.transform.rotation = enemyTpPoint.rotation;
                // int index = System.Array.IndexOf(Chapter1_Mgr.instance.teleportTriggerPoints, this.gameObject);

                //if (index != -1)
                //{
                //    Chapter1_Mgr.instance.Teleport_Enemy(
                //        Chapter1_Mgr.instance.Chase_Enemy,
                //        index,
                //        this.gameObject
                //    );
                //}
                //else
                //{
                //    Debug.LogWarning("트리거가 teleportTriggerPoints 배열에 없습니다.");
                //}

                if (autoDoor != null && !autoDoor.door.doorState)
                {
                    autoDoor.door.Select_Door();
                }
            }
        }
    }
}