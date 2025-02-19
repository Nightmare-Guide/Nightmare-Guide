using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Locker : MonoBehaviour
{
    public Transform setTr;//플레이어 목표 위치
    private bool isMovingToLocker = false; //도어 열림
    [SerializeField] GameObject door;
    [SerializeField] Door door_Obj;
    public float speed = 0.1f; // 플레이어가 락커로 들어가는 이동 속도

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
                pr.localRotation = Quaternion.Euler(0,180,0);
                Invoke("OffDoor", 0.5f);//문열리는 코루틴 종료후 실행
              
            }
            
        }

    }
    public void OffDoor()
    {
        door_Obj.Select_Door(); // 플레이어 입장후 문닫기
        Debug.Log("잠김");
        Camera_Rt.instance.Open_Camera();
    }
    public void avata(GameObject obj)
    {

    }
}
