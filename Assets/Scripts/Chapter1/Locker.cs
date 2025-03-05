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

    [Header("락커 열고 닫기")]
    public bool doorState; //true면 닫힌상태 false면 열린상태
    public BoxCollider boxcollider;

    [SerializeField] private Quaternion startRotationLocker;
    [SerializeField] private Quaternion endRotation;
    private float endTime = 0.5f; //회전 시간
    public bool isRotation = false; // false면 회전 안하고있음.
    public Coroutine currentCoroutine;
    public enum LockerStat
    {
        Open,
        Close
    }
    public LockerStat stat = LockerStat.Close;
    private void Start()
    {
       
        door_Obj = door.GetComponent<Door>();
        boxcollider = GetComponent<BoxCollider>();
        speed = 0.01f;
    }

    public void PlayerHide()
    {
        if (!PlayerController.instance.stat.Equals(PlayerController.PlayerState.Hide))
        {
            return;
        }
        Select_Locker();//락커 열기
        pr = Chapter1_Mgr.instance.player.transform;
        startPr = pr.position;
        //Debug.Log("처음 목표 좌표: " + startPr);
        if (lockPr)
        {
            startRotation = pr.rotation;
            targetRotation = Quaternion.Euler(0, startRotation.eulerAngles.y + rotateRange, 0);
           // Debug.Log("시작 회전: " + startRotation.eulerAngles);
           // Debug.Log("목표 회전: " + targetRotation.eulerAngles);
        }
        isMovingToLocker = true;
    }

    public void OpenLocker()//락커를 열고 나올 떄
    {
        if (!PlayerController.instance.stat.Equals(PlayerController.PlayerState.Idle))
        {
            return;
        }
        Select_Locker();// 락커 열기
        
        //Debug.Log("시작 좌표: " + pr.position);
        //Debug.Log("목표 좌표: " + startPr);
        outMovingToLocker = true;
        
    }

    private void FixedUpdate()
    {
        if (isMovingToLocker) //락커안까지 플레이어 이동 및 카메라 회전
        {
            
            pr.position = Vector3.MoveTowards(pr.position, setTr.position, Time.fixedDeltaTime*1.2f);//이동 속도 조절 필요
           
            if (Vector3.Distance(pr.position, setTr.position) < 0.01f && Quaternion.Angle(pr.rotation, targetRotation) < 1f)
            {            
                isMovingToLocker = false;
                pr.rotation = targetRotation;
                lockPr = false;
                //Debug.Log("최종 회전: " + pr.rotation.eulerAngles);

                if (currentCoroutine ==null)
                {
                    InLocker();//문열리는 코루틴 종료후 실행
                }
              
            }


            if (lockPr)
            {
                pr.rotation = Quaternion.Slerp(pr.rotation, targetRotation, Time.deltaTime * 5f);//카메라 회전 속도
                //Debug.Log("현재 회전: " + pr.rotation.eulerAngles);
            }
          
            
        }

        if (outMovingToLocker)
        {
            pr.position = Vector3.MoveTowards(pr.position, startPr, Time.fixedDeltaTime*1.2f);//나오는 속도 조절 필요
            if (Vector3.Distance(pr.position, startPr) < 0.01f)
            {
                outMovingToLocker = false;
                //Debug.Log("락커 탈출");
                if (currentCoroutine == null)
                {
                    OutLocker();//문열리는 코루틴 종료후 실행
                }
               
            }
        }

    }


    public void OutLocker()
    {
        Select_Locker(); // 플레이어 탈추후 문닫기
        Camera_Rt.instance.Open_Camera();
        PlayerController.instance.Open_PlayerController();
    }
    public void InLocker() //플레이어 진입후 도어 닫김
    {

     
        //Debug.Log("최종 회전2" + pr.rotation);
        Select_Locker(); // 플레이어 입장후 문닫기
       // Debug.Log("잠김");
        lockPr = false;
        Camera_Rt.instance.Open_Camera();

    }


    public void Select_Locker()//도어 열기
    {
        if (!isRotation)
        {
            isRotation = true;
            StartCoroutine(RotationDoor());
        }

    }

    private IEnumerator RotationDoor()//도어 열기
    {
        float startTime = 0f;
        float speedMultiplier = 2.5f; // 속도 증가
        startRotationLocker = transform.rotation;

        if (doorState)
            endRotation = Quaternion.Euler(0, startRotationLocker.eulerAngles.y + 110, 0);
        else
            endRotation = Quaternion.Euler(0, startRotationLocker.eulerAngles.y - 110, 0);

        istrigger_on();

        while (startTime < endTime)
        {
            float t = (startTime / endTime);
            transform.rotation = Quaternion.Slerp(startRotationLocker, endRotation, t);
            startTime += Time.deltaTime * speedMultiplier;
            yield return null;
        }

        transform.rotation = endRotation;
        doorState = !doorState;
        currentCoroutine = null;
        isRotation = false;
        istrigger_off();
    }

    public void istrigger_on()
    {
        boxcollider.isTrigger = true;
        // 부모의 MeshCollider를 비활성화
        transform.parent.GetComponent<MeshCollider>().enabled = false;
    }
    public void istrigger_off()
    {
        boxcollider.isTrigger = false;
        // 부모의 MeshCollider를 비활성화
        transform.parent.GetComponent<MeshCollider>().enabled = true;
    }

}
