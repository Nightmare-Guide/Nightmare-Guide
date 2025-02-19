using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Locker : MonoBehaviour
{
    public Transform setTr;//플레이어 목표 위치
    private bool isMovingToLocker = false;
    [SerializeField] GameObject door;
    [SerializeField] Door door_Obj;
    public float speed = 0.1f;

    private void Start()
    {
        door_Obj = door.GetComponent<Door>();
    }

    public void PlayerHide()
    {
        door_Obj.Select_Door();
        isMovingToLocker = true;
    }

    private void FixedUpdate()
    {
        if (isMovingToLocker)
        {
            Transform pr = Chapter1_Mgr.instance.player.transform;
            pr.position = Vector3.MoveTowards(pr.position, setTr.position, speed);

            if (Vector3.Distance(pr.position, setTr.position) < 0.01f)
            {
                isMovingToLocker = false;
                Debug.Log("플레이어가 락커 안으로 이동 완료");
                pr.rotation = Quaternion.Euler(0, 90, 0);
                door_Obj.Select_Door();
            }
        }

    }
}
