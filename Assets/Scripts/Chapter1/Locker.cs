using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityStandardAssets.Characters.FirstPerson;

public class Locker : MonoBehaviour
{
    public Transform setTr;//플레이어 목표 위치
    public Vector3 startPr;//플레이어 락커 연 시점의 위치
    private bool isMovingToLocker = false; //락커에 들어갈떄
    private bool outMovingToLocker = false; //락커에 나갈때
    [SerializeField] GameObject door; //도어 오브젝트
    [SerializeField] Door door_Obj; //도어 오브젝트 스크립트 컴포넌트
    public float speed = 0.1f; // 플레이어가 락커로 들어가는 이동 속도
    [Header("플레이어 회전")]
    public bool lockPr = false;//락커에 입장시 플레이어 회전
    Quaternion targetRotation; // 목표 회전 값
    Quaternion startRotation; // 시작 회전값
    Transform pr; // 플레이어 위치값
    //카메라 회전 범위
    public float rotateRange = 180f;
    private void Start()
    {
       
        door_Obj = door.GetComponent<Door>();

        speed = 0.1f;
    }

    public void PlayerHide()
    {
        door_Obj.Select_Door();
        pr = Chapter1_Mgr.instance.player.transform;
        startPr = pr.position;
        Debug.Log("처음 목표 좌표: " + startPr);
        if (lockPr)
        {
            startRotation = pr.rotation;
            targetRotation = Quaternion.Euler(0, startRotation.eulerAngles.y + rotateRange, 0);
            Debug.Log("시작 회전: " + startRotation.eulerAngles);
            Debug.Log("목표 회전: " + targetRotation.eulerAngles);
        }
        isMovingToLocker = true;
    }

    public void OpenLocker()//락커를 열고 나올 떄
    {
        door_Obj.Select_Door();// 락커 열기
       
        Debug.Log("시작 좌표: " + pr.position);
        Debug.Log("목표 좌표: " + startPr);
        outMovingToLocker = true;
        
    }

    private void FixedUpdate()
    {
        if (isMovingToLocker) //락커안까지 플레이어 이동 및 카메라 회전
        {
            
            pr.position = Vector3.MoveTowards(pr.position, setTr.position, speed);
           
            if (Vector3.Distance(pr.position, setTr.position) < 0.01f && Quaternion.Angle(pr.rotation, targetRotation) < 1f)
            {            
                isMovingToLocker = false;
                pr.rotation = targetRotation;
                lockPr = false;
                Debug.Log("최종 회전: " + pr.rotation.eulerAngles);

                Invoke("InLocker", 0.5f);//문열리는 코루틴 종료후 실행
            }


            if (lockPr)
            {
                pr.rotation = Quaternion.Slerp(pr.rotation, targetRotation, Time.deltaTime * 5f);
                Debug.Log("현재 회전: " + pr.rotation.eulerAngles);
            }
          
            
        }

        if (outMovingToLocker)
        {
            pr.position = Vector3.MoveTowards(pr.position, startPr, speed);
            if (Vector3.Distance(pr.position, startPr) < 0.01f)
            {
                outMovingToLocker = false;
                Debug.Log("락커 탈출");
                Invoke("OutLocker", 1f); // 문 닫기 실행
            }
        }

    }


    public void OutLocker()
    {
        door_Obj.Select_Door(); // 플레이어 탈추후 문닫기
        Camera_Rt.instance.Open_Camera();
        PlayerController.instance.Open_PlayerController();
    }
    public void InLocker() //플레이어 진입후 도어 닫김
    {

     
        Debug.Log("최종 회전2" + pr.rotation);
        door_Obj.Select_Door(); // 플레이어 입장후 문닫기
        Debug.Log("잠김");
        lockPr = false;
        Camera_Rt.instance.Open_Camera();

    }

}
